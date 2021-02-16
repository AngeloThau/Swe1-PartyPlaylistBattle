using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PartyPlaylistBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);

            //Database-reset for testing
            Database.DatabaseHandler db = new Database.DatabaseHandler();
            db.DeleteUser("altenhof");
            db.DeleteUser("kienboec");
            db.DeleteUser("admin");

            Console.WriteLine("Starting Server on Port 10001");
            HTTPServer.HTTPServer server = new HTTPServer.HTTPServer(10001);
            server.Start();
        }
    }
}
