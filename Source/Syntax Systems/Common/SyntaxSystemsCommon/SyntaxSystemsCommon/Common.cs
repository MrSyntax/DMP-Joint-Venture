using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageStream2;
using UnityEngine;

namespace SyntaxSystemsCommon
{
    static public class Common
    {


    }
    
    public class SyntaxGUIList
    {
        public Rect GUIListRect {get;set;}
        public Rect GUIListItemRect { get; set; }
        public Rect GUIListTitleRect { get; set; }
        //public Rect[] GUIListItemsRect { get; set; }
        //public string[] GUIListItemStrings { get; set; }
        public Texture2D GUIBackgroundTexture { get; set; }
        public SyntaxGUIListItem[] GUIListItems { get; set; }
        public GUIStyle ListGUIStyle = new GUIStyle();

        public GUIStyle GetGUIStyle
        {
            get
            {
                return ListGUIStyle;
            }
            set
            {
                ListGUIStyle = value;
            }
        }

        public GUIStyle ColorPanel()
        {
            GUIBackgroundTexture.SetPixel(0, 0, Color.gray);
            GUIBackgroundTexture.wrapMode = TextureWrapMode.Repeat;
            GUIBackgroundTexture.Apply();
            ListGUIStyle.normal.background = GUIBackgroundTexture;
            ListGUIStyle.fontSize = 16;
            ListGUIStyle.normal.textColor = Color.white;

            return ListGUIStyle;
        }
        public GUIStyle ColorPanel(Color backgroundColor)
        {
            GUIBackgroundTexture.SetPixel(0, 0, backgroundColor);
            GUIBackgroundTexture.wrapMode = TextureWrapMode.Repeat;
            GUIBackgroundTexture.Apply();
            ListGUIStyle.normal.background = GUIBackgroundTexture;
            ListGUIStyle.fontSize = 16;
            ListGUIStyle.normal.textColor = Color.white;

            return ListGUIStyle;
        }
        /// <summary>
        /// Use the default constructor. You must specify the rects and string values manuallly.
        /// </summary>
        public SyntaxGUIList()
        {
            // Setup default widths and heights for rects
            GUIListRect = new Rect();
            GUIListItemRect= new Rect();
            GUIListTitleRect = new Rect();
            ListGUIStyle = ColorPanel();
        }
        /// <summary>
        /// Use the recommended constructor. You must specify the string values only. Rects are automatically created.
        /// </summary>
        /// <param name="itemListValues"></param>
        public SyntaxGUIList(string[] itemListValues)
        {
            // Setup default widths and heights for rects
            GUIListRect = new Rect(15, 15, 200, 300);
            GUIListTitleRect = new Rect(15,15,200,20);
            GUIListItemRect = new Rect(15,30,200,20);
            //GUIListItemsRect = new Rect[itemListValues.Count()];
            GUIListItems = new SyntaxGUIListItem[itemListValues.Count()];

            int yOffset = 55;
            for(int i = 0; i < itemListValues.Count();i++)
            {
                //GUIListItemsRect[i] = new Rect(GUIListItemRect.xMin, yOffset, GUIListItemRect.width, GUIListItemRect.height);
                GUIListItems[i] = new SyntaxGUIListItem(itemListValues[i]);
                yOffset += 20;
            }
            ListGUIStyle = ColorPanel();
        }
        public SyntaxGUIList(string[] itemListValues, Color backgroundColor)
        {
            // Setup default widths and heights for rects
            GUIListRect = new Rect(15, 15, 200, 300);
            GUIListTitleRect = new Rect(15, 15, 200, 20);
            GUIListItemRect = new Rect(15, 30, 200, 20);
            //GUIListItemsRect = new Rect[itemListValues.Count()];
            GUIListItems = new SyntaxGUIListItem[itemListValues.Count()];
            int yOffset = 55;
            for (int i = 0; i < itemListValues.Count(); i++)
            {
                //GUIListItemsRect[i] = new Rect(GUIListItemRect.xMin, yOffset, GUIListItemRect.width, GUIListItemRect.height);
                GUIListItems[i] = new SyntaxGUIListItem(itemListValues[i]);
                yOffset += 20;
            }
            ListGUIStyle = ColorPanel(backgroundColor);
        }
        /// <summary>
        /// Use the advanced constructor. You must specify the GUIList rects and values directly.
        /// </summary>
        /// <param name="GuiListRect"></param>
        /// <param name="GuiListItemRect"></param>
        /// <param name="GuiListTitleRect"></param>
        /// <param name="GuiListItemsRect"></param>
        public SyntaxGUIList(Rect GuiListRect,Rect GuiListItemRect,Rect GuiListTitleRect, string[] GuiListItemsRect)
        {
            GUIListRect = GuiListRect;
            GUIListItemRect = GuiListItemRect;
            GUIListTitleRect = GuiListTitleRect;
            GUIListItems = new SyntaxGUIListItem[GuiListItemsRect.Count()];

            int yOffset = 55;
            for (int i = 0; i < GuiListItemsRect.Count(); i++)
            {
                //GUIListItemsRect[i] = new Rect(GUIListItemRect.xMin, yOffset, GUIListItemRect.width, GUIListItemRect.height);
                GUIListItems[i] = new SyntaxGUIListItem(GuiListItemsRect[i]);
                yOffset += 20;
            }
            ListGUIStyle = ColorPanel();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="offsetLeft"></param>
        /// <param name="offsetRight"></param>
        public void DrawList(Rect offsetLeft, Rect offsetRight)
        {

        }

        /// <summary>
        /// Syntax GUI List Item for the Syntax GUI List
        /// </summary>
        public class SyntaxGUIListItem
        {
            public Rect listitemRect = new Rect(15, 30, 200, 20);
            public string listitemValue = "";

            public SyntaxGUIListItem(string listitemvalue)
            {
                listitemValue = listitemvalue;
            }
            public SyntaxGUIListItem(Rect listitemrect,string listitemvalue)
            {
                listitemRect = listitemrect;
                listitemValue = listitemvalue;
            }

        }



    }


    public class SyntaxSystemsCommonMessage
    {
        public SyntaxSystemsCommonMessageType type;
        public byte[] data;
    }

    public enum SyntaxSystemsCommonMessageType
    {
        SYNTAX_BRIDGE = 1,
        PERMISSIONSYSTEMMESSAGE = 2,
        PERMISSIONSYSTEMGROUPMESSAGE = 3
    }
    
    public enum SyntaxAntiCheatMessageType
    {
        Check
    }

    public enum PermissionSystemMessageType
    {
        Check = 4,
        Claim = 5,
        Unclaim = 6
    }

    public enum PermissionSystemGroupMessageType
    {
        Create = 7,
        Invite = 8,
        Remove = 9
    }

    public enum TradingSystemMessageType
    {
        REGISTER,
        INVENTORY_ADD,
        INVENTORY_REMOVE,
        INVENTORY_UPDATE,
        BUYLIST_ADD,
        BUYLIST_REMOVE,
        BUYLIST_UPDATE,
        SELL,
        BUY
    }
}
