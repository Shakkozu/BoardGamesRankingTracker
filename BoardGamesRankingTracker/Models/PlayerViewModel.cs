using RankingTrackerLibrary;
using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BoardGamesRankingTracker.Models
{
    public class PlayerViewModel
    {
        public int Id { get; set; }

        public string Nickname { get; set; }

        public string EmailAddress { get; set; }

        public Dictionary<string, int> RankingPoints { get; set; }

        public DateTime JoinedOn { get; set; }


        public Dictionary<string, int> GamesPlayed { get; set; }

        public Dictionary<string, int> GamesWon { get; set; }

        public Dictionary<string, int> GamesLost { get; set; }

        public Dictionary<string, int> GamesTied { get; set; }

        public float WinRatio { get; set; }

        public string SelectedGame { get; set; }
    }

}