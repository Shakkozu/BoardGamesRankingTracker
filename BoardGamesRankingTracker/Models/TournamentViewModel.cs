using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGamesRankingTracker.Models
{
    public class TournamentViewModel
    {

    }

    public class TournamentCreateViewModel
    {
        public TournamentCreateViewModel()
        {

        }
        public TournamentCreateViewModel(List<string> gameTypes)
        {
            GameTypes = gameTypes;
        }
        public List<string> GameTypes { get;}


        public string SelectedGame { get; set; }


    }
    public class TournamentLobbyViewModel
    {
        public TournamentLobbyViewModel()
        {
            
        }

        public TournamentLobbyViewModel(Lobby lobby)
        {
            Lobby = lobby;
        }

        public Lobby Lobby { get; }
    }
}