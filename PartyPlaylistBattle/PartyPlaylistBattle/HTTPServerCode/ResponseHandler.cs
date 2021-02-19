using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace PartyPlaylistBattle.HTTPServerCode
{
    class ResponseHandler
    {
        public TcpClient client;
        public string type;
        public string command;
        public string authorization;
        public string body;
        public List<string> user;
        PartyPlaylistBattle.Database.DatabaseHandler db;
        public List<string> loggedInUser;
        static object lockObj = new object();
        public Tournament.Battle battleHandler = new Tournament.Battle();
        public List<Tournament.User> tournamentUsers = new List<Tournament.User>();
        

        public ResponseHandler(TcpClient _client, string _type, string _command, string _authorization, string _body, List<string> _user)
        {
            client = _client;
            type = _type;
            command = _command;
            authorization = _authorization;
            body = _body;
            db = new Database.DatabaseHandler();
            user = _user;
            loggedInUser = new List<string>();

        }
        private string ExtractUsername(string authorization)
        {
            string[] username = authorization.Split("-");
            return username[0];
        }
        public void Response(string status, string mime, string data)
        {
            StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            StringBuilder mystring = new StringBuilder();
            mystring.AppendLine("HTTP/1.1 " + status);
            mystring.AppendLine("Content-Type: " + mime);
            mystring.AppendLine("Content-Length: " + data.Length.ToString());
            mystring.AppendLine();
            mystring.AppendLine(data);
            System.Diagnostics.Debug.WriteLine(mystring.ToString());
            writer.Write(mystring.ToString());
        }

        public List<string> login(List<string> user)
        {
            lock (lockObj)
            {
                dynamic jasondata = JObject.Parse(body);
                string name = jasondata.Username;
                string password = jasondata.Password;

                if (!db.LoginUser(name, password))
                {
                    string mime = "text/plain";
                    string status = "404 Not found";
                    string data = "\nusername or password incorrect \n";
                    Response(status, mime, data);
                    return user;
                }
                else
                {
                    user.Add(name + "-ppbToken");
                    string mime = "text/plain";
                    string status = "200 OK";
                    string data = "\nuser OK \n";
                    Response(status, mime, data);
                    loggedInUser.Add(name);
                    return user;
                }
            }
        }

        public bool loggedIn(string name)
        {
            if (loggedInUser.Contains(name))
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        public void fromTypeToMethod(List<string> user)
        {
            if (loggedIn(ExtractUsername(authorization)))
            {
                switch (type)
                {
                    case "GET":
                        handleGet(user);
                        break;
                    case "PUT":
                        handlePut(user);
                        break;
                    case "POST":
                        handlePost(user);
                        break;
                    case "DELETE":
                        handleDelete(user);
                        break;
                    default:
                        invalidType();
                        break;
                }
            }
            else
            {
                string data = "\nLogIn required \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
        }
        public void handlePost(List<string> user)
        {
                switch (command)
                {
                    case "/lib":
                        AddToLibrary(ExtractUsername(authorization));
                        break;
                    case "/playlist":
                        AddToPlaylist(ExtractUsername(authorization));
                        break;
                    case "/battles":
                        Tournament(ExtractUsername(authorization));
                        break;
                default:
                        invalidCommand();
                        break;
                }
        }
     
        private void handlePut(List<string> user)
        {
            string playername = "";
            if (command.Contains("/users/") && command.Length > 7)
            {
                playername = command.Substring(7);
                if (ExtractUsername(authorization) == playername)
                {
                    changePlayersData(playername);
                }
                else if (ExtractUsername(authorization) != playername)
                {
                    string status = "404 Not Found";
                    string mime = "text/plain";
                    string data = "You can't change others data";
                    Response(status, mime, data);
                }
            }
            else
            {
                switch (command)
                {
                    case "/actions":
                        SetActions(ExtractUsername(authorization));
                        break;
                    case "/playlist":
                        ReorderPlaylist(ExtractUsername(authorization));
                        break;
                    default:
                        invalidCommand();
                        break;
                }
            }

        }
        private void handleGet(List<string> user)
        {
            if (command.Contains("/users/") && command.Length > 7)
            {
                string playername = "";
                string splitCommand = command;
                string[] temp = splitCommand.Split("/");
                if (temp.Length == 3)
                {
                    playername = temp[2];
                    if (temp[1] == "users" && ExtractUsername(authorization) == playername)
                    {
                        listUserData(playername);
                    }
                    else if (temp[1] == "users" && ExtractUsername(authorization) != playername)
                    {
                        string status = "404 Not Found";
                        string mime = "text/plain";
                        string data = "You can't see others data";
                        Response(status, mime, data);
                    }
                }
            }
            else
            {
                switch (command)
                {              
                    case "/score":
                        listScoreboard(user);
                        break;
                    case "/stats":
                        listStats(ExtractUsername(authorization));
                        break;
                    case "/lib":
                        listLibrary(ExtractUsername(authorization));
                        break;
                    case "/playlist":
                        listPlaylist();
                        break;
                    case "/actions":
                        listActions(ExtractUsername(authorization));
                        break;
                    default:
                        invalidCommand();
                        break;
                }
            }
        }
        private void handleDelete(List<string> user)
        {
            string songname = "";
            string splitCommand = command;
            string[] temp = splitCommand.Split("/");
            songname = temp[2];
            DeleteFromLibrary(ExtractUsername(authorization), songname);
        }


        public void invalidType()
        {
            string status = "404 Not Found";
            string mime = "text/plain";
            string data = "The type you're using is invalid";
            Response(status, mime, data);
        }

        public void invalidCommand()
        {
            string status = "404 Not Found";
            string mime = "text/plain";
            string data = "The Command you're using is invalid";
            Response(status, mime, data);
        }

        public void listUserData(string username)
        {
            if (db.UserExists(username))
            {
                string data = db.GetUserData(username);
                string status = "202 OK";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string status = "404 Not Found";
                string mime = "text/plain";
                string data = "The user you want to change does not exist..?";
                Response(status, mime, data);
            }
        }
 
        public void listScoreboard(List<string> user)
        {
            string scoreboard = db.GetScoreboard();
            if (scoreboard == "0")
            {
                string data = "\nUnexpected Database Error \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, scoreboard);
            }
        }

        public void listStats(string username)
        {
            string stats = db.GetStats(username);
            if (stats == "")
            {
                string data = "\nUnexpected Database Error \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, stats);
            }
        }

        public void listPlaylist()
        {
            string playlist = db.GetPlaylist();
            if (playlist == "")
            {
                string data = "\nUnexpected Database Error \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, playlist);
            }
        }
        public void listLibrary(string username)
        {
            string library = db.GetMediaLibrary(username);
            if (library == "")
            {
                string data = "\nUnexpected Database Error \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, library);
            }
        }

        public void listActions(string username)
        {
            string actions = db.GetActions(username);
            if (actions == "")
            {
                string data = "\nUnexpected Database Error \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, actions);
            }
        }

        public void AddToLibrary(string username)
        {
            dynamic jasondata = JObject.Parse(body);
            string name = jasondata.Name;
            string url = jasondata.Url;
            int rating = jasondata.Rating;
            string genre = jasondata.Genre;
            string title = jasondata.Title;
            string length = jasondata.Length;
            string album = jasondata.Album;

            if (db.AddToLibrary(name, username, url, rating, genre, title, length, album))
            {
                string data = "\nMedia sucessfully insterted\n";
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string data = "\nUnexpected Database Error\n";
                string status = "404";
                string mime = "text/plain";
                Response(status, mime, data);
            }
        }

        public void AddToPlaylist(string username)
        {
            dynamic jasondata = JObject.Parse(body);
            string name = jasondata.Name;

            if (db.SongInLibrary(username, name))
            {
                if (db.AddToPlaylist(name, username))
                {
                    string data = "\nMedia sucessfully insterted\n";
                    string status = "200 OK";
                    string mime = "text/plain";
                    Response(status, mime, data);
                }
                
            }
            else
            {
                string data = "\nCannot add Songs that are not in your library\n";
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, data);
            }
        }
        public void registerUser()
        {
            dynamic jasondata = JObject.Parse(body);
            string name = jasondata.Username;
            string password = jasondata.Password;
            if (db.NewUser(name, password))
            {
                string data = "\nPlayer successful created\n";
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string data = "\nUnexpected Database Error\n";
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, data);
            }
        }

        public void SetActions(string username)
        {
            string status = "";
            string mime = "";
            string data = "";
            dynamic jasondata = JObject.Parse(body);
            string actions = jasondata.actions;
            
            if (actions!=null)
            {
                char[] chactions = actions.ToCharArray();
                if(chactions.Length!=5)
                {
                    data = "\nWrong Actions (only this format: 'RPSLV' is allowed)\n";
                    status = "404 Not Found";
                    mime = "text/plain";
                    Response(status, mime, data);
                    return;
                }
                foreach(char ch in chactions)
                {
                    if(!ch.Equals('R') && !ch.Equals('P') && !ch.Equals('S') && !ch.Equals('L') && !ch.Equals('V'))
                    {
                        data = "\nWrong Actions (only this format: 'RPSLV' is allowed)\n";
                        status = "404 Not Found";
                        mime = "text/plain";
                        Response(status, mime, data);
                        return;
                    }
                }
                         
                
                db.ChangeActions(username, actions);

                data = "\nActions updated\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                data = "\nError when updating Actions\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }
        }

        public void DeleteFromLibrary(string username, string songname) //TODO-GET ERROR WHEN DELETED=0
        {
            string status = "";
            string mime = "";
            string data = "";
            if(db.DeleteMediaFromLibrary(username, songname))
            {
                data = "\nMedia Deleted\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                data = "\nError when Deleting Media (not in your library?)\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }
        }
        public void changePlayersData(string username)
        {
            string status = "";
            string mime = "";
            string data = "";
            dynamic jasondata = JObject.Parse(body);
            string newname = jasondata.Name;
            string bio = jasondata.Bio;
            string image = jasondata.Image;


            if (db.ChangeUser(username, newname, bio, image))
            {
                data = "\nData updated\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                data = "\nUpdating your data not OK\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }

        }
        public void ReorderPlaylist(string username)
        {
            string status = "";
            string mime = "";
            string data = "";
            dynamic jasondata = JObject.Parse(body);
            int fromPos = jasondata.FromPosition;
            int toPos = jasondata.ToPosition;
            
            if (db.UserIsAdmin(username))
            {
                if (db.ReorderPlaylist(username, fromPos, toPos))
                {
                    data = "\nPlaylist reordered\n";
                    status = "200 OK";
                    mime = "text/plain";
                    Response(status, mime, data);
                }
                else
                {
                    data = "\nError when reordering PLaylist\n";
                    status = "404 Not Found";
                    mime = "text/plain";
                    Response(status, mime, data);
                }
            }
            else
            {
                data = "\nOnly Admins can Change the Order of the Playlist\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }
        }

        public void Tournament(string username)
        {
            string status = "";
            string mime = "";
            string data = "";
            char[] set = db.GetActions(username).ToCharArray();
            char[] realSet = new char[5];
            realSet[0] = set[1];
            realSet[1] = set[2];
            realSet[2] = set[3];
            realSet[3] = set[4];
            realSet[4] = set[5];


            if (db.TournamentIsRunning())
            {
                data = "\nThere is an active Tournament right now, please wait\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }

               else if (db.UsersInTournament() == 0)
                {
                    if (db.RegisterUserInTournament(username))
                    {
                        Thread.Sleep(15000);

                        tournamentUsers = db.GetAllUsersInTournament().ToList();
                        battleHandler.Tournament(tournamentUsers);
                        db.ClearTournamentUsers();
                        data = battleHandler.log;
                        status = "200 OK";
                        mime = "text/plain";
                        Response(status, mime, data);
                    }
                    else
                    {
                        data = "\nUnexpected Error in registering User\n";
                        status = "404 Not Found";
                        mime = "text/plain";
                        Response(status, mime, data);
                    }
                }
                else if (db.UsersInTournament() > 0)
                {
                    if (db.RegisterUserInTournament(username))
                    {
                        Thread.Sleep(15000);
                        data = battleHandler.log;
                        status = "200 OK";
                        mime = "text/plain";
                        Response(status, mime, data);

                    }
                    else
                    {
                        data = "\nUnexpected Error in registering User\n";
                        status = "404 Not Found";
                        mime = "text/plain";
                        Response(status, mime, data);
                    }
                }
            
        }

    }
}
