using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.Common.Interfaces
{
    public interface IEthereumMessageSigner
    {
        string GetAddressFromSignedMessage(string message, string signature);
    }
}
