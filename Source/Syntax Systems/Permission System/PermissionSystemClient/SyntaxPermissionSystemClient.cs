using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;
using DarkMultiPlayer;
using SyntaxSystemsCommon;
using System.Timers;
using System.Globalization;
using DarkMultiPlayerCommon;

namespace PermissionSystem
{
    public static class SyntaxPermissionSystem
    {
        #region Permission System basics

        private static string currentVesselGuid = "";
        private static bool ownerVerified = false;
        
        public static bool spectateAllowed = false;

        public static bool OwnerVerified
        {
            get { return ownerVerified; }
        }

        internal static void PermissionSystemResponseHandler(ServerMessage message)
        {
            PermissionSystemMessageType messagetype;

            using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(message.data))
            {
                messagetype = (PermissionSystemMessageType)mr.Read<int>();

                switch (messagetype)
                {
                    case PermissionSystemMessageType.Check:
                        string ownerYesNo = mr.Read<string>();
                        string spectateYesNo = mr.Read<string>();
                        bool spectateAllowed = mr.Read<bool>();
                        AntiCheatSystemHandler.ReceiveACCheck(ownerYesNo, spectateYesNo, spectateAllowed);
                        break;
                    case PermissionSystemMessageType.Claim:
                        string responseLine = mr.Read<string>();
                        PermissionClaimResponse(responseLine);
                        break;
                    case PermissionSystemMessageType.Unclaim:
                        string unclaimresponse = mr.Read<string>();
                        PermissionUnClaimResponse(unclaimresponse);
                        break;
                }
            }

        }


        #endregion


        #region Claim command

