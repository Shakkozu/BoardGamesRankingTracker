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
            //TODO if player is already in active tournament, redirect to this tournamnet gameplay page
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
            //TOOD Create New Lobby;
            int? lobbyId;
            try
            {
                lobbyId = GlobalConfig.Connection.CreateLobby(lobbyCreator,gameType);
            }
            catch (Exception e)
            {
                string p = e.Message;
                return Redirect("Create");
            }
            //Redirect to this lobby if creation went succesfully
            return RedirectToAction("Lobby",new { lobbyId = lobbyId});//View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Join(string privateKey)
        {
            string ownerId = User.Identity.GetUserId();
            Player player = GlobalConfig.Connection.GetPlayer_ByOwnerId(ownerId);

            //TODO Join to lobby
            Lobby lobby = GlobalConfig.Connection.GetLobbyByPrivateKey(privateKey);
            if (lobby == null)
            {
                return RedirectToAction("Create");
            }
            else
            {
                GlobalConfig.Connection.CreateLobbyEntry(lobby.Id, player.Id);
                return RedirectToAction("Lobby", new { lobbyId = lobby.Id });
            }

        }
        
        [HttpGet]
        [Authorize]
        public ActionResult Lobby(int? lobbyId)
        {
            if (lobbyId == null)
                return RedirectToAction("Create");
            Lobby lobby = GlobalConfig.Connection.GetLobby_ById(lobbyId.GetValueOrDefault());
           
            if(lobby.Active == false)
                return RedirectToAction("Create");
            TournamentLobbyViewModel mdl = new TournamentLobbyViewModel(lobby);

            return View(mdl);
        }
    }
}