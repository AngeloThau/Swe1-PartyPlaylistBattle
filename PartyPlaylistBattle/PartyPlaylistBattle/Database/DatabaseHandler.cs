using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Database
{
    public class DatabaseHandler
    {
        private readonly string connection;
        public List<Tournament.User> tournamentUsers;

        public DatabaseHandler()
        {
            connection = "Host=localhost;Username=postgres;Password=Safetyfirst;Database=PartyPlaylistBattle";
            tournamentUsers = new List<Tournament.User>();
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
                var sql = "INSERT INTO users (username, password, admin, score, actions, bio, image, name) VALUES(@username, @password, false, 0, @actions, '', '', '')";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("actions", "RPSLV");

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
        public bool AddToLibrary(string name, string username, string url, int rating, string genre, string title, string length, string album)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "INSERT INTO library (name, username, url, rating, genre, title, length, album) VALUES(@name, @username, @url, @rating, @genre, @title, @length, @album)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("url", url);
                cmd.Parameters.AddWithValue("rating", rating);
                cmd.Parameters.AddWithValue("genre", genre);
                if(title != null)
                {
                    cmd.Parameters.AddWithValue("title", title);
                }
                else
                {
                    cmd.Parameters.AddWithValue("title", "");
                }

                if (length != null)
                {
                cmd.Parameters.AddWithValue("length", length);
                }
                else
                {
                cmd.Parameters.AddWithValue("length", "");
                }
                if (album != null)
                {
                    cmd.Parameters.AddWithValue("album", album);
                }
                else
                {
                    cmd.Parameters.AddWithValue("album", "");
                }
            
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
                var sql = "SELECT name, url, rating, genre, title, length, album FROM library WHERE username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
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
                var sql = "SELECT url FROM library WHERE username = @username AND name = @name";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                string url = "";
                while (reader.Read())
                {
                    url += reader.GetString(0);
                }
                 

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
                var sql = "DELETE FROM library WHERE username=@username AND name=@name";
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

        public bool SongInLibrary(string username, string name)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var query = "SELECT COUNT(*) FROM library WHERE name = @name AND username= @username";
                using NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                cmd.Parameters.AddWithValue("name",name);
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
                Console.WriteLine("Error getting Media");
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
                var sql = "SELECT position, songname, url FROM playlist ORDER BY position ASC";
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

        public string GetSongname(int position)
        {
            try
            {               
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "SELECT songname FROM playlist WHERE position= @position";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("position", position);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();        
                string data = "";
                while (reader.Read())
                {
                    data = reader.GetString(0);
                }
                con.Close();
                return data;

            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Songname");
                return "0";
            }
        }

        public bool ReorderPlaylist(string username, int fromPos, int toPos)
        {
            try
            {
                if(!UserIsAdmin(username))
                {
                    Console.WriteLine("Non-Admin tried to change Order");
                    return false;
                }

                //Get both Songnames
                string song1 = GetSongname(fromPos);
                string song2 = GetSongname(toPos);

                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //instert Statement
                var sql = "UPDATE playlist SET position= @toPos WHERE position= @fromPos AND songname = @songname";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("songname", song1);
                cmd.Parameters.AddWithValue("fromPos", fromPos);
                cmd.Parameters.AddWithValue("toPos", toPos);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
                Console.WriteLine("First Song shuffeled");
                con.Close();

                //2nd Statement
                con.Open();

                //instert Statement
                var sql2 = "UPDATE playlist SET position= @fromPos WHERE position= @toPos AND songname = @songname";
                using var cmd2 = new NpgsqlCommand(sql2, con);
                cmd2.Parameters.AddWithValue("songname", song2);
                cmd2.Parameters.AddWithValue("fromPos", fromPos);
                cmd2.Parameters.AddWithValue("toPos", toPos);
                cmd2.Prepare();

                cmd2.ExecuteNonQuery();
                Console.WriteLine("Second Song shuffeled");
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

        public string GetStats(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT score FROM users WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                string data = "";
                //int rating = reader.GetInt32(0) - 100;


                while (reader.Read())
                {
                    data += " \nYour Score: " + reader.GetInt32(0).ToString() + "\n See other Peoples Score with /scoreboard!";
                }
                con.Close();
                return data;
            }
           catch (Exception)
            {
                Console.WriteLine("Error displaying Rating");
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

        public bool ResetPlaylist()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Delete Statement
                var sql = "DELETE FROM playlist";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
                con.Close();

                //2nd Statement (resetting the serial)
              
                con.Open();

                var sql2 = "ALTER SEQUENCE playlist_position_seq RESTART WITH 1";
                using var cmd2 = new NpgsqlCommand(sql2, con);
                cmd2.Prepare();

                cmd2.ExecuteNonQuery();

                Console.WriteLine("Playlist Reset");
                con.Close();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reseting the Playlist");
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

        public string GetActions(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT actions FROM users WHERE username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();

            
                string actions ="";

                while (reader.Read())
                {
                    actions += "\n" + reader.GetString(0) + "\n";
                }
                con.Close();
                return actions;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Score of the User");
                return "0";
            }
        }

        public bool UserIsAdmin(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT * FROM users WHERE admin = true";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                
                string currentAdmin = "";
                while(reader.Read())
                {                   
                    currentAdmin += reader.GetString(0);
                }
                if(username == currentAdmin)
                {
                    con.Close();
                    return true;
                }
                con.Close();
                return false;

            }
            catch (Exception)
            {
                Console.WriteLine("Error comparing Admin");
                return false;
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

        //Tournament Functions
        public bool RegisterUserInTournament(string username)
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                string set = GetActions(username);
                
                //instert Statement
                var sql = "INSERT INTO tournament (username, set, isRunning) VALUES(@username, @set, false)";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("set", set);                

                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("user registered in Tournament");
                con.Close();
                return true;

            }
            catch (Exception)
            {
                Console.WriteLine("Error inserting a User into Tournament");
                return false;
            }
        }

        public bool ResetTournament()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Delete Statement
                var sql = "DELETE FROM tournament";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Console.WriteLine("Tournament Reset");
                con.Close();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reseting the Tournament");
                return false;
            }
        }

        public bool TournamentIsRunning()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT * FROM tournament WHERE isrunning = true";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();


                bool running = false;
                while (reader.Read())
                {
                    running = reader.GetBoolean(0);
                }
                if(running)
                {
                    con.Close();
                    return true;
                }
                con.Close();
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting status of Tournament");
                return false;
            }
        }

        public int UsersInTournament()
        {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT COUNT(*) FROM tournament";
                using var cmd = new NpgsqlCommand(sql, con);                
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();


                int count = 0;
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                con.Close();
                return count;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Count of Users in Tournament");
                return 0;
            }
        }
        public List<Tournament.User> GetAllUsersInTournament()
        {
           try
           {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT username, set FROM tournament";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
             
                while (reader.Read())
                {
                    char[] set = GetActions(reader.GetString(0)).ToCharArray();
                    char[] realSet = new char[5];
                    realSet[0] = set[1];
                    realSet[1] = set[2];
                    realSet[2] = set[3];
                    realSet[3] = set[4];
                    realSet[4] = set[5];
                    Tournament.User user = new Tournament.User(reader.GetString(0), realSet);
                    tournamentUsers.Add(user);
                }
                con.Close();
                return tournamentUsers;
            }
            catch (Exception)
            {
                Console.WriteLine("Error getting Users in Tournament");
                return null;
            }
        }
        public void ClearTournamentUsers()
        {
            tournamentUsers.Clear();
            ResetTournament();
        }

   

    public void AddScore(string username)
    {
            try
            {
                //Establishing Connection
                using var con = new NpgsqlConnection(connection);
                con.Open();

                //Select Statement
                var sql = "SELECT score FROM users WHERE username= @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                string data = "";
                //int rating = reader.GetInt32(0) - 100;
                int score = 0;

                while (reader.Read())
                {
                    score = reader.GetInt32(0);
                }
                con.Close();

                //2nd Statement
                score++;
                con.Open();

                //Select Statement
                var sql2 = "UPDATE users SET score= @score WHERE username= @username";
                using var cmd2 = new NpgsqlCommand(sql2, con);
                cmd2.Parameters.AddWithValue("username", username);
                cmd2.Parameters.AddWithValue("score", score);
                cmd2.Prepare();
                using NpgsqlDataReader reader2 = cmd2.ExecuteReader();                
              
                con.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error displaying Rating");
            }

        }


    }
}
