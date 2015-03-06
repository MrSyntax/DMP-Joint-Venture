using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkMultiPlayer;
using UnityEngine;
using SyntaxSystemsCommon;
using DarkMultiPlayerCommon;

namespace TradingSystem
{
    public class TradeItem
    {
        string itemname, amount, price;

        internal TradeItem(string itemName, string Amount, string Price)
        {
            itemname = itemName;
            amount = Amount;
            price = Price;
        }

        internal string GetTradeItem
        {
            get
            {
                return string.Format("Item: {0} | Amount: {1} | Price per: {2}", itemname, amount, price);
            }
        }
    }
    public static class SyntaxTradingSystem
    {

        static private Dictionary<string,TradeItem> tradeItemListCache = new Dictionary<string,TradeItem>();


        private static void TradingRequestMessageResponse(string tradingresponsemessage)
        {
            if (tradingresponsemessage == "success")
            {
                ScreenMessages.print("Success: Requested group has been created.");
            }
            else
            {
                ScreenMessages.print("Failure: Requested group could not be created.");
            }
        }
        internal static void TradingSystemResponseHandler(ServerMessage message)
        {
            TradingSystemMessageType messagetype;

            using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(message.data))
            {
                messagetype = (TradingSystemMessageType)mr.Read<int>();
                
                string responsepart = mr.Read<string>();
                string dataUpdate;

                switch (messagetype)
                {
                    case TradingSystemMessageType.REGISTER:
                        break;
                    case TradingSystemMessageType.SELL:
                        dataUpdate = mr.Read<string>();
                        HandleTrade(responsepart,dataUpdate);
                        break;
                    case TradingSystemMessageType.BUY:
                        dataUpdate = mr.Read<string>();
                        HandleTrade(responsepart, dataUpdate);
                        break;
                    case TradingSystemMessageType.INVENTORY_ADD:

                        break;
                    case TradingSystemMessageType.INVENTORY_UPDATE:
                        break;
                    case TradingSystemMessageType.INVENTORY_REMOVE:
                        break;
                    case TradingSystemMessageType.BUYLIST_ADD:
                        break;
                    case TradingSystemMessageType.BUYLIST_UPDATE:
                        break;
                    case TradingSystemMessageType.BUYLIST_REMOVE:
                        break;
                }
            }
        }

        static private void HandleTrade(string response, string dataUpdate)
        {


        }



        #region Trading requests
        internal static void RegisterTradeStation(string stationowner, string stationid, string personalOrGroup, string stationAccessType)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.REGISTER);
                mw.Write<string>(stationowner);
                mw.Write<string>(stationid);
                mw.Write<string>(personalOrGroup);
                mw.Write<string>(stationAccessType);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(),false);
            }
        }


        internal static void RequestAddToTraderSellList(string playername, string stationid, string itemName, string amount, string price)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.INVENTORY_ADD);
                mw.Write<string>(playername);
                mw.Write<string>(stationid);
                mw.Write<string>(itemName);
                mw.Write<string>(amount);
                mw.Write<string>(price);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false);
            }
        }

        internal static void RequestToSellItem(string tradestationid, string buyer, string itemToBuy, string amountToBuy)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.SELL);
                mw.Write<string>(tradestationid);
                mw.Write<string>(buyer);
                mw.Write<string>(itemToBuy);
                mw.Write<string>(amountToBuy);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), true);
            }
        }

        internal static void RequestToBuyItem(string tradestationid, string buyer, string itemToBuy, string amountToBuy)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.BUY);
                mw.Write<string>(tradestationid);
                mw.Write<string>(buyer);
                mw.Write<string>(itemToBuy);
                mw.Write<string>(amountToBuy);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), true);
            }
        }
        internal static void RequestRemoveFromTraderSellList(string playername, string stationid, string itemName, string amount, string price)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.INVENTORY_REMOVE);
                mw.Write<string>(playername);
                mw.Write<string>(stationid);
                mw.Write<string>(itemName);
                mw.Write<string>(amount);
                mw.Write<string>(price);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false);
            }
        }

        internal static void RequestAddToTraderBuyList(string playername, string stationid, string itemName, string amount, string price)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.BUYLIST_ADD);
                mw.Write<string>(playername);
                mw.Write<string>(stationid);
                mw.Write<string>(itemName);
                mw.Write<string>(amount);
                mw.Write<string>(price);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false);
            }
        }

        internal static void RequestRemoveFromTraderBuyList(string playername, string stationid, string itemName, string amount, string price)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)SyntaxSystemsCommonMessageType.SYNTAX_BRIDGE);
                mw.Write<int>((int)TradingSystemMessageType.BUYLIST_REMOVE);
                mw.Write<string>(playername);
                mw.Write<string>(stationid);
                mw.Write<string>(itemName);
                mw.Write<string>(amount);
                mw.Write<string>(price);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false);
            }
        }


        internal static void RequestUpdateTraderSellList(string playername, string stationid, string itemName, string amount, string price)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)TradingSystemMessageType.INVENTORY_UPDATE);
                mw.Write<string>(playername);
                mw.Write<string>(stationid);
                mw.Write<string>(itemName);
                mw.Write<string>(amount);
                mw.Write<string>(price);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false);
            }
        }

        internal static void RequestUpdateTraderBuyList(string playername, string stationid, string itemName, string amount, string price)
        {
            // send request
            using (MessageStream2.MessageWriter mw = new MessageStream2.MessageWriter())
            {
                mw.Write<int>((int)TradingSystemMessageType.BUYLIST_UPDATE);
                mw.Write<string>(playername);
                mw.Write<string>(stationid);
                mw.Write<string>(itemName);
                mw.Write<string>(amount);
                mw.Write<string>(price);
                NetworkWorker.fetch.SendModMessage(mw.GetMessageBytes(), false);
            }
        }
        #endregion
    }

 
