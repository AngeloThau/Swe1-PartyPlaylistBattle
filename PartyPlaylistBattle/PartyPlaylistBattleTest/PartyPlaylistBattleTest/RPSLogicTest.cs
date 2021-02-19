using NUnit.Framework;

namespace PartyPlaylistBattleTest
{
    [TestFixture]
    public class RPSLogicTest
    {
        //Variablen für Test

        [Test]
        public void RPSLogicDraw()
        {
            PartyPlaylistBattle.Tournament.RPSLogic logic = new PartyPlaylistBattle.Tournament.RPSLogic();
            Assert.AreEqual(logic.Round('R', 'R'), 0);
        }

        [Test]
        public void RPSLogicWin()
        {
            PartyPlaylistBattle.Tournament.RPSLogic logic = new PartyPlaylistBattle.Tournament.RPSLogic();
            Assert.AreEqual(logic.Round('R', 'S'), 1);
        }

        [Test]
        public void RPSLogicLose()
        {
            PartyPlaylistBattle.Tournament.RPSLogic logic = new PartyPlaylistBattle.Tournament.RPSLogic();
            Assert.AreEqual(logic.Round('R', 'P'), -1);
        }

    }
}