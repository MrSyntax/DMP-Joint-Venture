using PermissionSystemClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
//using GUIChangeWindow;
using KSP;

namespace SyntaxBridgeSystem.SyntaxGUI
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class GUITest : MonoBehaviour
    {
        #region Base GUI values
        static string groupingOption, vesselAccessibilityOption;
        static int personalOrGroupValue = 0;
        static int vesselAccessTypeValue = 0;
        static int offsetLeftRight = 190;
        static int offsetTopBottom = 66;
        static int width = Screen.width - 380;
        static int height = Screen.height - 132;
        static int currentWindow = 1;
        static int currentMainCat = 1;
        static int currentSubCatWindow = 1;
        static Rect window;
        static Rect currentContentWindow;
        static Texture2D textureTitle = new Texture2D(1, 1);
        static Texture2D texturePanel = new Texture2D(1, 1);
        static GUIStyle titleColorStyle = new GUIStyle();
        static GUIStyle texturePanelStyle = new GUIStyle();
        static Rect mainwindow = new UnityEngine.Rect(190, 66, width, height);
        static Rect subCatContentWindow = new Rect(15, 15, width - 30, height - 30);
        //Rect mainwindowTitleContainer = new UnityEngine.Rect(190, 66, width, 30);
        //Rect mainwindowMainButtonsContainer = new Rect(190, (66 + 30), width, 45);
        static bool mainCatsInitialized = false;
        static bool visible = false;
        //UIPanel contentPanelLeft = new UIPanel();
        //UIPanel contentPanelRight = new UIPanel();
        static UnityEngine.GUIStyle labelstyle = HighLogic.Skin.label;
        UnityEngine.GUIStyle boxstyle = HighLogic.Skin.box;
        static UnityEngine.GUIStyle textareastyle = HighLogic.Skin.textArea;
        UnityEngine.GUIStyle textfieldstyle = HighLogic.Skin.textField;
        static UnityEngine.GUIStyle togglestyle = HighLogic.Skin.toggle;
            static UnityEngine.GUISkin windowskin = HighLogic.Skin;
            static UnityEngine.GUIStyle buttonstyle = HighLogic.Skin.button;

            // Retrieve as many gui styles from ksp as possible.
            static UnityEngine.GUIStyle windowstyle = HighLogic.Skin.window;



        
            // Button rects
            // Claiming type

            static Rect lblClaimCurrentVesselRect;
            static Rect panelLeftInternalContainer;
            // vessel claiming options
            // label
            static Rect lblClaimingOptionsRect;
            // button options
            static Rect btnClaimingPersonalRect;
            static Rect btnClaimingGroupRect;

            // Vessel access type options
            // label
            static Rect lblClaimingTypeSettings;
            // button options
            static Rect btnPrivateRect;
            static Rect btnPublicRect;
            static Rect btnGroupRect;
            static Rect btnSpectateRect;
            // button submit claim request
            static Rect btnSubmitClaimRequest;
        static Rect lblCurrentClaimingTypeSelectionRect;
        static Rect lblCurrentClaimingVatRect;
        static Rect lblCurrentClaimingSelections;
            // Draw the panel title area
            static Rect labelTitleRect = new Rect(200, 75, 300, 25);
            // Draw the panel background areas
            static Rect panelLeft = new Rect(25, 125, width / 2 - 40, (height - 250));
            static Rect panelRight = new Rect(panelLeft.width + 50, panelLeft.yMin, panelLeft.width, panelLeft.height);
            static Rect panelRightInternalContainer;
            static GUIStyle textLabelstyle = labelstyle;
        #endregion

        #region GUIStyle methods
            internal static GUIStyle ColorTitle()
        {
            //textureTitle.SetPixel(0, 0, Color.white);
            //textureTitle.Apply();
            //titleColorStyle.normal.background = textureTitle;
            titleColorStyle.fontSize = 16;
            titleColorStyle.normal.textColor = Color.white;
            return titleColorStyle;
        }
        internal static GUIStyle ColorPanel()
        {
            texturePanel.SetPixel(0, 0, Color.gray);
            texturePanel.wrapMode = TextureWrapMode.Repeat;
            texturePanel.Apply();
            texturePanelStyle.normal.background = texturePanel;
            texturePanelStyle.fontSize = 16;
            texturePanelStyle.normal.textColor = Color.white;
            
            return texturePanelStyle;
        }
        internal static GUIStyle ColorPanel(Color backgroundColor)
        {
            texturePanel.SetPixel(0, 0, backgroundColor);
            texturePanel.wrapMode = TextureWrapMode.Repeat;
            texturePanel.Apply();
            texturePanelStyle.normal.background = texturePanel;
            texturePanelStyle.fontSize = 16;
            texturePanelStyle.normal.textColor = Color.white;

            return texturePanelStyle;
        }
            #endregion

        #region Draw GUI menus
        static void DrawMainCats()
        {
            GUI.depth = 3;
            buttonstyle.normal.textColor = ColorTitle().normal.textColor;

            buttonstyle.fontSize = 16;
            if (UnityEngine.GUI.Button(new Rect(55, 5, 145, 35), "Permissions", buttonstyle))
            {
                currentWindow = 2;
            }
            if (UnityEngine.GUI.Button(new Rect(225, 5, 145, 35), "Commerce", buttonstyle))
            {
                currentWindow = 3;
            }
            mainCatsInitialized = true;
            DrawSubCats(currentWindow);
        }
        static void DrawSubCats(int windowid)
        {
            GUI.depth = 4;
            buttonstyle.normal.textColor = ColorTitle().normal.textColor;

            switch (windowid)
            {
                case 1:
                    break;
                case 2:
                    //print("permissions mainbutton pressed");
                    buttonstyle.fontSize = 14;
                    if (UnityEngine.GUI.Button(new Rect(15, 45, 125, 25), "Overview", buttonstyle)) { currentSubCatWindow = 2 ; print("Overview button pressed"); }
                    if (UnityEngine.GUI.Button(new Rect(150, 45, 125, 25), "Claiming", buttonstyle)) { currentSubCatWindow = 3; print("Claiming button pressed"); }
                    if (UnityEngine.GUI.Button(new Rect(285, 45, 125, 25), "Usergroups", buttonstyle)) { currentSubCatWindow = 4; print("Usergroups button pressed"); }
                    break;
                case 3:
                    //print("commerce mainbutton pressed");
                    buttonstyle.fontSize = 14;
                    if (UnityEngine.GUI.Button(new Rect(15, 45, 125, 25), "Overview", buttonstyle)) { currentSubCatWindow = 5; print("Overview button pressed"); }
                    if (UnityEngine.GUI.Button(new Rect(150, 45, 125, 25), "Trade", buttonstyle)) { currentSubCatWindow = 6; print("Overview button pressed"); }
                    if (UnityEngine.GUI.Button(new Rect(285, 45, 125, 25), "Trading Stations", buttonstyle)) { currentSubCatWindow = 7; print("Overview button pressed"); }
                    if (UnityEngine.GUI.Button(new Rect(420, 45, 125, 25), "Statistics", buttonstyle)) { currentSubCatWindow = 8; print("Overview button pressed"); }
                    break;
            }
            if(currentSubCatWindow == 3)
            {
                DrawClaimingButtons();
                DrawClaimingOptions();
            }

            //DrawSubCatContents(currentSubCatWindow);
        }
        #endregion

        #region Draw Sub category : Claiming methods
        static public void DrawClaimingButtons()
        {
            // buttons
            DrawPanelBackgrounds();
            // Retrieve the personal or group settings
            if (UnityEngine.GUI.Button(btnClaimingPersonalRect, "Personal", buttonstyle))
            {
                personalOrGroupValue = 1;
            }
            if (UnityEngine.GUI.Button(btnClaimingGroupRect, "Group", buttonstyle))
            {
                personalOrGroupValue = 2;
            }

            // Retrieve vessel access type settings
            if (UnityEngine.GUI.Button(btnPrivateRect, "Private", buttonstyle)) { vesselAccessTypeValue = 1; print("button pressed: Private"); }
            if (UnityEngine.GUI.Button(btnPublicRect, "Public", buttonstyle))
            {
                vesselAccessTypeValue = 2;
            }
            if (UnityEngine.GUI.Button(btnGroupRect, "Group", buttonstyle))
            {
                vesselAccessTypeValue = 3;
            }
            if (UnityEngine.GUI.Button(btnSpectateRect, "Spectate", buttonstyle))
            {
                vesselAccessTypeValue = 4;
            }
            if(UnityEngine.GUI.Button(btnSubmitClaimRequest,"Submit claim request",buttonstyle))
            {
                if (personalOrGroupValue != 0 && vesselAccessTypeValue != 0)
                {
                    ProcessButtonClaimValues(personalOrGroupValue, vesselAccessTypeValue);
                }
                if(FlightGlobals.ActiveVessel.id.ToString() != "" && FlightGlobals.ActiveVessel.id.ToString() != null)
                {
                    string vid = FlightGlobals.ActiveVessel.id.ToString();
                    SyntaxGuiInteraction.VesselClaiming.SaveClaimVesselRequestArguments(vid, groupingOption, vesselAccessibilityOption);
                }
            }
            if (personalOrGroupValue != 0 && vesselAccessTypeValue != 0)
            {
                ProcessButtonClaimValues(personalOrGroupValue, vesselAccessTypeValue);
            }
        }
        static public void DrawClaimingOptions()
        {
            panelLeftInternalContainer = panelLeft;
            panelLeftInternalContainer.yMin += 10;
            panelLeftInternalContainer.xMin += 10;
            panelLeftInternalContainer.width -= 20;
            panelLeftInternalContainer.height -= 20;
            
            // Configure layout for claiming a vessel options
            //GUI.TextArea(panelLeftInternalContainer, "", ColorPanel());
            // Button rects
            // Claiming type

            lblClaimCurrentVesselRect = new Rect(panelLeftInternalContainer.xMin + 15, panelLeftInternalContainer.yMin + 15, 175, 30);

            // vessel claiming options
            // label
            lblClaimingOptionsRect = new Rect(lblClaimCurrentVesselRect.width + 25, panelLeftInternalContainer.yMin + 15, 250, 30);
            // button options
            btnClaimingPersonalRect = new Rect(lblClaimingOptionsRect.xMin, lblClaimingOptionsRect.yMin + 35, 100, 30);
            btnClaimingGroupRect = new Rect(btnClaimingPersonalRect.xMin + 115, btnClaimingPersonalRect.yMin, 100, 30);

            // Vessel access type options
            // label
            lblClaimingTypeSettings = new Rect(lblClaimCurrentVesselRect.width + 25, lblClaimCurrentVesselRect.yMin + 75, 200, 30);
            // button options
            btnPrivateRect = new Rect(lblClaimingTypeSettings.xMin, lblClaimingTypeSettings.yMin + 35, 100, 30);
            btnPublicRect = new Rect(btnPrivateRect.xMin + 115, btnPrivateRect.yMin, 100, 30);
            btnGroupRect = new Rect(lblClaimingTypeSettings.xMin, btnPublicRect.yMin + 35, 100, 30);
            btnSpectateRect = new Rect(btnGroupRect.xMin + 115, btnGroupRect.yMin, 100, 30);
            btnSubmitClaimRequest = new Rect(btnGroupRect.xMin, btnGroupRect.yMin + 35, 200, 30);

            lblCurrentClaimingSelections = new Rect(panelLeftInternalContainer.xMin + 15, panelLeftInternalContainer.yMin + 45, 150, 20);
            lblCurrentClaimingTypeSelectionRect = new Rect(panelLeftInternalContainer.xMin + 15, lblCurrentClaimingSelections.yMin + 40, 175, 20);
            lblCurrentClaimingVatRect = new Rect(panelLeftInternalContainer.xMin + 15, lblCurrentClaimingTypeSelectionRect.yMin + 30, 175, 20);

            // Labels
            //Rect personalOrGroupLabelRect = new Rect(btnClaimingGroupRect.xMin + 15, btnClaimingGroupRect.yMin, 100, 30);
            textLabelstyle.normal.textColor = Color.black;
            textLabelstyle.fontSize = 14;
            GUI.Label(lblClaimingOptionsRect, "Claiming type options:  ", textLabelstyle);
            GUI.Label(lblClaimCurrentVesselRect, "Claim current vessel: ", textLabelstyle);
            GUI.Label(lblClaimingTypeSettings, "Vessel Access type settings: ", textLabelstyle);
            GUI.Label(lblCurrentClaimingSelections, "Current selected claiming values:", textLabelstyle);
            GUI.Label(lblCurrentClaimingTypeSelectionRect,groupingOption,textLabelstyle);
            GUI.Label(lblCurrentClaimingVatRect, vesselAccessibilityOption, textLabelstyle);
            //GUI.Label(new Rect(panelLeftInternalContainer.xMin + 15, panelLeftInternalContainer.yMin + 45, 250, 30), "Vessel Access options:  ", textLabelstyle);

            
        }
        static public void DrawClaimingRightPanel()
        {

            panelRightInternalContainer = panelRight;
            panelRightInternalContainer.yMin += 10;
            panelRightInternalContainer.xMin += 10;
            panelRightInternalContainer.width -= 20;
            panelRightInternalContainer.height -= 20;
            SyntaxSystemsCommon.SyntaxGUIList claimedVesselsList = new SyntaxSystemsCommon.SyntaxGUIList(new string[] { }, Color.gray);
            List<SyntaxSystemsCommon.SyntaxGUIList.SyntaxGUIListItem> claimedvessels = new List<SyntaxSystemsCommon.SyntaxGUIList.SyntaxGUIListItem>();
            
            





        }

        #endregion

        static public void ProcessButtonClaimValues(int personalorgroup, int vesselaccesstype)
        {
            print(string.Format("Selected vessel claiming values: {0} :: {1} ",personalorgroup,vesselaccesstype));
            if(personalorgroup ==1)
            {
                groupingOption = "Personal";
            }
            else if(personalorgroup == 2)
            {
                groupingOption = "Group";
            }

            if (vesselaccesstype == 1)
            {
                vesselAccessibilityOption = "Private";
            }
            else if (vesselaccesstype == 2)
            {
                vesselAccessibilityOption = "Public";
            }
            else if (vesselaccesstype == 3)
            {
                vesselAccessibilityOption = "Group";
            }
            else if (vesselaccesstype == 4)
            {
                vesselAccessibilityOption = "Spectate";
            }
        }

        #region Window methods
        void DrawWindow()
        {
            GUIContent titleContent = new GUIContent("Syntax Systems window", textureTitle);

            windowstyle.fontSize = 16;
            buttonstyle.normal.textColor = ColorTitle().normal.textColor;
            windowstyle.normal.textColor = ColorTitle().normal.textColor;
            labelstyle.normal.textColor = ColorTitle().normal.textColor;
            window = UnityEngine.GUI.Window(currentWindow, mainwindow, Window, "Syntax Systems window", windowstyle);
        }

        static void DrawWindowContents()
        {
            DrawMainCats();
        }
        static void Window(int id)
        {
            GUI.depth = 2;
            switch(id)
            {
                case 1:
                    //DrawMainCats();
                    //DrawSubCats(currentWindow);
                    DrawWindowContents();
                    break;
                case 2: // change window to permission : overview
                    // CreateGUIButtons("Permissions");
                    //DrawSubCats(currentWindow);
                    DrawWindowContents();
                    break;
                case 3: // change window to commerce : overview
                    //CreateGUIButtons("Commerce");
                    //DrawSubCats(currentWindow);
                    DrawWindowContents();
                    break;
                case 4: // sub cat window 2
                    DrawWindowContents();
                    break;
                case 5: // sub cat window 3
                    DrawWindowContents();
                    break;
                case 6:
                    DrawWindowContents();
                    break;
            }
        }
        private void InitializeGUI()
        {
            mainCatsInitialized = false;
            GUI.depth = 1;
            UnityEngine.GUI.contentColor = Color.black;
            

            //UnityEngine.GUISkin skin = HighLogic.Skin.
            if (visible)
            {
                DrawWindow();
            }
            
        }

            // Make a background box
            //UnityEngine.GUI.Box(new Rect(190, 66, width, height), "Loader Menu");
        void OnGUI()
        {

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER | HighLogic.LoadedScene == GameScenes.FLIGHT | HighLogic.LoadedScene == GameScenes.TRACKSTATION)
            {
                if (GameObject.Find("SYNTAX") == null)
                {
                    RegisterSyntaxGUIBtn();
                }
                else
                {
                    GameObject.Find("SYNTAX").SetActive(true);
                }
            }
            if (visible)
            {
                InitializeGUI();
            }
            //CreateMainCatButtons();
            //CreateSubCatButtons("Permissions"); // default window state
        }

        static public void DrawPanelBackgrounds()
        {
            // Draw the panel colors
            //GUI.Box(panelLeft, "", ColorPanel());
            GUI.DrawTexture(panelLeft, texturePanel, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(panelRight, texturePanel, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(new Rect(panelLeftInternalContainer.xMin+10,panelLeftInternalContainer.yMin + 35,130,150), texturePanel, ScaleMode.StretchToFill, true);
        }

        #endregion

        // Onscreen button to enable/disable the window
        public void RegisterSyntaxGUIBtn()
        {
            UnityEngine.GUIStyle buttonstyle = HighLogic.Skin.button;
            buttonstyle.normal.textColor = ColorTitle().normal.textColor;
            buttonstyle.fontSize = 16;
            if (UnityEngine.GUI.Button(new Rect(offsetLeftRight, 0, 75, 35), "SYNTAX", buttonstyle))
            {
                if(!visible)
                {
                    visible = true;
                }
                else if(visible)
                {
                    visible = false;
                }
            }
        }

        static public void DrawSubCatContents(int subCatID)
        {
            GUI.depth = 5;
            labelstyle.normal.textColor = ColorTitle().normal.textColor;
            labelstyle.fontSize = ColorTitle().fontSize;
            //GUI.Label(new Rect(panelLeft.xMin + 15, panelLeft.yMin + 15, 100, 30), "", labelstyle); // PREFAB FOR USAGE ON LABELS PANEL LEFT
            //GUI.Label(new Rect(panelRight.xMin + 15, panelRight.yMin + 15, 100, 30), "", labelstyle); // PREFAB FOR USAGE ON LABELS PANEL LEFT
            // Draw button specific contents
            switch (subCatID)
            {
                case 1:
                    //print("Drawing content for SYNTAX GUI : HOME");
                    string lineOne = "V0.7.2 : Integrated the permissions and commerce commands into the windows today..";
                    string linetwo = "V0.7.1 : GUI INTERGRATION: from this day forward we can now use the SYNTAX GUI! Commands will work as i work on them, pleace be patient.";
                    string linethree = "";
                    string linefour = "";
                    string linefive = "";
                    GUI.Label(labelTitleRect, "SYNTAX GUI HOME", labelstyle);
                    GUI.Label(new Rect(panelLeft.xMin + 15, panelLeft.yMin + 15, 100, 30), "Whats new?", labelstyle);
                    GUI.TextArea(new Rect(panelLeft.xMin + 15, (panelLeft.yMin + 45), panelLeft.width - 30, 200), string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}", lineOne, linetwo, linethree, linefour, linefive), textareastyle);


                    break;
                case 2:
                    //print("Drawing content for subcat: Permissions : Overview");
                    GUI.Label(labelTitleRect, "Permissions : Overview", labelstyle);
                    // What do we want to draw?? options for client?
                    Rect permissionsOverview = panelLeft;
                    permissionsOverview.yMin += 10;
                    permissionsOverview.xMin += 10;
                    permissionsOverview.width -= 20;
                    permissionsOverview.height -= 20;
                    GUI.TextArea(permissionsOverview, "", ColorPanel());

                    textLabelstyle.normal.textColor = Color.black;
                    GUI.Label(new Rect(permissionsOverview.xMin + 15, permissionsOverview.yMin + 15, 300, 30), "Your current permissions: ", textLabelstyle);
                    textLabelstyle.fontSize = 14;
                    GUI.Label(new Rect(permissionsOverview.xMin + 15, permissionsOverview.yMin + 45, 100, 30), "Whats new?", textLabelstyle);
                    GUI.Label(new Rect(permissionsOverview.xMin + 15, permissionsOverview.yMin + 75, 100, 30), "Whats new?", textLabelstyle);
                    GUI.Label(new Rect(permissionsOverview.xMin + 15, permissionsOverview.yMin + 105, 100, 30), "Whats new?", textLabelstyle);


                    break;
                case 3:
                    //print("Drawing content for subcat: Permissions : Claiming");
                    UnityEngine.GUI.Label(labelTitleRect, "Permissions : Claiming", labelstyle);
                    // What do we want to draw?? options for client?



                    //GUI.Label(new Rect(panelLeftInternalContainer.xMin + 15, panelLeftInternalContainer.yMin + 75, 100, 30), "Claim current vessel for group", textLabelstyle);
                    //GUI.Label(new Rect(panelLeftInternalContainer.xMin + 15, panelLeftInternalContainer.yMin + 105, 150, 30), "Unclaim current vessel: ", textLabelstyle);


                    break;
                case 4:
                    print("Drawing content for subcat: Permissions : Usergroups");
                    UnityEngine.GUI.Label(labelTitleRect, "Permissions : Usergroups", labelstyle);
                    // What do we want to draw?? options for client?
                    break;
                case 5:
                    print("Drawing content for subcat: Commerce : Overview");
                    UnityEngine.GUI.Label(labelTitleRect, "Commerce : Overview", labelstyle);
                    // What do we want to draw?? options for client?
                    break;
                case 6:
                    print("Drawing content for subcat: Commerce : Trade");
                    UnityEngine.GUI.Label(labelTitleRect, "Commerce : Trade", labelstyle);
                    // What do we want to draw?? options for client?
                    break;
                case 7:
                    print("Drawing content for subcat: Commerce : Trading Stations");
                    UnityEngine.GUI.Label(labelTitleRect, "Commerce : Trading Stations", labelstyle);
                    // What do we want to draw?? options for client?
                    break;
                case 8:
                    print("Drawing content for subcat: Commerce : Statistics");
                    UnityEngine.GUI.Label(labelTitleRect, "Commerce : Statistics", labelstyle);
                    // What do we want to draw?? options for client?
                    break;
            }

        }
    }


    public static class SyntaxGuiInteraction
    {
        public static class VesselClaiming
        {
            public static string vesselid, vesselClaimingType, vesselAccessType;

            public static void SaveClaimVesselRequestArguments(string vid, string personalOrPublic, string vat)
            {
                vesselid = vid;
                vesselClaimingType = personalOrPublic;
                vesselAccessType = vat;
                Debug.Log("Vessel claim request saved. Calling claiming method." + string.Format(" Vesseldetails: {0},{1},{2}",vesselid,vesselClaimingType,vesselAccessType));
                PermissionSystem.SyntaxPermissionSystem.PermissionClaimGUI(vesselClaimingType, vesselAccessType, vesselid);
                Debug.Log("PermissionSystem: Vessel claim requested.");
            }
        }
    }
}