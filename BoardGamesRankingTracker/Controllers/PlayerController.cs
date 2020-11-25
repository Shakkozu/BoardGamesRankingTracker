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
        public ActionResult Details(int? id, string currentFilter, int? page)
        {
            //Set default filter is none is given
            if (String.IsNullOrEmpty(currentFilter))
            {
                currentFilter = "Chess";
            }

            Player result = new Player();
            //If player was selected from search list / id was given
            if (id != null)
            {
                try
                {
                    result = GlobalConfig.Connection.GetPlayer_ById((int)id);
                }
                //in case there's no player with given id redirect to search
                catch (Exception e)
                {
                    string msg = e.Message;

                    return RedirectToAction("Search", new { Message = PlayerMessages.InvalidId });
                }
            }
            //if user is logged in, redirect to his account page
            else
            {
                var userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    result = GlobalConfig.Connection.GetPlayer_ByOwnerId(userId);
                }
            }
            //if player has been found get information about him
            if (result.EmailAddress != null)
            {

                Dictionary<string, List<Matchup>> matchups = GlobalConfig.Connection.GetMatchups_ByPlayerId(result.Id);


                Dictionary<string, PagedList.IPagedList<MatchupViewModel>> matchupViewModels = new Dictionary<string, IPagedList<MatchupViewModel>>();
                //TODO Convert Matchup model to matchup model
                int pageSize = 3;
                int pageNumber = (page ?? 1);

                //convert list of matchup model to list of matchupviewmodel
                foreach (string key in matchups.Keys)
                {
                    matchupViewModels.Add(key, ConvertMatchupListToMatchupViewModelsList(matchups[key], pageNumber, pageSize));
                }



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
                    GamesTied = result.GamesTied,
                    PlayerMatchups = matchupViewModels
                };

                return View(viewModel);
            }
            return RedirectToAction("Search",PlayerMessages.InvalidId);
        }

        private static PagedList.IPagedList<MatchupViewModel> ConvertMatchupListToMatchupViewModelsList(List<Matchup> matchups, int pageNumber, int pageSize)
        { 
            List<MatchupViewModel> vm = new List<MatchupViewModel>();
            foreach (var item in matchups)
            {
                vm.Add(new MatchupViewModel { PlayedOn = item.PlayedOn, Players = item.Players, Winner = item.WinnerId });
            }
            return vm.ToPagedList(pageNumber,pageSize);
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
