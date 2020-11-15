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

        public Dictionary<string,int> RankingPoints { get; set; }

        public Dictionary<string,int> GamesPlayed { get; set; }

        public Dictionary<string, int> GamesWon { get; set; }

        public Dictionary<string, int> GamesTied { get; set; }

        public Dictionary<string, int> GamesLost { get; set; }

        public Dictionary<string, float> WinPercentage { get; set; }

        public DateTime Joined { get; set; }








    }
}
