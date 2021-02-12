using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Tournament
{
    class User
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
    }
}
