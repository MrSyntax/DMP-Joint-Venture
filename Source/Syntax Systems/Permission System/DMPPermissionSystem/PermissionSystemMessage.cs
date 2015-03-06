using DarkMultiPlayerCommon;
using MessageStream2;
using PermissionSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SyntaxSystemsCommon;
using DarkMultiPlayerServer;
using AntiCheatSystemServer;


namespace PermissionSystem.Messages
{
    public class PermissionSystemMessage
    {
        // Handles all the requests client/server messages could have.

        static public void HandlePermissionClaimRequest(ClientObject client, string playername, string vesselguid, string personalOrGroup, string AccessType)
        {
            if (playername != client.playerName)
            {
                client.disconnectClient = true;
                DarkLog.Debug("Kicked client for stealing a vessel from permission message. Section 3.");
                DarkMultiPlayerServer.Messages.ConnectionEnd.SendConnectionEnd(client, "Stealing vessels is not allowed.");
                
                // .ClientHandler.DisconnectClient(client);
                return;
            }
            // Has been determined the requesting player is the same player as the ClientObject client, so continue.
            if (!PermissionSystem.PermissionSystemAccess.Player.isProtected(client.playerName))
            {
                playername = client.playerName;
                DarkLog.Debug("Saving player to file.");
                PermissionSystem.PermissionSystemAccess.Player.SaveCredentials(playername);
            }
            DarkLog.Debug("Handling player claim request " + client.playerName);
            if (HandleClaimRequest(playername, personalOrGroup, AccessType, vesselguid))
            {
                // Report the vessel has been claimed.
                DarkLog.Debug("Vessel has been claimed and handled.");
                HandleResponse(client, "claim", true);
            }
            else
            {
                // Report the vessel cannot be claimed because it has already been claimed by someone else
                HandleResponse(client, "claim", false);
            }

        }
        static public void HandlePermissionCheckRequest(ClientObject client, string playername, string vesselguid)
        {
            // update to use personal message system
            if (!AntiCheatSystemCore.AntiCheatSystem.SAHSCheck(client, vesselguid))
            {
                // Kick the client for failed anti-cheat check for some reason
                // Failed check only comes up when attemting to steal vessels.
                // So issue a kick command for it.
                client.disconnectClient = true;
                DarkLog.Debug("Kicked client for stealing a vessel from permission message. Section 3.");
                DarkMultiPlayerServer.Messages.ConnectionEnd.SendConnectionEnd(client, "Stealing vessels is not allowed.");
                //ClientHandler.DisconnectClient(client);
                return;
            }
            else
            {
                // Report the client has been recognised as the owner of the vessel
                bool spectateAllowed = PermissionSystem.PermissionSystemAccess.PVessel.SpectatingAllowed(vesselguid);
                string responseLine = "No";
                string spectate = "No";

                if (spectateAllowed)
                {
                    spectate = "Yes";
                }
                if (PermissionSystem.PermissionSystemAccess.PVessel.IsOwner(client.playerName, client.activeVessel))
                {
                    responseLine = "Yes";
                }
                responseLine = string.Format("{0},{1}", responseLine, spectate);
                if (!PermissionSystem.PermissionSystemAccess.PVessel.IsProtected(vesselguid))
                {
                    responseLine = "VesselNotProtected";
                    spectate = "Yes";
                }
                HandleResponse(client, "ownercheck", responseLine, spectate);
            }


            //DarkLog.Debug("Handling permission handled.");
        }
        static private void HandleResponse(ClientObject client, string command, string ownerYesNo,string spectateYesNo)
        {
            ServerMessage returnMessage = new ServerMessage();

            returnMessage.type = ServerMessageType.MOD_DATA;
            //string messageline = string.Format("{0},{1}", command, response);
            bool spectate = false;
            //string[] args = response.Split(',');
            if(ownerYesNo == "Yes")
            {
                spectate = true;
            }
            try
            {
                // Convert the messageline into bytes
                using (MessageWriter mw = new MessageWriter())
                {
                    mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                    mw.Write<int>((int)PermissionSystemMessageType.Check);
                    mw.Write<string>(ownerYesNo);
                    mw.Write<string>(spectateYesNo);
                    mw.Write<bool>(spectate);
                    returnMessage.data = mw.GetMessageBytes();
                }

                // Send the message to the client
                //DarkLog.Debug("Sending respone message to client..");

                SendSystemMessage(client, returnMessage, true);
                DarkLog.Debug("Permission System: Permission Check Response message sent to client" + client.playerName + " succesfully.");
            }
            catch
            {
                DarkLog.Debug("Permission System - Permission Check Response handling failed!");
            }
        }
        static private void HandleResponse(ClientObject client, string command, bool vesselClaimSuccess)
        {
            ServerMessage returnMessage = new ServerMessage();
            returnMessage.type = ServerMessageType.MOD_DATA;
            try
            {
                string claimResponse = "UNKNOWN";
                if(vesselClaimSuccess)
                {
                    claimResponse = "Vessel has been claimed for client: " + client.playerName;
                }
                else
                {
                    claimResponse = "CannotBeClaimed";
                }
                // Convert the messageline into bytes
                using (MessageWriter mw = new MessageWriter())
                {
                    mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                    mw.Write<int>((int)PermissionSystemMessageType.Claim);
                    mw.Write<string>(claimResponse);
                    returnMessage.data = mw.GetMessageBytes();
                }

                // Send the message to the client
                //DarkLog.Debug("Sending respone message to client..");
                SendSystemMessage(client, returnMessage, true);
                DarkLog.Debug("Permission System: Permission Check Response message sent to client" + client.playerName + " succesfully.");
            }
            catch
            {
                DarkLog.Debug("Permission System - Claim Response handling failed!");
            }
        }
        static private bool HandleClaimRequest(string pName, string personalOrGroup, string vesselAccessType, string vesselGuid)
        {
            bool flag = false;
            string personalorgroupFormatted = personalOrGroup.ToLower();
            string vat = CapitalizeWord(vesselAccessType);
            //DarkLog.Normal("Determining player vessel arguments");
            //DarkLog.Normal(string.Format("SyntaxCode: For debug sake show arguments: {0},{1},{2},{3}", pName, personalorgroupFormatted, vat, vesselGuid));
            if (personalorgroupFormatted == "personal")
            {
                //DarkLog.Normal("SyntaxCodes: Claiming vessel: " + vesselGuid + " for player " + pName);
                if (PermissionSystem.PermissionSystemAccess.PVessel.ClaimVessel(pName, vesselGuid, (PermissionSystemCore.VesselAccessibilityTypes)Enum.Parse(typeof(PermissionSystemCore.VesselAccessibilityTypes), vat)))
                {
                    flag = true;
                    //DarkLog.Normal("SyntaxCodes: Vessel claimed as "+ vesselAccessType + " .");
                }
                else
                {
                    //DarkLog.Debug("Vessel claiming unsuccesfull.");
                }
            }
            else if (personalorgroupFormatted == "group")
            {
                if (PermissionSystem.PermissionSystemAccess.Player.HasGroup(pName))
                {
                    if (PermissionSystem.PermissionSystemAccess.PGroup.ClaimVesselForGroup(pName, vesselGuid, (PermissionSystemCore.VesselAccessibilityTypes)Enum.Parse(typeof(PermissionSystemCore.VesselAccessibilityTypes), vat)))
                    {
                        flag = true;
                        //DarkLog.Normal("SyntaxCodes: Vessel claimed as " + vesselAccessType + " vessel within group.");
                    }
                    else
                    {
                        //DarkLog.Debug("Vessel claiming unsuccesfull.");
                    }
                }
            }
            else
            {
                // report invalid /claim command
                DarkLog.Debug("Vessel claiming message unreadable.");
            }
            return flag;
        }

