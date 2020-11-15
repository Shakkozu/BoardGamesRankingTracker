using System.Collections.Generic;

namespace RankingTrackerLibrary.Models
{
    public class Matchup
    {
        /// <summary>
        /// Unique id number for storing in database
        /// </summary>
        public int Id { get; set; }

        public int TournamentId { get; set; }

        /// <summary>
        /// Property used to find winners by database ID
        /// This property is a list, because in case of tie, both players will be marked as winners
        /// </summary>
        public int? WinnerId { get; set; }

        /// <summary>
        /// Property is a list, because in case of tie, both players will be makred as winners.
        /// </summary>
        public bool Finished { get; set; }


    }
}