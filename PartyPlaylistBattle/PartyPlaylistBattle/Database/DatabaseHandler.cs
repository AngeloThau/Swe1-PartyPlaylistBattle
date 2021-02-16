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
                var sql = "INSERT INTO users (username, password, admin, score, actions, bio, image, name) VALUES(@username, @password, false, 100, @actions, '', '', '')";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("actions", "RRRRR");

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

        public bool ChangeActions(string username, string actions)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "UPDATE users SET actions= @actions WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("actions", actions);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Actions Updated");
                con.Close();
                return true;

            }
            catch (Exception)
            {
                Console.WriteLine("Error updating actions");
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
                var sql = "DELETE FROM users WHERE username=@username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("User deleted");
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

        public bool ChangeUser(string username, string newname, string bio, string image)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Update Statement
                var sql = "UPDATE users SET name= @newname, bio = @bio, image = @image WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("bio", bio);
                cmd.Parameters.AddWithValue("image", image);
                cmd.Parameters.AddWithValue("newname", newname);
                cmd.Prepare();

                int n = cmd.ExecuteNonQuery();
                con.Close();

                if (n == 1)
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
                Console.WriteLine("Error changing User Data");
                return false;
            }
        }

        //MediaLibrary-Functions
        public bool AddToLibrary(string name, string user, string url, int rating, string genre, string title, string length, string album)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "INSERT INTO library (name, user, url, rating, genre, title, length, album) VALUES(@name, @user, @url, @rating, @genre, @title, @length, @album)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("user", user);
                cmd.Parameters.AddWithValue("url", url);
                cmd.Parameters.AddWithValue("rating", rating);
                cmd.Parameters.AddWithValue("genre", genre);
                cmd.Parameters.AddWithValue("title", title);
                cmd.Parameters.AddWithValue("length", length);
                cmd.Parameters.AddWithValue("album", album);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Media added to Userlibrary");
                con.Close();
                return true;

            }
            catch (Exception)
            {
                Console.WriteLine("Error inserting the Media");
                return false;
            }
        }

        public string GetMediaLibrary(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT name, url, rating, genre, title, length, album FROM library WHERE user = @user";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("user", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                int x = 1;
                string library = "\n----------Library----------\n";

                while (reader.Read())
                {
                    library += "\nName: " + reader.GetString(0) + " /Url: " + reader.GetString(1) + " /Rating: " + reader.GetInt32(2).ToString() + " /Genre: " + reader.GetString(3) + " /Title: " + reader.GetString(4) + " /Length: " + reader.GetString(5) + " /Album: " + reader.GetString(6);
                    x++;
                }
                con.Close();
                return library;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Library of the User");
                return "0";
            }
        }

        public string GetMediaUrl(string username, string name)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT url FROM library WHERE user = @user AND name = @name";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("user", username);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                string url = reader.GetString(0);

                con.Close();
                return url;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Url of Media");
                return "0";
            }
        }
        public bool DeleteMediaFromLibrary(string username, string name)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Delete Statement
                var sql = "DELETE FROM library WHERE user=@username AND name=@name";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Media deleted");
                con.Close();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error deleting the Media");
                return false;
            }
        }

        public bool AddToPlaylist(string songname, string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                string url = GetMediaUrl(username, songname);
                //instert Statement
                var sql = "INSERT INTO playlist (songname, url) VALUES(@songname, @url)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("songname", songname);
                cmd.Parameters.AddWithValue("url", url);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Media added to Playlist");
                con.Close();
                return true;

            }
            catch (Exception)
            {
                Console.WriteLine("Error inserting to Playlist");
                return false;
            }
        }

        public string GetPlaylist()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT order, songname, url FROM playlist ORDER BY order ASC";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                int x = 1;
                string playlist = "\n----------Playlist----------\n";

                while (reader.Read())
                {
                    playlist += "\nOrder: " + reader.GetInt32(0).ToString() + " /Songname: " + reader.GetString(1) + " /Url: " + reader.GetString(2);
                    x++;
                }
                con.Close();
                return playlist;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Playlist");
                return "0";
            }
        }

        public bool ReorderPlaylist(string username, int fromPos, int toPos)
        {
            try
            {
                if(username != this.currentAdmin)
                {
                    Console.WriteLine("Non-Admin tried to change Order");
                    return false;
                }

                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "UPDATE playlist SET order= @toPos WHERE oder= @fromPos";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                //cmd.Parameters.AddWithValue("password", password);
               //cmd.Parameters.AddWithValue("newname", newname);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Media added to Playlist");
                con.Close();
                return true;

            }
            catch (Exception)
            {
                Console.WriteLine("Error inserting to Playlist");
                return false;
            }
        }

        //Get-Functions of User and Debug/Admin functions
        public string GetUserData(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT username, name, score, admin, actions, bio, image FROM users WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                string data = "";
                while (reader.Read())
                {
                    data += " \nUsername: "+ reader.GetString(0) + " \nName: " + reader.GetString(1) + " /Score: " + reader.GetInt32(2).ToString() + " /Is Admin: " + reader.GetBoolean(3).ToString() + " /Actions: " + reader.GetString(4) + " /Bio: " + reader.GetString(5) + " /Image: " + reader.GetString(6);
                }
                con.Close();
                return data;
            }
            catch (Exception)
            {
                Console.WriteLine("Error logging in the User");
                return "0";
            }
        }

        public bool SetAdmin(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "UPDATE users SET admin = true WHERE username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();               
                con.Close();
                this.currentAdmin = username;
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error setting Admin");
                return false;
            }
        }

        public bool ResetAdmin()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Delete Statement
                var sql = "UPDATE users SET admin = false WHERE admin = true";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Admin Reset");
                con.Close();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reseting the Admin");
                return false;
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
                string scoreboard = "\n----------Scoreboard----------\n";

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
                cmd.Parameters.AddWithValue("name", username);
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
