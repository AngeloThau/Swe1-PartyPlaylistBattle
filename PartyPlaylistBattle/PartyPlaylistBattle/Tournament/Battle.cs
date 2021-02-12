using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Tournament
{
    class Battle
    {
        public List<User> users;
        public int score;
        public RPSLogic logic = new RPSLogic();

        public Battle(char[] one, char[] two)
        {
            score = 0;
        }

        public int BattleRound(User one, User two)
        {
            for(int i=0; i<5; i++)
            {
                score += logic.Round(one.set[i], two.set[i]); 
            }
            return score;
        }
    }
}
