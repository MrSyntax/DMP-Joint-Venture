﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkMultiPlayerServer;
using DarkMultiPlayerServer.Messages;
using PermissionSystem;
using System.Reflection;


namespace PermissionSystem
{
    public class PermissionSystemHandler
    {
        // Access codes for the codes behind the anti hijack and cheat system.
        public static class AntiCheatSystem
        {
            private static bool AntiHijackVessel(ClientObject client, string requestedvesselid, out bool spectateAllowed)
            {
                bool flag = false;
                bool spectate = false;
                if (!PermissionSystemAccess.PVessel.IsProtected(requestedvesselid))
                {
                    spectateAllowed = true;
                    return true;
                }
                if (PermissionSystemAccess.PVessel.IsProtected(requestedvesselid))
                {
                    if (PermissionSystemAccess.PVessel.IsOwner(client.playerName, requestedvesselid))
                    {
                        // allow vessel request
                        flag = true;

                    }
                    else
                    {
                        string accesstype;
                        if (PermissionSystemAccess.PVessel.GetAccessibility(requestedvesselid, out accesstype))
                        {
                            if (accesstype.ToString() == "public")
                            {
                                // allow vessel request
                                flag = true;
                            }
                            else if (accesstype.ToString() == "spectate")
                            {
                                // allow vessel spectate, but lock vessel
                                flag = true;
                                spectate = true;
                            }
                            else
                            {
                                // deny vessel request
                                flag = false;
                            }
                        }
                    }
                }
                else
                {
                    // Vessel has not been claimed so allow control and spectating
                    DarkLog.Debug("SyntaxCode: SAHS Check passed - Vessel not claimed.");
                    flag = true;
                    spectate = true;
                }
                spectateAllowed = spectate;
                return flag;
            }

        }
    }
}
//    //        /// <summary>
//    //        /// Syntax Anti Hijack System Check(SAHS): Checks wether the requested vessel belongs to the client requesting it.
//    //        /// If it belongs to the client, it will allow sending it. If not, it will only send the vessel if the AccessType is either 'public'
//    //        /// or 'spectate'. In case of 'spectate' it will keep the vessel locked to prevent taking over the vessel.
//    //        /// </summary>
//    //        /// <param name="client">The client requesting the vessel</param>
//    //        /// <param name="requestedVesselGuid">The requested vessel</param>
//    //        /// <returns>True if allowed to request, false if not</returns>
//    //        public static bool SAHSCheck(ClientObject client, string requestedVesselGuid)
//    //        {
//    //            bool flag = false;
//    //            bool spectatingIsAllowed = false;

//    //            if (SPSHandler.SAHSCheck(client, requestedVesselGuid, out spectatingIsAllowed))
//    //            {
//    //                flag = true;
//    //            }
//    //            else
//    //            {
//    //                if (spectatingIsAllowed)
//    //                {
//    //                    // Allow spectating of the vessel but lock the spectator out of the controls.
//    //                    //KeyValuePair<string, string> newLockEntry = new KeyValuePair<string, string>();
//    //                    KeyValuePair<string, PermissionSystemCore.PlayerVessel> foundVesselEntry = new KeyValuePair<string, PermissionSystemCore.PlayerVessel>();
//    //                    if (PermissionSystemAccess.PVessel.FindLockedPlayerVessel(requestedVesselGuid, out foundVesselEntry))
//    //                    {
//    //                        //newLockEntry = new KeyValuePair<string, string>(foundVesselEntry.Key, foundVesselEntry.Value.VesselID);
//    //                        // TODO: lock controls
//    //                        //LockVesselControls(newLockEntry.Value);
//    //                        flag = true;
//    //                    }
//    //                }
//    //                else
//    //                {
//    //                    // Report attempt to overwrite a lock has been diagnosed
//    //                    // Or an attempt to take over a protected vessel
//    //                    client.disconnectClient = true;
//    //                    ConnectionEnd.SendConnectionEnd(client, "Kicked for trying to take over a protected vessel.");
//    //                    //ClientHandler.DisconnectClient(client);
//    //                    DarkLog.Debug("Client kicked from permission handler. Section 2");
//    //                    return false;
//    //                }
//    //            }
//    //            return flag;
//    //        }

//    //        // Locks the vessel controls to allow for spectatemode
//    //        internal static void LockVesselControls(string vesselGuid)
//    //        {
//    //            throw new NotImplementedException();
//    //        }
//    //    }

//    //    // Codes behind the Syntax Anti cheat/hijack system
//    //    // handles the requests to and from the syntax permission system to avoid abuse on clientside
//    //    private class PermissionSystemInternalHandler
//    //    {
//    //        public bool SAHSCheck(ClientObject client, string requestedVesselid, out bool SpectatingAllowed)
//    //        {
//    //            bool flag = false;
//    //            bool spectateIsAllowed = false;

//    //            if (AntiHijackVessel(client, requestedVesselid, out spectateIsAllowed))
//    //            {
//    //                // Allow request because requesting client is the confirmed owner or requested vesselid has permissions set to public
//    //                flag = true;

