using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingSystemServer;

namespace Commerce.Currency
{
    /// <summary>
    /// The currency supported behind the trading system
    /// </summary>
    public class Currency
    {
        public long universeCurrencyAmount { get; set; }
        public Dictionary<string, long> universeItemValues { get; set; }
        public long universeCurrencyValue { get; set; }



        // define the class values


        /// <summary>
        /// Retrieves the currency data from file and returns the universe currency amount.
        /// </summary>
        public long InitializeCurrency
        {
            get
            {
                universeCurrencyAmount = ReadCurrencyFromFile();
                return universeCurrencyAmount;
            }
        }





        // TODO //

        private long ReadCurrencyFromFile()
        {
            // TODO : Read currency from file




            return 32134234;
        }

        private void WriteCurrencyToFile()
        {
            // TODO : Write currency to file


        }
    }

    /// <summary>
    /// The currency statistics
    /// </summary>
    static public class Statistics
    {
        static public long curencyAverage
        {
            get
            {
                long avg = 0000000;

                // calculate the currency value


                return avg;
            }
        }


    }
}
