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
                    user.Add(name + "-PPBToken");
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
                        SetActions(user);
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
            //deleteTrading(id);
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
        /*public void listCards(List<string> user)
        {
            string name = ExtractUsername(authorization);

            List<string> allCardsofPlayer = db.getAllCardsOfPlayer(name);
            string list = "";
            foreach (string card in allCardsofPlayer)
            {
                list += card + "\n\n";
            }
            string status = "200 OK";
            string mime = "text/plain";
            Response(status, mime, list);
            return;
        }
        
        public void listDeck(List<string> user)
        {

            string name = ExtractUsername(authorization);

            List<string> allCardsofPlayer = db.getAllCardsInDeck(name);
            string list = "";
            foreach (string card in allCardsofPlayer)
            {
                list += card + "\n\n";
            }
            string status = "200 OK";
            string mime = "text/plain";
            Response(status, mime, list);
            return;
        }

        public void listStats(List<string> user)
        {
            string name = ExtractUsername(authorization);
            string points = db.getPoints(name).ToString();
            if (points != "0")
            {
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, points);
            }
            else
            {
                string data = "\nUnexpected Database Error \n";
                string status = "404 Not found";
                string mime = "text/plain";
                Response(status, mime, data);
                return;
            }
        }
        */
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

        /*public void listTradings(List<string> user)
        {
            if (db.existTradingOffer())
            {
                List<string> allTrades = new List<string>();
                allTrades = db.showAllTradings();
                string myString = "";
                foreach (string line in allTrades)
                {
                    myString += line;
                }
                string data = myString;
                string status = "200 OK";
                string mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                string data = "No tradings found, sorry...";
                string status = "404 Not Found";
                string mime = "text/plain";
                Response(status, mime, data);
            }
        }*/

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
        /*public void addNewPackage(List<string> user)
        {
            if (authorization != "admin-mtcgToken")
            {
                string status = "text/plain";
                string mime = "404 Not Found";
                string data = "\nonly admin is allowed to create packages";
                Response(status, mime, data);
                return;
            }
            else
            {
                JArray jasarray = JArray.Parse(body);
                var obj = jasarray[0];
                int packid = db.getMaxIDfromPackage() + 1;

                for (int i = 0; i < jasarray.Count; i++)
                {
                    string cardid = (string)jasarray[i]["Id"];
                    string cardname = (string)jasarray[i]["Name"];
                    double damage = (double)jasarray[i]["Damage"];
                    //string type = (string)jasarray[i]["Cardtype"];
                    //string element = (string)jasarray[i]["Element"];
                    db.addCard(cardid, cardname, damage, "-", "-");
                    db.addPackage(cardid, packid, false);
                }

                string data = "\nPackage created OKfully\n";
                string status = "200 OK";
                string mime = "text/plain";

                Response(status, mime, data);
            }
        }

        public void buyPackage(List<string> user)
        {
            string status = "";
            string mime = "";
            string data = "";
            string name = ExtractUsername(authorization);
            if (db.getCoins(name) < 5)
            {
                data = "\nSorry, you need at least 5 coins to buy a package\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
                return;
            }
            else if (!db.getAvailablePackages())
            {
                data = "\nSorry, no package left\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
                return;
            }
            else
            {
                int packid = db.getIDfromPackage();
                foreach (var cardid in db.getCardsOfPackage(packid))
                {
                    db.buyPackage(name, cardid);
                    db.sellPackage(cardid, true);
                }
                db.updateCoins(name, 5);

                data = "\nPackage aquired OKfully\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
        }

        public void setDeck(List<string> user)
        {
            string status = "";
            string mime = "";
            string data = "";
            string name = ExtractUsername(authorization);
            JArray jasarray = JArray.Parse(body);
            if (jasarray.Count < 4)
            {
                data = "\nnot enough cards: you need 4 cards in your deck\n";
                status = "404 Not found";
                mime = "text/plain";
                Response(status, mime, data);
                return;
            }
            else if (jasarray.Count > 4)
            {
                data = "\ntoo many cards: you need 4 cards in your deck\n";
                status = "404 Not found";
                mime = "text/plain";
                Response(status, mime, data);
                return;
            }
            else if (db.existCardsInDeck(name) != 0)
            {
                data = "\nDeck is already set\n";
                status = "404 Not found";
                mime = "text/plain";
                Response(status, mime, data);
                return;
            }
            else if (jasarray.Count == 4 && db.existCardsInDeck(name) == 0)
            {
                foreach (string cardid in jasarray)
                {
                    db.defineDeck(name, cardid, true);
                }
                data = "\nDeck is ready to rumble\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
        }

        public void unsetDeck(List<string> user)
        {
            string status = "";
            string mime = "";
            string data = "";
            string name = ExtractUsername(authorization);

            db.unsetDeck(name);
            data = "\nDeck is not ready to rumble anymore\n";
            status = "200 OK";
            mime = "text/plain";
            Response(status, mime, data);
        }

        public void newTradingDeal(List<string> user)
        {
            string status = "";
            string mime = "";
            string data = "";
            string tradeid = "";
            string cardid = "";
            string type = "";
            string requirement = "";
            string playername = ExtractUsername(authorization);
            dynamic jasondata = JObject.Parse(body);
            tradeid = (string)jasondata["Id"];
            cardid = (string)jasondata["CardToTrade"];
            type = (string)jasondata["Type"];
            requirement = (string)jasondata["MinimumDamage"];
            if (db.existsCardInDeckWithID(cardid))
            {
                data = "\nCan't trade with card in your deck\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                if (!db.existTradingOfferWithID(tradeid))
                {
                    db.newTradingEntry(cardid, tradeid, type, requirement, playername);
                    data = "\nTrading deal set\n";
                    status = "200 OK";
                    mime = "text/plain";
                    Response(status, mime, data);
                }
                else
                {
                    data = "\nTradeID already used\n";
                    status = "404 Not Found";
                    mime = "text/plain";
                    Response(status, mime, data);
                }
            }
        }

        public void deleteTrading(string id)
        {
            string status = "";
            string mime = "";
            string data = "";
            if (db.existTradingOfferWithID(id))
            {
                db.deleteTradingEntry(id);
                data = "\nTrading deal delete OKfully\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                data = "\nTradeID already usednot found\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }
        }
        public void doTrading(string tradeId)
        {
            string status = "";
            string mime = "";
            string data = "";
            string name = ExtractUsername(authorization);
            dynamic jasondata = JProperty.Parse(body);
            string yourCardid = jasondata;
            string otherName = db.getNameOfTradingOffer(tradeId);
            string tradeCardId = db.getCardIDfromTrade(tradeId);
            if (db.getNameOfTradingOffer(tradeId) != name)
            {
                if (db.changeCardOwner(tradeCardId, name))
                {
                    db.tradeWithYourCard(yourCardid, otherName);
                    db.deleteTradingEntry(tradeId);
                    data = "\nTrading deal done\n";
                    status = "200 OK";
                    mime = "text/plain";
                    Response(status, mime, data);
                }
                else
                {
                    data = "\nSorry, no trading deal for you... \n";
                    status = "404 Not Found";
                    mime = "text/plain";
                    Response(status, mime, data);
                }
            }
            else
            {
                data = "\nSorry, you can't trade with yourself... \n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }

        }*/
        public void SetActions(List<string> username)
        {
            string status = "";
            string mime = "";
            string data = "";
            dynamic jasondata = JObject.Parse(body);
            string newname = jasondata.Name;
            string password = jasondata.Password;

            if (false==true)
            {
                data = "\nBattle Commencing\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                data = "\nError when starting Tournament\n";
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
            string password = jasondata.Password;


            if (db.ChangeUser(username, password, newname))
            {
                data = "\nData updated\n";
                status = "200 OK";
                mime = "text/plain";
                Response(status, mime, data);
            }
            else
            {
                data = "\nUpdating your data not OKful\n";
                status = "404 Not Found";
                mime = "text/plain";
                Response(status, mime, data);
            }

        }
    }
}
