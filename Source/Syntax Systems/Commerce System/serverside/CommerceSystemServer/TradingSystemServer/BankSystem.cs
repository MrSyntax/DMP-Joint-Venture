using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingSystemServer.BankingSystem.BankingSystemObjects;

namespace TradingSystemServer.BankingSystem
{
    public class BankSystem
    {
        // Define the users that have loans
        private static Dictionary<string, BankClient> bankingClients = new Dictionary<string, BankClient>();
        // Define the users that have bank accounts
        private static Dictionary<string, BankAccountClient> bankingAccountClients = new Dictionary<string, BankAccountClient>();

        // Define how much money the bank has by default
        private static int bankMoney = 10000000;
        private static int bankingaccountsCount = 0;
        
        #region Finished coding
        #region Bank Rules

        // Bank System loan rules
        private static int maxLoansPerNewClient = 1;
        private static int maxLoanAmountNewClient = 2500000;
        private static int newClientLoanInterest = 25;
        private static int maxMonthsToPayBackNewClient = 48;

        // Loan rules if you don't pay back on time
        private static int maxLoansClientsBackDue = 0;

        // Bank loan interest percentages
        public static int loanInterestLow = 5;
        public static int loanInterestMed = 8;
        public static int loanInterestHigh = 15;

        // Bank Account rules
        private static int accountInterestRateLow = 5;
        private static int accountInterestRateMed = 8;
        private static int accountInterestRateHigh = 15;
        #endregion

        #region Properties
        public BankSystem GetActiveBankSystem
        {
            get { return this; }
        }

        public int GetAmountOfBankingAccounts
        {
            get { return bankingaccountsCount; }
        }

        public static Dictionary<string,int> GetSettings
        {
            get
            {
                Dictionary<string, int> settings = new Dictionary<string, int>();

                settings.Add("BankMoney", bankMoney);
                settings.Add("BankAccountsCount", bankingaccountsCount);
                settings.Add("MaxLoansPerNewClient", maxLoansPerNewClient);
                settings.Add("MaxLoanAmountPerNewClient", maxLoanAmountNewClient);
                settings.Add("LoanInterestEachNewClient", newClientLoanInterest);
                settings.Add("MaxMonthsToPayBackNewClient", maxMonthsToPayBackNewClient);
                settings.Add("MaxLoansClientsAllowedBackDue", maxLoansClientsBackDue);
                settings.Add("LoanInterestLow", loanInterestLow);
                settings.Add("LoanInterestmed", loanInterestMed);
                settings.Add("LoanInteresthigh", loanInterestHigh);
                settings.Add("AccountInterestRateLow", accountInterestRateLow);
                settings.Add("AccountInterestRatemed", accountInterestRateMed);
                settings.Add("AccountInterestRatehigh", accountInterestRateHigh);
                return settings;
            }
        }

        public static Dictionary<string,int> SetSettings
        {
            set
            {
                Dictionary<string, int> settings = value;

                bankMoney = settings["BankMoney"];
                bankingaccountsCount = settings["BankAccountsCount"];
                maxLoansPerNewClient = settings["MaxLoansPerNewClient"];
                maxLoanAmountNewClient = settings["MaxLoanAmountPerNewClient"];
                newClientLoanInterest = settings["LoanInterestEachNewClient"];
                maxMonthsToPayBackNewClient = settings["MaxMonthsToPayBackNewClient"];
                maxLoansClientsBackDue = settings["MaxLoansClientsAllowedBackDue"];
                loanInterestLow = settings["LoanInterestLow"];
                loanInterestMed = settings["LoanInterestmed"];
                loanInterestHigh = settings["LoanInteresthigh"];
                accountInterestRateLow = settings["AccountInterestRateLow"];
                accountInterestRateMed = settings["AccountInterestRatemed"];
                accountInterestRateHigh = settings["AccountInterestRatehigh"];
            }
        }

        #endregion

        // Constructor
        public BankSystem()
        {
             
        }

