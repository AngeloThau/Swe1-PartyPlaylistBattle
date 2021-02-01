using NUnit.Framework;

namespace PartyPlaylistBattleTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Database.DatabaseHandler database = new Database.DatabaseHandler();

            database.NewUser("user1", "pw1");
            database.DeleteUser("user1");
            
        }
    }
}