using NUnit.Framework;

namespace PartyPlaylistBattleTest
{
    [TestFixture]
    public class DatabaseHandlerTest
    {
        //Variablen für Test

        [Test]
        public void DatabaseHandlerExists()
        {
            PartyPlaylistBattle.Database.DatabaseHandler test = new PartyPlaylistBattle.Database.DatabaseHandler();
            Assert.IsNotNull(test);
        }
        [Test]
        public void DatabaseHandlerLoginUser()
        {
            PartyPlaylistBattle.Database.DatabaseHandler test = new PartyPlaylistBattle.Database.DatabaseHandler();
            Assert.IsTrue(test.LoginUser("Firsty", "first"));
        }

        [Test]
        public void DatabaseHandlerSetAdmin()
        {
            PartyPlaylistBattle.Database.DatabaseHandler test = new PartyPlaylistBattle.Database.DatabaseHandler();
            Assert.IsTrue(test.SetAdmin("Firsty"));
        }

        [Test]
        public void DatabaseHandlerResetAdmin()
        {
            PartyPlaylistBattle.Database.DatabaseHandler test = new PartyPlaylistBattle.Database.DatabaseHandler();
            Assert.IsTrue(test.ResetAdmin());
        }

    }
}