using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public interface IPointingPokerRepository
    {
        Task<PointingTableDom> CreateTable(CreateTable request);

        Task<PointingTableDom> AddUser(JoinTable request);
        Task<bool> RemoveUser(int userId, string clientId);

        Task<bool> IsTableExists(int tableId);
        Task<bool> IsUserExists(int userId);

        //Task<bool> RemoveUser(Guid tableId, Guid userId);

        //Task<List<User>> GetUsersFromBoard(Guid boardId);

        Task<bool> UpdateVote(int userId, string clientId, int points);

        Task<ResetPageHistoryData> NextRound(int tableId, string clientId, int userId);

        //Task<bool> ClearVote(Guid tableId);

        //Task<bool> ShowVote(Guid tableId, Guid userId);
    }
}
