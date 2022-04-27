using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace LaserGame
{
    public class Player
    {
        internal int subIpAddr;
        internal string pseudo;
        internal int id;
        internal string team;
        internal int score;
        internal int kill;
        internal int death;
        internal MainWindow.GridLine gridlineRef;
        internal string last_buffer = "";
        internal int position_buffer = 0;
        internal bool isInit = false;

        public Player(int _subIpAddr, string _pseudo, string _team)
        {
            id = -1;
            subIpAddr = _subIpAddr;
            pseudo = _pseudo;
            team = _team;
        }

        public int getSubIpAddr()
        {
            return subIpAddr;
        }
    }
}
