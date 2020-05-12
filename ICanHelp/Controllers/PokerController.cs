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
    public class PokerController : Controller
    {
        private IMemoryCache _cache;
        private IHubContext<PokerHub> _hub;
        //private List<PointingTable> _tables = new List<PointingTable>();

        public PokerController(IMemoryCache cache, IHubContext<PokerHub> hub)
        {
            _cache = cache;
            _hub = hub;
        }

        // GET: Poker
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateTable request)
        {
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
            return View("Pointing", result);

            //    return RedirectToAction(nameof(Pointing), new { User = user });
        }

        [HttpPost]
        public async Task<IActionResult> Join(JoinTable request)
        {
            //Pull table and add user to it
            PointingTable table = _cache.Get<PointingTable>(request.TableId);

            //Add error handling to deal with invalid session cases

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
            return View("Pointing", result);
            // return RedirectToAction(nameof(Pointing), new { User = user });
        }

        // GET: Poker/Details/5
        [HttpGet]
        public async Task<IActionResult> Pointing(User currentUser)
        {
            var table = _cache.Get<PointingTable>(currentUser.TableId);
            PointingTableDom result = new PointingTableDom(table);
            result.CurrentUser = currentUser;
            return View(result);
        }
    }
}

//// GET: Poker/CreateTable
//public ActionResult CreateTable()
//{
//    return View();
//}

//// POST: Poker/Create
//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult CreateTable(PointingTable request)
//{
//    try
//    {
//        // TODO: Add insert logic here
//        Random rnd = new Random();
//        request.Id = rnd.Next(0, 99999);

//        //Stub data
//        request.Users = new List<User>();
//        request.Users.Add(new User { Name = "Anirudh" });
//        request.Users.Add(new User { Name = "Anvi" });
//        request.Users.Add(new User { Name = "Vaishnavi" });
//        request.Users.Add(new User { Name = "Dummi" });
//        request.Users.Add(new User { Name = "Omkara" });

//        _cache.Set(request.Id, request);
//        return RedirectToAction(nameof(Pointing), new { id = request.Id });
//    }
//    catch
//    {
//        return View();
//    }
//}

//// GET: Poker/Edit/5
//public ActionResult Edit(int id)
//{
//    return View();
//}

//// POST: Poker/Edit/5
//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult Edit(int id, IFormCollection collection)
//{
//    try
//    {
//        // TODO: Add update logic here

//        return RedirectToAction(nameof(Index));
//    }
//    catch
//    {
//        return View();
//    }
//}

//// GET: Poker/Delete/5
//public ActionResult Delete(int id)
//{
//    return View();
//}

//// POST: Poker/Delete/5
//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult Delete(int id, IFormCollection collection)
//{
//    try
//    {
//        // TODO: Add delete logic here

//        return RedirectToAction(nameof(Index));
//    }
//    catch
//    {
//        return View();
//    }
//}