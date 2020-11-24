using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingTrackerLibrary.Models
{
    public class Lobby
    {
        public Lobby()
        {
        }

        public Lobby(Player lobbyCreator, string gameType)
        {
            Players.Add(lobbyCreator);
            GameType = gameType;

        }

        public int Id { get; set; }

        public string PrivateKey { get; set; }

        public int MaxPlayers { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();

        public string GameType { get; set; }

        public int GameId { get; set; }



    }
}
