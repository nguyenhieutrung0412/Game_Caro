using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        ChessBoardManager chessBoard;
        public Form1()
        {
            #region Properties
            
            #endregion
            InitializeComponent();
            chessBoard = new ChessBoardManager(pnChess,txbName,picbAvatar);
            chessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            chessBoard.EndedGame += ChessBoard_EndedGame;
            prbarCountDown.Step = Cons.cool_down_step;
            prbarCountDown.Maximum = Cons.countDownTime;
            prbarCountDown.Value = 0;

            tmcountdown.Interval = Cons.cool_down_interval;
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
        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmcountdown.Start();
            prbarCountDown.Value = 0;
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

      
    }
}
