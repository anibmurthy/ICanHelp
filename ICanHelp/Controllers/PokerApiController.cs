using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICanHelp.Contracts;
using ICanHelp.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace ICanHelp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokerApiController : ControllerBase
    {
        private IMemoryCache _cache;
        private IHubContext<PokerHub> _hub;

        public PokerApiController(IMemoryCache cache, IHubContext<PokerHub> hub)
        {
            _cache = cache;
            _hub = hub;
        }

        [HttpGet]
        [Route("Vote/{userId}")]
        public async Task<IActionResult> Vote(int userId, int vote)
        {
            if (userId < 1 && vote < 1)
            {
                return BadRequest("userId and/or vote cannot be 0!");
            }

            User user = _cache.Get<User>(userId);

            if (user == null)
            {
                return BadRequest("User ID does not exist!");
            }
            PointingTable table = _cache.Get<PointingTable>(user.TableId);

            User tableUser = table.Users.Select(u => u).Where(p => p.Id == userId).First();

            if (tableUser.Vote == 0)
            {
                tableUser.Vote = vote;
                table.VotesRecorded++;
            }
            else
            {
                tableUser.Vote = vote;
            }

            _cache.Set(table.Id, table);
            await _hub.Clients.Group(table.Id.ToString()).SendAsync("Voted", userId, vote);
            if (table.VotesRecorded == table.TotalVoters)
            {
                await _hub.Clients.Group(table.Id.ToString()).SendAsync("ShowResults", CalculateResult(table));
                //await _hub.Clients.Group(table.Id.ToString()).SendAsync("AddVotesToTable", data.userData);
            }

            return Ok(true);
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
                        dataset.backgroundColor.Add("#f39c12");
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
            // response.chartData = new PointingResultData();
            //  response.chartData = result;
            return result;
        }

        [HttpGet]
        [Route("NextRound/{tableId}")]
        public async Task<IActionResult> NextRound(int tableId, int userId)
        {
            if (tableId < 1 && userId < 1)
            {
                return BadRequest("Invalid tableId and/or userId!");
            }

            PointingTable table = _cache.Get<PointingTable>(tableId);

            foreach (var user in table.Users)
            {
                user.Vote = 0;
            }
            table.VotesRecorded = 0;

            _cache.Set(tableId, table);

            await _hub.Clients.Group(table.Id.ToString()).SendAsync("ResetPage", userId);

            return Ok("Table cleared");
        }
    }
}