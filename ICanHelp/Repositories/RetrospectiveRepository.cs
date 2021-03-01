using ICanHelp.Contracts;
using ICanHelp.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Repositories
{
    public class RetrospectiveRepository : IRetrospectiveRepository
    {
        private IMemoryCache _cache;
        private IHubContext<PokerHub> _hub;
        private SQLiteDBContext _sqlLite;

        public RetrospectiveRepository(IMemoryCache cache, IHubContext<PokerHub> hub, SQLiteDBContext sqlite)
        {
            _cache = cache;
            _hub = hub;
            _sqlLite = sqlite;
        }
        public async Task<int> CreateRetroBoard(RetroCreate request)
        {
            RetroBoard board = new RetroBoard();

            Random rnd = new Random();
            board.Id = rnd.Next(10000, 99999);
            while (await IsBoardExists(board.Id))
            {
                board.Id = rnd.Next(10000, 99999);
            }

            board.TeamName = request.TeamName;
            if (request.Headings != null && request.Headings.Count > 0)
            {
                if (request.Headings[0] != null)
                    board.Column1 = new Heading(request.Headings[0]);
                if (request.Headings[1] != null)
                    board.Column2 = new Heading(request.Headings[1]);
                if (request.Headings[2] != null)
                    board.Column3 = new Heading(request.Headings[2]);
                if (request.Headings[3] != null)
                    board.Column4 = new Heading(request.Headings[3]);
            }
            else
            {
                board.Column1 = new Heading("What went well?");
                board.Column2 = new Heading("What didnt go well?");
                board.Column3 = new Heading("Action Items");
                board.Column4 = new Heading("Thank someone?");
            }

            //_cache.Set(board.Id, board);
            // Temporary work
            SQLiteBridge bridge = new SQLiteBridge();
            bridge.Id = board.Id;
            bridge.Data = JsonConvert.SerializeObject(board);
            _sqlLite.Add(bridge);
            _sqlLite.SaveChanges();

            return board.Id;
        }

        public async Task<RetroBoard> GetBoard(int boardId)
        {
            // Temporary work
            SQLiteBridge bridge = _sqlLite.Board.Select(b => b).Where(e => e.Id == boardId).FirstOrDefault();

            if (bridge == null || string.IsNullOrWhiteSpace(bridge.Data))
                return null;
            
            RetroBoard board = JsonConvert.DeserializeObject<RetroBoard>(bridge.Data);

            return board;
        }

        public async Task<bool> IsBoardExists(int boardId)
        {
            SQLiteBridge bridge = _sqlLite.Board.Select(b => b).Where(e => e.Id == boardId).FirstOrDefault();

            if (bridge != null)
                return true;
            else
                return false;
        }

        public async Task<bool> DeleteComment(string clientId, int boardId, int column, int commentId)
        {
            // Temporary work
            SQLiteBridge bridge = _sqlLite.Board.Select(b => b).Where(e => e.Id == boardId).FirstOrDefault();

            if (bridge == null || string.IsNullOrWhiteSpace(bridge.Data))
                return false;

            RetroBoard board = JsonConvert.DeserializeObject<RetroBoard>(bridge.Data);

            switch (column)
            {
                case 1:
                    board.Column1.Comments.RemoveAll(i => i.Id == commentId);
                    break;
                case 2:
                    board.Column2.Comments.RemoveAll(i => i.Id == commentId);
                    break;
                case 3:
                    board.Column3.Comments.RemoveAll(i => i.Id == commentId);
                    break;
                case 4:
                    board.Column4.Comments.RemoveAll(i => i.Id == commentId);
                    break;
            }

            //_cache.Set(board.Id, board);
            // Temporary work
            //SQLiteBridge bridge = new SQLiteBridge();
            bridge.Id = board.Id;
            bridge.Data = JsonConvert.SerializeObject(board);
            _sqlLite.Update(bridge);
            _sqlLite.SaveChanges();
            await _hub.Clients.GroupExcept(board.Id.ToString(), clientId).SendAsync("CommentDeleted", commentId);

            return true;
        }

        public async Task<int> AddComment(string clientId, int boardId, int column, string comment)
        {
            // Temporary work
            SQLiteBridge bridge = _sqlLite.Board.Select(b => b).Where(e => e.Id == boardId).FirstOrDefault();

            if (bridge == null || string.IsNullOrWhiteSpace(bridge.Data))
                return 0;

            RetroBoard board = JsonConvert.DeserializeObject<RetroBoard>(bridge.Data);

            Random rnd = new Random();
            int id = rnd.Next(0, 999);

            switch (column)
            {
                case 1:
                    board.Column1.Comments.Add(new Comment { Id = id, Text = comment, HeadingId = board.Column1.HeadingId });
                    await _hub.Clients.GroupExcept(board.Id.ToString(), clientId).SendAsync("CommentAdded1", id, comment);
                    break;
                case 2:
                    board.Column2.Comments.Add(new Comment { Id = id, Text = comment, HeadingId = board.Column2.HeadingId });
                    await _hub.Clients.GroupExcept(board.Id.ToString(), clientId).SendAsync("CommentAdded2", id, comment);
                    break;
                case 3:
                    board.Column3.Comments.Add(new Comment { Id = id, Text = comment, HeadingId = board.Column3.HeadingId });
                    await _hub.Clients.GroupExcept(board.Id.ToString(), clientId).SendAsync("CommentAdded3", id, comment);
                    break;
                case 4:
                    board.Column4.Comments.Add(new Comment { Id = id, Text = comment, HeadingId = board.Column4.HeadingId });
                    await _hub.Clients.GroupExcept(board.Id.ToString(), clientId).SendAsync("CommentAdded4", id, comment);
                    break;
            }

            //_cache.Set(board.Id, board);
            // Temporary work
            //SQLiteBridge bridge = new SQLiteBridge();
            bridge.Id = board.Id;
            bridge.Data = JsonConvert.SerializeObject(board);
            _sqlLite.Update(bridge);
            _sqlLite.SaveChanges();

            return id;
        }

        public async Task<bool> EditComment(string clientId, int boardId, int column, int commentId, string comment)
        {
            // Temporary work
            SQLiteBridge bridge = _sqlLite.Board.Select(b => b).Where(e => e.Id == boardId).FirstOrDefault();

            if (bridge == null || string.IsNullOrWhiteSpace(bridge.Data))
                return false;

            RetroBoard board = JsonConvert.DeserializeObject<RetroBoard>(bridge.Data);

            Comment data = new Comment();

            switch (column)
            {
                case 1:
                    data = board.Column1.Comments.Find(c => c.Id == commentId);
                    data.Text = comment;
                    break;
                case 2:
                    data = board.Column2.Comments.Find(c => c.Id == commentId);
                    data.Text = comment;
                    break;
                case 3:
                    data = board.Column3.Comments.Find(c => c.Id == commentId);
                    data.Text = comment;
                    break;
                case 4:
                    data = board.Column4.Comments.Find(c => c.Id == commentId);
                    data.Text = comment;
                    break;
            }

            //_cache.Set(board.Id, board);
            // Temporary work
            //SQLiteBridge bridge = new SQLiteBridge();
            bridge.Id = board.Id;
            bridge.Data = JsonConvert.SerializeObject(board);
            _sqlLite.Update(bridge);
            _sqlLite.SaveChanges();

            await _hub.Clients.GroupExcept(board.Id.ToString(), clientId).SendAsync("CommentUpdated", commentId, comment);

            return true;
        }

        public async Task<string> CopyBoard(int boardId)
        {
            string result = string.Empty;

            // Temporary work
            SQLiteBridge bridge = _sqlLite.Board.Select(b => b).Where(e => e.Id == boardId).FirstOrDefault();

            if (bridge == null || string.IsNullOrWhiteSpace(bridge.Data))
                return "There is nothing to copy!";

            RetroBoard board = JsonConvert.DeserializeObject<RetroBoard>(bridge.Data);

            if (board.Column1 != null && board.Column1.Comments != null && board.Column1.Comments.Any())
            {
                result += board.Column1.Name + "\r\n";
                result += "-------------------------------------------------------------------\r\n";
                foreach (var comment in board.Column1.Comments)
                {
                    result += comment.Text.Trim() + "\r\n";
                }
                result += "\r\n";
            }

            if (board.Column2 != null && board.Column2.Comments != null && board.Column2.Comments.Any())
            {
                result += board.Column2.Name + "\r\n";
                result += "-------------------------------------------------------------------\r\n";
                foreach (var comment in board.Column2.Comments)
                {
                    result += comment.Text.Trim() + "\r\n";
                }
                result += "\r\n";
            }
            if (board.Column3 != null && board.Column3.Comments != null && board.Column3.Comments.Any())
            {
                result += board.Column3.Name + "\r\n";
                result += "-------------------------------------------------------------------\r\n";
                foreach (var comment in board.Column3.Comments)
                {
                    result += comment.Text.Trim() + "\r\n";
                }
                result += "\r\n";
            }
            if (board.Column4 != null && board.Column4.Comments != null && board.Column4.Comments.Any())
            {
                result += board.Column4.Name + "\r\n";
                result += "-------------------------------------------------------------------\r\n";
                foreach (var comment in board.Column4.Comments)
                {
                    result += comment.Text.Trim() + "\r\n";
                }
                result += "\r\n";
            }
            return result;
        }
    }
}
