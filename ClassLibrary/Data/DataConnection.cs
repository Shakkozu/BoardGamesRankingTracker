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
        public void CreatePlayer(Player player)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString()))
            {
                var p = new DynamicParameters();
                p.Add("@Nickname", player.Nickname);
                p.Add("@EmailAddress", player.EmailAddress);
                p.Add("@OwnerId", player.OwnerId);
                p.Add("@JoinedOn", player.Joined);
                p.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPlayers_Insert", p, commandType: CommandType.StoredProcedure);
                player.Id = p.Get<int>("@Id");

            }
        }
        public string Test()
        {
            return "Hej, działam!";
        }

        public List<Player> GetPlayers_All()
        {
            List<Player> result = new List<Player>();


            return result;
        }

    }


}
