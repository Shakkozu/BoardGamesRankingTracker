using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingTrackerLibrary.Models
{
    public class Tournament
    {
        public int Id { get; set; }

        public List<Player> Players { get; set; }

        public List<Matchup> Matchups { get; set; }

        public int NumberOfBoards { get; set; }

        public int AvailableBoards { get; set; }

        public GameType GameType { get; set; }

        public Player Winner { get; set; }

    }
}
