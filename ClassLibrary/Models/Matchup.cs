using System.Collections.Generic;

namespace RankingTrackerLibrary.Models
{
    public class Matchup
    {
        /// <summary>
        /// Unique id number for storing in database
        /// </summary>
        public int Id { get; set; }

        public List<Player> Competitors { get; set; }

        /// <summary>
        /// Property used to find winners by database ID
        /// This property is a list, because in case of tie, both players will be marked as winners
        /// </summary>
        public List<int> WinnerId { get; set; }

        /// <summary>
        /// Property is a list, because in case of tie, both players will be makred as winners.
        /// </summary>
        public List<Player> Winner { get; set; }


    }
}