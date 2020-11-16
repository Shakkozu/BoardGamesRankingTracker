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

        public int MaxPlayers { get; set; }

        public string CreatorId { get; set; }
        public string SelectedGame { get; set; }


    }
}