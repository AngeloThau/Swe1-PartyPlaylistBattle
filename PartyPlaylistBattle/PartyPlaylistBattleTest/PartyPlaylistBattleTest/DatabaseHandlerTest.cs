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

    }
}