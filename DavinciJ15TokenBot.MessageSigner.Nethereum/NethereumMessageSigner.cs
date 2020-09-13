using DavinciJ15TokenBot.Common.Interfaces;
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.MessageSigner.Nethereum
{
    public class NethereumMessageSigner : IEthereumMessageSigner
    {
        public string GetAddressFromSignedMessage(string message, string signature)
        {
            var signer = new EthereumMessageSigner();

            return signer.EncodeUTF8AndEcRecover(message, signature);
        }
    }
}
