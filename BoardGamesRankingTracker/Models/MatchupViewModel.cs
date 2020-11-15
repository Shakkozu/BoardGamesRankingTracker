using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGamesRankingTracker.Models
{
    public class MatchupViewModel
    {
        public List<Player> Players { get; set; }

        public int? Winner { get; set; }

        public DateTime PlayedOn { get; set; }
    }
}