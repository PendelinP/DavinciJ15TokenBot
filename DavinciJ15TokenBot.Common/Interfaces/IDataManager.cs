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

        Task DeleteMemberAsync(int memberId);

        Task<IEnumerable<Member>> GetAllMembersAsync();
    }
}
