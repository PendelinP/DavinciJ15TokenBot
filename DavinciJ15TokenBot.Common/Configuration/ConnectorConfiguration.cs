using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.Common.Configuration
{
    public class ConnectorConfiguration
    {
        public ConnectorMode Mode { get; set; }

        // etherscan config settings
        public string EtherscanApiKey { get; set; }

        // eth node config settings
        public string NodeAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
