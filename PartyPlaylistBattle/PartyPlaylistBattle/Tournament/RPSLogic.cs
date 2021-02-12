using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.Tournament
{
    class RPSLogic
    {
              

        public int Round(char input1, char input2)
        {
            switch (input1)
            {
                case 'R':
                    if (input2 == 'R')
                    { return 0;}
                    else if(input2 == 'S'||input2=='L')
                    { return 1; }
                    else if(input2 == 'V' || input2 == 'P')
                    { return -1; }
                break;
                case 'P':
                    if (input2 == 'P')
                    { return 0; }
                    else if (input2 == 'R' || input2 == 'V')
                    { return 1; }
                    else if (input2 == 'L' || input2 == 'S')
                    { return -1; }
                    break;
                case 'S':
                    if (input2 == 'S')
                    { return 0; }
                    else if (input2 == 'P' || input2 == 'L')
                    { return 1; }
                    else if (input2 == 'V' || input2 == 'R')
                    { return -1; }
                    break;
                case 'V':
                    if (input2 == 'V')
                    { return 0; }
                    else if (input2 == 'S' || input2 == 'R')
                    { return 1; }
                    else if (input2 == 'L' || input2 == 'P')
                    { return -1; }
                    break;
                case 'L':
                    if (input2 == 'L')
                    { return 0; }
                    else if (input2 == 'V' || input2 == 'P')
                    { return 1; }
                    else if (input2 == 'R' || input2 == 'S')
                    { return -1; }
                    break;
                default:
                    return 0;                  
            }
            return 0;
        }
    }
}
