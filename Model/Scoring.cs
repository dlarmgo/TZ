using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace TZ.Model
{
    public static class Scoring
    {
        private static readonly ILogger _logger = Program.LF.CreateLogger("Scores");
        private static int numberOfTop = 10;
        private static int numberOfAll = 1000;
        public static List<Tuple<string, int>> ScoresList = getScoresFullList(numberOfTop);
        //public static Dictionary<string, int> ScoresDict = getHighestScores(numberOfTop);
        private static Dictionary<string, int> _ScoresDict = getHighestScores(numberOfTop);
        public static Dictionary<string, int> ScoresDict
        {
            get { return _ScoresDict; }
            set { _ScoresDict = value; }

        };

        public static void updateScores()
        {
            ScoresDict = getHighestScores(numberOfTop);
            ScoresList = getScoresFullList(numberOfTop);
        }

        private static Dictionary<string, int> getHighestScores(int count)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            try
            {
                using (SqlConnection Connection = new SqlConnection(Program.builder.ConnectionString))
                {
                    Connection.Open();
                    string sqlQuery = $"SELECT TOP {count} Login, MAX(Score) Score FROM Scores GROUP BY Login";
                    using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string User = reader.GetString(0).TrimEnd().ToString();
                                int Score = reader.GetInt32(1);
                                dict.Add(User, Score);
                                //_logger.LogInformation($"Got from DB: User - {User}, Score - {Score}");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogInformation($"Something wrong with DB connection in getScores! {ex.Message}");
            }
            _logger.LogInformation("Done");
            return dict;
        }

        private static List<Tuple<string, int>> getScoresFullList(int count)
        {
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            try
            {
                using (SqlConnection Connection = new SqlConnection(Program.builder.ConnectionString))
                {
                    Connection.Open();
                    string sqlQuery = $"SELECT TOP {numberOfAll} Login, Score from Scores";
                    using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string User = reader.GetString(0).TrimEnd().ToString();
                                int Score = reader.GetInt32(1);
                                list.Add((User, Score).ToTuple());
                                //_logger.LogInformation($"Got from DB: User - {User}, Score - {Score}");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogInformation($"Something wrong with DB connection in getScores! {ex.Message}");
            }
            _logger.LogInformation("Done");
            list.Sort((a, b) =>
            {
                return b.Item2.CompareTo(a.Item2);
            });
            return list;
        }




    }
}
