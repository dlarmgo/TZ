using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace TZ.Model.Account
{
    public static class Account
    {
        public static readonly ILogger _logger = Program.LF.CreateLogger("Account");
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static bool SignedState { get; set; }


        public static void LogOut()
        {
            SignedState = false;
        }

        public static void TryToLogIn()
        {
            if (Login != "")
            {
                int ans = 0;
                try
                {
                    using (SqlConnection Connection = new SqlConnection(Program.builder.ConnectionString))
                    {
                        _logger.LogInformation(Program.builder.ConnectionString);
                        Connection.Open();
                        string sqlQuery = $"select case when EXISTS (SELECT * FROM Accounts WHERE Account=\'{Login}\' AND Password=\'{Password}\') then 1 else 0 end";
                        _logger.LogInformation(sqlQuery);
                        using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                        {
                            ans = (int)command.ExecuteScalar();
                        }
                    }



                }
                catch (System.Exception ex)
                {
                    _logger.LogInformation($"Something wrong with DB connection in TryToRegistrate! {ex.Message}");
                }
                _logger.LogInformation($"Done with: TryToLogIn, answer is {ans}");
                if (ans == 1)
                {
                    SignedState = true;
                }

            }
        }

        public static void addScore(int Score)
        {
            if (SignedState == true)
            {
                _logger.LogInformation($"game ended with scores {Score} for {Login}");
                string sqlQueryGet = $"SELECT Score FROM dbo.Scores  WHERE Login=\'{Login}\'";
                string sqlQueryPut = $"INSERT INTO dbo.Scores (Login, Score) VALUES (\'{Login}\', \'{Score}\')";
                try
                {
                    using (SqlConnection Connection = new SqlConnection(Program.builder.ConnectionString))
                    {
                        Connection.Open();
                        //int ans;
                        //using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                        //{
                        //    _logger.LogInformation($"TEST");

                        //    using (SqlDataReader reader = command.ExecuteReader())
                        //    {
                        //        reader.Read();
                        //        ans = reader.GetInt32(0);
                        //        _logger.LogInformation($"get {ans}");
                        //    }
                        //}
                        //if (Score > ans)
                        //{
                            using (SqlCommand command_put = new SqlCommand(sqlQueryPut, Connection))
                            {
                                command_put.ExecuteNonQuery();
                            }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"{ex.Message}");
                }

            }
        }

        public static void TryToRegistrate()
        {
            if (SignedState == false)
            {

                try
                {
                    using (SqlConnection Connection = new SqlConnection(Program.builder.ConnectionString))
                    {
                        Connection.Open();
                        string sqlQuery = $"INSERT INTO dbo.Accounts (Account, Password) VALUES (\'{Login}\', \'{Password}\')";
                        using (SqlCommand command = new SqlCommand(sqlQuery, Connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    SignedState = true;


                }
                catch (System.Exception ex)
                {
                    _logger.LogInformation($"Something wrong with DB connection in TryToRegistrate! {ex.Message}");
                }
                _logger.LogInformation("Done");

            }
        }


        static Account()
        {
            SignedState = false;
        }
    }
}
