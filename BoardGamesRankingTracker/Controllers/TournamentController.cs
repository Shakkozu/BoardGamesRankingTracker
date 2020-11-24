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

            //get creator
            Player lobbyCreator = GlobalConfig.Connection.GetPlayer_ByOwnerId(creatorId);
            //get gameType
            string gameType = model.SelectedGame;

            //TOOD Create New Lobby
            Lobby lobby = new Lobby(lobbyCreator, gameType);
            try
            {
                lobby.Id = GlobalConfig.Connection.CreateLobby(lobby);
            }
            catch (Exception e)
            {
                string p = e.Message;
                return Redirect("Create");
            }
            //Redirect to this lobby if creation went succesfully
            return RedirectToAction("Lobby",lobby.Id);//View();
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
        
        [Authorize]
        public ActionResult Lobby(int? lobbyId)
        {
            Lobby lobby = GlobalConfig.Connection.GetLobby_ById(lobbyId.GetValueOrDefault());
           
            TournamentLobbyViewModel mdl = new TournamentLobbyViewModel(lobby);
            return View();
        }
    }
}