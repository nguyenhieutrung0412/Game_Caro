using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro
{
    public class Player
    {
        private string namePlayer;
        private Image mark;

        public string NamePlayer { get => namePlayer; set => namePlayer = value; }
        public Image Mark { get => mark; set => mark = value; }

        public Player( string namePlayer, Image mark)
        {
            this.namePlayer = namePlayer;
            this.mark = mark;
        }
    }
}
