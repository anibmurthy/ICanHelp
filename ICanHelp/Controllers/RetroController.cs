using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICanHelp.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ICanHelp.Controllers
{
    public class RetroController : Controller
    {
        private IMemoryCache _cache;
        private IRetrospectiveRepository _retroRepo;
        //private IHubContext<PokerHub> _hub;
        //private IPointingPokerRepository _pokerRepo;
        public RetroController(IMemoryCache cache, IRetrospectiveRepository retroRepo)
        {
            _cache = cache;
            _retroRepo = retroRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RetroCreate request)
        {
            int boardId = await _retroRepo.CreateRetroBoard(request);

            return RedirectToAction("Board", new { boardId });
        }

        //public IActionResult Join()
        //{
        //    return View();
        //}

        [Route("Join/{boardId}")]
        public async Task<IActionResult> Join(int boardId)
        {
            return RedirectToAction("Board", new { boardId });
        }

        [HttpGet]
        public async Task<IActionResult> Board(int boardId)
        {
            RetroBoard board = await _retroRepo.GetBoard(boardId);
            return View("Board", board);
        }
    }
}