//    //            }
//    //            else
//    //            {
//    //                // block request because requesting client is not allowed to control or see the requested vesselid.
//    //            }
//    //            SpectatingAllowed = spectateIsAllowed;
//    //            return flag;
//    //        }


//    //        // Checks wether a client is the actual owner of the vesselid, and if not wether the vesselid has an accesstype of public or spectate
//    //        private bool AntiHijackVessel(ClientObject client, string requestedvesselid, out bool spectateAllowed)
//    //        {
//    //            bool flag = false;
//    //            bool spectate = false;
//    //            if (!PermissionSystemAccess.PVessel.IsProtected(requestedvesselid))
//    //            {
//    //                spectateAllowed = true;
//    //                return true;
//    //            }
//    //            if (PermissionSystemAccess.PVessel.IsProtected(requestedvesselid))
//    //            {
//    //                if (PermissionSystemAccess.PVessel.IsOwner(client.playerName, requestedvesselid))
//    //                {
//    //                    // allow vessel request
//    //                    flag = true;
                        
//    //                }
//    //                else
//    //                {
//    //                    string accesstype;
//    //                    if (PermissionSystemAccess.PVessel.GetAccessibility(requestedvesselid, out accesstype))
//    //                    {
//    //                        if(accesstype.ToString() == "public")
//    //                        {
//    //                            // allow vessel request
//    //                            flag = true;
//    //                        }
//    //                        else if(accesstype.ToString() == "spectate")
//    //                        {
//    //                            // allow vessel spectate, but lock vessel
//    //                            flag = true;
//    //                            spectate = true;
//    //                        }
//    //                        else
//    //                        {
//    //                            // deny vessel request
//    //                            flag = false;
//    //                        }
//    //                    }
//    //                }
//    //            }
//    //            else
//    //            {
//    //                // Vessel has not been claimed so allow control and spectating
//    //                DarkLog.Debug("SyntaxCode: SAHS Check passed - Vessel not claimed.");
//    //                flag = true;
//    //                spectate = true;
//    //            }
//    //            spectateAllowed = spectate;
//    //            return flag;
//    //        }

//    //        // Checks wether people are hijacking vessels or not. If so, it either throws them back to kerbal space command or disconnects them.
//    //        private void AntiHijackCheck()
//    //        {
//    //            ClientObject[] clients = ClientHandler.GetClients(); // get the dmp server clients

//    //            foreach(ClientObject client in clients)
//    //            {
//    //                if (PermissionSystemAccess.PVessel.IsProtected(client.playerName, client.activeVessel))
//    //                {
//    //                    if (PermissionSystemAccess.PVessel.IsOwner(client.playerName, client.activeVessel))
//    //                    {
//    //                        // give control over the vessel
//    //                    }
//    //                    else
//    //                    {
//    //                        string accesstype;
//    //                        if (PermissionSystemAccess.PVessel.GetAccessibility(client.activeVessel, out accesstype))
//    //                        {
//    //                            if(accesstype.ToString() == "public")
//    //                            {
//    //                                // give control over the vessel
//    //                            }
//    //                            else if(accesstype.ToString() == "spectate")
//    //                            {
//    //                                // give spectate control only
//    //                            }
//    //                            else
//    //                            {
//    //                                // Block control and spectate
//    //                            }
//    //                        }
//    //                    }
//    //                }
//    //            }

//    //        }
//    //    }
//    //}
//    public class PermissionSystemPlugin : DMPPlugin
//    {
//        // Check client version, if not right, send request to update and if accepted, send the respective dll files
//        public override void OnClientAuthenticated(ClientObject client)
//        {
//            base.OnClientAuthenticated(client);
//        }
//        public override void OnServerStart()
//        {
//            PermissionSystemAccess.InitializePermissionFolder();
//        }
//        public override void OnServerStop()
//        {
//            PermissionSystemAccess.SaveToFile();
//        }

//        public override void OnUpdate()
//        {
//            HandleAntiCheatCheck();


//        }

//        public override void OnMessageReceived(ClientObject client, DarkMultiPlayerCommon.ClientMessage messageData)
//        {
//            byte[] messagebytes;
//            if(messageData.type == DarkMultiPlayerCommon.ClientMessageType.MOD_DATA)
//            {

//                using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(messageData.data))
//                {
//                    object messagetype = mr.Read<object>();
//                    if((int)messagetype == (int)SyntaxSystemsCommon.SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE)
//                    {





//                    }

//                }
//                messagebytes = messageData.data;
                



//            }
//            PermissionSystemMessage.HandlePermissionSystem(client, messageData.data);

//        }


//        private void HandleAntiCheatCheck()
//        {
//            ClientObject[] connectedClients = ClientHandler.GetClients();

//            foreach (ClientObject client in connectedClients)
//            {
//                PermissionSystemHandler.AntiCheatSystem.SAHSCheck(client, client.activeVessel);
//            }
//        }
//    }
//}
