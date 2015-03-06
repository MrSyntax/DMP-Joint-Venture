using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingSystemServer.BankingSystem.BankingSystemObjects
{
    /// <summary>
    /// A Bank Client can only be created if he/she is requesting a loan.
    /// </summary>
    public class BankClient
    {
        private string playername;

        #region Bank System Loaning
        private int loanInterest;
        private int maxLoans;
        private int maxLoanAmount;
        private int maxLoanAmountPerLoan;
        private int maxMonthsToPayBackALoan = 48;
        Dictionary<int, BankLoan> playerLoans;
        private int moneyOverPayed = 0;
        #endregion

        /// <summary>
        /// Add a new client aswell as a new bankloan at the sametime
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="loan"></param>
        /// <param name="clientLoanInterest"></param>
        public BankClient(string playerName, BankLoan loan, int clientLoanInterest)
        {
            playername = playerName;
            playerLoans = new Dictionary<int, BankLoan>();
            playerLoans.Add(playerLoans.Keys.Count + 1, loan);
            loanInterest = clientLoanInterest;
            if (loanInterest == 25)
            {
                maxLoans = 1;
                maxLoanAmount = 2500000;
            }
            else if (loanInterest == 15)
            {
                maxLoans = 3;
                maxLoanAmount = 3500000;
            }
            else if (loanInterest == 8)
            {
                maxLoans = 6;
                maxLoanAmount = 4500000;
            }
            else if (loanInterest == 5)
            {
                maxLoans = 9;
                maxLoanAmount = 5500000;
            }
            else if (loanInterest == 2)
            {
                maxLoans = 15;
                maxLoanAmount = 1500000;
            }
            maxLoanAmountPerLoan = CalculateMaxLoanAmountPerLoan(maxLoans, maxLoanAmount);
        }

        private int CalculateMaxLoanAmountPerLoan(int maxloans, int maxloanAmount)
        {
            return (maxloanAmount / maxloans);
        }

        #region Properties
        public string GetPlayerName
        {
            get { return playername; }
        }
        public Dictionary<int, BankLoan> GetPlayerLoans
        {
            get { return playerLoans; }
        }
        public int GetAmountOfLoans
        {
            get
            {
                return playerLoans.Count;
            }
        }
        public int GetMaxAmountOfLoans
        {
            get { return maxLoans; }
        }
        public int GetMaxLoanAmount
        {
            get { return maxLoanAmountPerLoan; }
        }
        public int GetMoneyLend
        {
            get
            {
                int moneyLend = 0;

                foreach (BankLoan loan in playerLoans.Values)
                {
                    moneyLend += loan.GetMoneyAmount;
                }
                return moneyLend;
            }
        }
        public int GetMoneyPaidOff
        {
            get
            {
                int moneyPaidOff = 0;

                foreach (BankLoan loan in playerLoans.Values)
                {
                    moneyPaidOff += loan.GetMoneyPaidOff;
                }
                return moneyPaidOff;
            }
        }
        public int GetMoneyOwed
        {
            get
            {
                int moneyOwed = 0;

                foreach (BankLoan loan in playerLoans.Values)
                {
                    moneyOwed += loan.GetMoneyOwed;
                }

                return moneyOwed;
            }
        }
        public int GetLoanInterest
        {
            get { return loanInterest; }
        }
        internal int SetLoanInterest
        {
            set { loanInterest = value; }
        }

        #endregion

        #region Object specific methods
        /// <summary>
        /// Request a new loan for a client. Only use AFTER it has been verified the client is allowed more loans with the specified parameters.
        /// </summary>
        /// <param name="loanAmount">The amount to loan</param>
        /// <param name="paybackTimeSpan">The timespan in which to pay the loan back in Months</param>
        /// <returns>True if successful and not crashed.</returns>
        public bool NewLoan(int loanAmount, int paybackTimeSpan)
        {
            bool flag = false;
            try
            {
                BankLoan loan = new BankLoan(loanAmount, paybackTimeSpan, loanInterest);
                playerLoans.Add(playerLoans.Keys.Count + 1, loan);
                DarkMultiPlayerServer.DarkLog.Debug("Loan request successful. Loan information: " + loan.ToString());
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                DarkMultiPlayerServer.DarkLog.Debug("Error whilst assingning new loan: ErrorCode: " + ex.ToString());
            }
            return flag;
        }
        /// <summary>
        /// Checks wether for the specified banking client the requested loan is allowed.
        /// </summary>
        /// <param name="loanAmount">The amount to loan</param>
        /// <param name="paybackTimeSpan">The timespan in which to pay the loan back in months</param>
        /// <returns></returns>
        public bool LoanAllowed(int loanAmount, int paybackTimeSpan)
        {
            bool flag = false;

            if (maxLoans != 0)
            {
                if ((loanAmount < maxLoanAmount || loanAmount < maxLoanAmountPerLoan) && paybackTimeSpan < maxMonthsToPayBackALoan)
                {
                    flag = true;
                    maxLoans--;
                }

            }
            return flag;
        }

        /// <summary>
        /// Pays off a specific amount of money for a specific loan.
        /// </summary>
        /// <param name="loanID">The loanID to pay off the loan with the specified amount for</param>
        /// <param name="amountToPayOff">The amount to pay off for the loan</param>
        /// <returns>True if successful</returns>
        public bool PayOffLoanDebt(int loanID, int amountToPayOff)
        {
            bool flag = false;
            bool loanFinalPaymentHandled = false;

            if (LoanExists(loanID))
            {
                if (playerLoans[loanID].PayOffDebt(amountToPayOff))
                {
                    flag = true;
                }
                else
                {
                    if (playerLoans[loanID].DebtPaidOff)
                    {
                        flag = true;
                    }
                }
                // If amount successfully paid off, check if we didn't overcharge the client..
                if (flag)
                {
                    if (playerLoans[loanID].DebtPaidOff)
                    {
                        // If the debt is paid off, check how much was overpaid and send it back to the client core banking account.
                        if (playerLoans[loanID].GetOverpaidIsPaidOut)
                        {
                            moneyOverPayed += playerLoans[loanID].GetMoneyOverPayed;
                            loanFinalPaymentHandled = true;
                        }
                    }
                }
            }



            return flag;
        }

        /// <summary>
        /// Check wether a loan exists in the client his/her banking system account
        /// </summary>
        /// <param name="loanID">The loanID of the client loan to check</param>
        /// <returns>True if loanID exists</returns>
        public bool LoanExists(int loanID)
        {
            return playerLoans.ContainsKey(loanID);
        }
        #endregion

        public string[] SaveToFile()
        {
            List<string> tmpList = new List<string>();
            tmpList.Add(string.Format("playername:{0}",playername));
            tmpList.Add(string.Format("loaninterest:{0}",loanInterest));
            tmpList.Add(string.Format("maxloans:{0}",maxLoans));
            tmpList.Add(string.Format("maxloanamount:{0}",maxLoanAmount));
            tmpList.Add(string.Format("maxloanamountperloan:{0}",maxLoanAmountPerLoan));
            tmpList.Add(string.Format("maxmonthstopaybackaloan:{0}",maxMonthsToPayBackALoan));
            tmpList.Add(string.Format("moneyoverpaid:{0}", moneyOverPayed));

            tmpList.Add("userloans");
            foreach(int loanid in playerLoans.Keys)
            {
                tmpList.Add(string.Format("loanid:{0}",loanid.ToString()));
                tmpList.AddRange(playerLoans[loanid].SaveToFile());
            }
            return tmpList.ToArray();
        }

    }
    public class BankLoan
    {
        private int moneyAmount;
        private int paybackMonths;
        private int moneyPaidOff;
        private int loanInterest;
        private int moneyStillOwed;
        private bool debtpaidoff;
        private int moneyOverPayedForLoan = 0;
        private bool moneyLeftOverPaidOut = false;

        public BankLoan(int moneyamount, int monthsToPayBack, int clientLoanInterest)
        {
            moneyAmount = moneyamount;
            paybackMonths = monthsToPayBack;
            loanInterest = clientLoanInterest;
            moneyPaidOff = 0;
            moneyStillOwed = GetMoneyOwed;
            debtpaidoff = false;
        }
        
        #region base object methods
        public bool GetOverpaidIsPaidOut
        {
            get
            {
                return moneyLeftOverPaidOut;


            }
        }
        public int GetMoneyAmount
        {
            get { return (moneyAmount * loanInterest); }
        }
        public int PayBackMonthsTime
        {
            get { return paybackMonths; }
        }
        public int GetMoneyPaidOff
        {
            get { return moneyPaidOff; }
        }
        /// <summary>
        /// Returns a calculated value of what should still be owed.
        /// </summary>
        public int GetMoneyOwed
        {
            get { return ((moneyAmount - moneyPaidOff) * loanInterest); }
        }
        /// <summary>
        /// Returns the amount still owed on file.
        /// </summary>
        public int GetMonetStillOwed
        {
            get { return moneyStillOwed; }
        }
        public bool DebtPaidOff
        {
            get { return debtpaidoff; }
        }
        /// <summary>
        /// Pays off debt untill moneyStillOwed is 0.
        /// </summary>
        /// <param name="amountToPayOff">The amount to pay off.</param>
        /// <returns>True if successful and no crashes occured.</returns>
        public bool PayOffDebt(int amountToPayOff)
        {
            if (moneyStillOwed == 0)
            {
                debtpaidoff = true;
                return false; // return false because the debt has been payed.
            }

            if (moneyStillOwed > GetMoneyOwed)
            {
                if (amountToPayOff < GetMoneyOwed)
                {
                    moneyPaidOff += amountToPayOff;
                }
                else if (amountToPayOff > GetMoneyOwed)
                {
                    // Calculate the final payment, and finalize it.
                    int difference = (amountToPayOff - GetMoneyOwed);
                    int finalpayment = (amountToPayOff - difference);
                    moneyPaidOff += finalpayment;
                    moneyOverPayedForLoan = difference;
                }
                moneyStillOwed = GetMoneyOwed;
                if (moneyStillOwed == 0)
                {
                    debtpaidoff = true;
                }
            }
            return true;
        }
        public int GetMoneyOverPayed
        {
            get { return moneyOverPayedForLoan; }
        }

        /// <summary>
        /// Returns The specified bankloan in format: foreach variable a newline.
        /// Variables: LoanAmount, LoanInterest, AmountPaidOff, MonthsToPayBack
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"LoanAmount: {0} \r\nLoanInterest: {1} \r\nAmountPaidOff: {2} \r\nMonthsToPayBack: {3}", moneyAmount, loanInterest, moneyPaidOff, paybackMonths);
        }
        #endregion

        public string[] SaveToFile()
        {
            List<string> tmpList = new List<string>();

            tmpList.Add("{");
            tmpList.Add(string.Format("{0}:{1}", "moneyamount",moneyAmount));
            tmpList.Add(string.Format("{0}:{1}", "paybackmonths",paybackMonths));
            tmpList.Add(string.Format("{0}:{1}", "moneypaidoff",moneyPaidOff));
            tmpList.Add(string.Format("{0}:{1}", "loaninterest",loanInterest));
            tmpList.Add(string.Format("{0}:{1}", "moneystillowed",moneyStillOwed));
            tmpList.Add(string.Format("{0}:{1}", "debtpaidoff", debtpaidoff));
            tmpList.Add(string.Format("{0}:{1}", "moneyoverpaid", moneyOverPayedForLoan));
            tmpList.Add(string.Format("{0}:{1}", "moneyoverpaidIsPaidout", moneyLeftOverPaidOut));
            tmpList.Add("}");


            return tmpList.ToArray();
        }
    }
    public class BankAccount
    {
        private int bankAccountID;
        private long cash;
        private int interestRate;
        private int bankClientHolderID;

        public BankAccount(int bankaccountID, long cashToStore, int interestrate, int bankAccountClientHolderID)
        {
            bankAccountID = bankaccountID;
            cash = cashToStore;
            interestRate = interestrate;
            bankClientHolderID = bankAccountClientHolderID;
        }

        public int GetBankAccountID
        {
            get { return bankAccountID; }
        }

        public int GetInterestRate
        {
            get { return interestRate; }
        }

        public long GetCashAmount
        {
            get { return cash; }
        }
        public int GetBankClientHolderID
        {
            get { return bankClientHolderID; }
        }

        public override string ToString()
        {
            return string.Format("BankAccount: {0} \r\nClientHolder: {1} \r\nInterestRate: {2} \r\nCash: {3}", bankAccountID, bankClientHolderID, interestRate, cash);
        }

        /// <summary>
        /// Returns a formatted string[] array with the object information contained within the current object.
        /// </summary>
        /// <returns></returns>
        public string[] SaveToFile()
        {
            List<string> tmpList = new List<string>();
            tmpList.Add(string.Format("{0}:{1}", "bankaccountid", bankAccountID.ToString()));
            tmpList.Add(string.Format("{0}:{1}", "bankclientholderid", bankClientHolderID.ToString()));
            tmpList.Add(string.Format("{0}:{1}", "bankaccountinterestrate", interestRate.ToString()));
            tmpList.Add(string.Format("{0}:{1}", "bankaccountcashamount", cash.ToString()));
            return tmpList.ToArray();
        }

    }
    public class BankAccountClient
    {
        private int bankaccountClientID;
        private string clientName;
        Dictionary<int, BankAccount> clientBankAccounts;

        public BankAccountClient(int bankAccountClientID, string clientname, BankAccount newBankAccount)
        {
            bankaccountClientID = bankAccountClientID;
            clientBankAccounts = new Dictionary<int, BankAccount>();
            clientName = clientname;
            clientBankAccounts.Add(newBankAccount.GetBankAccountID, newBankAccount);
        }

        public string GetBankAccount(BankAccount bankAccount)
        {
            string tmp = "";
            foreach (BankAccount account in clientBankAccounts.Values)
            {
                if (account == bankAccount)
                {
                    tmp = account.ToString();
                }
            }
            return tmp;
        }

        public int GetBankClientID
        {
            get { return bankaccountClientID; }
        }

        public string GetBankClientName
        {
            get { return clientName; }
        }

        public Dictionary<int,BankAccount> GetClientBankAccounts
        {
            get { return clientBankAccounts; }
        }

        /// <summary>
        /// Adds a new bank account to the specific client if the account doesn't already exist.
        /// </summary>
        /// <param name="newBankAccount">The new bank account to add</param>
        /// <returns>True if successful</returns>
        public bool NewBankAccount(BankAccount newBankAccount)
        {
            bool flag = false;
            if (!clientBankAccounts.ContainsValue(newBankAccount))
            {
                clientBankAccounts.Add(newBankAccount.GetBankAccountID, newBankAccount);
                flag = true;
            }
            return flag;
        }

        public bool RemoveBankAccount(BankAccount bankAccountToRemove)
        {
            bool flag = false;

            if (clientBankAccounts.ContainsValue(bankAccountToRemove))
            {
                foreach (BankAccount bankAccount in clientBankAccounts.Values)
                {
                    if (bankAccount == bankAccountToRemove)
                    {
                        clientBankAccounts.Remove(bankAccount.GetBankAccountID);
                        flag = true;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Retrieves all relative information that is stored within the commerce banking system of the currently selected client.
        /// </summary>
        /// <returns>string[] array to be written to file</returns>
        public string[] SaveToFile()
        {
            List<string> tmpList = new List<string>();

            tmpList.Add(string.Format("{0}:{1}", "bankaccountclientid",bankaccountClientID.ToString()));
            tmpList.Add(string.Format("{0}:{1}", "bankacountclientname", clientName));

            tmpList.Add("bankaccounts");
            foreach(int bankaccountID in clientBankAccounts.Keys)
            {
                tmpList.Add("{");
                tmpList.AddRange(clientBankAccounts[bankaccountID].SaveToFile());
                tmpList.Add("}");
            }

            return tmpList.ToArray();
        }

        public override string ToString()
        {
            return string.Format("BankAccountClient: {0} \r\nID: {1} \r\nNo.BankAccounts: {2}", clientName, bankaccountClientID, clientBankAccounts.Keys.Count);
        }
    }

}
