using ICanHelp.Contracts;
using ICanHelp.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Repositories
{
    public class PointingPokerRepository : IPointingPokerRepository
    {
        private IMemoryCache _cache;
        private IHubContext<PokerHub> _hub;

        public PointingPokerRepository(IMemoryCache cache, IHubContext<PokerHub> hub)
        {
            _cache = cache;
            _hub = hub;
        }

        public async Task<PointingTableDom> CreateTable(CreateTable request)
        {
            if (request == null)
            {
                return null;
            }

            //Create new poker table
            PointingTable table = new PointingTable();

            Random rnd = new Random();
            table.Id = rnd.Next(0, 99999);
            table.Name = request.TableName;
            table.Users = new List<User>();

            User user = new User();
            user.Name = request.UserName;
            user.IsOwner = true;
            user.Id = rnd.Next(111, 999);
            user.TableId = table.Id;
            user.IsVoting = request.IsVoting;
            if (request.IsVoting)
                table.TotalVoters = 1;

            table.Owner = user;
            table.Users.Add(user);

            _cache.Set(table.Id, table);
            _cache.Set(user.Id, user);

            PointingTableDom result = new PointingTableDom(table);
            result.CurrentUser = user;

            return result;
        }

        public async Task<PointingTableDom> AddUser(JoinTable request)
        {
            if (request == null)
            {
                return null;
            }

            //Pull table and add user to it
            PointingTable table = _cache.Get<PointingTable>(request.TableId);

            Random rnd = new Random();

            //Create User
            User user = new User
            {
                Name = request.UserName,
                IsOwner = false,
                Id = rnd.Next(111, 999),
                IsVoting = request.IsVoting,
                TableId = table.Id
            };

            table.Users.Add(user);
            if (request.IsVoting)
                table.TotalVoters++;

            _cache.Set(table.Id, table);
            _cache.Set(user.Id, user);

            await _hub.Clients.Group(table.Id.ToString()).SendAsync("UserAdded", user);

            PointingTableDom result = new PointingTableDom(table);
            result.CurrentUser = user;
            return result;
        }

        public async Task<bool> IsTableExists(int tableId)
        {
            //Pull table and add user to it
            PointingTable table = _cache.Get<PointingTable>(tableId);

            if (table != null)
                return true;
            else
                return false;
        }

        public async Task<bool> IsUserExists(int userId)
        {
            User user = _cache.Get<User>(userId);

            if (user != null)
                return true;
            else
                return false;
        }

        public async Task<bool> RemoveUser(int userId, string clientId)
        {
            User user = _cache.Get<User>(userId);

            if (user == null)
            {
                EventLog.WriteEntry("ICanHelp", $"Nothing to remove. UserId: {userId} not found.", EventLogEntryType.Error);
                return false;
            }

            PointingTable table = _cache.Get<PointingTable>(user.TableId);

            if (table == null)
            {
                return false;
            }

            if (table.Users.Remove(user))
            {
                if (user.IsVoting)
                    table.TotalVoters--;
                if (user.Vote != 0)
                    table.VotesRecorded--;
            }

            if (table.Users.Count < 1)
            {
                _cache.Remove(table.Id);
                _cache.Remove(userId);
                await _hub.Clients.Group(table.Id.ToString()).SendAsync("Message", "Table removed from pointing hub");
                EventLog.WriteEntry("ICanHelp", $"Table Id: {table.Id} removed as no users are present at the table. Last user to leave was UserId: {userId}.", EventLogEntryType.Information);
            }
            else
            {
                _cache.Remove(userId);
                await _hub.Clients.Group(table.Id.ToString()).SendAsync("UserRemoved", user);
            }

            return true;
        }

        public async Task<bool> UpdateVote(int userId, string clientId, int points)
        {
            bool result = false;

            try
            {
                //Pull table and add user to it
                User user = _cache.Get<User>(userId);

                PointingTable table = _cache.Get<PointingTable>(user.TableId);

                User tableUser = table.Users.Select(u => u).Where(p => p.Id == userId).First();

                if (tableUser.Vote == 0)
                {
                    tableUser.Vote = points;
                    table.VotesRecorded++;
                }
                else
                {
                    tableUser.Vote = points;
                }

                if (table.VotesRecorded == table.TotalVoters)
                {
                    table.IsComplete = true;
                }
                _cache.Set(table.Id, table);

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    await _hub.Clients.Group(table.Id.ToString()).SendAsync("Voted", userId, points);
                }
                else
                {
                    await _hub.Clients.GroupExcept(table.Id.ToString(), clientId).SendAsync("Voted", userId, points);
                }

                if (table.VotesRecorded == table.TotalVoters)
                {
                    await _hub.Clients.Group(table.Id.ToString()).SendAsync("ShowResults", CalculateResult(table));
                }
                result = true;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("ICanHelp", e.Message, EventLogEntryType.Error);
            }
            return result;
        }

        public async Task<bool> ClearVote(Guid tableId)
        {
            return true;
        }

        public async Task<ResetPageHistoryData> NextRound(int tableId, string clientId, int userId)
        {
            PointingTable table = _cache.Get<PointingTable>(tableId);

            ResetPageHistoryData resetData = new ResetPageHistoryData();

            foreach (var user in table.Users)
            {
                if (user.Id == userId)
                    resetData.HistoryFooter = "Table cleared by: " + user.Name;
                if (user.IsVoting)
                {
                    resetData.HistoryBody = string.Concat(resetData.HistoryBody, "[", user.Vote, " - ", user.Name, "]");
                    user.Vote = 0;
                }
            }

            table.VotesRecorded = 0;

            _cache.Set(tableId, table);
            resetData.Extra = "Table cleared";

            if (string.IsNullOrWhiteSpace(clientId))
            {
                await _hub.Clients.Group(table.Id.ToString()).SendAsync("ResetPage", resetData);
            }
            else
            {
                await _hub.Clients.GroupExcept(table.Id.ToString(), clientId).SendAsync("ResetPage", resetData);
            }

            return resetData;
        }

        public async Task<bool> ShowVote(Guid tableId, Guid userId)
        {
            return true;
        }

        private PointingResultData CalculateResult(PointingTable table)
        {
            // PointingTableResponse response = new PointingTableResponse();
            // response.userData = new Dictionary<int, int>();

            PointingResultData result = new PointingResultData();
            Dictionary<int, int> resultData = new Dictionary<int, int>();
            foreach (var user in table.Users)
            {
                if (user.IsVoting)
                {
                    if (resultData.ContainsKey(user.Vote))
                        resultData[user.Vote]++;
                    else
                        resultData.Add(user.Vote, 1);
                    // response.userData.Add(user.Id, user.Vote);
                }
            }

            result.Labels = new List<string>();
            result.datasets = new List<PointingDataSet>();
            PointingDataSet dataset = new PointingDataSet();
            dataset.backgroundColor = new List<string>();
            dataset.data = new List<int>();
            foreach (var entry in resultData)
            {
                switch (entry.Key)
                {
                    case 1:
                        result.Labels.Add("One");
                        dataset.backgroundColor.Add("#eff224");
                        dataset.data.Add(entry.Value);
                        break;
                    case 2:
                        result.Labels.Add("Two");
                        dataset.backgroundColor.Add("#00a65a");
                        dataset.data.Add(entry.Value);
                        break;
                    case 3:
                        result.Labels.Add("Three");
                        dataset.backgroundColor.Add("#6a2af5"); // #f39c12 - Brown
                        dataset.data.Add(entry.Value);
                        break;
                    case 5:
                        result.Labels.Add("Five");
                        dataset.backgroundColor.Add("#00c0ef");
                        dataset.data.Add(entry.Value);
                        break;
                    case 8:
                        result.Labels.Add("Eight");
                        dataset.backgroundColor.Add("#e80000");
                        dataset.data.Add(entry.Value);
                        break;
                    case 100:
                        result.Labels.Add("??");
                        dataset.backgroundColor.Add("#d2d6de");
                        dataset.data.Add(entry.Value);
                        break;
                }
            }
            result.datasets.Add(dataset);
            return result;
        }
    }
}
