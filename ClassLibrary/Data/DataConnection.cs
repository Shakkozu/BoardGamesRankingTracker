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
