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
            db.DeleteMediaFromLibrary("altenhof", "Best_song_ever");
            db.DeleteMediaFromLibrary("altenhof", "Super_Mario_song");
            db.DeleteMediaFromLibrary("kienboec", "Good_Mood_Song");
            db.ResetPlaylist();
            db.ResetTournament();

            Console.WriteLine("Starting Server on Port 10001");
            HTTPServerCode.HTTPServer server = new HTTPServerCode.HTTPServer(10001);
            server.Start();
        }
    }
}
