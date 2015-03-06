using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkMultiPlayerServer;
using SyntaxSystemsCommon;

namespace SyntaxServerMessageHandler
{
    public static class SyntaxCodeHandler
    {

        
                //mw.Write<int>((int)PermissionSystemMessageType.Claim);
                //mw.Write<string>(playerName);
                //mw.Write<string>(personalorgroup);
                //mw.Write<string>(accesstype);
                //mw.Write<string>(vesselguid);

        internal static void HandlePermissionSystemModMessage(ClientObject client, byte[] modData)
        {
            if (modData != null)
            {
                DarkLog.Debug("Receiving information for modname: SYNRAXCODEHANDLER PERMISSIONSYSTEM");
                using (MessageStream2.MessageReader modReader = new MessageStream2.MessageReader(modData))
                {
                    SyntaxSystemsCommonMessageType permissionsystemMessageType = (SyntaxSystemsCommonMessageType)modReader.Read<int>();
                    DarkLog.Debug("PERMISSIONSYSTEM: Done Receiving Messagebytes from client..: modbytes " + permissionsystemMessageType.ToString());
                    PermissionSystemMessageType standardmessage = PermissionSystemMessageType.Check;
                    PermissionSystemGroupMessageType messagetype = PermissionSystemGroupMessageType.Create;
                    switch (permissionsystemMessageType)
                    {
                        case SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMMESSAGE:
                            DarkLog.Debug("Determined to be normal message");
                            string pname, vguid;
                            standardmessage = (PermissionSystemMessageType)modReader.Read<int>();
                            DarkLog.Debug("Determined to be normal message " + standardmessage.ToString());
                            switch (standardmessage)
                            {
                                case PermissionSystemMessageType.Check:
                                    DarkLog.Debug("Permission System: Check request recognised");
                                    pname = modReader.Read<string>();
                                    vguid = modReader.Read<string>();
                                    PermissionSystem.Messages.PermissionSystemMessage.HandlePermissionCheckRequest(client, pname, vguid);
                                    break;
                                case PermissionSystemMessageType.Claim:
                                    DarkLog.Debug("Permission System: Claim request recognised");
                                    pname = modReader.Read<string>();
                                    vguid = modReader.Read<string>();
                                    string pOrG = modReader.Read<string>();
                                    string aT = modReader.Read<string>();
                                    PermissionSystem.Messages.PermissionSystemMessage.HandlePermissionClaimRequest(client, pname, vguid, pOrG, aT);
                                    break;
                                case PermissionSystemMessageType.Unclaim:
                                    DarkLog.Debug("Permission System: UnClaim request recognised");
                                    pname = modReader.Read<string>();
                                    vguid = modReader.Read<string>();
                                    PermissionSystem.Messages.PermissionSystemMessage.HandlePermissionUnClaimRequest(client, pname, vguid);
                                    break;
                                default:
                                    // report unknown messagetype
                                    DarkLog.Debug("Unknown messagetype." + standardmessage.ToString());
                                    break;
                            }
                            break;
                        case SyntaxSystemsCommonMessageType.PERMISSIONSYSTEMGROUPMESSAGE:
                            messagetype = (PermissionSystemGroupMessageType)modReader.Read<int>();
                            DarkLog.Debug("Determined to be group message");
                            switch (messagetype)
                            {
                                case PermissionSystemGroupMessageType.Create:
                                    break;
                                case PermissionSystemGroupMessageType.Invite:
                                    break;
                                case PermissionSystemGroupMessageType.Remove:
                                    break;
                                default:
                                    // report unknown messagetype
                                    break;
                            }
                            PermissionSystem.Messages.PermissionSystemMessage.HandlePermissionSystemUserGroup(client, modData);
                            break;

                        default:
                            DarkLog.Debug("Unknown main syntax bridge messagetype : " + permissionsystemMessageType.ToString());
                            break;
                    }
                    client.receiveMessage.handled = true;
                }
            }
            else
            {
                DarkLog.Debug("Ignored information because modData is null. ");
            }

        }
    }
}
