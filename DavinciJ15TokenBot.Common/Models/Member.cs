using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.Common.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? LastCheckedUtc { get; set; }
    }
}
