using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingSystemServer.BankingSystem.BankingSystemObjects;

namespace TradingSystemServer.TradingSystem.TradingObjects
{
    public class RegisteredUser
    {
        private int userID;
        private string playername;
        private long currencyAmount;
        private BankAccountClient playerBankaccount;

        public int GetUserID { get { return userID; } }

        public RegisteredUser(int uid, string playerName, BankAccountClient playerBankAccount)
        {
            userID = uid;
            playername = playerName;
            playerBankaccount = playerBankAccount;
        }

        public bool NewBankAccountNumbr()
        {
            bool flag = false;

            //playerBankaccount.NewBankAccount(new BankAccount(0,currencyAmount))



            return flag;
        }

        public string[] SaveToFile()
        {
            List<string> tmpList = new List<string>();
            tmpList.Add(userID.ToString());
            tmpList.Add(playername);
            tmpList.Add(currencyAmount.ToString());


            return tmpList.ToArray();
        }
    }

    public class RegisteredTradingStation
    {
        private int userID;
        private int stationID;
        private string stationname;
        private Dictionary<int, string> SellList = new Dictionary<int, string>();
        private Dictionary<int, string> BuyList = new Dictionary<int, string>();

        public RegisteredTradingStation(int uid, int sid, string stationName)
        {
            userID = uid;
            stationID = sid;
            stationname = stationName;
        }
        public RegisteredTradingStation(int uid, int sid, string stationName, Dictionary<int,string> _sellListOrBuyList,bool sellOrBuyList)
        {
            userID = uid;
            stationID = sid;
            stationname = stationName;
            if(sellOrBuyList)
            {
                SellList = _sellListOrBuyList;
            }
            else if(!sellOrBuyList)
            {
                BuyList = _sellListOrBuyList;
            }
        }
        //public RegisteredTradingStation(int uid, int sid, string stationName,  Dictionary<int, string> _buyList)
        //{
        //    userID = uid;
        //    stationID = sid;
        //    stationname = stationName;
        //    BuyList = _buyList;
        //}
        public RegisteredTradingStation(int uid, int sid, string stationName, Dictionary<int, string> _sellList, Dictionary<int, string> _buyList)
        {
            userID = uid;
            stationID = sid;
            stationname = stationName;
            SellList = _sellList;
            BuyList = _buyList;
        }

        public bool AddToSellList() { return false; }
        public bool RemoveFromSellList() { return false; }
        public bool AddToBuyList() { return false; }
        public bool RemoveFromBuyList() { return false; }




    }

}
