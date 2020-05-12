using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public class PointingPokerRepository : IPointingPokerRepository
    {
        public async Task<bool> CreateTable(PointingTable table)
        {
            return true;
        }

        public async Task<bool> AddUser(Guid tableId, User user)
        {
            return true;
        }

        public async Task<bool> DeleteUser(Guid tableId, Guid userId)
        {
            return true;
        }

        public async Task<bool> UpdateVote(Guid tableId, Guid userId, int point)
        {
            return true;
        }
        public async Task<bool> ClearVote(Guid tableId)
        {
            return true;
        }

        public async Task<bool> ShowVote(Guid tableId, Guid userId)
        {
            return true;
        }
    }
}
