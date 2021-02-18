using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace PartyPlaylistBattle.HTTPServerCode
{

    public class HTTPServer
    {
        public const String _version = "HTTP/1.1";
        private bool running = false;
        private TcpListener listener;
        public List<string> user;
        public List<string> challenger;
        public string GameLog;
        static object waitingForPlayer = new object();
        Database.DatabaseHandler db = new Database.DatabaseHandler();
        public List<string> Log;
        
        


        public HTTPServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            GameLog = "";
            challenger = new List<string>();
            user = new List<string>();
            Log = new List<string>();
        }

        public void Start()
        {
            db.ResetAdmin();
            Thread thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private void Run()
        {
            running = true;
            listener.Start();

            while (running)
            {
                Console.WriteLine("\nWaiting for connections...");
                TcpClient client = listener.AcceptTcpClient();
                // Client myclient = new Client(client);
                Console.WriteLine("Client connected OKfully!");
                //Thread th = new Thread(new ParameterizedThreadStart(ClientHandler));
                //th.Start(myclient);
                //ClientHandler(client);
                //client.Close();
                ThreadPool.QueueUserWorkItem(ClientHandler, client);
            }

            running = false;
            listener.Stop();
        }

        public void ClientHandler(Object obj)
        //private void ClientHandler(TcpClient client)
        {
            TcpClient newclient = (TcpClient)obj;
            //TcpClient newclient = (TcpClient)client;
            String message = "";
            StreamReader reader = new StreamReader(newclient.GetStream(), leaveOpen: true);
            while (reader.Peek() != -1)
            {
                message += (char)reader.Read();
            }

            RequestHandler request = new RequestHandler(message);
            foreach (var x in request.Rest)
            {
                Console.WriteLine(x.ToString());
            }

            ResponseHandler msghandler = new ResponseHandler(newclient, request.Type, request.Command, request.Authorization, request.Body, user);

            if (request.Type == "POST")
            {
                if (request.Command == "/users")
                {
                    msghandler.registerUser();
                }
                else if (request.Command == "/sessions")
                {
                    user = msghandler.login(user);
                }
                
                else
                {
                    msghandler.handlePost(user);
                }
            }
            else
            {
                msghandler.fromTypeToMethod(user);
            }
        }

        public void Tournament(string username)
        {
            
        }

    } 
}

