using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Tournament
{
    public class User
    {
        public string username;
        public char[] set;
        public int personalScore;

        public User(string username, char[] set)
        {
            this.username = username;
            this.set = set;
            personalScore = 0;
        }
        public User()
        {
            this.username = "";
            this.set = null;
            personalScore = 0;
        }
    }
}
