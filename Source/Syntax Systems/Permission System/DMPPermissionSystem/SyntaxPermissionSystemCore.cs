﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PermissionSystem
{
    public partial class PermissionSystemCore
    {
        static public string universedirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Universe");
        // Contains all the core codes behind the syntax permission system
        public class PlayerSecurity
        {
            //Dictionary<string, string> playercredentials = new Dictionary<string, string>(); // All existing player credentials
            List<PlayerDetails> playercredentials = new List<PlayerDetails>();
            Dictionary<string, List<PlayerVessel>> playerVessels = new Dictionary<string, List<PlayerVessel>>(); // All existing player vessels
            List<PlayerGroup> playerGroups = new List<PlayerGroup>(); // All existing playing groups
            Dictionary<string, List<PlayerVessel>> groupVessels = new Dictionary<string, List<PlayerVessel>>(); // All existing group vessels

            public bool HasGroup(string playername)
            {
                bool flag = false;

                foreach(PlayerDetails pdetails in playercredentials)
                {
                    if(pdetails.PlayerName == playername)
                    {
                        if(pdetails.GroupName != "")
                        {
                            flag = true;
                        }
                    }
                }


                return flag;
            }
            internal bool ExistingVessel(string vesselid)
            {
                bool flag = false;
                
                foreach(string playername in playerVessels.Keys)
                {
                    foreach(PlayerVessel protectedvessel in playerVessels[playername])
                    {
                        if (protectedvessel.VesselID == vesselid)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if(flag)
                    {
                        break;
                    }
                }
                return flag;
            }
            internal bool ExistingVessel(string pname, string vesselid)
            {
                bool flag = false;

                foreach (string playername in playerVessels.Keys)
                {
                    foreach (PlayerVessel protectedvessel in playerVessels[playername])
                    {
                        if (protectedvessel.VesselID == vesselid)
                        {
                            if (protectedvessel.Owner == pname)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                return flag;
            }
            public KeyValuePair<string, PlayerVessel> ReturnPlayerVessel(string playername, string vesselid)
            {
                KeyValuePair<string, PlayerVessel> returndata = new KeyValuePair<string, PlayerVessel>();
                bool entryfound = false;
                foreach(string playerWithVessel in playerVessels.Keys)
                {
                    if(playerWithVessel == playername)
                    {
                        foreach(PlayerVessel playerVesselOwned in playerVessels[playerWithVessel])
                        {
                            if(playerVesselOwned.VesselID == vesselid)
                            {
                                returndata = new KeyValuePair<string, PlayerVessel>(playerWithVessel, playerVesselOwned);
                                entryfound = true;
                                break;
                            }
                        }
                    }
                    if(entryfound)
                    { 
                        break;
                    }
                }

                if(!entryfound)
                {
                    returndata = new KeyValuePair<string, PlayerVessel>();
                }
                return returndata;
            }
            public KeyValuePair<string, PlayerVessel> ReturnLockedPlayerVessel(string vesselid)
            {
                KeyValuePair<string, PlayerVessel> returndata = new KeyValuePair<string, PlayerVessel>();
                bool entryfound = false;
                foreach (string playerWithVessel in playerVessels.Keys)
                {
                    foreach (PlayerVessel playerVesselOwned in playerVessels[playerWithVessel])
                    {
                        if (playerVesselOwned.VesselID == vesselid)
                        {
                            returndata = new KeyValuePair<string, PlayerVessel>(playerWithVessel, playerVesselOwned);
                            entryfound = true;
                            break;
                        }

                    }
                    if (entryfound)
                    {
                        break;
                    }
                }

                if (!entryfound)
                {
                    returndata = new KeyValuePair<string, PlayerVessel>();
                }
                return returndata;
            }
            public PlayerVessel GetPlayerVessel(string playername, string vesselid)
            {
                PlayerVessel pv = new PlayerVessel();
                if(ExistingVessel(playername,vesselid))
                {
                    foreach(PlayerVessel vessel in playerVessels[playername])
                    {
                        if(vessel.VesselID == vesselid)
                        {
                            pv = vessel;
                            break;
                        }
                    }
                }
                return pv;
            }


            public string GetVesselAccessType(string vesselid)
            {
                string accesstype = "";
                bool vesselfound = false;
                foreach(List<PlayerVessel> vesselList in playerVessels.Values)
                {
                    foreach(PlayerVessel vessel in vesselList)
                    {
                        if(vessel.VesselID == vesselid)
                        {
                            accesstype = vessel.AccessType.ToString();
                            vesselfound = true;
                            break;
                        }
                    }
                    if(vesselfound)
                    {
                        break;
                    }
                }
                return accesstype;
            }

            #region Initialization
            // Constructor
            public PlayerSecurity()
            {
                if(DirectoriesAndFilesCheck())
                {
                    init();
                }
                // TODO // How doe sthe locking mechanism work?
                //lock (playercredentials) { };
                //lock (playerVessels) { };
                //lock (playerGroups) { };
                //lock (groupVessels) { };
            }

            /// <summary>
            /// Initializes protection of vessels owned by players with credentials
            /// </summary>
            protected void init()
            {
                RetrieveCredentialsFromFile();
                //DarkMultiPlayerServer.DarkLog.Debug(string.Format("Retrieved {0} clients from credentials file.",playercredentials.Count.ToString()));
                RetrieveProtectedVessels();
                //DarkMultiPlayerServer.DarkLog.Debug(string.Format("Retrieved {0} protected personal vessel lists from protected vessels file.", playerVessels.Count.ToString()));
                ReadGroupsFromFile();
                //DarkMultiPlayerServer.DarkLog.Debug(string.Format("Retrieved {0} groups from usergroups file.", playerGroups.Count.ToString()));
                RetrieveGroupVessels();
                //DarkMultiPlayerServer.DarkLog.Debug(string.Format("Retrieved {0} group protected vessels from group vessels file.", groupVessels.Count.ToString()));
            }

            public bool SaveAll()
            {
                bool flag = false;
                try
                {
                    WriteCredentialsToFile();
                    WriteProtectedVesselsToFile();
                    WriteGroupsToFile();
                    flag = true;
                }
                catch
                {
                    flag = false;
                }
                return flag;
            }
            #endregion

            #region Protection methods
            /// <summary>
            /// Protects the given vessel under the given playername
            /// </summary>
            /// <param name="_playername">player name</param>
            /// <param name="_vesselid">vessel to protect</param>
            public bool ProtectVessel(string _playername, string _vesselid, VesselAccessibilityTypes _at)
            {
                bool flag = false;

                if (playerVessels.ContainsKey(_playername))
                {
                    //DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Player already has protected vessels. Adding vessel " + _vesselid + " to the player vessel list.");
                    bool vesselfound = false;
                    foreach (PlayerVessel vessel in playerVessels[_playername])
                    {
                        if (vessel.VesselID == _vesselid)
                        {
                            DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Can't add vessel to player vessel list because it has already been claimed.");
                            vesselfound = true;
                            break;
                        }
                    }
                    if (!vesselfound)
                    {
                        if (!CheckVesselIsClaimed(_vesselid))
                        {
                            //DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Adding vessel to player vessel list.");
                            playerVessels[_playername].Add(new PlayerVessel(_vesselid, _at,_playername));
                            flag = true;
                            WriteProtectedVesselsToFile();
                            DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Vessel added. Section 1");
                            return flag;
                        }
                    }
                }
                else
                {
                    //DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Player doesn't have any protected vessels yet, adding player to the player vessel list.");
                    if (CheckPlayerIsProtected(_playername))
                    {
                        if (!CheckVesselIsClaimed(_vesselid))
                        {
                            DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Vessel has not been claimed yet, claiming(adding) it for the player.");
                            playerVessels.Add(_playername, new List<PlayerVessel>() { new PlayerVessel(_vesselid, _at,_playername) });
                            flag = true;
                            WriteProtectedVesselsToFile();
                            DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Vessel added. Section 2");
                            return flag;
                        }
                        else
                        {

                            //DarkMultiPlayerServer.DarkLog.Normal("Syntax Codes - Vessel adding failed because the vessel has already been claimed by someone else.");
                        }
                    }
                }
                return flag;
            }

            /// <summary>
            /// Claims a vessel for the group (adds to groupvessel list) the player is part of, then removes the vessel from the playervessels list.
            /// </summary>
            /// <param name="pname">Playername</param>
            /// <param name="_gname">Player groupname</param>
            /// <param name="_vesselid">VesselID</param>
            /// <param name="_at">Chosen Vessel Accesstype</param>
            /// <returns>Succesful or not</returns>
            public bool ProtectGroupVessel(string pname, string _gname, string _vesselid, VesselAccessibilityTypes _at)
            {
                bool flag = false;
                bool vesselalreadyclaimed = false;
                // first check if the vessel is already claimed by a group
                //DarkMultiPlayerServer.DarkLog.Debug("Entering claiming for vessel method, looping through existing vessels. SyntaxPermissionSystem Code 1.");
                foreach (string group in groupVessels.Keys)
                {
                    foreach (PlayerVessel vessel in groupVessels[group])
                    {
                        if (vessel.VesselID == _vesselid)
                        {
                            vesselalreadyclaimed = true;
                            break;
                        }
                    }

                    if (vesselalreadyclaimed)
                    {
                        break;
                    }
                }

                //DarkMultiPlayerServer.DarkLog.Debug("Entering claiming for vessel method. SyntaxPermissionSystem Code 5.");
                // Determine if the vessel is already claimed and if not it will protect it as requested
                if (!vesselalreadyclaimed)
                {
                    if(!CheckPlayerIsProtected(pname))
                    {
                        SaveCredentials(pname);
                    }
                    PlayerVessel vesselobject = new PlayerVessel(_vesselid, _at, pname) { GroupName = _gname };
                    //DarkMultiPlayerServer.DarkLog.Debug("Entering claiming for vessel method. SyntaxPermissionSystem Code 6.");
                    if(!groupVessels.ContainsKey(_gname))
                    {
                        groupVessels.Add(_gname, new List<PlayerVessel>());
                    }
                    groupVessels[_gname].Add(vesselobject);
                    if(playerVessels.ContainsKey(pname))
                    {
                        playerVessels[pname].Remove(vesselobject);
                    }
                    flag = true;

                    WriteGroupVesselsToFile();
                }
                else
                {
                    //DarkMultiPlayerServer.DarkLog.Debug("Exiting claiming for vessel method. SyntaxPermissionSystem Code 9.");
                    flag = false; // Can't claim the vessel because it has already been claimed by either someone not part of the same group or another group.
                }

                return flag;
            }

            public bool RemoveVesselFromProtection(string pname, string vesselguid)
            {
                bool flag = false;
                if(CheckPlayerIsProtected(pname) && playerVessels.ContainsKey(pname))
                {
                    if(ExistingVessel(pname,vesselguid))
                    {
                        playerVessels[pname].Remove(GetPlayerVessel(pname, vesselguid));
                        DarkMultiPlayerServer.DarkLog.Debug("Permission System: Handling UnClaim request: playervessel has been removed.");
                        WriteProtectedVesselsToFile(); // Rewrite all vessels to file to refresh the file backup
                        flag = true;
                    }
                }
                return flag;
            }

            public bool RemoveVesselFromGroup(string _pname, string _gname, string _vesselid)
            {
                bool flag = false;

                foreach (PlayerDetails pdetails in playercredentials)
                {
                    bool vesselremoved = false;
                    if (pdetails.PlayerName == _pname)
                    {
                        foreach (PlayerVessel vessel in groupVessels[_gname])
                        {
                            if (vessel.VesselID == _vesselid && vessel.Owner == _pname)
                            {
                                groupVessels[_gname].Remove(vessel);
                                vesselremoved = true;
                                break;
                            }
                        }
                    }
                    if (vesselremoved)
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }

            public bool FindPlayerVessel(string _pname, string _vesselid, out PlayerVessel vesselfound)
            {
                bool flag = false;
                PlayerVessel pv = new PlayerVessel();

                if (CheckPlayerIsProtected(_pname))
                {
                    foreach (PlayerVessel vessel in playerVessels[_pname])
                    {
                        if (vessel.VesselID == _vesselid)
                        {
                            pv = vessel;
                            flag = true;
                            break;
                        }
                    }
                }

                vesselfound = pv;
                return flag;
            }

            /// <summary>
            /// Change the accessibility of a specific personal vessel
            /// </summary>
            /// <param name="_pname">playername</param>
            /// <param name="_vesselid">vesselid</param>
            /// <param name="_at">Access Type to change to</param>
            /// <returns></returns>
            public bool ChangeAccessibility(string _pname, string _vesselid, VesselAccessibilityTypes _at)
            {
                bool flag = false;
                if (CheckPlayerIsProtected(_pname))
                {
                    foreach (string vesselowner in playerVessels.Keys)
                    {
                        foreach (PlayerVessel vessel in playerVessels[vesselowner])
                        {
                            if (vessel.VesselID == _vesselid)
                            {
                                vessel.AccessType = _at;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                }
                return flag;
            }
            /// <summary>
            /// Change the accessibility of a specific group vessel
            /// </summary>
            /// <param name="_pname">vessel owner</param>
            /// <param name="_gname">vesselowner group</param>
            /// <param name="_vesselid">vesselid</param>
            /// <param name="_at">Access type to change to</param>
            /// <returns></returns>
            public bool ChangeAccessibility(string _pname, string _gname, string _vesselid, VesselAccessibilityTypes _at)
            {
                bool flag = false;
                string groupname;
                if (CheckPlayerIsProtected(_pname, out groupname))
                {
                    if (groupname == _gname)
                    {
                        foreach (PlayerVessel vessel in groupVessels[groupname])
                        {
                            if (vessel.VesselID == _vesselid)
                            {
                                vessel.AccessType = _at;
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                return flag;
            }

            /// <summary>
            /// Saves the player credentials to the internal list
            /// </summary>
            /// <param name="_playername">Player name to add</param>
            /// <param name="_pass">Player password to add</param>
            public void SaveCredentials(string _playername)
            {
                bool flag = false;
                foreach (PlayerDetails pdetails in playercredentials)
                {
                    if (pdetails.PlayerName == _playername)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    playercredentials.Add(new PlayerDetails(_playername, ""));
                }
                WriteCredentialsToFile();
            }
            #endregion

            #region File read/write methods

            #region Directory control
            public void InitialDirectoryAndFileCreation()
            {
                DirectoriesAndFilesCheck();
            }
            protected bool DirectoriesAndFilesCheck()
            {
                bool flag = false;
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                if (!Directory.Exists(Path.Combine(universedirectory, syntaxdirectory)))
                {
                    Directory.CreateDirectory(Path.Combine(universedirectory, syntaxdirectory));
                    if (!File.Exists(Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "playercredentials.txt"))))
                    {
                        File.CreateText(Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "playercredentials.txt")));
                    }
                    if (!File.Exists(Path.Combine(universedirectory, Path.Combine(syntaxdirectory , "protectedvessels.txt"))))
                    {
                        File.CreateText(Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "protectedvessels.txt")));
                    }
                    if (!File.Exists(Path.Combine(universedirectory, Path.Combine(syntaxdirectory , "groupvessels.txt"))))
                    {
                        File.CreateText(Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "groupvessels.txt")));
                    }
                    if (!File.Exists(Path.Combine(universedirectory, Path.Combine(syntaxdirectory , "groups.txt"))))
                    {
                        File.CreateText(Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "groups.txt")));
                    }
                }
                else
                {
                    flag = true;
                }
                return flag;
            }
            #endregion

            #region Credentials
            /// <summary>
            /// Retrieves user credentials from saved serverfile
            /// Format: username
            /// </summary>
            protected void RetrieveCredentialsFromFile()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "playercredentials.txt")); // DMP server subfolder
                StreamReader sr = new StreamReader(filepath);
                DirectoriesAndFilesCheck();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != "" && line != null)
                    {
                        string[] credentials = line.Split(',');

                        if (!CheckPlayerIsProtected(credentials[0]))
                        {
                            if (credentials.Count() > 1)
                            {
                                playercredentials.Add(new PlayerDetails(credentials[0], credentials[1]));
                            }
                            else
                            {
                                playercredentials.Add(new PlayerDetails(credentials[0], ""));
                            }
                        }
                    }
                }
                sr.Close();
            }

            protected void WriteCredentialsToFile()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "playercredentials.txt"));
                StreamWriter sw = new StreamWriter(filepath);
                DirectoriesAndFilesCheck();
                foreach (PlayerDetails pdetails in playercredentials)
                {
                    sw.WriteLine(pdetails.ToString());
                }
                sw.Close();
            }
            #endregion

            #region Vessels
            /// <summary>
            /// Retrieves user protected vessels
            /// Format: username$vesselid1$vesselid2$vesselid3, ..
            /// </summary>
            protected void RetrieveProtectedVessels()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "protectedvessels.txt")); // DMP server subfolder
                StreamReader sr = new StreamReader(filepath);
                DirectoriesAndFilesCheck();
                while (!sr.EndOfStream)
                {
                    // Read and seperate playername and protected vessels
                    string line = sr.ReadLine();

                    if (line != "" && line != null)
                    {
                        string[] playervessels = line.Split(',');
                        string playername = "";
                        // Retrieve player specific vessels
                        List<PlayerVessel> foundPlayerVessels = new List<PlayerVessel>();

                        #region Retrieve player vessels
                        foreach (string vesselDetails in playervessels)
                        {
                            if (vesselDetails != "" && vesselDetails != null)
                            {
                                // Split each vessel into seperate string
                                string[] vesselparts = vesselDetails.Split('#');
                                DarkMultiPlayerServer.DarkLog.Debug("vesselparts loading: " + vesselparts[0] + vesselparts[1]);
                                playername = vesselparts[0];
                                string[] vesseldata = vesselparts[1].Split('$');
                                DarkMultiPlayerServer.DarkLog.Debug("vesseldata loading: " + vesselparts[0] + vesselparts[1]);
                                //foreach (string part in vesseldata)
                                //{
                                //    DarkMultiPlayerServer.DarkLog.Debug("QUICK DEBUG : " + part);
                                //}
                                string vesselid = "";
                                VesselAccessibilityTypes vesseltype = VesselAccessibilityTypes.Spectate;
                                if (vesseldata.Count() > 2)
                                {
                                    vesselid = vesseldata[1];
                                    vesseltype = (VesselAccessibilityTypes)Enum.Parse(typeof(VesselAccessibilityTypes), vesseldata[2]);
                                    DarkMultiPlayerServer.DarkLog.Debug("vesseldata loading: " + vesselid + " :: " + vesseltype);
                                }
                                else if (vesseldata.Count() == 2)
                                {
                                    vesselid = vesseldata[0];
                                    vesseltype = (VesselAccessibilityTypes)Enum.Parse(typeof(VesselAccessibilityTypes), vesseldata[1]);
                                    DarkMultiPlayerServer.DarkLog.Debug("vesseldata loading: " + vesselid + " :: " + vesseltype);
                                }
                                // Add the vessel to the list under the user
                                PlayerVessel retrievedVessel = new PlayerVessel(vesselid, vesseltype, playername);
                                DarkMultiPlayerServer.DarkLog.Debug("Vessel object created: " + retrievedVessel.Owner + " :: " + retrievedVessel.VesselID + " :: " + retrievedVessel.AccessType);

                                bool vesselAlreadyClaimed = false;
                                foreach (string pname in playerVessels.Keys)
                                {
                                    if (playerVessels[pname].Contains(retrievedVessel))
                                    {
                                        vesselAlreadyClaimed = true;
                                        break;
                                    }
                                }
                                if (!vesselAlreadyClaimed)
                                {
                                    if (!playerVessels.ContainsKey(playername))
                                    {
                                        foundPlayerVessels.Add(retrievedVessel);
                                        DarkMultiPlayerServer.DarkLog.Debug("Vessel found and added to internal library: " + retrievedVessel.Owner + " :: " + retrievedVessel.VesselID + " :: " + retrievedVessel.AccessType);
                                    }
                                }
                                else
                                {
                                    DarkMultiPlayerServer.DarkLog.Debug("Vessel found. Errorcode: Vessel already claimed in internal library: " + retrievedVessel.Owner + " :: " + retrievedVessel.VesselID + " :: " + retrievedVessel.AccessType);
                                }

                            }
                        #endregion

                            // Add the player vessel list to the register
                            // No check for existance of player because method is used upon initialization of the server
                            if (!playerVessels.ContainsKey(playername))
                            {
                                playerVessels.Add(playername, foundPlayerVessels);
                            }
                            else if (playerVessels[playername] != foundPlayerVessels)
                            {
                                playerVessels[playername] = foundPlayerVessels;
                            }

                        }
                    }
                }
                sr.Close();
            }
            protected void WriteProtectedVesselsToFile()
            {
                //DarkMultiPlayerServer.DarkLog.Normal("SyntaxCodes: Protected vessels - Writing vessels to file..");
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "protectedvessels.txt"));
                StreamWriter sw = new StreamWriter(filepath);
                DirectoriesAndFilesCheck();
                foreach (string playerWithProtectedVessels in playerVessels.Keys)
                {
                    //DarkMultiPlayerServer.DarkLog.Normal("SyntaxCodes: Player with protected vessel: " + playerWithProtectedVessels);
                    string playervesselsLine = "";
                    foreach (PlayerVessel vessel in playerVessels[playerWithProtectedVessels])
                    {
                        //DarkMultiPlayerServer.DarkLog.Normal("SyntaxCodes: Playervessel: Owner: " + vessel.Owner + ", vesselid: " + vessel.VesselID);
                        if (playerVessels[playerWithProtectedVessels].IndexOf(vessel) == playerVessels[playerWithProtectedVessels].Count)
                        {
                            playervesselsLine += vessel.ToString();
                        }
                        else
                        {
                            playervesselsLine += vessel.ToString() + ",";
                        }

                        //DarkMultiPlayerServer.DarkLog.Normal("SyntaxCodes: Added playervessel " + vessel.VesselID + " to player vessellist.");
                    }
                    sw.WriteLine(playervesselsLine);
                    //DarkMultiPlayerServer.DarkLog.Normal("SyntaxCodes: Saved player vessels to file.");
                }
                //DarkMultiPlayerServer.DarkLog.Normal("SyntaxCodes: Saved all players with protected vessels to file.");
                sw.Close();
            }

            protected void RetrieveGroupVessels()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "groupvessels.txt"));
                StreamReader sr = new StreamReader(filepath);
                DirectoriesAndFilesCheck();
                while(!sr.EndOfStream)
                {
                    string readLine = sr.ReadLine();

                    if(readLine != "" && readLine != null)
                    {
                        //DarkMultiPlayerServer.DarkLog.Debug("Retrieving vessel line..");
                        // split groupname from vesseldata
                        string[] vessels = readLine.Split(',');
                        foreach (string vessel in vessels)
                        {
                            if (vessel.Length > 1)
                            {
                                string[] vesselDetails = vessel.Split('#');
                                string groupname = vesselDetails[0];
                                //DarkMultiPlayerServer.DarkLog.Debug("Retrieving vessel line vessel details..");
                                // split vesseldetails and reuse container
                                string[] vesselDetails2 = vesselDetails[1].Split('$');
                                string ownername = vesselDetails2[0];
                                string vesselguid = vesselDetails2[1];
                                string vesselAccesstype = vesselDetails2[2];

                                //DarkMultiPlayerServer.DarkLog.Debug("Done assigning vessel details.");
                                if (!groupVessels.Keys.Contains(groupname))
                                {
                                    //DarkMultiPlayerServer.DarkLog.Debug("group doesn't exist yet , so adding it and the vessel to it.");
                                    groupVessels.Add(groupname, new List<PlayerVessel>() { new PlayerVessel(vesselguid, (VesselAccessibilityTypes)Enum.Parse(typeof(VesselAccessibilityTypes), vesselAccesstype), ownername) { GroupName = groupname } });
                                    //DarkMultiPlayerServer.DarkLog.Debug("Retrieved vessel for non-existing group. Created group in the process.");
                                }
                                else
                                {
                                    //DarkMultiPlayerServer.DarkLog.Debug("group exists , so adding it and the vessel to it.");
                                    groupVessels[groupname].Add(new PlayerVessel(vesselguid, (VesselAccessibilityTypes)Enum.Parse(typeof(VesselAccessibilityTypes), vesselAccesstype), ownername) { GroupName = groupname });
                                    //DarkMultiPlayerServer.DarkLog.Debug("Retrieved vessel for existing group.");
                                }
                            }
                        }
                    }
                }

            }

            protected void WriteGroupVesselsToFile()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "groupvessels.txt"));
                StreamWriter sw = new StreamWriter(filepath);
                DirectoriesAndFilesCheck();
                foreach(string groupname in groupVessels.Keys)
                {
                    foreach(PlayerVessel groupvessel in groupVessels[groupname])
                    {
                        string newline = string.Format("{0}#{1}",groupname,groupvessel.ToString());
                        sw.WriteLine(newline);
                    }
                }
                //DarkMultiPlayerServer.DarkLog.Debug("Permission System: Groupvessels written to file.");
                sw.Close();
            }

            #endregion

            #region Groups
            /// <summary>
            /// Retrieves user groups
            /// Format: groupadmin,groupname,groupvesselaccesstype
            /// </summary>
            protected void WriteGroupsToFile()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "groups.txt"));
                StreamWriter sw = new StreamWriter(filepath);
                DirectoriesAndFilesCheck();
                foreach (PlayerGroup group in playerGroups)
                {
                    sw.WriteLine(group.ToString());
                }
                sw.Close();
                //DarkMultiPlayerServer.DarkLog.Debug("Writing groups to file..");
            }
            protected void ReadGroupsFromFile()
            {
                string syntaxdirectory = Path.Combine("SyntaxPlugins" , "PermissionSystem");
                string filepath = Path.Combine(universedirectory, Path.Combine(syntaxdirectory, "groups.txt"));
                StreamReader sr = new StreamReader(filepath);
                DirectoriesAndFilesCheck();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line != "" && line != null)
                    {
                        string[] groupinfo = line.Split(',');
                        playerGroups.Add(new PlayerGroup(groupinfo[1], groupinfo[0], (VesselAccessibilityTypes)Enum.Parse(typeof(VesselAccessibilityTypes), groupinfo[2])));
                    }
                }
                sr.Close();

            }
            #endregion

            #endregion

            #region Existance checks
            /// <summary>
            /// Checks wether a player exists in the server player protection list
            /// </summary>
            /// <param name="_playername">The playername to check</param>
            /// <returns>True or false depending on existance of player in protection list</returns>
            public bool CheckPlayerIsProtected(string _playername)
            {
                bool flag = false;
                foreach (PlayerDetails pdetails in playercredentials)
                {
                    if (pdetails.PlayerName == _playername)
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            public bool CheckPlayerIsProtected(string _playername, out string _gname)
            {
                bool flag = false;
                string playergroup = "";
                foreach (PlayerDetails pdetails in playercredentials)
                {
                    if (pdetails.PlayerName == _playername)
                    {
                        playergroup = pdetails.GroupName;
                        flag = true;
                        break;
                    }
                }
                _gname = playergroup;
                return flag;
            }

            /// <summary>
            /// Checks wether a vessel has already been claimed
            /// </summary>
            /// <param name="_vesselid">The vessel to check</param>
            /// <returns>True or false depending on wether vessel has been claimed by a protected player</returns>
            public bool CheckVesselIsClaimed(string _vesselid)
            {
                bool flag = false;

                foreach (List<PlayerVessel> claimedVessels in playerVessels.Values)
                {
                    bool vesselIsClaimed = false;
                    foreach (PlayerVessel vessel in claimedVessels)
                    {
                        if (vessel.VesselID == _vesselid)
                        {
                            vesselIsClaimed = true;
                            break;
                        }
                    }
                    if (vesselIsClaimed)
                    {
                        flag = true;
                        break;
                    }
                }

                return flag;
            }
            public bool CheckVesselIsClaimed(string _vesselid, out string ownername)
            {
                bool flag = false;
                string vesselOwner = "";
                foreach (List<PlayerVessel> claimedVessels in playerVessels.Values)
                {
                    bool vesselIsClaimed = false;
                    foreach (PlayerVessel vessel in claimedVessels)
                    {
                        if (vessel.VesselID == _vesselid)
                        {
                            vesselIsClaimed = true;
                            vesselOwner = vessel.Owner;
                            break;
                        }
                    }
                    if (vesselIsClaimed)
                    {
                        flag = true;
                        break;
                    }
                }
                ownername = vesselOwner;
                return flag;
            }

            #endregion

            #region Group methods
            public bool AddPlayerGroup(string _groupName, VesselAccessibilityTypes _groupVesselAccessType, string _groupAdmin)
            {
                bool flag = false;
                bool groupExists = false;

                foreach (PlayerGroup pgroup in playerGroups)
                {
                    if (pgroup.GroupName == _groupName)
                    {
                        groupExists = true;
                        break;
                    }
                }

                if (!groupExists)
                {
                    playerGroups.Add(new PlayerGroup(_groupName, _groupAdmin, _groupVesselAccessType));
                    flag = true;
                }

                return flag;
            }

            public bool EditPlayerGroupVesselAccess(string _groupName, string _groupAdmin, VesselAccessibilityTypes _groupVesselAccessType)
            {
                bool flag = false;

                foreach (PlayerGroup pgroup in playerGroups)
                {
                    if (pgroup.GroupName == _groupName)
                    {
                        if (pgroup.GroupAdmin == _groupAdmin)
                        {
                            pgroup.VesselAccessType = _groupVesselAccessType;
                            flag = true;
                            break;
                        }
                    }
                }

                return flag;
            }

            public bool InvitePlayerToGroup(string _groupName, string _inviter, string _invited)
            {
                PlayerDetails invitedplayer = null;
                bool flag = false;
                foreach (PlayerDetails pdetails in playercredentials)
                {
                    if (pdetails.PlayerName == _invited)
                    {
                        invitedplayer = pdetails;
                        break;
                    }
                }
                if (invitedplayer != null)
                {
                    foreach (PlayerDetails pdetails2 in playercredentials)
                    {
                        if (pdetails2.PlayerName == _inviter)
                        {
                            playercredentials[playercredentials.IndexOf(invitedplayer)].GroupName = pdetails2.GroupName;
                            flag = true;
                            break;
                        }
                    }
                }
                return flag;
            }

            public bool CheckPlayerGroup(string _pname, out string _gname)
            {
                string groupname = "";
                bool flag = false;

                foreach (PlayerDetails pdetails in playercredentials)
                {
                    if (pdetails.PlayerName == _pname)
                    {
                        groupname = pdetails.GroupName;
                        flag = true;
                        break;
                    }
                }


                _gname = groupname;
                return flag;
            }
            #endregion
        }
        public class PlayerVessel
        {
            string ownername;
            string vesselid;
            string groupname;
            VesselAccessibilityTypes accessType;

            public PlayerVessel()
            {

            }
            public PlayerVessel(string _vid, VesselAccessibilityTypes _at, string owner)
            {
                vesselid = _vid;
                accessType = _at;
                ownername = owner;
            }
            public string VesselID
            {
                get { return vesselid; }
            }
            public VesselAccessibilityTypes AccessType
            {
                get { return accessType; }
                set { accessType = value; }
            }
            public string GroupName
            {
                get { return groupname; }
                set { groupname = value; }
            }
            public string Owner
            {
                get { return ownername; }
            }

            /// <summary>
            /// Returns Vessel in format: vesselid$groupname$accesstype
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("{0}#{1}${2}${3}", ownername, groupname, vesselid, accessType);
            }
        }

        private class PlayerGroup
        {
            string groupname;
            string groupadmin;
            VesselAccessibilityTypes vesselAccessType;
            //int membercount; extra feature for lateron

            internal PlayerGroup(string _gname, string _gadmin, VesselAccessibilityTypes _vat)
            {
                groupname = _gname;
                groupadmin = _gadmin;
                vesselAccessType = _vat;
            }

            #region Properties
            public string GroupName
            {
                get { return groupname; }
            }
            public string GroupAdmin
            {
                get { return groupadmin; }
                set { groupadmin = value; }
            }
            public VesselAccessibilityTypes VesselAccessType
            {
                get { return vesselAccessType; }
                set { vesselAccessType = value; }
            }
            #endregion

            public override string ToString()
            {
                return string.Format("{0},{1},{2}", groupadmin, groupname, vesselAccessType);
            }
        }

        private class PlayerDetails
        {
            string playername, groupname;

            internal PlayerDetails(string _pname, string _gname)
            {
                playername = _pname;
                groupname = _gname;
            }

            #region Properties
            public string PlayerName
            {
                get { return playername; }
            }
            public string GroupName
            {
                get { return groupname; }
                set { groupname = value; }
            }
            #endregion

            public override string ToString()
            {
                return string.Format("{0},{1}", playername, groupname);
            }
        }
        public enum VesselAccessibilityTypes
        {
           Private, Spectate, Public, Group
        }
    }
}

