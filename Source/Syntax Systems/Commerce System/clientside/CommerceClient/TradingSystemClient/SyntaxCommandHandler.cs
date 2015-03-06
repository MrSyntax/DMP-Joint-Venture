using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DarkMultiPlayer;
using SyntaxBridgeSystem;

namespace TradingSystemClient
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre,false)]
    public class SyntaxTradeCommandHandler
    {
        private static bool modCommandsRegistered = false;
        public SyntaxTradeCommandHandler()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER | HighLogic.LoadedScene == GameScenes.FLIGHT | HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                modCommandsRegistered = true;
            }
            RegisterTradeChatCommands();
        }

        public bool isRegistered
        {
            get { return modCommandsRegistered; }
        }

        public void RegisterTradeChatCommands()
        {
            ChatWorker.RegisterPluginChatCommand("registertradestation", SyntaxTradeCommandHandler.RequestTradeStationRegistration, "Requests trading station registration. Format: /registertradestation [personal/group] [private/public/group/spectate]");
            ChatWorker.RegisterPluginChatCommand("inventory add", SyntaxTradeCommandHandler.RequestAddToSellList, "Adds an item to the station sell list. Format: /inventory add [itemname] [amount] [price]");
            ChatWorker.RegisterPluginChatCommand("inventory update", SyntaxTradeCommandHandler.RequestUpdateSellList, "Updates an item to the station sell list. Format: /inventory add [itemname] [amount] [price]");
            ChatWorker.RegisterPluginChatCommand("inventory remove", SyntaxTradeCommandHandler.RequestRemoveFromSellList, "Removes an item to the station sell list. Format: /inventory remove [itemname] [amount] [price]");
            ChatWorker.RegisterPluginChatCommand("buylist add", SyntaxTradeCommandHandler.RequestAddToBuyList, "Adds an item to the station buy list. Format: /wishlist add [itemname] [amount] [price]");
            ChatWorker.RegisterPluginChatCommand("buylist update", SyntaxTradeCommandHandler.RequestUpdateBuyList, "Updates an item to the station buy list. Format: /wishlist add [itemname] [amount] [price]");
            ChatWorker.RegisterPluginChatCommand("buylist remove", SyntaxTradeCommandHandler.RequestRemoveFromBuyList, "Removes an item to the station buy list. Format: /wishlist remove [itemname] [amount] [price]");
            ChatWorker.RegisterPluginChatCommand("sell", SyntaxTradeCommandHandler.RequestBuyItem, "Sell something directly to a docked player. Format: /sell [buyername] [itemname] [amount]");
            ChatWorker.RegisterPluginChatCommand("buy", SyntaxTradeCommandHandler.RequestSellItem, "Buy something directly from a trading station. Format: /buy [buyername] [itemname] [amount]");
            



        }

        #region Trading

        #region User Interaction
        public static void RequestBuyItem(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string tradestationid = VesselWorker.fetch.VesselDockedTo;
            string buyer = DarkMultiPlayer.Settings.fetch.playerName;
            string itemToBuy = args[1];
            string amountToBuy = args[2];
            TradingSystem.SyntaxTradingSystem.RequestToBuyItem(tradestationid, buyer, itemToBuy, amountToBuy);
        }
        public static void RequestSellItem(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string tradestationid = VesselWorker.fetch.VesselDockedTo;
            string buyer = args[0];
            string itemToBuy = args[1];
            string amountToBuy = args[2];
            TradingSystem.SyntaxTradingSystem.RequestToSellItem(tradestationid, buyer, itemToBuy, amountToBuy);
        }
        public static void RequestTradeStationRegistration(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string stationowner = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string personalOrGroup = args[0];
            string stationAccessType = args[1];
            TradingSystem.SyntaxTradingSystem.RegisterTradeStation(stationowner, stationid, personalOrGroup, stationAccessType);
        }
        #endregion

        #region Sell List
        public static void RequestAddToSellList(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string itemName = args[0];
            string amount = args[1];
            string price = args[2];
            TradingSystem.SyntaxTradingSystem.RequestAddToTraderSellList(playername, stationid, itemName, amount, price);
        }
        public static void RequestUpdateSellList(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string itemName = args[0];
            string amount = args[1];
            string price = args[2];
            TradingSystem.SyntaxTradingSystem.RequestUpdateTraderSellList(playername, stationid, itemName, amount, price);
        }
        public static void RequestRemoveFromSellList(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string itemName = args[0];
            string amount = args[1];
            string price = args[2];
            TradingSystem.SyntaxTradingSystem.RequestRemoveFromTraderSellList(playername, stationid, itemName, amount, price);
        }
        #endregion

        #region Buy List
        public static void RequestUpdateBuyList(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string itemName = args[0];
            string amount = args[1];
            string price = args[2];
            TradingSystem.SyntaxTradingSystem.RequestUpdateTraderBuyList(playername, stationid, itemName, amount, price);
        }
        public static void RequestAddToBuyList(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string itemName = args[0];
            string amount = args[1];
            string price = args[2];
            TradingSystem.SyntaxTradingSystem.RequestAddToTraderBuyList(playername, stationid, itemName, amount, price);
        }
        public static void RequestRemoveFromBuyList(string commandArgs)
        {
            string[] args = commandArgs.Split(' ');
            string playername = DarkMultiPlayer.Settings.fetch.playerName;
            string stationid = FlightGlobals.ActiveVessel.id.ToString();
            string itemName = args[0];
            string amount = args[1];
            string price = args[2];
            TradingSystem.SyntaxTradingSystem.RequestRemoveFromTraderBuyList(playername, stationid, itemName, amount, price);
        }
        #endregion

        #endregion

        // Send to server
        //public static void SendToClient(ClientObject client, ServerMessage message, bool highPriority)
        //{
        //    //Because we dodge the queue, we need to lock it up again...
        //    lock (client.sendLock)
        //    {
        //        if (message == null)
        //        {
        //            return;
        //        }
        //        //All messages have an 8 byte header
        //        client.bytesQueuedOut += 8;
        //        if (message.data != null)
        //        {
        //            //Count the payload if we have one.
        //            client.bytesQueuedOut += message.data.Length;
        //        }
        //        if (highPriority)
        //        {
        //            client.sendMessageQueueHigh.Enqueue(message);
        //        }
        //        else
        //        {
        //            client.sendMessageQueueLow.Enqueue(message);
        //            //If we need to optimize
        //            if (client.bytesQueuedOut > OPTIMIZE_QUEUE_LIMIT)
        //            {
        //                //And we haven't optimized in the last 5 seconds
        //                long currentTime = DateTime.UtcNow.Ticks;
        //                long optimizedBytes = 0;
        //                if ((currentTime - client.lastQueueOptimizeTime) > 50000000)
        //                {
        //                    client.lastQueueOptimizeTime = currentTime;
        //                    DarkLog.Debug("Optimizing " + client.playerName + " (" + client.bytesQueuedOut + " bytes queued)");

        //                    //Create a temporary filter list
        //                    List<ServerMessage> oldClientMessagesToSend = new List<ServerMessage>();
        //                    List<ServerMessage> newClientMessagesToSend = new List<ServerMessage>();
        //                    //Steal all the messages from the queue and put them into a list
        //                    ServerMessage stealMessage = null;
        //                    while (client.sendMessageQueueLow.TryDequeue(out stealMessage))
        //                    {
        //                        oldClientMessagesToSend.Add(stealMessage);
        //                    }
        //                    //Clear the client send queue
        //                    List<string> seenProtovesselUpdates = new List<string>();
        //                    List<string> seenPositionUpdates = new List<string>();
        //                    //Iterate backwards over the list
        //                    oldClientMessagesToSend.Reverse();
        //                    foreach (ServerMessage currentMessage in oldClientMessagesToSend)
        //                    {
        //                        if (currentMessage.type != ServerMessageType.VESSEL_PROTO && currentMessage.type != ServerMessageType.VESSEL_UPDATE)
        //                        {
        //                            //Message isn't proto or position, don't skip it.
        //                            newClientMessagesToSend.Add(currentMessage);
        //                        }
        //                        else
        //                        {
        //                            //Message is proto or position
        //                            if (currentMessage.type == ServerMessageType.VESSEL_PROTO)
        //                            {
        //                                using (MessageReader mr = new MessageReader(currentMessage.data))
        //                                {
        //                                    //Don't care about the send time, it's already the latest in the queue.
        //                                    mr.Read<double>();
        //                                    string vesselID = mr.Read<string>();
        //                                    if (!seenProtovesselUpdates.Contains(vesselID))
        //                                    {
        //                                        seenProtovesselUpdates.Add(vesselID);
        //                                        newClientMessagesToSend.Add(currentMessage);
        //                                    }
        //                                    else
        //                                    {
        //                                        optimizedBytes += 8 + currentMessage.data.Length;
        //                                    }
        //                                }
        //                            }

        //                            if (currentMessage.type == ServerMessageType.VESSEL_UPDATE)
        //                            {
        //                                using (MessageReader mr = new MessageReader(currentMessage.data))
        //                                {
        //                                    //Don't care about the send time, it's already the latest in the queue.
        //                                    mr.Read<double>();
        //                                    string vesselID = mr.Read<string>();
        //                                    if (!seenPositionUpdates.Contains(vesselID))
        //                                    {
        //                                        seenPositionUpdates.Add(vesselID);
        //                                        newClientMessagesToSend.Add(currentMessage);
        //                                    }
        //                                    else
        //                                    {
        //                                        //8 byte message header plus payload
        //                                        optimizedBytes += 8 + currentMessage.data.Length;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    //Flip it back to the right order
        //                    newClientMessagesToSend.Reverse();
        //                    foreach (ServerMessage putBackMessage in newClientMessagesToSend)
        //                    {
        //                        client.sendMessageQueueLow.Enqueue(putBackMessage);
        //                    }
        //                    float optimizeTime = (DateTime.UtcNow.Ticks - currentTime) / 10000f;
        //                    client.bytesQueuedOut -= optimizedBytes;
        //                    DarkLog.Debug("Optimized " + optimizedBytes + " bytes in " + Math.Round(optimizeTime, 3) + " ms.");
        //                }
        //            }
        //        }
        //        client.sendEvent.Set();
        //    }
        //}

    }

    public class PluginInit : IPluginBridge
    {
        public override void InitializePlugin()
        {
            SyntaxTradeCommandHandler STCH = new SyntaxTradeCommandHandler();
            if (!STCH.isRegistered)
            {
                STCH.RegisterTradeChatCommands();
            }
            SyntaxBridgeSystem.PluginHandling.PluginBridge.New("TRADESYSTEM", TradeResponseHandler);
        }

        private void TradeResponseHandler(byte[] messagedata)
        {
            //throw new NotImplementedException();
        }
    }
}
