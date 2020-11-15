using BoardGamesRankingTracker.Models;
using Dapper;
using Microsoft.AspNet.Identity;
using RankingTrackerLibrary.Data;
using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BoardGamesRankingTracker.Controllers
{
    public class PlayerController : Controller
    {
        // GET: Player
        public ActionResult Details(int? id, string currentFilter)
        {
            if (String.IsNullOrEmpty(currentFilter))
            {
                currentFilter = "Chess";
            }
            Player result = new Player();
            if (id != null)
            {
                try
                {
                    result = GlobalConfig.Connection.GetPlayer_ById((int)id);
                }
                catch (Exception e)
                {
                    string msg = e.Message;

                    return RedirectToAction("Search", new { Message = PlayerMessages.InvalidId });
                }
            }
            else
            {
                var userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    result = GlobalConfig.Connection.GetPlayer_ByOwnerId(userId);
                }
            }
            if (result.EmailAddress != null)
            {
                PlayerViewModel viewModel = new PlayerViewModel
                {
                    Id = result.Id,
                    JoinedOn = result.Joined,
                    Nickname = result.Nickname,
                    RankingPoints = result.RankingPoints,
                    SelectedGame = currentFilter,
                    GamesLost = result.GamesLost,
                    GamesPlayed = result.GamesPlayed,
                    GamesWon = result.GamesWon,
                    GamesTied = result.GamesTied

                };

                return View(viewModel);
            }
            return RedirectToAction("Search",PlayerMessages.InvalidId);
        }


        //GET : Search
        public ViewResult Search(string searchString, int? page, string currentFilter, PlayerMessages? Message)
        {
            if (String.IsNullOrEmpty(currentFilter))
            {
                currentFilter = "Chess";
            }
            //TODO Fix showing viewbag status
            
            
            List<Player> players = GlobalConfig.Connection.GetPlayers_All();
            List<PlayerViewModel> viewModels = new List<PlayerViewModel>();
            //TODO Update PlayerViewModel Creation (GamesLost,Won, etc. needs to be derived from source)
            players.ForEach(x => viewModels.Add(new PlayerViewModel
            {
                Id = x.Id,
                Nickname = x.Nickname,
                RankingPoints = x.RankingPoints,
                SelectedGame = currentFilter,
                GamesLost = x.GamesLost,
                GamesPlayed = x.GamesPlayed,
                GamesWon=x.GamesWon,
                GamesTied=x.GamesTied
            }));

            if(!String.IsNullOrEmpty(searchString))
            {
                page = 1;
                viewModels = viewModels.Where(s => s.Nickname.Contains(searchString)).ToList();
            }
           
            ViewBag.CurrentFilter = currentFilter;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

           ViewBag.Status = Message == PlayerMessages.InvalidId ? "Nieprawidłowe Id Gracza." : "";
            return View(viewModels.ToPagedList(pageNumber,pageSize));
        }

        public enum PlayerMessages
        {
            InvalidId,
        }
    }
    
}
