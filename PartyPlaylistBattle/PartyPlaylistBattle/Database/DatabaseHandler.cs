using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Database
{
    public class DatabaseHandler
    {
        private readonly string connection;
        private string currentAdmin;

        public DatabaseHandler()
        {
            connection = "Host=localhost;Username=postgres;Password=Safetyfirst;Database=PartyPlaylistBattle";
        }

        //User-Functions
        public bool NewUser(string username, string password)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "INSERT INTO users (username, password, admin, score) VALUES(@username, @password, false, 100)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("user inserted");
                con.Close();
                return true;

            }
            catch(Exception)
            {
                Console.WriteLine("Error inserting a new User");
                return false;
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
                var sql = "DELETE FROM users WHERE username='" + username + "'";
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

        

        public bool LoginUser(string username, string password)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Get Credentials
                var sql = "SELECT COUNT(*) FROM users WHERE username= @username AND password = @password";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                int count = 0;
                while(reader.Read())
                {
                    count = reader.GetInt32(0);

                }
                if (count != 0)
                {
                    con.Close();
                    return true;
                }
                else
                {
                    con.Close();
                    return false;
                }
         
            }
            catch (Exception)
            {
                Console.WriteLine("Error logging in the User");
                return false;
            }
        }

        public bool ChangeUser(string username, string password, string newname)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Update Statement
                var sql = "UPDATE users SET username= @newname, password= @password WHERE username= @username)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("newname", newname);
                cmd.Prepare();

                int n = cmd.ExecuteNonQuery();
                con.Close();

                if(n==1)
                {
                    Console.WriteLine("user updated");
                    return true;
                }
                else
                {
                    Console.WriteLine("Error updating User (in try)");
                    return false;
                }
                
                
            }
            catch (Exception)
            {
                Console.WriteLine("Error updating User Data");
                return false;
            }
        }

        //MediaLibrary-Functions
        public void AddMediaLink()
        { 
        
        }

        public string GetMediaLink()
        {
            return "0";
        }
        public void DeleteMediaLink()
        {

        }

        public void AddToPlaylist()
        {

        }


        //Get-Functions and Debug-Functions
        public string GetUserData(string username)
        {
            try
            {               
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT username, score, admin FROM users WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                string data = "";
                while(reader.Read())
                {
                    data += "\n" + reader.GetString(0) + " /Score: " + reader.GetInt32(1).ToString() +
                    " /Is Admin: " + reader.GetBoolean(2).ToString();
                }
                con.Close();
                return data;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Score of the User");
                return "0";
            }
        }

        public void SetAdmin(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "UPDATE admin FROM users WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();               
                con.Close();
                this.currentAdmin = username;
            }
            catch (Exception)
            {
                Console.WriteLine("Error setting Admin");
            }
        }

        public void ResetAdmin()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Delete Statement
                var sql = "UPDATE users SET admin = false)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Admin Reset");
                con.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error reseting the Admin");
            }
        }
        public string GetScoreboard()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT username, score FROM users ORDER BY score desc";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                int x = 1;
                string scoreboard = "----------Scoreboard----------";

                while (reader.Read())
                {
                    scoreboard += "\n" + x.ToString() + ". Place: " + reader.GetString(0) + " /Points: " + reader.GetInt32(1).ToString();
                    x++;
                }
                con.Close();
                return scoreboard;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Score of the User");
                return "0";
            }
        }

        public bool UserExists(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Sql Statement
                var query = "SELECT COUNT(*) FROM users WHERE username = @name";
                using NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                int count = 0;
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                if (count != 0)
                {
                    con.Close();
                    return true;
                }
                else
                {
                    con.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error finding the User");
                return false;
            }
        }
    }
}
