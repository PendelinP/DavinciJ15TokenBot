using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Common.Interfaces
{
    public interface IEthereumConnector
    {
        Task<decimal> GetAccountBalanceAsync(string address, string contractAddress, int decimals);
    }
}
