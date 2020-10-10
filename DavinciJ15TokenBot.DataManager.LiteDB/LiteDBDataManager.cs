using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.DataManager.LiteDB
{
    public class LiteDBDataManager : IDataManager
    {
        public Task AddOrUpdateMemberAsync(Member member)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMemberByTelegramIdAsync(int telegramId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetMemberByAddressAsync(string adddress)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetMemberByTelegramIdAsync(int telegramId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetMembersToCheckAsync(TimeSpan holdingsTimeWindow)
        {
            throw new NotImplementedException();
        }
    }
}
