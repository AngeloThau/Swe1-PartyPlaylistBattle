using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PartyPlaylistBattle.HTTPServer
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

            /*string tradeId = "";
            if (command.Contains("/tradings/") && command.Length > 10)
            {
                tradeId = command.Substring(10);
            }
            if (tradeId != "")
            {
                doTrading(tradeId);
            }
            else
            {
                switch (command)
                {
                    case "/packages":
                        addNewPackage(user);
                        break;
                    case "/transactions/packages":
                        buyPackage(user);
                        break;
                    case "/tradings":
                        newTradingDeal(user);
                        break;
                    default:
                        invalidCommand();
                        break;
                }
            }*/
            invalidCommand();
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
                    //case "/deck/unset":
                       // unsetDeck(user);
                       // break;
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
                    default:
                        invalidCommand();
                        break;
                }
            }
        }
        private void handleDelete(List<string> user)
        {
            string id = "";
            string splitCommand = command;
            string[] temp = splitCommand.Split("/");
            id = temp[2];
            invalidCommand();
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
    }
}
