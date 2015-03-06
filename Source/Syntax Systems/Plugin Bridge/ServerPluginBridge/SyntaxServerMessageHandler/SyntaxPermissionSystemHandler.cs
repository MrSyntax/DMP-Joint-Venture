using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkMultiPlayerServer;
using DarkMultiPlayerServer.Messages;
using PermissionSystem;
using System.Reflection;
using AntiCheatSystemServer;
using SyntaxSystemsCommon;
using SyntaxServerMessageHandler;
using PermissionSystem.Messages;

namespace PermissionSystem
{
    public class PermissionSystemPlugin : DMPPlugin
    {
        // Check client version, if not right, send request to update and if accepted, send the respective dll files
        public override void OnClientAuthenticated(ClientObject client)
        {
            base.OnClientAuthenticated(client);
        }
        public override void OnServerStart()
        {
            PermissionSystemAccess.InitializePermissionFolder();
            DMPModInterface.RegisterModHandler("PERMISSIONSYSTEM",SyntaxCodeHandler.HandlePermissionSystemModMessage);
        }


        public override void OnServerStop()
        {
            PermissionSystemAccess.SaveToFile();
        }

        public override void OnUpdate()        { }

        
                //mw.Write<int>((int)PermissionSystemMessageType.Claim);
                //mw.Write<string>(playerName);
                //mw.Write<string>(personalorgroup);
                //mw.Write<string>(accesstype);
                //mw.Write<string>(vesselguid);

        public override void OnMessageReceived(ClientObject client, DarkMultiPlayerCommon.ClientMessage messageData)
        {
            if (messageData.type == DarkMultiPlayerCommon.ClientMessageType.MOD_DATA)
            {
                ModData.HandleModDataMessage(client, messageData.data);
                messageData.handled = true;
                return;
            }
        }


        private void HandleAntiCheatCheck()
        {
            ClientObject[] connectedClients = ClientHandler.GetClients();

            foreach (ClientObject client in connectedClients)
            {
                AntiCheatSystemCore.AntiCheatSystem.SAHSCheck(client, client.activeVessel);
            }
        }
        //public static void HandlePermissionSystem(ClientObject client, byte[] data)
        //{
        //    DarkLog.Debug("Permission System: Entering permission system handler");
        //    using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(data))
        //    {
        //        mr.Read<int>();
        //        PermissionSystemMessageType messagetype = (PermissionSystemMessageType)Enum.Parse(typeof(PermissionSystemMessageType), mr.Read<string>());
        //        string pname, vguid;
        //        switch (messagetype)
        //        {
        //            case PermissionSystemMessageType.Check:
        //                DarkLog.Debug("Permission System: Check request recognised");
        //                pname = mr.Read<string>();
        //                vguid = mr.Read<string>();
        //                PermissionSystem.Messages.PermissionSystemMessage.HandlePermissionCheckRequest(client, pname, vguid);
        //                break;
        //            case PermissionSystemMessageType.Claim:
        //                DarkLog.Debug("Permission System: Claim request recognised");
        //                pname = mr.Read<string>();
        //                string pOrG = mr.Read<string>();
        //                string aT = mr.Read<string>();
        //                vguid = mr.Read<string>();
        //                PermissionSystem.Messages.PermissionSystemMessage.HandlePermissionClaimRequest(client, pname, vguid, pOrG, aT);
        //                break;
        //            case PermissionSystemMessageType.Unclaim:

        //                // TODO

        //                break;
        //            default:
        //                // report unknown messagetype
        //                break;
        //        }
        //    }
        //    client.receiveMessage.handled = true;
        //}
        //public static void HandlePermissionSystemUserGroup(ClientObject client, byte[] data)
        //{
        //    DarkLog.Debug("Permission System: Entering permission system handler");
        //    using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(data))
        //    {
        //        PermissionSystemGroupMessageType messagetype = (PermissionSystemGroupMessageType)Enum.Parse(typeof(PermissionSystemGroupMessageType), mr.Read<string>());

        //        switch (messagetype)
        //        {
        //            case PermissionSystemGroupMessageType.Create:
        //                break;
        //            case PermissionSystemGroupMessageType.Invite:
        //                break;
        //            case PermissionSystemGroupMessageType.Remove:
        //                break;
        //            default:
        //                // report unknown messagetype
        //                break;
        //        }
        //    }
        //}
    }
}
//if (messageData.data != null)
//{
//    SyntaxSystemsCommonMessageType msgType = SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE;
//    string username, vesselid;
//    string personalORgroup, accessType;
//    //DarkLog.Debug("AntiCheat: Reading Messagebytes from client..");
//    using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(messageData.data))
//    {
//        //SyntaxSystemsCommonMessageType syntaxbridgeType = (SyntaxSystemsCommonMessageType)mr.Read<int>();
//        string modname = mr.Read<string>();
//        DarkLog.Debug("Receiving information for modname: " + modname);
//        if (modname == "PERMISSIONSYSTEM")
//        {
//            //DarkLog.Debug("AntiCheat: Done Receiving Messagebytes from client..: modname " + modname);
//            bool relay = mr.Read<bool>();
//            //DarkLog.Debug("AntiCheat: Done Receiving Messagebytes from client..: relay ");
//            bool priorityHigh = mr.Read<bool>();
//            //DarkLog.Debug("AntiCheat: Done Receiving Messagebytes from client..: priority ");

//            using (MessageStream2.MessageReader modReader = new MessageStream2.MessageReader(mr.Read<byte[]>()))
//            {
//                msgType = (SyntaxSystemsCommonMessageType)modReader.Read<int>();
//                username = modReader.Read<string>();
//                vesselid = modReader.Read<string>();
//                DarkLog.Debug(string.Format(""));
//                if (msgType == SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE)
//                {
//                    personalORgroup = modReader.Read<string>();
//                    accessType = modReader.Read<string>();
//                    DarkLog.Debug(string.Format("username: {0} vesselid: {1} personal or group: {2} accesstype: {3}", username, vesselid, personalORgroup, accessType));
//                }
//                DarkLog.Debug("PERMISSIONSYSTEM: MessageType: " + msgType.ToString() + " user: " + username + " vesselguid: " + vesselid);
//            }

//            switch (msgType)
//            {
//                case SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE:
//                    if (client.activeVessel == null)
//                    {
//                        return;
//                    }
//                    DarkLog.Debug("PERMISSIONSYSTEM: Action chosen");
//                    break;
//                case SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMGROUPMESSAGE:
//                    DarkLog.Debug("PERMISSIONSYSTEM GROUP: Action chosen");
//                    break;
//                default:
//                    DarkLog.Debug("Syntax Anti Cheat messagetype unknown.");
//                    break;
//            }

//            messageData.handled = true;
//        }
//        else
//        {
//            DarkLog.Debug("Ignored information because not directed at anti cheat plugin for modname: " + modname);
//        }
//    }
//}
//else
//{
//    DarkLog.Debug("Client MessageData is null. Client name: " + client.playerName);
//}