using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Tournament
{
    class Battle
    {
        public RPSLogic logic;       
        public string log = "";
        Database.DatabaseHandler db = new Database.DatabaseHandler();

        public Battle()
        {
            logic = new RPSLogic();           
        }

        int BattleRound(User one, User two)
        {
            int roundScore = 0;
            for(int i=0; i<5; i++)
            {
                roundScore += logic.Round(one.set[i], two.set[i]); 
            }
            return roundScore;
        }

        public bool RegisterUser(string username, char[] set, List<User> users)
        {
            User register = new User(username, set);
            users.Add(register);
            if(users.Contains(register))
            {
                return true;
            }
            return false;
        }

        public void Tournament(List<User> users)
        {
            
            User winner = new User();
            log = "";
            //Mehr als 1 User im Tournament
            while(users.Count>1)
            {
                for (int i = 0; i < users.Count;)
                {
                    for(int z = i+1; z < users.Count; z++)
                    {
                        if (BattleRound(users[i], users[z]) == 0)
                        {
                            this.log += users[i].username + " and " + users[z].username + " had a draw, both will be removed\n";
                            users.RemoveAt(i);
                            users.RemoveAt(0);
                            break;
                        }
                        else if (BattleRound(users[i], users[z]) > 0)
                        {
                            log += users[i].username + " won, +1 to Tournament Score!\n";
                            users[i].personalScore++;
                        }
                        else if (BattleRound(users[i], users[z]) < 0)
                        {
                            log += users[z].username + " won, +1 to Tournament Score\n";
                            users[z].personalScore++;
                        }
                    }
                    i++;
                }

                //Get the Winner (if 2 have same amount of point first in list will win, if no one is left return)   
                if(users.Count == 0)
                {
                    log += " Nobody is Left!\n";
                    return;
                }
                winner = users[0];
                for (int i = 1; i < users.Count; i++)
                {
                    if(winner.personalScore < users[i].personalScore)
                    {
                        winner = users[i];
                    }
                }

                log += winner.username + "is the winner!";
                db.SetAdmin(winner.username);
                return;
            }
            
            //1 User übrig
            if (users.Count == 1)
            {
                log += users[0].username + " is the winner by default";
               
                db.SetAdmin(users[0].username);
                return;
            }

           
        }
    }
}
