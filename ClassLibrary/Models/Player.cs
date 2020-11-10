using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingTrackerLibrary.Models
{
    public class Player
    {
        public int Id { get; set; }

        public string Nickname { get; set; }

        public string EmailAddress { get; set; }

        public Dictionary<GameType,int> RankingPoints { get; set; }




    }
}
