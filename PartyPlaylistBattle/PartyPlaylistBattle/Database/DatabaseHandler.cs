using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Database
{
    class DatabaseHandler
    {
        private readonly string connection = "Host=loclhost;Username=postgres;Password=Safetyfirst;Database=PartyPlaylistBattle";


        public void NewUser(string username, string password)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "INSERT INTO user (Username, Password, Admin, Score) VALUES(@username, @password, false, 100";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("user inserted");
                con.Close();
            }
            catch(Exception)
            {
                Console.WriteLine("Error inserting a new User");
            }
        }

        public void DeleteUser(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Delete Statement
                var sql = "DELETE FROM user WHERE Username='" + username + "'";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("user deleted");
                con.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error deleting the User");
            }
        }

        public int GetScore(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Get Score
                var sql = "SELECT Score FROM user WHERE Username='" + username + "'";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                int count = (int)cmd.ExecuteScalar();

                Console.WriteLine("User has a score of" + count);
                con.Close();
                return count;

            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Score of the User");
                return 0; 
            }
        }

        public bool LoginUser(string username, string password)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Get Credentials
                var sql = "SELECT password FROM user WHERE Username='" + username + "'";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                string pw = (string)cmd.ExecuteScalar();

                if(pw==password)
                {
                    //user-login funktion
                    Console.WriteLine("User is logged in");
                    con.Close();
                    return true;
                }
                else
                {
                    Console.WriteLine("Wrong Password");
                    con.Close();
                    return false;
                }                
                
             

            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Score of the User");
                return false;
            }
        }
    }
}
