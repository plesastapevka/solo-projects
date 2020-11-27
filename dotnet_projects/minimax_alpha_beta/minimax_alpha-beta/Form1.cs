using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace minimax_alpha_beta
{
    public partial class Form1 : Form
    {
        public class Position
        {
            public static int ID = 0;

            public int id;
            public int who { get; set; }
            public int x { get; set; }
            public int y { get; set; }

            public Position() { who = -1; x = 0; y = 0; id = ID; ID++;  }
            public Position(int who1, int x1, int y1) { who = who1; x = x1; y = y1; id = ID; ID++; }
        }

        

        class EstimateMove
        {
            public int o { set; get; }
            public int toX { set; get; }
            public int toY { set; get; }
            public List<Position> m;

            public EstimateMove() { o = 0; m = new List<Position>(); }
            public EstimateMove(int o1, List<Position> m1) { o = o1; m = DeepClone(m1); }
        }

        public List<Position> state; //vsebina pos so pozicije posameznih polj
        int turn = 0;

        public const int NA = -1; //not assigned
        public const int MAX = 0; //igralec
        public const int MIN = 1; //PC

        public int WIN;
        public int LOSE;
        public int DRAW;

        void InitState()
        {
            state = new List<Position>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    state.Add(new Position(NA, i, j));
                }
            }
        }

        void DrawBoard()
        {
            String name = "button";

            for (int i = 0; i < 9; i++)
            {
                if(state[i].who == 0) //nariši X, to pomeni da sem jaz dal gor iks
                {
                    name += (i+1).ToString();
                    Button b = this.Controls.Find(name, true)[0] as Button;
                    b.Image = Image.FromFile(@"..\\..\\img\\x.png");
                }
                else if(state[i].who == 1)
                {
                    name += (i+1).ToString();
                    Button b = this.Controls.Find(name, true)[0] as Button;
                    b.Image = Image.FromFile(@"..\\..\\img\\o.png");
                    b.Enabled = false;
                }
                name = "button";
            }
        }

        public Form1()
        {
            InitializeComponent();
            diff.SelectedIndex = 0;
            InitState();
            WIN = 0;
            LOSE = 0;
            DRAW = 0;
        }

        int heu(List<Position> P)
        {

            int[,] b = new int[3,3];

            foreach(var x in P)
            {
                b[x.x, x.y] = x.who;
            }

            for (int row = 0; row < 3; row++)
            {
                if (b[row, 0] == b[row, 1] && b[row, 1] == b[row, 2])
                {
                    if (b[row, 0] == 0)
                        return +10;
                    else if (b[row, 0] == 1)
                        return -10;
                }
            }

            for (int col = 0; col < 3; col++)
            {
                if (b[0, col] == b[1, col] && b[1, col] == b[2, col])
                {
                    if (b[0, col] == 0)
                        return +10;
                    else if (b[0, col] == 1)
                        return -10;
                }
            }

            if (b[0, 0] == b[1, 1] && b[1, 1] == b[2, 2])
            {
                if (b[0, 0] == 0)
                    return +10;
                else if (b[0, 0] == 1)
                    return -10;
            }
            if (b[0, 2] == b[1, 1] && b[1, 1] == b[2, 0])
            {
                if (b[0, 2] == 0)
                    return +10;
                else if (b[0, 2] == 1)
                    return -10;
            }

            return 0;
        }

        //VRNE MNOŽICO STANJ             PREJME ENO STANJE
        List<List<Position>> DevelopMoves(List<Position> d, int turn) //turn = kdo je na vrsti
        {
            //TODO: razvoj potez in shranjevanje v nov List<List<Position>>
            List<List<Position>> lod = new List<List<Position>>(); //sem se shranjujejo stanja
            List<Position> tmp = new List<Position>(); //tmp eno stanje
            for(int i = 0; i < 9; i++)
            {
                if(d[i].who == -1)
                {
                    tmp = DeepClone(d);
                    tmp[i].who = turn; //tisti ki je na vrsti
                    lod.Add(tmp); //dodamo to stanje v lod
                }
            }

            return lod; //vrne seznam vseh možnih stanj
        }
        
        bool CheckIfLeaf(List<Position> P)
        {
            for(int i = 0; i < 9; i++)
            {
                if(P[i].who == -1)
                {
                    return false;
                }
            }
            return true;
        }

        EstimateMove minimax(List<Position> P, int ig) //0 je player MAX (x), 1 je pc MIN (o)
        {

            int est; //ocena
            
            if(CheckIfLeaf(P)) //če pridemo do konca igre se zaključi
            {
                EstimateMove firstRet = new EstimateMove(heu(P), null);
                return firstRet;
            }

            if(ig == MAX) //če je na vrsti igralec se mu nastavi -infinite ocena
            {
                est = -1000000;
            }
            else          //če je na vrsti pc se mu nastavi +infinite ocena
            {
                est =  1000000;
            }

            List<Position> poteza = null; //poteza je NULL
            List<Position> Pi; //Pi je NULL
            //list<position> je eno stanje, rabim seznam stanj, da jih lahko shranjujem
            List<List<Position>> M = DevelopMoves(P, ig); //M množica možnih potez igralca v trenutnem stanju
            EstimateMove em = new EstimateMove(); //ta class vsebuje oceno in stanje, se pravi: {o, m}
            foreach(var mi in M) //za vsako potezo mi v M, mi = List<Position>
            {
                Pi = DeepClone(mi); //izvede potezo mi v P in jo shrani v Pi
                if(ig == MAX)
                {
                    em = minimax(Pi, MIN);
                }
                else
                {
                    em = minimax(Pi, MAX);
                }
                
                if((ig == MAX && em.o > est) || (ig == MIN && em.o < est)) {
                    est = em.o;
                    poteza = DeepClone(mi);
                }
            }
            return new EstimateMove(est, poteza);
        }

        void xoxo(Button x)
        {
            StringBuilder sb = new StringBuilder();
            string selSize = sizeBox.SelectedItem.ToString();
            info.Visible = true;

            foreach (char c in selSize)
            {
                if (c != 'x')
                {
                    sb.Append(c);
                }
                else break;
            }

            int n = Int32.Parse(sb.ToString());

            x.Image = Image.FromFile(@"..\\..\\img\\x.png");
            x.Enabled = false;
            EstimateMove MOVE = minimax(state, MIN); //sem dobi shranjen optimalni move

            if (turn == 4)
            {
                DRAW++;
                label5.Text = DRAW.ToString();
                //Thread.Sleep(5000);
                string message = "Izenačeno.";
                string caption = "Izenačeno.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                ResetBoard();
            }

            if (MOVE.m == null)
            {
                return;
            }

            state = DeepClone(MOVE.m);
            turn++;

            if (heu(state) > 0)
            {
                WIN++;
                label4.Text = WIN.ToString();
                Thread.Sleep(5000);
            }
            else if(heu(state) < 0)
            {
                LOSE++;
                label6.Text = LOSE.ToString();
                //Thread.Sleep(5000);
                string message = "Poraz.";
                string caption = "Poraz.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                ResetBoard();
            }
            
            
            DebugBoard();
            DrawBoard();
            
        }

        public void ResetBoard()
        {
            
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;

            button1.Image = null;
            button2.Image = null;
            button3.Image = null;
            button4.Image = null;
            button5.Image = null;
            button6.Image = null;
            button7.Image = null;
            button8.Image = null;
            button9.Image = null;

            turn = 0;
            Position.ID = 0;
            state.Clear();
            InitState();
        }

        public void DebugBoard()
        {
            //int index = 0;
            for(int i = 0; i < 9; i++)
            {
                if (i % 3 == 0)
                {
                    Debug.Write("\n");
                }
                Debug.Write(state[i].who.ToString());
            }
            Debug.Write("\n");
        }

        private void Replace(int id, int x1, int y1, int who1)
        {
            state[id].who = who1;
            state[id].x = x1;
            state[id].y = y1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Replace(0, 0, 0, 0);
            xoxo(sender as Button);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Replace(1, 0, 1, 0);
            xoxo(sender as Button);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Replace(2, 0, 2, 0);
            xoxo(sender as Button);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Replace(3, 1, 0, 0);
            xoxo(sender as Button);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Replace(4, 1, 1, 0);
            xoxo(sender as Button);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Replace(5, 1, 2, 0);
            xoxo(sender as Button);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Replace(6, 2, 0, 0);
            xoxo(sender as Button);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Replace(7, 2, 1, 0);
            xoxo(sender as Button);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Replace(8, 2, 2, 0);
            xoxo(sender as Button);
        }

        public static List<Position> DeepClone(List<Position> oldList)
        {
            if (oldList == null) return null;

            List<Position> newList = new List<Position>(oldList.Count);
            Position pos;
            for (int i = 0; i < 9; i++)
            {
                pos = new Position(oldList[i].who, oldList[i].x, oldList[i].y);
                pos.id = oldList[i].id;
                newList.Insert(pos.id, pos);
            }

            return newList;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
