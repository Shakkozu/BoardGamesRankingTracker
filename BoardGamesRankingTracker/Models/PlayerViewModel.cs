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

        public Dictionary<GameType, int> RankingPoints { get; set; }

        public DateTime JoinedOn { get; set; }


        public int GamesPlayed { get; set; }

        public int GamesWon { get; set; }

        public int GamesLost { get; set; }

        public int GamesTied { get; set; }

        public float WinRatio { get; set; }
    }
}