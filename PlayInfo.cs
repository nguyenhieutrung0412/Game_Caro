using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro
{
    public class PlayInfo
    {
        private Point point;
        private int currentplayer;

        public int Currentplayer { get => currentplayer; set => currentplayer = value; }
        public Point Point { get => point; set => point = value; }
        public PlayInfo(Point point, int currentplayer)
        {
            this.Point = point;
            this.Currentplayer = currentplayer;
        }
    }
}
