﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro
{
    [Serializable]
    public class SocketData
    {
        private int command;
        private Point point;
        private string message;

        public int Command { get => command; set => command = value; }

        public string Message { get => message; set => message = value; }
        public Point Point { get => point; set => point = value; }

        public SocketData(int command,string message, Point point )
        {
            this.Command = command;
            this.Message = message;
            this.Point = point;
        }
        public enum SocketCommand
        {
            SEND_POINT,
            NOTIFY,
            NEWGAME,
            UNDO,
            END_GAME,
            TIME_OUT,
            QUIT
        }

    }
}
