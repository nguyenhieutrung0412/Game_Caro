using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public class ChessBoardManager
    {
        #region Properties
        private Panel pnChess;
        private List<Player> player;
        private int currentPlayer;
        private TextBox txbPlayerName;
        private PictureBox picbMark;
        private List<List<Button>> matrix;
        private Stack<PlayInfo> playTimeLine;


        private event EventHandler<ButtonClickEvent> playerMarked;
        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value; 
            }
        }

        public Panel PnChess { get => pnChess; set => pnChess = value; }
        public List<Player> Player { get => player; set => player = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public TextBox TxbPlayerName { get => txbPlayerName; set => txbPlayerName = value; }
        public PictureBox PicbMark { get => picbMark; set => picbMark = value; }
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        public Stack<PlayInfo> PlayTimeLine { get => playTimeLine; set => playTimeLine = value; }
        #endregion

        #region Initialize
        public ChessBoardManager(Panel pnChess,TextBox txbPlayerName, PictureBox picbMark)
        {
            this.pnChess = pnChess;
            this.txbPlayerName = txbPlayerName;
            this.picbMark = picbMark;
            this.Player = new List<Player>()
            {
                new Player("Player",Image.FromFile( Application.StartupPath + ("\\Resources\\P1.jpg"))),
                new Player("Player2",Image.FromFile( Application.StartupPath + ("\\Resources\\P2.png")))
            };

          
         
        }
        #endregion

        #region Methods
        public void DrawChessBoard()
        {
            pnChess.Enabled = true;
            pnChess.Controls.Clear();
            playTimeLine = new Stack<PlayInfo>();
            currentPlayer = 0;

            ChangeLayer();
            matrix = new List<List<Button>>();

            Button btnOld = new Button() { Width = 0, Location = new Point(0, 0) };

            for (int i = 0; i < Cons.Height_board; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.Width_board; j++)
                {
                    Button btnChess = new Button()
                    {
                        Width = Cons.Width_chess,
                        Height = Cons.Height_chess,
                        Location = new Point(btnOld.Location.X + btnOld.Width, btnOld.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                        
                    };
                    btnChess.Click += BtnChess_Click;
                    pnChess.Controls.Add(btnChess);

                    Matrix[i].Add(btnChess);
                    btnOld = btnChess;
                }
                btnOld.Location = new Point(0, btnOld.Location.Y + Cons.Height_chess);
                btnOld.Width = 0;
                btnOld.Height = 0;
            }
        }

        private void BtnChess_Click(object sender, EventArgs e)
        {
         
            Button btn = sender as Button;
       
            if (btn.BackgroundImage != null)
                return;

            Mark(btn);
            PlayTimeLine.Push(new PlayInfo(getChessPoint(btn), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangeLayer();
            if (playerMarked != null)
            {
                playerMarked(this, new ButtonClickEvent(getChessPoint(btn)));
            }
            if ( isEndGame(btn))
            {
                endGame();
            } 
        }
        public void OtherPlayerMark(Point point)
        {
            Button btn = Matrix[point.Y][point.X];

            if (btn.BackgroundImage != null)
             
                return;
            Mark(btn);
            PlayTimeLine.Push(new PlayInfo(getChessPoint(btn), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangeLayer();
           
            if (isEndGame(btn))
            {
                endGame();
            }
        }
        public void endGame()
        {
            //MessageBox.Show("Kết thúc game");
            if(endedGame != null)
            {
                endedGame(this, new EventArgs());
            }
        }
        public bool Undo()
        { 
            if(PlayTimeLine.Count <= 0)
            {
                return false;
            }
            bool isUndo1 = UndoAStep();
            bool isUndo2 = UndoAStep();
            PlayInfo oldPoint = PlayTimeLine.Peek();
            currentPlayer = oldPoint.Currentplayer == 1 ? 0 : 1;
            
            return isUndo1 && isUndo2;
        }
        public bool UndoAStep()
        {
            if (PlayTimeLine.Count <= 0)
            {
                return false;
            }
            PlayInfo oldPoint = PlayTimeLine.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];

            btn.BackgroundImage = null;



            if (PlayTimeLine.Count <= 0)
            {
                currentPlayer = 0;
            }
            else
            {
                oldPoint = PlayTimeLine.Peek();
                currentPlayer = oldPoint.Currentplayer == 1 ? 0 : 1;
            }
            ChangeLayer();
            return true;
        }
        private bool isEndGame(Button btn)
        {
            return isEndHorizontal(btn) || isEndVertical(btn) || isEndPrimary(btn) || isEndSub(btn);
        }
        // hàm lấy tọa độ
        private Point getChessPoint(Button btn)
        {
           
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);
            Point point = new Point(horizontal,vertical);

            return point;
        }
        private bool isEndHorizontal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = point.X + 1; i < Cons.Width_board; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }

            return countLeft +  countRight == 5;
        }
        private bool isEndVertical(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = point.Y + 1; i < Cons.Height_board; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool isEndPrimary(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                //kiểm tra tránh trường hợp ra khỏi mảng (outside of index)
                if (point.Y - i < 0 || point.X - i < 0)
                    break;
                if (Matrix[point.Y -i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.Width_board - point.X; i++)
            {
                //kiểm tra tránh trường hợp ra khỏi mảng (outside of index)
                if (point.Y + i >= Cons.Height_board || point.X + i >= Cons.Width_board)
                    break;
                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool isEndSub(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                //kiểm tra tránh trường hợp ra khỏi mảng (outside of index)
                if ( point.X + i > Cons.Width_board || point.Y - i < 0 )
                    break;

                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.Width_board - point.X; i++)
            {
                //kiểm tra tránh trường hợp ra khỏi mảng (outside of index)
                if (point.Y + i >= Cons.Height_board || point.X - i < 0)
                    break;
                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private void Mark(Button btn)
        {
            btn.BackgroundImage = Player[currentPlayer].Mark;

          
        }
        private void ChangeLayer()
        {
            txbPlayerName.Text = Player[currentPlayer].NamePlayer;
            picbMark.Image = Player[currentPlayer].Mark;
        } 
        #endregion
        // kiểm tra sự kiện click để đếm ngược

    }

    public class ButtonClickEvent:EventArgs
    {
        private Point clickPoint;


        public Point ClickPoint { get => clickPoint; set => clickPoint = value; }
        public ButtonClickEvent(Point point)
        {
            this.clickPoint = point;
        }
    }
}
