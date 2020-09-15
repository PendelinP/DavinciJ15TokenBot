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
        public Task<Member> AddOrUpdateMemberAsync(Member member)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMemberAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
