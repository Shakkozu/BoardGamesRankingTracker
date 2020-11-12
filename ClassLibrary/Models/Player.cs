using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingTrackerLibrary.Models
{
    public class  Player 
    {

        public int Id { get; set; }

        /// <summary>
        /// Property used to call to a registered account.
        /// </summary>
        public string OwnerId { get; set; }

        public string Nickname { get; set; }

        public string EmailAddress { get; set; }

        public Dictionary<GameType,int> RankingPoints { get; set; }

        public int GamesPlayed { get; set; }

        public int GamesWon { get; set; }

        public int GamesTied { get; set; }

        public float WinPercentage { get; set; }

        public DateTime Joined { get; set; }








    }
}
