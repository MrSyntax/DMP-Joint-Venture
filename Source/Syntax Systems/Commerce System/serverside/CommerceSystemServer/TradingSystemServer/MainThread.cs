using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TradingSystemServer.BankingSystem;
using TradingSystemServer.TradingSystem;


namespace CommerceSystem.Server
{
    /// <summary>
    /// Main thread for the trading system serverside. Runs alongside DMP - JV Servercode.
    /// </summary>
    public partial class MainThread
    {

        #region Base values

        // Initialize the main classes and values for the Trade system mainthread
        BankSystem banksystem = new BankSystem();
        TradeSystemCore tradesystemcore = new TradeSystemCore();
        public bool isInitialized { get; set; }
        public bool isAcceptingRequests { get; set; }
        public bool isReceivingRequest { get; set; }
        public string[] GetSystemRequests { get; set; }

        #endregion

        #region Directory and file locations

        static string universeDir = DarkMultiPlayerServer.Server.universeDirectory;
        static string syntaxCommerceBaseDir = Path.Combine("SyntaxPlugins", "Commerce");
        static string filepathBaseDir = Path.Combine(universeDir, syntaxCommerceBaseDir);
        static string tradingFilePathDir = Path.Combine(filepathBaseDir, "Trading");
        static string bankingFilePathDir = Path.Combine(filepathBaseDir, "Banking");
        static string userdataFilePathDir = Path.Combine(filepathBaseDir, "Userdata");


        #endregion

        string[] ConfigFiles = new string[10];

        #region Class manipulation methods

        /// <summary>
        /// Initializes the commerce system if it hasn't been initialized already
        /// </summary>
        public MainThread()
        {
            if(!isInitialized)
            {
                DarkMultiPlayerServer.DarkLog.Debug("Trading System Server: Initializing Commerce System");
                if (Init())
                {
                    DarkMultiPlayerServer.DarkLog.Debug("Trading System Server: Initialized Commerce System");
                    isInitialized = true;
                }
                else
                {
                    DarkMultiPlayerServer.DarkLog.Debug("Trading System Server: Failed to initialize Commerce System. Section One");
                    return;
                }
            }
        }

        #region Properties

        /// <summary>
        /// Set accepting connections to true or false.
        /// </summary>
        public bool StartAcceptingConnections
        {
            set
            {
                isAcceptingRequests = value;
            }
        }

        /// <summary>
        /// Returns the internal Banking System core from the mainthread.
        /// </summary>
        public BankSystem GetInternalBankingSystem
        {
            get { return banksystem; }
        }

        /// <summary>
        /// Returns the internal Trading System core from the mainthread.
        /// </summary>
        public TradeSystemCore GetInternalTradingSystem
        {
            get { return tradesystemcore; }
        }

        #endregion

        #endregion

