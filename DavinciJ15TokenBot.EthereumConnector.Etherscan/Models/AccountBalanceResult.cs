using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.EthereumConnector.Etherscan.Models
{
    internal class AccountBalanceResult
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public long Result { get; set; }
    }
}