        #region Common Methods
        /// <summary>
        /// Checks wether the specified playername already exists in the banking system clientlist.
        /// </summary>
        /// <param name="playername">The playername to check</param>
        /// <returns>True if client exists in bankingsystem.</returns>
        public static bool ClientExists(string playername)
        {
            return bankingClients.ContainsKey(playername);
        }
        /// <summary>
        /// Requests to add a new client to the bankingsystem clients list.
        /// </summary>
        /// <param name="playername">The client playername</param>
        /// <param name="moneyAmountRequested">The amount to loan</param>
        /// <param name="paybackTimespanRequested">The timespan in which to pay back in Months</param>
        /// <returns>True if new client and successful -or- client already exists and loan is allowed.</returns>
        public bool AddClient(string playername, int moneyAmountRequested, int paybackTimespanRequested)
        {
            bool flag = false;
            if(!ClientExists(playername))
            {
                if (moneyAmountRequested <= maxLoanAmountNewClient && paybackTimespanRequested <= maxMonthsToPayBackNewClient)
                {
                    bankingClients.Add(playername, new BankClient(playername, new BankLoan(moneyAmountRequested, paybackTimespanRequested, newClientLoanInterest), newClientLoanInterest));
                    flag = true;
                    return flag;
                }
            }
            if(ClientExists(playername))
            {
                if(bankingClients.ContainsKey(playername))
                {
                    if(bankingClients[playername].LoanAllowed(moneyAmountRequested,paybackTimespanRequested))
                    {
                        // Loan is allowed , so add it.
                        bankingClients[playername].NewLoan(moneyAmountRequested, paybackTimespanRequested);

                        flag = true;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Requests a new loan for the specified client playername and with the specified loan parameters.
        /// </summary>
        /// <param name="playername">The client playername to request the loan for</param>
        /// <param name="moneyAmountRequested">The amount of money to loan</param>
        /// <param name="paybackTimeSpanRequested">The timespan in which to pay the loan back (in MONTHS)</param>
        /// <returns>True if successful and didn't crash.</returns>
        public bool RequestLoan(string playername, int moneyAmountRequested, int paybackTimeSpanRequested)
        {
            bool flag = false;

            if(ClientExists(playername))
            {
                if(bankingClients[playername].LoanAllowed(moneyAmountRequested,paybackTimeSpanRequested))
                {
                    if(bankingClients[playername].NewLoan(moneyAmountRequested, paybackTimeSpanRequested))
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Pays off the specified loan for the specified player with the specified amount to pay off.
        /// </summary>
        /// <param name="playername"></param>
        /// <param name="loanID"></param>
        /// <param name="amountToPayOff"></param>
        /// <returns></returns>
        public bool PayOffDebt(string playername, int loanID, int amountToPayOff)
        {
            bool flag = false;

            if(ClientExists(playername))
            {
                if(bankingClients[playername].PayOffLoanDebt(loanID,amountToPayOff))
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Adds the specified client with the specified amount of money to the Banking System Account Holders List
        /// </summary>
        /// <param name="playername">The client playername to add</param>
        /// <param name="AmountToBeStored">The amount of cash to add</param>
        /// <returns>True if successful</returns>
        public bool AddBankClient(string playername, int AmountToBeStored)
        {
            bool flag = false;

            if(!bankingAccountClients.Keys.Contains(playername))
            {
                int newClientID = bankingAccountClients.Keys.Count+1;
                int newBankAccountID = GetAmountOfBankingAccounts + 1;
                // Create the new client objects
                BankAccount newBankAccount = new BankAccount(newBankAccountID, AmountToBeStored, loanInterestLow, newClientID);
                BankAccountClient newbankingClient = new BankAccountClient(newClientID, playername, newBankAccount);
                // Add it to the banking clients list
                bankingAccountClients.Add(playername, newbankingClient);
                DarkMultiPlayerServer.DarkLog.Debug(string.Format("New client added to banking system: New normal Account: {0}\r\n BankAccountInfo: {1}",newbankingClient.ToString(),newBankAccount.ToString()));
                flag = true;
            }
            return flag;
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets all userdata from the Banksystem and sends it back as an string array.
        /// </summary>
        /// <param name="username">The username to retrieve the data for</param>
        /// <returns>KeyValuePair with string[] clientLoans , string[] clientAccounts</returns>
        internal static KeyValuePair<string[],string[]> GetUserData(string username)
        {
            return new KeyValuePair<string[], string[]>(GetUserLoans(username), GetUserAccounts(username));
        }

        // Internal usage only
        private static string[] GetUserLoans(string username)
        {
            List<string> returndata = new List<string>();
            if(bankingClients.ContainsKey(username))
            {
                if(bankingClients[username].GetAmountOfLoans != 0)
                {
                    Dictionary<int, BankLoan> retrievedLoans = bankingClients[username].GetPlayerLoans;
                    foreach(int loanID in retrievedLoans.Keys)
                    {
                        returndata.Add(retrievedLoans[loanID].ToString());
                    }
                }
            }
            return returndata.ToArray();
        }
        private static string[] GetUserAccounts(string username)
        {
            List<string> returndata = new List<string>();

            if(bankingAccountClients.ContainsKey(username))
            {
                Dictionary<int,BankAccount> retrievedClientAccounts = bankingAccountClients[username].GetClientBankAccounts;
                foreach(int accountID in retrievedClientAccounts.Keys)
                {
                    returndata.Add(retrievedClientAccounts[accountID].ToString());
                }
            }
            return returndata.ToArray();
        }

        public bool SaveBankingSystemToFile()
        {
            bool flag = false;
            Dictionary<string, string[]> allPlayerLoanData = new Dictionary<string, string[]>();
            Dictionary<string, string[]> allPlayerAccountData = new Dictionary<string, string[]>();

            #region Retrieve all banking system data

            // Retrieve all banking player loans data
            foreach(string player in bankingClients.Keys)
            {
                string[] informationToWrite = bankingClients[player].SaveToFile();
                allPlayerLoanData.Add(player, informationToWrite);
            }

            // Retrieve all banking player accounts data
            foreach(string player in bankingAccountClients.Keys)
            {
                string[] informationToWrite = bankingAccountClients[player].SaveToFile();
                allPlayerAccountData.Add(player, informationToWrite);
            }

            #endregion

            return flag;
        }

    }


}