/*
This module will create a new GUI window and button, on button press the part will explode, close the GUI, and be removed.
*/
    //public class SyntaxToolbarButton
    //{
    //    RUIToggleButton btn = new RUIToggleButton();
    //    //IButton dmpTradingBtn;
    //    public PackedSprite buttonTexture = new PackedSprite();
    //    internal SyntaxToolbarButton()
    //    {
    //        SpriteText spritetext = new SpriteText();
    //        spritetext.text = "Trading";
    //        btn.text = spritetext;
            
    //        //btn = ToolbarManager.Instance.add("DarkMultiPlayer", "GUIButton");
    //    }
        
    //    internal void Toggle()
    //    {

    //    }

    //    internal void ToggleHover()
    //    {
    //    }
    //}
    //[KSPAddon(KSPAddon.Startup.EveryScene,false)]
    //public class SyntaxGUI : MonoBehaviour
    //{
    //    protected Rect windowPos;
    //    SyntaxToolbarButton syntaxBtn = new SyntaxToolbarButton();
        

    //    public SyntaxGUI()
    //    {
    //        OnGameStart();
    //    }

    //    private void DoNothing() { }

    //    private void OnGameStart()
    //    {
    //        if(HighLogic.LoadedSceneIsEditor)
    //        {
    //            return; // todo
    //        }
    //        if(HighLogic.LoadedScene == GameScenes.TRACKSTATION)
    //        {
    //            return; // todo
    //        }
    //        drawGUI();
    //        ApplicationLauncherButton appBtn = ApplicationLauncher.Instance.AddModApplication(syntaxBtn.Toggle, syntaxBtn.Toggle, DoNothing, DoNothing, DoNothing, DoNothing, ApplicationLauncher.AppScenes.ALWAYS, syntaxBtn.buttonTexture);
    //        appBtn.VisibleInScenes = ApplicationLauncher.AppScenes.SPACECENTER;
    //        appBtn.Enable();

    //    }
        

    //    private void WindowGUI(int windowID)
    //    {
    //        GUIStyle mySty = new GUIStyle(GUI.skin.button); 
    //        mySty.normal.textColor = mySty.focused.textColor = Color.white;
    //        mySty.hover.textColor = mySty.active.textColor = Color.yellow;
    //        mySty.onNormal.textColor = mySty.onFocused.textColor = mySty.onHover.textColor = mySty.onActive.textColor = Color.green;
    //        mySty.padding = new RectOffset(8, 8, 8, 8);
            
    

    //        GUILayout.BeginVertical();
    //    GUILayout.EndVertical();
 
    //            //DragWindow makes the window draggable. The Rect specifies which part of the window it can by dragged by, and is 
    //            //clipped to the actual boundary of the window. You can also pass no argument at all and then the window can by
    //            //dragged by any part of it. Make sure the DragWindow command is AFTER all your other GUI input stuff, or else
    //            //it may "cover up" your controls and make them stop responding to the mouse.
    //            GUI.DragWindow(new Rect(0, 0, 10000, 20));
 
    //    }
    //    private void drawGUI()
    //    {
    //        GUI.skin = HighLogic.Skin;
    //        windowPos = GUILayout.Window(1, windowPos, WindowGUI, "Trading", GUILayout.MinWidth(100));	 
    //    }
    //}

}
