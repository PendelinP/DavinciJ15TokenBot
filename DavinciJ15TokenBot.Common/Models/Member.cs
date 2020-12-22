using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.Common.Models
{
    public class Member
    {
        public Guid Id { get; set; }

        public int TelegramId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? LastCheckedUtc { get; set; }

        public DateTime? MemberSinceUtc { get; set; }

        public DateTime? RegistrationValidSinceUtc { get; set; }

        public long? TelegramChatId { get; set; }

        public DateTime? KickedAtUtc { get; set; }
    }
}
