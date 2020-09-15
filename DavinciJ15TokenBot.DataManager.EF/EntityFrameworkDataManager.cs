using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.DataManager.EF
{
    public class EntityFrameworkDataManager : IDataManager
    {
        private readonly Func<DataContext> contextFactory;

        public EntityFrameworkDataManager(Func<DataContext> contextFactory)
        {
            this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task AddOrUpdateMemberAsync(Member member)
        {
            using (var context = this.contextFactory())
            {
                if (member.Id == default)
                {
                    member.Id = Guid.NewGuid();

                    context.Members.Add(member);
                }
                else
                {
                    var existingMember = await context.Members.SingleOrDefaultAsync(p => p.Id == member.Id);

                    existingMember.Amount = member.Amount;
                    existingMember.LastCheckedUtc = member.LastCheckedUtc;
                    existingMember.Name = member.Name;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteMemberByTelegramIdAsync(int telegramId)
        {
            using (var context = this.contextFactory())
            {
                var member = await context.Members.SingleOrDefaultAsync(p => p.TelegramId == telegramId);

                if (member != null)
                {
                    context.Members.Remove(member);

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            using (var context = this.contextFactory())
            {
                return await context.Members.ToListAsync();
            }
        }
    }
}
