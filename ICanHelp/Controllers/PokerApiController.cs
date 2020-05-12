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

            _cache.Set(table.Id, table);
            await _hub.Clients.Group(table.Id.ToString()).SendAsync("Voted", userId);

            return Ok(true);
        }
    }
}