using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Contracts
{
    public interface IRetrospectiveRepository
    {
        Task<int> CreateRetroBoard(RetroCreate request);
        Task<RetroBoard> GetBoard(int boardId);
        Task<bool> DeleteComment(string clientId, int boardId, int column, int commentId);
        //Task<bool> EditComment(int boardId, int column, int commentId, string comment);
        Task<int> AddComment(string clientId, int boardId, int column, string comment);
        Task<bool> IsBoardExists(int boardId);
        Task<bool> EditComment(string clientId, int boardId, int column, int commentId, string comment);
        //Task<bool> IsCommentExists(int boardId, int column, int commentId);
    }
}