        public static string CapitalizeWord(string word)
        {
            char firstChar = word[0];
            string restOfWord = word.Remove(0,1);
            char capChar = char.ToUpper(firstChar);
            string capword = capChar + restOfWord;
            DarkLog.Debug("Capitalized firt char of user command: " + capword);
            return capword;
        }

        public static void HandlePermissionUnClaimRequest(ClientObject client, string playername, string vesselguid)
        {
            DarkLog.Debug("Permission System: Handling UnClaim request");
            if (playername != client.playerName || !PermissionSystemAccess.Player.isProtected(playername))
            {
                client.disconnectClient = true;
                DarkLog.Debug("Kicked client for stealing a vessel from permission message. Section 3.");
                DarkMultiPlayerServer.Messages.ConnectionEnd.SendConnectionEnd(client, "Stealing vessels is not allowed.");
                // .ClientHandler.DisconnectClient(client);
                return;
            }

            if(PermissionSystemAccess.Player.isProtected(playername))
            {
                DarkLog.Debug("Permission System: Handling UnClaim request : Player is recognised.");
                if (PermissionSystemAccess.PVessel.IsOwner(playername, vesselguid))
                {
                    DarkLog.Debug("Permission System: Handling UnClaim request : Player is owner");
                    if (PermissionSystemAccess.PVessel.UnClaimVessel(playername, vesselguid))
                    {
                        DarkLog.Debug("Permission System: Handling UnClaim request succesfully executed.");
                    }
                    else
                    {
                        DarkLog.Debug("Permission System: Handling UnClaim request failed to execute.");
                    }
                }
            }
        }


        // TODO //

        static public void SendSystemMessage(ClientObject client, ServerMessage message, bool highPriority)
        {
             // TODO
        }

        public static void HandlePermissionSystemUserGroup(ClientObject client, byte[] p)
        {
            // TODO
        }
    }
}
