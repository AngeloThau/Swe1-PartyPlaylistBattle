using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Tournament
{
    class Battle
    {
        public List<User> users;
        public RPSLogic logic = new RPSLogic();

        public Battle(char[] one, char[] two)
        { 
        }

        public int BattleRound(User one, User two)
        {
            int roundScore = 0;
            for(int i=0; i<5; i++)
            {
                roundScore += logic.Round(one.set[i], two.set[i]); 
            }
            return roundScore;
        }

        public bool RegisterUser(string username, char[] set)
        {
            User register = new User(username, set);
            this.users.Add(register);
            if(users.Contains(register))
            {
                return true;
            }
            return false;
        }

        public string Tournament(List<User> users)
        {
            User winner = new User();
            //Mehr als 1 User im Tournament
            while(users.Count>1)
            {
                for (int i = 0; i < users.Count;)
                {
                    for(int z = i+1; z < users.Count; z++)
                    {
                        if (BattleRound(users[i], users[z]) == 0)
                        {
                            Console.WriteLine(users[i].username + "and " + users[z].username + "had a draw, both will be removed");
                            users.RemoveAt(i);
                            users.RemoveAt(0);
                            break;
                        }
                        else if (BattleRound(users[i], users[z]) > 0)
                        {
                            users[i].personalScore++;
                        }
                        else if (BattleRound(users[i], users[z]) < 0)
                        {
                            users[z].personalScore++;
                        }
                    }
                    i++;
                }

                //Get the Winner (if 2 have same amount of point first in list will win)    
                winner = users[0];
                for (int i = 1; i < users.Count; i++)
                {
                    if(winner.personalScore < users[i].personalScore)
                    {
                        winner = users[i];
                    }
                }
                Console.WriteLine(winner.username + "is the winner!");
                return winner.username;
            }
            
            //1 User übrig
            if (users.Count == 1)
            {
                Console.WriteLine(users[0].username + "is the winner by default");
                return users[0].username;
            }

            return winner.username;
        }
    }
}
