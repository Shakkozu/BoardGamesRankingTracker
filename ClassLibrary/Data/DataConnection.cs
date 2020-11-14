﻿using Dapper;
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
                                             where
                    ranking.PlayerId == result.Id &&
                    game.Id == ranking.GameId
                                             select ranking.Points;
                    playerRanking.Add(game.GameName, point.First());
                }
                result.RankingPoints = playerRanking;
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
                //TODO Update Player Ranking Dicitonary
                List<Game> games = connection.Query<Game>("dbo.spGames_GetAll", p, commandType: CommandType.StoredProcedure).ToList();
                List<Ranking> rankings = connection.Query<Ranking>("dbo.spRankings_GetAll").ToList();
                foreach (Player player in result)
                {
                    //Populate Rankings
                    Dictionary<string, int> playerRanking = new Dictionary<string, int>();
                    foreach (Game game in games)
                    {
                        IEnumerable<int> point = from ranking in rankings where 
                                                 ranking.PlayerId == player.Id &&
                                                 game.Id == ranking.GameId
                                                 select ranking.Points;
                        playerRanking.Add(game.GameName, point.First());
                    }
                    player.RankingPoints = playerRanking;
                }
                
            }
            return result;
        }

        public Player GetPlayer_ByOwnerId(string ownerId)
        {
            Player result;
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@OwnerId", ownerId);
                result = connection.Query<Player>("dbo.spPlayers_GetByOwnerId", p, commandType: CommandType.StoredProcedure).First();
                
                //Populate JoinedOn
                var c = new DynamicParameters();
                c.Add("@Id", result.Id);
                DateTime value = connection.Query<DateTime>("dbo.spPlayers_GetJoinedById", c, commandType: CommandType.StoredProcedure).First();
                result.Joined = value;

                //Populate Ranking
                List<Game> games = connection.Query<Game>("dbo.spGames_GetAll", p, commandType: CommandType.StoredProcedure).ToList();
                List<Ranking> rankings = connection.Query<Ranking>("dbo.spRankings_GetAll").ToList();
                Dictionary<string, int> playerRanking = new Dictionary<string, int>();
                foreach (Game game in games)
                {
                    IEnumerable<int> point = from ranking in rankings
                                             where
                    ranking.PlayerId == result.Id &&
                    game.Id == ranking.GameId
                                             select ranking.Points;
                    playerRanking.Add(game.GameName, point.First());
                }
                result.RankingPoints = playerRanking;

            }
            return result;
        }

    }


}
