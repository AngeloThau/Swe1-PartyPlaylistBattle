using System;

namespace PartyPlaylistBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.DatabaseHandler database = new Database.DatabaseHandler();

            database.NewUser("user1", "pw1");
            database.DeleteUser("user1");
        }
    }
}
