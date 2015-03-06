using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DarkMultiPlayer;
using SyntaxBridgeSystem;
using SyntaxBridgeSystem.PluginHandling;

namespace PermissionSystemClient
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class SyntaxPermissionCommandHandler
    {
        private static bool modCommandsRegistered = false;
        public SyntaxPermissionCommandHandler()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER | HighLogic.LoadedScene == GameScenes.FLIGHT | HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                modCommandsRegistered = true;
            }
            RegisterPermissionCommands();
        }
        public bool isRegistered
        {
            get { return modCommandsRegistered; }
        }
        
        /// <summary>
        /// Registers the permission commands with the chatworker
        /// </summary>
        public void RegisterPermissionCommands()
        {
            ChatWorker.RegisterPluginChatCommand("claim", SyntaxPermissionCommandHandler.ClaimVessel, "Claims the current vessel. Format: /claim [personal/group] [private/public/group/spectate]");
            ChatWorker.RegisterPluginChatCommand("unclaim", SyntaxPermissionCommandHandler.UnClaimVessel, "Unclaims the current vessel.");
            ChatWorker.RegisterPluginChatCommand("newgroup", SyntaxPermissionCommandHandler.CreateUserGroup, "Creates a new usergroup. [groupname] [vesselaccesstype] Format: /newgroup [groupname] [private/public/group/spectate");
            ChatWorker.RegisterPluginChatCommand("invite", SyntaxPermissionCommandHandler.InviteToGroup, "Invites a player to your usergroup. Format: /invite [playername]");
            ChatWorker.RegisterPluginChatCommand("gremove", SyntaxPermissionCommandHandler.RemovePlayerFromGroup, "Removes a player from your usergroup. Format: /gremove [playername] [reason]");
            
        }

        #region Syntax Commands

        #region Vessel Claiming
        public static void ClaimVessel(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string personalOrGroup = args[0];
            string privateOrPublic = args[1];
            string vesselid = FlightGlobals.ActiveVessel.id.ToString();
            PermissionSystem.SyntaxPermissionSystem.PermissionClaim(playername, personalOrGroup, privateOrPublic, vesselid);
            DarkLog.Debug("PermissionSystem: Vessel claim requested.");
        }
        public static void UnClaimVessel(string commandArgs)
        {
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string vesselguid = FlightGlobals.ActiveVessel.id.ToString();
            PermissionSystem.SyntaxPermissionSystem.UnClaimVessel(playername, vesselguid);
        }
        #endregion

        #region Usergroups
        public static void CreateUserGroup(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string groupname = args[0];
            string groupvesselaccesstype = args[1];
            PermissionSystem.SyntaxPermissionSystem.CreateUserGroup(groupname, playername, groupvesselaccesstype);
        }
        public static void InviteToGroup(string commandArgs)
        {
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string playerToInvite = commandArgs;
            PermissionSystem.SyntaxPermissionSystem.InvitePlayerToGroup(playername, playerToInvite);
        }
        public static void RemovePlayerFromGroup(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string playerToRemove = args[0];
            string reason = args[1];
            PermissionSystem.SyntaxPermissionSystem.RemovePlayerFromGroup(playername, playerToRemove, reason);
        }
        #endregion

        #endregion
    }

    public class PermissionSystemInterface : IPluginBridge
    {
        public override void InitializePlugin()
        {
            SyntaxPermissionCommandHandler SPCH = new SyntaxPermissionCommandHandler();
            if (!SPCH.isRegistered)
            {
                SPCH.RegisterPermissionCommands();
            }
            SyntaxBridgeSystem.PluginHandling.PluginBridge.New("PERMISSIONSYSTEM", PermissionSystemResponseHandler);
        }

        private void PermissionSystemResponseHandler(byte[] messagedata)
        {
            //throw new NotImplementedException();
        }
    }

}
