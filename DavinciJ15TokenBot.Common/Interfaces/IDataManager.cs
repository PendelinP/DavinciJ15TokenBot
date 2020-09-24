using DavinciJ15TokenBot.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Common.Interfaces
{
    public interface IDataManager
    {
        Task AddOrUpdateMemberAsync(Member member);

        Task DeleteMemberByTelegramIdAsync(int telegramId);

        Task<Member> GetMemberByTelegramIdAsync(int telegramId);

        Task<Member> GetMemberByAddressAsync(string adddress);

        Task<IEnumerable<Member>> GetAllMembersAsync();

        Task<IEnumerable<Member>> GetMembersToCheckAsync(TimeSpan holdingsTimeWindow);
    }
}
