using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace PartyPlaylistBattle.HTTPServer
{

    public class HTTPServer
    {

        private bool running = false;
        private TcpListener listener;
        public List<string> login;
        static object mylock = new object();
        static object isThereAnoterPlayerLock = new object();
        private string gamelog;
        private int gamelockcheck;
        private List<string> wantToBattle;
        //private ITcpClient myTcpClient;


        public HTTPServer(int port)
        {
            login = new List<string>();
            listener = new TcpListener(IPAddress.Any, port);

            gamelog = "";
            gamelockcheck = 0;
            wantToBattle = new List<string>();
        }




        //Prgramstart

        //-------------------------------run the connection-----------------------------------------------------
        public void Run()
        {

            running = true;
            listener.Start();

            while (running)
            {

                Console.WriteLine("Waiting for connection...");
                TcpClient client = listener.AcceptTcpClient();  //Connection with client


                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleClient), client);

            }
            running = false;
            listener.Stop();

        }

        //------------------------------------------handle the client----------------------------------------------
        private void HandleClient(Object myclient)
        {
            Console.WriteLine("Client connected");
            TcpClient myTcpClient = ((TcpClient)myclient);
            String msg = ReadStream(myTcpClient);

            Debug.WriteLine(msg);

            Request req = new Request(msg);  //create Request Object that has all the Request infos

            foreach (var item in req.HeadRest)
            {
                Console.WriteLine(item.ToString());
            }

            IMessageHandler handler = new MessageHandler(login, myTcpClient, req.Type, req.Order, req.authorization, req.body);

            if (req.Type == "POST" && req.Order == "/sessions")
            {
                lock (mylock)
                {
                    login = handler.LoginPlayer(login);
                }
            }
            else if (req.Type == "POST" && req.Order == "/users")
            {
                handler.HandlePostUsers();
            }
            else if (req.Type == "POST" && req.Order == "/battles")
            {

                bool isOnline = false;
                for (int i = 0; i < login.Count; i++)
                {
                    if (login[i] == req.authorization)
                    {
                        isOnline = true;
                    }
                }
                if (!isOnline)
                {
                    string data = "\nuser is not logged in \n";
                    string status = "404 Not found";
                    string mime = "text/plain";
                    handler.ServerResponse(status, mime, data);
                }

                else
                {
                    bool done = DoBattle(req.authorization);
                    if (!done)
                    {
                        string data = "\nNo Battle was found \n";
                        string status = "404 Not found";
                        string mime = "text/plain";
                        handler.ServerResponse(status, mime, data);
                        gamelockcheck = 0;

                    }
                    else
                    {
                        string status = "200 Success";
                        string mime = "text/plain";
                        handler.ServerResponse(status, mime, gamelog);
                        if (gamelockcheck == 2)
                        {
                            gamelockcheck = 0;
                            gamelog = "";
                        }
                    }

                }

            }
            else
            {
                handler.CheckType(login);
            }

            myTcpClient.Close();

        }

        //-------------------gets the TcpClient--------------------------
        private TcpClient GetClient()
        {
            TcpClient client = listener.AcceptTcpClient();

            return client;
        }

        //--------------------------------gets the Stream by crerating a StreamReader and reads from it------------------------------------------
        private string ReadStream(TcpClient myTcpClient)
        {

            StreamReader reader = new StreamReader(myTcpClient.GetStream(), leaveOpen: true); //create reader


            string msg = "";
            while (reader.Peek() != -1)  //read from the Stream and save it in "msg"
            {
                msg += (char)reader.Read();
            }
            //reader.Close();
            return msg;
        }


        //--------------------------------------do the Battle with two Threads------------------------------
        private bool DoBattle(string authorization)
        {

            lock (isThereAnoterPlayerLock)
            {
                int lenght = authorization.IndexOf("-mtcgToken");
                string playername = authorization.Substring(0, lenght);
                wantToBattle.Add(playername);
                int players = wantToBattle.Count;

                if (players >= 2)
                {
                    string firstPlayer = wantToBattle[players - 2];
                    string secondPlayer = wantToBattle[players - 1];
                    Battle myBattle = new Battle(firstPlayer, secondPlayer);
                    myBattle.BattleHandler();
                    string message = "";
                    //this is second player 
                    foreach (string line in myBattle.Log)
                    {
                        message += line;
                    }
                    gamelog = message;
                    gamelockcheck++;
                    return true;
                }
            }
            bool gamePlayed = false;
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(2000);
                if (gamelockcheck > 0)
                {
                    gamePlayed = true;
                    i = 30;
                }
            }

            if (!gamePlayed)
            {
                return false;
            }
            gamelockcheck++;
            return true;


        }

    }
}