        private bool Init()
        {
            bool flag = false;

            // Upon initialization, first check wether the directories and files exist
            DarkMultiPlayerServer.DarkLog.Debug("Trading System Server: Checking file directories..");
            try
            {
                if (CheckDirectoriesAndFiles())
                {
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system server: Directories and files check passed!");
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system server: Reading directories and files..");

                    flag = true;
                }
                else
                {
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system server: Directories and files did not exist, thus were automatically created.");
                    flag = true;
                }
            }
            catch(Exception ex)
            {
                DarkMultiPlayerServer.DarkLog.Debug("Commerce system server: Directories and files check passed failed! Directory and File retrieval and writing failed!");
                DarkMultiPlayerServer.DarkLog.Debug("Directory and File retrieval and writing failed!");
                DarkMultiPlayerServer.DarkLog.Debug("Errorcode: " + ex.ToString());
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// Retrieves data for the specified system. Possible parameter values:
        /// 1 = Trading system
        /// 2 = Banking system
        /// 3 = Userdata storage
        /// 4 = Commerce system config
        /// </summary>
        /// <param name="SYSTEMID">The system to retrieve from file</param>
        /// <returns>Wether retrieval was succesful or not</returns>
        private bool RetrieveData(int SYSTEMID)
        {
            bool flag = false;
            StreamReader reader;
            string _filepath = "";

            #region Filepath switch
            switch (SYSTEMID)
            {
                case 1:
                    _filepath = tradingFilePathDir;
                    break;
                case 2:
                    _filepath = bankingFilePathDir;
                    break;
                case 3:
                    _filepath = userdataFilePathDir;
                    RetrieveCommerceUserData();
                    break;
                case 4:
                    _filepath = filepathBaseDir;
                    break;
                default:
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system: Data retrieval: Invalid SYSTEMID entered.");
                    return false;
            }
            #endregion

            // TODO //
            //
            // foreach file within each of the given directories
            //
            // END OF TODO //
            if(_filepath != null && _filepath != "")
            {
                reader = new StreamReader(_filepath);
                try
                {
                    List<string> readlines = new List<string>();
                    DarkMultiPlayerServer.DarkLog.Debug("Retrieving datalines from file..");
                    while(!reader.EndOfStream)
                    {
                        readlines.Add(reader.ReadLine());
                    }
                    DarkMultiPlayerServer.DarkLog.Debug("Interpreting read datalines from file..");
                    if (ReadSystemDataLines(SYSTEMID,readlines.ToArray())) // Interpret the read datalines..
                    {
                        flag = true; // Data has been read from file and inserted into the system without failures/errors.
                    }
                    else
                    {
                        flag = false; // The Data to retrieve and insert caused failures/errors.
                    }
                }
                catch(Exception ex)
                {
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system: Data retrieval: Failed to read data from file!");
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system: Data retrieval: Errorcode: " + ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
            return flag;
        }

        /// <summary>
        /// Returns the content of the valid datafiles and calls the processing method for any config files inbetween the datafiles
        /// </summary>
        /// <param name="SYSTEMID">ID of the systemfiles to read</param>
        /// <param name="filenames">The systemfiles to read</param>
        /// <returns></returns>
        private Dictionary<string,List<string>> RetrieveSystemFileData(string[] filenames)
        {
            string configExt = "cfg";
            string dataExt = "txt";
            Dictionary<string, List<string>> tempFileList = new Dictionary<string, List<string>>();

            foreach (string file in filenames)
            {
                if (file.EndsWith(configExt))
                {
                    // Call config processing method to process the config file settings

                    if(ProcessConfigFile(file))
                    {
                        DarkMultiPlayerServer.DarkLog.Debug("Syntax Systems: Config file processed successfully!");
                    }
                    else
                    {
                        DarkMultiPlayerServer.DarkLog.Debug("Syntax Systems: FAILED to process config file. Filename: " + file.ToString());
                    }
                }
                else if (file.EndsWith(dataExt))
                {
                    // Process the file and
                    // Add it to the return list to let the interpreter read and add the data to their respective internal lists
                    StreamReader sr = new StreamReader(file);
                    tempFileList.Add(file, new List<string>());
                    while(!sr.EndOfStream)
                    {
                        tempFileList[file].Add(sr.ReadLine());
                    }
                }
            }
            return tempFileList;
        }

        // TODO
        // Reads the config files
        private bool ProcessConfigFile(string configFileName)
        {
            bool flag = false;
            try
            {

            }
            catch(Exception ex)
            {
                DarkMultiPlayerServer.DarkLog.Debug("Syntax Systems: Failed proccess errorcode: " + ex.ToString());
                return false;
            }

            return flag;
        }

        private void GetFiles()
        {
            // First get all the files within the given system directories
            string[] mainDirFiles = Directory.GetFiles(filepathBaseDir);
            string[] bankingDirFiles = Directory.GetFiles(bankingFilePathDir);
            string[] tradingDirFiles = Directory.GetFiles(tradingFilePathDir);

            // Then retrieve all the data from each file
            // Format: filename, file contents
            Dictionary<string, List<string>> mainSystemData = RetrieveSystemFileData(mainDirFiles);
            Dictionary<string, List<string>> bankingSystemData = RetrieveSystemFileData(bankingDirFiles);
            Dictionary<string, List<string>> tradingSystemData = RetrieveSystemFileData(tradingDirFiles);

            ProcessSystemData(1, tradingSystemData);
            ProcessSystemData(2, bankingSystemData);
            ProcessSystemData(4, mainSystemData);




        }

        // Process system file contents into the internal active library
        private void ProcessSystemData(int _SYSTEMID, Dictionary<string, List<string>> systemData)
        {
            Dictionary<string, List<string>> tempSwitchList = new Dictionary<string, List<string>>();

            // Read every retrieved line for each retrieved file
            foreach (string filename in systemData.Keys)
            {
                foreach (string filecontent in systemData[filename])
                {
                    if(filecontent != null && filecontent != "")
                    {
                        if(filecontent == "{" || filecontent == "}" || filecontent[0] == '#')
                        {
                            // Do nothing because it's the start or end of a section in the users' file
                            // Or it's a comment in the file itself
                        }
                        //else if()
                        //{
                            // Process the data for the given system

                            switch(_SYSTEMID)
                            {
                                case 1: // Process the data for the trading system
                                    

                                    break;
                                case 2:// Process the data for the banking system
                                    break;
                                case 3:// Process the data for the userdata system
                                    break;
                                case 4:// Process the data for the banking system
                                    break;
                                default:
                                    DarkMultiPlayerServer.DarkLog.Debug("Syntax Systems: Invalid SYSTEM ID!");
                                    break;
                            }



                        //}
                        //else
                        //{
                        //    DarkMultiPlayerServer.DarkLog.Debug("Syntax Systems: Failed to read content line from file: " + filename + " :: Content: " + filecontent + " ::");
                        //}
                    }
                }
            }


        }



        // System file dataline formats:
        // Trading system   : 
        // Banking system   : 
        // userdata system  : Stored in seperate files: 'username.txt' : 
        // config system    : 

        /// <summary>
        /// Reads the retrieved data lines and adds these to the specified system correctly.
        /// </summary>
        /// <param name="_SYSTEMID">The system to manipulate</param>
        /// <param name="dataLines">The data to insert</param>
        /// <returns></returns>
        private bool ReadSystemDataLines(int _SYSTEMID, string[] dataLines)
        {
            bool flag = false;

            switch(_SYSTEMID)
            {
                case 1: // Trading system

                    break;
                case 2: // Banking system
                    break;
                case 3: // Userdata storage

                    break;
                case 4: // Commerce system config
                    break;
                default:
                    DarkMultiPlayerServer.DarkLog.Debug("Commerce system: Data lines interpretation failed. Invalid SYSTEMID !");
                    break;
            }
            return flag;
        }

        // TODO //

        private void ReadUserDataFiles() { throw new NotImplementedException(); }
        private void ReadUserDataFile() { throw new NotImplementedException(); }
        private void RetrieveCommerceUserData() { throw new NotImplementedException(); }
        private void ReadTradingFiles() { throw new NotImplementedException(); }
        private void ReadBankingFiles() { throw new NotImplementedException(); }
        private void ReadConfigFiles() { throw new NotImplementedException(); }

        // END OF TODO //

        /// <summary>
        /// Checks COMMERCE directories and files for existance, if non-existant it will create them.
        /// </summary>
        /// <returns></returns>
        private bool CheckDirectoriesAndFiles()
        {
            bool flag = false;
            DarkMultiPlayerServer.DarkLog.Debug("Checking Directory and file existances");
            if (!Directory.Exists(filepathBaseDir)) // Commerce base directory doesn't exist, create all commerce files.
            {
                DarkMultiPlayerServer.DarkLog.Debug("Determined Commerce BASE directory doesn't exist. Creating all directories and files needed..");

                // Base directories
                Directory.CreateDirectory(filepathBaseDir); // create base dir
                Directory.CreateDirectory(tradingFilePathDir); // create trading dir
                Directory.CreateDirectory(bankingFilePathDir); // create banking dir
                Directory.CreateDirectory(userdataFilePathDir); // create userdata dir

                // Base files
                File.CreateText(Path.Combine(filepathBaseDir,"commerce.cfg"));
                File.CreateText(Path.Combine(filepathBaseDir,"currency.cfg"));
                File.CreateText(Path.Combine(filepathBaseDir,"users.txt"));

                // Data files
                // Trading system
                File.CreateText(Path.Combine(tradingFilePathDir, "itemvalues.cfg"));
                File.CreateText(Path.Combine(tradingFilePathDir, "tradingstations.cfg"));
                File.CreateText(Path.Combine(tradingFilePathDir, "itemvalues.cfg"));

                // Banking system
                File.CreateText(Path.Combine(bankingFilePathDir, "bankdata.txt"));
                File.CreateText(Path.Combine(bankingFilePathDir, "loans.txt"));
                File.CreateText(Path.Combine(bankingFilePathDir, "accounts.txt"));
            }
            else if (!Directory.Exists(tradingFilePathDir))
            {
                DarkMultiPlayerServer.DarkLog.Debug("Determined Commerce TRADING directory doesn't exist. Creating all directories and files needed..");

                Directory.CreateDirectory(tradingFilePathDir); // create trading dir
                File.CreateText(Path.Combine(tradingFilePathDir, "itemvalues.cfg"));
                File.CreateText(Path.Combine(tradingFilePathDir, "tradingstations.cfg"));
                File.CreateText(Path.Combine(tradingFilePathDir, "itemvalues.cfg"));
            }
            else if (!Directory.Exists(bankingFilePathDir))
            {
                DarkMultiPlayerServer.DarkLog.Debug("Determined Commerce BANKING directory doesn't exist. Creating all directories and files needed..");

                Directory.CreateDirectory(bankingFilePathDir); // create banking dir
                File.CreateText(Path.Combine(bankingFilePathDir, "bankdata.txt"));
                File.CreateText(Path.Combine(bankingFilePathDir, "loans.txt"));
                File.CreateText(Path.Combine(bankingFilePathDir, "accounts.txt"));
            }
            else if (!Directory.Exists(userdataFilePathDir))
            {
                DarkMultiPlayerServer.DarkLog.Debug("Determined Commerce USERDATA directory doesn't exist. Creating all directories and files needed..");

                Directory.CreateDirectory(userdataFilePathDir); // create userdata dir
            }
            else
            {
                flag = true;
            }

            DarkMultiPlayerServer.DarkLog.Debug("Directories and files check handled!");
            return flag;
        }
    }
}
