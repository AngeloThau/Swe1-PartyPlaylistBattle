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

            Console.WriteLine("Starting Server on Port 8080");
            HTTPServer.HTTPServer server = new HTTPServer.HTTPServer(8080);
            server.Start();
        }
    }
}
