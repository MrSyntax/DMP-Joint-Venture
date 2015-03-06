using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingSystemServer.BankingSystem;
using TradingSystemServer.BankingSystem.BankingSystemObjects;
using TradingSystemServer.TradingSystem.TradingObjects;

namespace TradingSystemServer.TradingSystem
{
    public class TradeSystemCore
    {
        // Define the userdata
        private static Dictionary<string, RegisteredUser> registeredUsers = new Dictionary<string, RegisteredUser>();
        //                      username,           stationid, stationdetails
        private static Dictionary<string, Dictionary<string,RegisteredTradingStation>> registeredTradingStations = new Dictionary<string, Dictionary<string,RegisteredTradingStation>>();


        //                                                                                                     username,        stationid,details   detailid,detailvalue                            stationid,        itemlists,        ,itemid,amount,price    
        public static void RegisterUser(string username,Dictionary<string,string> userBankingDetails, Dictionary<string,Dictionary<string,Dictionary<string,string>>> userTradingStations, Dictionary<string,Dictionary<string,Dictionary<int,string>>> stationBuySellLists)
        {
            if(!BankSystem.ClientExists(username) && !RegisteredUserExists(username))
            {
                // register the user with the banking details provided
                registeredUsers.Add(username,new RegisteredUser(1,username,new BankAccountClient(int.Parse(userBankingDetails["clientid"]), username,new BankAccount(int.Parse(userBankingDetails["accountid"]),long.Parse(userBankingDetails["cashamount"]),int.Parse(userBankingDetails["interestrate"]),int.Parse(userBankingDetails["clientid"])))));
                // register any trading stations

                Dictionary<string, RegisteredTradingStation> userregstations = new Dictionary<string, RegisteredTradingStation>();
                foreach(string _stationOwner in userTradingStations.Keys)
                {
                    if(_stationOwner != "userdetails")
                    {
                        if (username == _stationOwner)
                        {
                            Dictionary<string, Dictionary<string, string>> userStations = userTradingStations[_stationOwner];

                            foreach(string stationid in userStations.Keys)
                            {
                                Dictionary<string,string> stationdetails = userStations[stationid];

                                // Retrieve the station sell and buy lists, if any
                                // Create the station sell and buy lists
                                Dictionary<int, string> stationSellList = stationBuySellLists[stationid]["sell_list"];
                                Dictionary<int, string> stationBuyList = stationBuySellLists[stationid]["buy_list"];

                                // Add the station to the internal list
                                userregstations.Add(stationid, new RegisteredTradingStation(registeredUsers[username].GetUserID,int.Parse(stationdetails["stationid"]),stationdetails["stationname"],stationSellList,stationBuyList));
                            }


                            //RegisteredTradingStation newRegStation = new RegisteredTradingStation(int.Parse(tradingstationDetails["userid"]),int.Parse(tradingstationDetails["stationid"]),tradingstationDetails[""],tradingstationDetails[""],tradingstationDetails[""])
                        }
                    }
                }




            }
        }

        public static bool RegisteredUserExists(string username)
        {
            return registeredUsers.ContainsKey(username);
        }



    }




}
