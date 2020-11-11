using System.Collections.Generic;

namespace RankingTrackerLibrary.Models
{
    public class Matchup
    {
        public int Id { get; set; }

        public List<Player> Competitors { get; set; }

        public Player Winner { get; set; }


    }
}