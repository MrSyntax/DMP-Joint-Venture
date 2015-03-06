using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TradingSystemServer;
using TradingSystemServer.BankingSystem;

namespace TradingSystemServer
{
    public static class CommerceInitialization
    {
        public static void init() 
        {
            if(CommerceFileControl.DirectoryAndFileExistanceChecks())
            {
                ReadFiles();
            }
        }


        // Read/write all files
        public static void ReadFiles() { }
        public static void WriteFiles() { }

        // Read/write the files for the given user
        public static void WriteFile(string username) { }
        public static void ReadFile(string username) { }



    }
    /// <summary>
    /// Contains all the read/write methods needed for the Syntax Commerce System
    /// </summary>
    internal static class CommerceFileControl
    {
        internal static bool DirectoryAndFileExistanceChecks()
        {
            bool flag = false;
            bool writeDefaultConfig = false;
            string universedirectory = DarkMultiPlayerServer.Server.universeDirectory;
            string syntaxdirectory = Path.Combine("SyntaxPlugins", "Commerce");
            string commercedirectory = Path.Combine(universedirectory, syntaxdirectory);
            string bankingdirectory = Path.Combine(commercedirectory, "BankingSystem");
            string userdatadirectory = Path.Combine(bankingdirectory, "UserData");
            if (!Directory.Exists(commercedirectory))
            {
                Directory.CreateDirectory(commercedirectory);
            }
            if (!Directory.Exists(userdatadirectory))
            {
                Directory.CreateDirectory(userdatadirectory);
            }

            if (!File.Exists(Path.Combine(commercedirectory, "config.txt")))
            {
                File.CreateText(Path.Combine(commercedirectory, "config.txt"));
                writeDefaultConfig = true;
            }
            if(writeDefaultConfig)
            {
                if(WriteInitConfig())
                {
                    flag = true;
                }
            }
            return flag;
        }

        internal static Dictionary<string,string[]> RetrieveUserData(string username)
        {
            Dictionary<string, string[]> retrievedUserData = new Dictionary<string, string[]>();

            // Retrieve the userdata from banking system
            KeyValuePair<string[], string[]> BankUserData = BankSystem.GetUserData(username);

            retrievedUserData.Add("USERLOANS", BankUserData.Key);
            retrievedUserData.Add("USERACCOUNTS", BankUserData.Value);

            // Retrieve the userdata from trading system
            // TODO

            return retrievedUserData;
        }

        // Read/write userdata files belonging to the username
        internal static bool WriteUserDataFile(string username) { return false; }
        internal static bool ReadUserDataFile(string username) { return false; }

        // Read/write Banking files belonging to the username
        internal static bool ReadBankingFiles(string username) { return false; }
        internal static bool WriteBankingFiles(string username) { return false; }

        // Read/write all Banking from/to ffiles
        internal static bool ReadAllBankingFiles() { return false; }
        internal static bool WriteAllBankingFiles() { return false; }

        // Read/write Trading Station files belonging to the username
        internal static bool ReadUserTradingStations(string username) { return false; }
        internal static bool WriteUserTradingStations(string username) { return false; }

        // Read/write all Trading Stations from/to files
        internal static bool ReadAllTradingStations() { return false; }
        internal static bool WriteAllTradingStations() { return false; }

        // Read/write Trading files belonging to the username
        internal static bool ReadTradingFiles(string username) { return false; }
        internal static bool WriteTradingFiles(string username) { return false; }

        // Read/write all Trading from/to ffiles
        internal static bool ReadAllTradingFiles() { return false; }
        internal static bool WriteAllTradingFiles() { return false; }

        // Read/write of Commerce config
        internal static bool WriteInitConfig()
        {
            Dictionary<string, int> settings = new Dictionary<string, int>();
            settings.Add("BankMoney", 10000000);
            settings.Add("BankAccountsCount", 0);
            settings.Add("MaxLoansPerNewClient", 1);
            settings.Add("MaxLoanAmountPerNewClient", 2500000);
            settings.Add("LoanInterestEachNewClient", 25);
            settings.Add("MaxMonthsToPayBackNewClient", 48);
            settings.Add("MaxLoansClientsAllowedBackDue", 0);
            settings.Add("LoanInterestLow", 5);
            settings.Add("LoanInterestmed", 8);
            settings.Add("LoanInteresthigh", 15);
            settings.Add("AccountInterestRateLow", 5);
            settings.Add("AccountInterestRatemed", 8);
            settings.Add("AccountInterestRatehigh", 15);

            string universedirectory = DarkMultiPlayerServer.Server.universeDirectory;
            string syntaxdirectory = Path.Combine("SyntaxPlugins", "Commerce");
            string filepath = Path.Combine(universedirectory, syntaxdirectory);
            bool flag = false;
            Dictionary<string, int> settingsToWrite = BankSystem.GetSettings;
            StreamWriter sw = new StreamWriter(Path.Combine(filepath, "config.txt"));
            foreach (string setting in settingsToWrite.Keys)
            {
                string settingLine = string.Format("{0};{1}", setting, settingsToWrite[setting].ToString());
                sw.WriteLine(settingLine);
                flag = true;
            }
            sw.Close();
            return flag;
        }

        internal static bool WriteConfig()
        {
            bool flag = false;
            Dictionary<string, int> settingsToWrite = BankSystem.GetSettings;
            string universedirectory = DarkMultiPlayerServer.Server.universeDirectory;
            string syntaxdirectory = Path.Combine("SyntaxPlugins", "Commerce");
            string filepath = Path.Combine(universedirectory, syntaxdirectory);
            StreamWriter sw = new StreamWriter(Path.Combine(filepath,"config.txt"));
            foreach(string setting in settingsToWrite.Keys)
            {
                string settingLine = string.Format("{0};{1}",setting,settingsToWrite[setting].ToString());
                sw.WriteLine(settingLine);
                flag = true;
            }
            sw.Close();
            return flag;
        }

        internal static bool ReadConfig()
        {
            bool flag = false;
            string universedirectory = DarkMultiPlayerServer.Server.universeDirectory;
            string syntaxdirectory = Path.Combine("SyntaxPlugins", "Commerce");
            string filepath = Path.Combine(universedirectory, syntaxdirectory);
            StreamReader sr = new StreamReader(Path.Combine(filepath, "config.txt"));
            Dictionary<string, int> retrievedSettings = new Dictionary<string, int>();
            
            while(!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] settingParts = line.Split(';');
                int settingValue = 0;
                string settingname = settingParts[0];
                int.TryParse(settingParts[1], out settingValue);
                retrievedSettings.Add(settingname, settingValue);
            }
            if(BankSystem.GetSettings != null)
            {
                BankSystem.SetSettings = retrievedSettings;
            }


            return flag;
        }
    }
}
