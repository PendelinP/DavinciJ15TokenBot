using DavinciJ15TokenBot.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.EthereumConnector.EthNode
{
    public class EthNodeEthereumConnector : IEthereumConnector
    {
        public Task<decimal> GetAccountBalanceAsync(string address, string contractAddress, int decimals)
        {
            throw new NotImplementedException();
        }
    }
}
