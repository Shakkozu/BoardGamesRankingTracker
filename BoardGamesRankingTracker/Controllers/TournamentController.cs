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
            
            //Get Player 
            Player player = GlobalConfig.Connection.GetPlayer_ByOwnerId(ownerId);
                
            //Get Lobby
            Lobby lobby = GlobalConfig.Connection.GetLobbyByPrivateKey(privateKey);
            
            //If lobby wasn't found, redirect to create view
            if (lobby == null)
            {
                return RedirectToAction("Create");
            }
            //If lobby was found, join it, and redirect to Lobby View
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

            //Get Player
            string ownerId = User.Identity.GetUserId();

            Player player = GlobalConfig.Connection.GetPlayer_ByOwnerId(ownerId);
            if(player == null)
                return RedirectToAction("Create");

            //If Player, who's trying to get to lobby is within this lobby, show it to him, otherwise redirect to 'Create' view
            var res = lobby.Players.Where(x => x.Id == player.Id).FirstOrDefault();
            if(res == null)
                return RedirectToAction("Create");
            

            return View(mdl);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Leave(string privateKey)
        {
            string ownerId = User.Identity.GetUserId();

            //Get Player 
            Player player = GlobalConfig.Connection.GetPlayer_ByOwnerId(ownerId);

            Lobby lobby = GlobalConfig.Connection.GetLobbyByPrivateKey(privateKey);

            
            GlobalConfig.Connection.RemovePlayerFromLobby(lobby.Id, player.Id);
            return RedirectToAction("Create");


        }
    }
}