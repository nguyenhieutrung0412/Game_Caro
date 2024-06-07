using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager chessBoard;
        SocketManager socket;
        #endregion

        public Form1()
        {
            #region Properties
            
            #endregion
            InitializeComponent();
          //Control.CheckForIllegalCrossThreadCalls = false;
            chessBoard = new ChessBoardManager(pnChess,txbName,picbAvatar);
            chessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            chessBoard.EndedGame += ChessBoard_EndedGame;
            prbarCountDown.Step = Cons.cool_down_step;
            prbarCountDown.Maximum = Cons.countDownTime;
            prbarCountDown.Value = 0;

            tmcountdown.Interval = Cons.cool_down_interval;
            socket = new SocketManager();
            NewGame();
            
            //tmcountdown.Start();
        }
        void EndGame()
        {
            tmcountdown.Stop();
            pnChess.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            MessageBox.Show("Kết thúc game");
        }
        private void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            tmcountdown.Start();
            pnChess.Enabled = false;
            prbarCountDown.Value = 0;
            socket.Send(new SocketData((int)SocketData.SocketCommand.SEND_POINT,"", e.ClickPoint));

            Listen();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmcountdown_Tick(object sender, EventArgs e)
        {
            prbarCountDown.PerformStep();
            if(prbarCountDown.Value >= prbarCountDown.Maximum)
            {
               
                EndGame();
               
            }    
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát game không?", "Thông Báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;

        }
        void NewGame()
        {

            prbarCountDown.Value = 0;
            tmcountdown.Stop();
            chessBoard.DrawChessBoard();
            undoToolStripMenuItem.Enabled = true;
        }
        void Quit()
        {
           
                Application.Exit();
        }
        void undo()
        {
            chessBoard.Undo();
        }

      

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void btnLan_Click(object sender, EventArgs e)
        {
            socket.IP = txbIP.Text;
            if (!socket.ConnectSever())
            {
                socket.isServer = true;
                pnChess.Enabled = true;
                socket.CreateServer(); 
            }
            else
            {
                socket.isServer = false;
                pnChess.Enabled = false;
                Listen();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if(string.IsNullOrEmpty(txbIP.Text))
            {
                txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }
        void Listen()
        {
           
                Thread listenThread = new Thread(() =>
                {
                    try
                    {
                        SocketData data = (SocketData)socket.Receive();
                        ProcessData(data);
                    }
                    catch 
                    {
                    }
                });
                listenThread.IsBackground = true;
                listenThread.Start();
           
           

        }
        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketData.SocketCommand.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketData.SocketCommand.NEWGAME:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketData.SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        prbarCountDown.Value = 0;
                        pnChess.Enabled = true;
                        tmcountdown.Start();
                        chessBoard.OtherPlayerMark(data.Point);
                    }));
               
                   
                    break;
                case (int)SocketData.SocketCommand.UNDO:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketData.SocketCommand.QUIT:
                    MessageBox.Show(data.Message);
                    break;
                default:
                    break;
            }
            Listen();
        }
    }
}
