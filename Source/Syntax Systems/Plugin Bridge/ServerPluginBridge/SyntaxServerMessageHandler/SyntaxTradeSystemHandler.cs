using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkMultiPlayerServer;
using DarkMultiPlayerServer.Messages;
using AntiCheatSystemServer;
using TradingSystemServer;

namespace SyntaxServerMessageHandler
{
    public class SyntaxTradeSystemHamdler : DMPPlugin
    {
        public override void OnClientAuthenticated(ClientObject client)
        {
            // TODO : Retrieve client data from file
        }
        public override void OnMessageReceived(ClientObject client, DarkMultiPlayerCommon.ClientMessage messageData)
        {
            if (messageData.type == DarkMultiPlayerCommon.ClientMessageType.MOD_DATA)
            {
                ModData.HandleModDataMessage(client, messageData.data);
                messageData.handled = true;
                return;
            }
        }
        public override void OnServerStart()
        {
            // TODO : Initialize trading folders
            DMPModInterface.RegisterModHandler("TRADINGSYSTEM", TradingSystemServer.TradingSystemAccess.TradingSystemModMessageHandler);
        }
        public override void OnServerStop()
        {
            // TODO : Save memory data to file on stop
        }
        public override void OnClientDisconnect(ClientObject client)
        {
            // TODO : Save memory data regarding client to file
        }
    }
}
