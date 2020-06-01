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
        private IPointingPokerRepository _pokerRepo;

        public PokerApiController(IMemoryCache cache, IHubContext<PokerHub> hub, IPointingPokerRepository pokerRepo)
        {
            _cache = cache;
            _hub = hub;
            _pokerRepo = pokerRepo;
        }

        [HttpGet]
        [Route("Vote/{userId}")]
        public async Task<IActionResult> Vote(int userId, int vote)
        {
            if (userId < 1 && vote < 1)
            {
                return BadRequest("userId and/or vote cannot be 0!");
            }

            if (!await _pokerRepo.IsUserExists(userId))
            {
                return BadRequest("User ID does not exist!");
            }

            string clientId = Request.Headers["x-conn-id"];

            if (await _pokerRepo.UpdateVote(userId, clientId, vote))
                return Ok("Vote added or updated");
            else
                return new ObjectResult("Could not add or update vote") { StatusCode = 500 };

            //PointingTable table = _cache.Get<PointingTable>(user.TableId);

            //User tableUser = table.Users.Select(u => u).Where(p => p.Id == userId).First();

            //if (tableUser.Vote == 0)
            //{
            //    tableUser.Vote = vote;
            //    table.VotesRecorded++;
            //}
            //else
            //{
            //    tableUser.Vote = vote;
            //}

            //_cache.Set(table.Id, table);
            //string clientId = Request.Headers["x-conn-id"];
            //if (string.IsNullOrWhiteSpace(clientId))
            //{
            //    await _hub.Clients.Group(table.Id.ToString()).SendAsync("Voted", userId, vote);
            //}
            //else
            //{
            //    await _hub.Clients.GroupExcept(table.Id.ToString(), clientId).SendAsync("Voted", userId, vote);
            //}

            //if (table.VotesRecorded == table.TotalVoters)
            //{
            //    await _hub.Clients.Group(table.Id.ToString()).SendAsync("ShowResults", CalculateResult(table));
            //    //await _hub.Clients.Group(table.Id.ToString()).SendAsync("AddVotesToTable", data.userData);
            //}
        }

        [HttpGet]
        [Route("NextRound/{tableId}")]
        public async Task<IActionResult> NextRound(int tableId, int userId)
        {
            if (tableId < 1 || !await _pokerRepo.IsTableExists(tableId))
            {
                return BadRequest("Invalid tableId!");
            }

            if (userId < 1 || !await _pokerRepo.IsUserExists(userId))
            {
                return BadRequest("Invalid userId!");
            }

            string clientId = Request.Headers["x-conn-id"];
            var result = await _pokerRepo.NextRound(tableId, clientId, userId);

            return Ok(result);

            //PointingTable table = _cache.Get<PointingTable>(tableId);

            //ResetPageHistoryData resetData = new ResetPageHistoryData();

            //foreach (var user in table.Users)
            //{
            //    if (user.Id == userId)
            //        resetData.HistoryFooter = "Table cleared by: " + user.Name;
            //    if (user.IsVoting)
            //    {
            //        resetData.HistoryBody = string.Concat(resetData.HistoryBody, "[", user.Vote, " - ", user.Name, "]");
            //        user.Vote = 0;
            //    }
            //}

            //table.VotesRecorded = 0;

            //_cache.Set(tableId, table);
            //resetData.Extra = "Table cleared";

            //string clientId = Request.Headers["x-conn-id"];
            //if (string.IsNullOrWhiteSpace(clientId))
            //{
            //    await _hub.Clients.Group(table.Id.ToString()).SendAsync("ResetPage", resetData);
            //}
            //else
            //{
            //    await _hub.Clients.GroupExcept(table.Id.ToString(), clientId).SendAsync("ResetPage", resetData);
            //}
        }

        [HttpGet]
        [Route("UpdateJira/{tableId}")]
        public async Task<IActionResult> UpdateJira(string tableId, string data)
        {
            await _hub.Clients.GroupExcept(tableId, Request.Headers["x-conn-id"]).SendAsync("UpdateJira", data);
            return Ok($"Server received Jira: {data}");
        }

        [HttpGet]
        [Route("UpdateDesc/{tableId}")]
        public async Task<IActionResult> UpdateDesc(string tableId, string data)
        {
            await _hub.Clients.GroupExcept(tableId, Request.Headers["x-conn-id"]).SendAsync("UpdateDesc", data);
            return Ok($"Server received Desc: {data}");
        }
    }
}