using Dapper;
using RankingTrackerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingTrackerLibrary.Data
{
    public class DataConnection
    {
        //TODO REFACTOR GetPlayer_ Methods (in the name of DRY methodology)
        public void CreatePlayer(Player player)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@Nickname", player.Nickname);
                p.Add("@EmailAddress", player.EmailAddress);
                p.Add("@OwnerId", player.OwnerId);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPlayers_Insert", p, commandType: CommandType.StoredProcedure);
                player.Id = p.Get<int>("@Id");

            }
        }

        public List<string> GetGameNames()
        {
            List<string> result = new List<string>();
            using(SqlConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                result = connection.Query<string>("dbo.spGames_GetAllNames").ToList();
            }
            return result;
        }

        /// <summary>
        /// Creates new lobby in database
        /// </summary>
        /// <param name="lobby">Lobby model containing Creator and GameName</param>
        /// <returns>Created Lobby ID or -1 if failed</returns>
        public int? CreateLobby(Player creator, string gameName)
        {
            int? result = null;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                
                //get game id
                var p = new DynamicParameters();
                Game game = GetGameByGameName(gameName);
                if (game == null)
                    return result;
                p.Add("@GameId", game.Id);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                //get private key
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string privateKey = new string(Enumerable.Repeat(chars, 9).Select(s => s[random.Next(s.Length)]).ToArray());
                p.Add("@PrivateKey", privateKey);

                //create lobby
                connection.Execute("dbo.spLobbies_Insert", p, commandType: CommandType.StoredProcedure);
                result = p.Get<int>("Id");

                //create lobby Entry for creator
                var c = new DynamicParameters();
                c.Add("@PlayerId", creator.Id);
                c.Add("@LobbyId", result.Value);
                connection.Execute("dbo.spLobbyEntries_Insert", c, commandType: CommandType.StoredProcedure);
            }
            return result;
        }

        public void CreateLobbyEntry(int lobbyId, int playerId)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@PlayerId", playerId);
                p.Add("@LobbyId", lobbyId);
                connection.Execute("dbo.spLobbyEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        public Lobby GetLobbyByPrivateKey(string privateKey)
        {
            Lobby result = new Lobby();
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@PrivateKey", privateKey);
                result = connection.Query<Lobby>("dbo.spLobbies_GetByPrivateKey", p, commandType: CommandType.StoredProcedure).FirstOrDefault();

                //If found lobby is invalid, return empty object
                if (result == null)
                    return result;

                
                //Wire up Lobby GameType
                Game game = GetGameByGameId(result.GameId);
                result.GameType = game;

                // Wire Up Lobby Players
                var c = new DynamicParameters();
                c.Add("@Lobby", result.Id);
                List<Player> players = new List<Player>();
                List<LobbyEntry> lobbyEntries = connection.Query<LobbyEntry>("dbo.spLobbyEntries_GetByLobbyId", c, commandType: CommandType.StoredProcedure).ToList();

                foreach (var lobbyEntry in lobbyEntries)
                {
                    int playerId = lobbyEntry.PlayerId;
                    players.Add(GetPlayer_ById(playerId));
                }
                result.Players = players;
            }
            return result;
        }

        public void RemovePlayerFromLobby(int lobbyId, int playerId)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@LobbyId", lobbyId);
                p.Add("@PlayerId", playerId);
                connection.Execute("dbo.spLobbyEntries_Delete",p,commandType:CommandType.StoredProcedure);
            }
        }

        public Lobby GetLobby_ById(int id)
        {
            Lobby result = new Lobby();
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@LobbyId", id);
                result = connection.Query<Lobby>("dbo.spLobbies_GetById", p,commandType: CommandType.StoredProcedure).FirstOrDefault();

                //Wire up Lobby GameType
                Game game = GetGameByGameId(result.GameId);
                result.GameType = game;

                // Wire Up Lobby Players
                var c = new DynamicParameters();
                c.Add("@Lobby", id);
                List<Player> players = new List<Player>();
                List<LobbyEntry> lobbyEntries = connection.Query<LobbyEntry>("dbo.spLobbyEntries_GetByLobbyId",c,commandType: CommandType.StoredProcedure).ToList();

                foreach (var lobbyEntry in lobbyEntries)
                {
                    int playerId = lobbyEntry.PlayerId;
                    players.Add(GetPlayer_ById(playerId));
                }
                result.Players = players;

                
            }
            return result;
        }

        private Game GetGameByGameId(int gameId)
        {
            Game result;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@GameId", gameId);

                result = connection.Query<Game>("dbo.spGames_GetByGameId", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }

        public void CreateGame(string gameName)
        {
            //TODO Remove any spaces from gameName, it's mandatory due to other Game Related Methods/Procedures!
            gameName = gameName.Replace(" ", "");
            throw new NotImplementedException();
        }

        public Game GetGameByGameName(string gameName)
        {
            Game result;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@GameName", gameName);

                result = connection.Query<Game>("dbo.spGames_GetByGameName", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return result;
        }
        public Player GetPlayer_ById(int id)
        {
            Player result;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                Dictionary<string, int> gamesWon = new Dictionary<string, int>();
                Dictionary<string, int> gamesTied = new Dictionary<string, int>();
                Dictionary<string, int> gamesPlayed = new Dictionary<string, int>();
                Dictionary<string, int> gamesLost = new Dictionary<string, int>();

                //Get Player Info
                var p = new DynamicParameters();
                p.Add("@Id", id);
                result = connection.Query<Player>("dbo.spPlayers_GetById", p, commandType: CommandType.StoredProcedure).First();


                //Populate JoinedOn
                var c = new DynamicParameters();
                c.Add("@Id", result.Id);
                DateTime value = connection.Query<DateTime>("dbo.spPlayers_GetJoinedById", c, commandType: CommandType.StoredProcedure).First();
                result.Joined = value;

                //Populate Ranking
                List<Game> games = connection.Query<Game>("dbo.spGames_GetAll").ToList();
                List<Ranking> rankings = connection.Query<Ranking>("dbo.spRankings_GetAll").ToList();
                Dictionary<string, int> playerRanking = new Dictionary<string, int>();
                foreach (Game game in games)
                {
                    IEnumerable<int> point = from ranking in rankings
                                             where ranking.PlayerId == result.Id &&
                                             game.Id == ranking.GameId
                                             select ranking.Points;
                    playerRanking.Add(game.GameName, point.First());

                    //Get Matchups
                    var parameters = new DynamicParameters();
                    parameters.Add("@PlayerId", result.Id);
                    parameters.Add("@GameName", game.GameName);

                    List<Matchup> matchups = connection.Query<Matchup>("dbo.spMatchups_GetByPlayerId", parameters, commandType: CommandType.StoredProcedure).ToList();

                    //games won
                    gamesWon.Add(game.GameName, matchups.Where(x => x.WinnerId == result.Id).Count());

                    //games played
                    gamesPlayed.Add(game.GameName, matchups.Count);


                    //games tied
                    gamesTied.Add(game.GameName, matchups.Where(x => x.WinnerId == null).Count());

                    //games Lost
                    gamesLost.Add(game.GameName, (matchups.Count - (gamesWon[game.GameName] + gamesTied[game.GameName])));

                }
                result.RankingPoints = playerRanking;
                result.GamesWon = gamesWon;
                result.GamesLost= gamesLost;
                result.GamesTied = gamesTied;
                result.GamesPlayed= gamesPlayed;

            }
            return result;
        }

        public Dictionary<string, List<Matchup>> GetMatchups_ByPlayerId(int playerId)
        {
            Dictionary<string, List<Matchup>> result = new Dictionary<string, List<Matchup>>();
            using (SqlConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                
                List<Game> games = connection.Query<Game>("dbo.spGames_GetAll").ToList();
                List<Player> players = connection.Query<Player>("dbo.spPlayers_GetAll").ToList();
                    foreach (Game game in games)
                {
                    //Get Matchups
                    var parameters = new DynamicParameters();
                    parameters.Add("@PlayerId", playerId);
                    parameters.Add("@GameName", game.GameName);
                    //TODO REWORK THIS METHOD SO MATCHUPS GET INFO ABOUT PLAYERS PARTICIPATING
                    List<Matchup> matchups = connection.Query<Matchup>("dbo.spMatchups_GetByPlayerId", parameters, commandType: CommandType.StoredProcedure).ToList();
                    foreach (Matchup matchup in matchups)
                    {
                        //create players list for matchup
                        List<Player> matchupPlayers = new List<Player>();

                        var p = new DynamicParameters();
                        //Get MatchupEntries for this certain matchup
                        p.Add("@MatchupId", matchup.Id);
                        List<MatchupEntry> matchupEntries =
                            connection.Query<MatchupEntry>("dbo.spMatchupEntries_GetByMatchupId", p, commandType: CommandType.StoredProcedure).ToList();
                        //Get players info from matchup Entries
                        foreach (MatchupEntry matchupEntry in matchupEntries)
                        {
                            matchupPlayers.Add(players.Where(x => x.Id == matchupEntry.PlayerCompetingId).FirstOrDefault());
                        }
                        //Add information about participants to matchup
                        matchup.Players = matchupPlayers;
                    }
                    

                    result.Add(game.GameName, matchups);
                }
            }
            return result;
        }

        public List<Player> GetPlayers_All()
        {
            List<Player> result = new List<Player>();

            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
               

                result = connection.Query<Player>("dbo.spPlayers_GetAll", p, commandType: CommandType.StoredProcedure).ToList();

                List<Game> games = connection.Query<Game>("dbo.spGames_GetAll", p, commandType: CommandType.StoredProcedure).ToList();
                List<Ranking> rankings = connection.Query<Ranking>("dbo.spRankings_GetAll").ToList();

                
                foreach (Player player in result)
                {
                    Dictionary<string, int> gamesWon = new Dictionary<string, int>();
                    Dictionary<string, int> gamesTied = new Dictionary<string, int>();
                    Dictionary<string, int> gamesPlayed = new Dictionary<string, int>();
                    Dictionary<string, int> gamesLost = new Dictionary<string, int>();

                    //Populate Rankings
                    Dictionary<string, int> playerRanking = new Dictionary<string, int>();
                    foreach (Game game in games)
                    {
                        IEnumerable<int> point = from ranking in rankings where
                                                 ranking.PlayerId == player.Id &&
                                                 game.Id == ranking.GameId
                                                 select ranking.Points;
                        playerRanking.Add(game.GameName, point.First());

                        player.RankingPoints = playerRanking;


                        //Get Matchups
                        var parameters = new DynamicParameters();
                        parameters.Add("@PlayerId", player.Id);
                        parameters.Add("@GameName", game.GameName);

                        List<Matchup> matchups = connection.Query<Matchup>("dbo.spMatchups_GetByPlayerId", parameters, commandType: CommandType.StoredProcedure).ToList();

                        //games won
                        gamesWon.Add(game.GameName,matchups.Where(x => x.WinnerId == player.Id).Count());

                        //games played
                        gamesPlayed.Add(game.GameName,matchups.Count);


                        //games tied
                        gamesTied.Add(game.GameName,matchups.Where(x => x.WinnerId == null).Count());

                        //games Lost
                        gamesLost.Add(game.GameName, (matchups.Count - (gamesWon[game.GameName] + gamesTied[game.GameName])));

                    }
                    player.GamesPlayed = gamesPlayed;
                    player.GamesTied = gamesTied;
                    player.GamesWon = gamesWon;
                    player.GamesLost = gamesLost;
                }
            }
            return result;
        }

        public Player GetPlayer_ByOwnerId(string ownerId)
        {
            Player result;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                Dictionary<string, int> gamesWon = new Dictionary<string, int>();
                Dictionary<string, int> gamesTied = new Dictionary<string, int>();
                Dictionary<string, int> gamesPlayed = new Dictionary<string, int>();
                Dictionary<string, int> gamesLost = new Dictionary<string, int>();

                //Get Player Info
                var p = new DynamicParameters();
                p.Add("@OwnerId", ownerId);
                result = connection.Query<Player>("dbo.spPlayers_GetByOwnerId", p, commandType: CommandType.StoredProcedure).First();


                //Populate JoinedOn
                var c = new DynamicParameters();
                c.Add("@Id", result.Id);
                DateTime value = connection.Query<DateTime>("dbo.spPlayers_GetJoinedById", c, commandType: CommandType.StoredProcedure).First();
                result.Joined = value;

                //Populate Ranking
                List<Game> games = connection.Query<Game>("dbo.spGames_GetAll").ToList();
                List<Ranking> rankings = connection.Query<Ranking>("dbo.spRankings_GetAll").ToList();
                Dictionary<string, int> playerRanking = new Dictionary<string, int>();
                foreach (Game game in games)
                {
                    IEnumerable<int> point = from ranking in rankings
                                             where ranking.PlayerId == result.Id &&
                                             game.Id == ranking.GameId
                                             select ranking.Points;
                    playerRanking.Add(game.GameName, point.First());

                    //Get Matchups
                    var parameters = new DynamicParameters();
                    parameters.Add("@PlayerId", result.Id);
                    parameters.Add("@GameName", game.GameName);

                    List<Matchup> matchups = connection.Query<Matchup>("dbo.spMatchups_GetByPlayerId", parameters, commandType: CommandType.StoredProcedure).ToList();

                    //games won
                    gamesWon.Add(game.GameName, matchups.Where(x => x.WinnerId == result.Id).Count());

                    //games played
                    gamesPlayed.Add(game.GameName, matchups.Count);


                    //games tied
                    gamesTied.Add(game.GameName, matchups.Where(x => x.WinnerId == null).Count());

                    //games Lost
                    gamesLost.Add(game.GameName, (matchups.Count - (gamesWon[game.GameName] + gamesTied[game.GameName])));

                }
                result.RankingPoints = playerRanking;
                result.GamesWon = gamesWon;
                result.GamesLost = gamesLost;
                result.GamesTied = gamesTied;
                result.GamesPlayed = gamesPlayed;

            }

            return result;
        }

        

    }


}