        /// <summary>
        /// Claims a vessel given the vessel hasn't been claimed yet.
        /// </summary>
        /// <param name="playerName">The playername to claim the vessel for</param>
        /// <param name="personalorgroup">Personal or Group vessel</param>
        /// <param name="privateorpublic">Private or Public vessel</param>
        /// <param name="vesselguid">The vesselguid of the vessel to claim</param>
        internal static void PermissionClaim(string playerName, string personalorgroup, string accessChoice, string vesselguid)
        {
            // reformat for automated enum
            DarkLog.Debug("PermissionSystem: Writing vessel claim request..");
            string accesstype = accessChoice.ToLower();
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE);
                mw.Write<int>((int)PermissionSystemMessageType.Claim);
                mw.Write<string>(playerName);
                mw.Write<string>(vesselguid);
                mw.Write<string>(personalorgroup);
                mw.Write<string>(accesstype);
                DarkLog.Debug("PermissionSystem: Done writing vessel claim request.");
                DarkLog.Debug("PermissionSystem: Sending vessel claim requested.");
                DarkMultiPlayer.DMPModInterface.fetch.SendDMPModMessage("PERMISSIONSYSTEM", mw.GetMessageBytes(), false, false);
                //NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false); OBSOLETE SINCE MIGRATION TO PLUGIN SYSTEM
            }
        }

        public static void PermissionClaimGUI(string personalorgroup, string accessChoice, string vesselguid)
        {
            // reformat for automated enum
            DarkLog.Debug("PermissionSystem: Writing vessel claim request..");
            string accesstype = accessChoice.ToLower();
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE);
                mw.Write<int>((int)PermissionSystemMessageType.Claim);
                mw.Write<string>(DarkMultiPlayer.Settings.fetch.playerName);
                mw.Write<string>(vesselguid);
                mw.Write<string>(personalorgroup);
                mw.Write<string>(accesstype);
                DarkLog.Debug("PermissionSystem: Done writing vessel claim request.");
                DarkLog.Debug("PermissionSystem: Sending vessel claim requested.");
                DarkMultiPlayer.DMPModInterface.fetch.SendDMPModMessage("PERMISSIONSYSTEM", mw.GetMessageBytes(), false, false);
                //NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false); OBSOLETE SINCE MIGRATION TO PLUGIN SYSTEM
            }
        }

        private static void PermissionClaimResponse(string responseline)
        {
            if (responseline == "" || responseline == "Unknown")
            {
                // report error in debug for internal permission system error
                DarkLog.Debug("Permission System: Claim response internal error diagnosed.");
                return;
            }
            if (responseline == "CannotBeClaimed")
            {
                ScreenMessages.print("Vessel Claiming failed - Vessel can't be claimed.");
                return;
            }
            else
            {
                // We have determined every other outcome, thus the vessel has been claimed.
                // Thus relaying the success message.
                ScreenMessages.print(responseline);
                // todo: print the message in the chatbox, visible only to the vesselowner.
                return;
            }

        }

        private const ControlTypes BLOCK_ALL_CONTROLS = ControlTypes.ALL_SHIP_CONTROLS | ControlTypes.ACTIONS_ALL | ControlTypes.EVA_INPUT | ControlTypes.TIMEWARP | ControlTypes.MISC | ControlTypes.GROUPS_ALL | ControlTypes.CUSTOM_ACTION_GROUPS;

        public static void UnClaimVessel(string playername, string vesselguid)
        {
            // reformat for automated enum
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE);
                mw.Write<int>((int)PermissionSystemMessageType.Unclaim);
                mw.Write<string>(playername);
                mw.Write<string>(vesselguid);
                DarkMultiPlayer.DMPModInterface.fetch.SendDMPModMessage("PERMISSIONSYSTEM", mw.GetMessageBytes(), false, false);
            }
        }
        private static void PermissionUnClaimResponse(string responseline)
        {
            if (responseline == "" || responseline == "Unknown")
            {
                // report error in debug for internal permission system error
                Debug.Log("Permission System: UnClaim response internal error diagnosed.");
                return;
            }
            if (responseline == "CannotBeUnClaimed")
            {
                ScreenMessages.print("Vessel UnClaiming failed - Vessel can't be unclaimed.");
                return;
            }
            else
            {
                // We have determined every other outcome, thus the vessel has been claimed.
                // Thus relaying the success message.
                ScreenMessages.print(responseline);
                // todo: print the message in the chatbox, visible only to the vesselowner.
                return;
            }

        }
        #endregion

        #region Usergroups

        internal static void CreateUserGroup(string groupname,string groupadmin, string groupVesselAccessType)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)PermissionSystemGroupMessageType.Create);
                mw.Write<string>(groupname);
                mw.Write<string>(groupadmin);
                mw.Write<string>(groupVesselAccessType);
                DarkMultiPlayer.DMPModInterface.fetch.SendDMPModMessage("PERMISSIONSYSTEM", mw.GetMessageBytes(), false, false);
            }
        }

        private static void GroupRequestMessageResponse(string groupresponsemessage)
        {
            if (groupresponsemessage == "" || groupresponsemessage == null || groupresponsemessage == "unknown")
            {
                Debug.Log("empty, null or unknown response message.");
                ScreenMessages.print("Failed: empty, null or unknown response message.");
                return;
            }
            if(groupresponsemessage == "creation success")
            {
                ScreenMessages.print("Success: Requested group has been created.");
            }
            else if(groupresponsemessage == "invitation success")
            {
                ScreenMessages.print("Success: Requested player has been invited.");
            }
            else if (groupresponsemessage == "remove player success")
            {
                ScreenMessages.print("Success: Requested player has been removed.");
            }
            //else if (groupresponsemessage == "")
            //{
            //    ScreenMessages.print("Success: Requested group has been created.");
            //}

        }

        internal static void InvitePlayerToGroup(string playername, string playerToInvite)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)PermissionSystemGroupMessageType.Invite);
                mw.Write<string>(playername);
                mw.Write<string>(playerToInvite);
                DarkMultiPlayer.DMPModInterface.fetch.SendDMPModMessage("PERMISSIONSYSTEM", mw.GetMessageBytes(), false, false);
            }
        }

        internal static void RemovePlayerFromGroup(string playername, string playerToRemove, string reason)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)PermissionSystemGroupMessageType.Remove);
                mw.Write<string>(playername);
                mw.Write<string>(playerToRemove);
                mw.Write<string>(reason);
                DarkMultiPlayer.DMPModInterface.fetch.SendDMPModMessage("PERMISSIONSYSTEM", mw.GetMessageBytes(), false, false);
            }
        }
        #endregion


        // outdated since removal of bool usage to return ownership
        //private static bool WaitAsSpectator()
        //{
        //    DarkLog.Debug("Setting spectate lock accordingly whilst awaiting ownership determination");
        //    LockToSpectator();
        //    ScreenMessages.print("Waiting for ownership determination..");
        //    return true; // return true since the vessel is locked anyway.
        //}



    }
}

//Planetarium

//private void AuthenticateUser(){}
//private void GetVesselID(){}
//private void SendRequest(){}
//private void BlockAccess(){}
//private void BlockSpectate(){}
//private void Log(){}

