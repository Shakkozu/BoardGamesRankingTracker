using BoardGamesRankingTracker.Models;
using RankingTrackerLibrary.Data;
using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BoardGamesRankingTracker.Controllers
{
    public class TournamentController : Controller
    {
        // GET: Tournament
        public ActionResult Index()
        {
            return View();
        }


        //GET /Tournament/Create
        [Authorize]
        public ActionResult Create()
        {
            TournamentCreateViewModel model = new TournamentCreateViewModel(GlobalConfig.Connection.GetGameNames());
            return View(model);
        }

        //POST: /Tournament/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TournamentCreateViewModel model)
        {
            string creatorId = User.Identity.GetUserId();
            //TOOD Create New Lobby
            return Redirect("Create");//View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Join(string privateKey)
        {
            string userId = User.Identity.GetUserId();

            //TODO Join to lobby

            return RedirectToAction("Create");
        }
    }
}