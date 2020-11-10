using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingTrackerLibrary.Models
{
    class Lobby
    {
        public int Id { get; set; }

        public string PrivateKey { get; set; }

        public int MaxPlayers { get; set; }

        public List<Player> Players { get; set; }


    }
}
