using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public interface IPointingPokerRepository
    {
        Task<bool> CreateTable(PointingTable table);

        Task<bool> AddUser(Guid tableId, User user);

        Task<bool> DeleteUser(Guid tableId, Guid userId);

        //Task<List<User>> GetUsersFromBoard(Guid boardId);

        Task<bool> UpdateVote(Guid tableId, Guid userId, int point);

        Task<bool> ClearVote(Guid tableId);

        Task<bool> ShowVote(Guid tableId, Guid userId);
    }
}
