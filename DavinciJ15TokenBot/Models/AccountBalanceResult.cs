using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Models
{
    public class AccountBalanceResult
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public long Result { get; set; }
    }
}
