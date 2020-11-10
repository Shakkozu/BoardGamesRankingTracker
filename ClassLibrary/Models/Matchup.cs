using System.Collections.Generic;

namespace RankingTrackerLibrary.Models
{
    public class Matchup
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public List<Player> Competitors { get; set; }

        public int WinnerId { get; set; }

    }
}