using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace local_search_algos
{
    public partial class Form1 : Form
    {
        const int SIZE = 450;

        List<Queen> queens;
        List<Queen> tmpState;
        List<Queen> C1, C2; //starša za genetic algo
        List<List<Queen>> states;
        Random rand = new Random();

        public void HillClimbAlgo(int steps)
        {
            int tmp, localMin,
                n = queens.Count(),
                stepCount = 0, takeCount = 0;
            Random rnd = new Random();
            states = new List<List<Queen>>();
            
            bool climb = true;

            while (climb)
            {
                localMin = heuristicCalc(queens); //zračuna heuristic
                //optimal = false;

                //skozi vsak stolpec
                for (int i = 0; i < n; i++)
                {
                    //skozi vsako vrstico
                    for (int j = 0; j < n; j++)
                    {
                        //preskoči trenutno kraljico
                        if (j != queens[i].getY())
                        {
                            //move q in shrani stanje
                            tmp = queens[i].getY();
                            queens[i].MoveQueen(j);
                            states.Add(Methods.cloneList(queens));
                            queens[i].MoveQueen(tmp);
                        }
                    }
                }

                int min, heu, index = 0;
                min = heuristicCalc(states[0]);
                for(int i = 0; i < states.Count(); i++) //za vsako stanje v states preveri heuristic in izberi tistega z <=
                {
                    heu = heuristicCalc(states[i]);
                    if(heu <= min)
                    {
                        tmpState = states[i];
                        min = heu;
                        index = i; //stanje z enako heu
                    }
                } //do tu dobi stanje z min ali enakim heu v tmpState

                if (heuristicCalc(tmpState) < heuristicCalc(queens)) //če ima X boljše nižjo hevristiko kot S
                {
                    queens = tmpState;
                }
                else
                {
                    if(heuristicCalc(tmpState) == localMin && stepCount < steps) //premik v enakovredno stanje
                    {
                        queens = tmpState;
                        stepCount++;
                    }
                    else //če nemore izvest ničesar od zgoraj navedenega izbere naključno stanje, da ne nadaljuje z istim
                    {
                        info.Text = "I don't feel so good.";
                        int rndom = rnd.Next(states.Count());
                        queens = states[rndom];
                    }
                }

                states.Clear();
                takeCount++;

                if (heuristicCalc(queens) == 0) climb = false;
            }
            drawBoardOnly(n);
            heuCount.Text = heuristicCalc(queens).ToString();
            stepCounter.Text = takeCount.ToString();
            info.Text = "Solution found.";
        }

        public void SimAnnealAlgo(int temperature, int delta)
        {
            float T = temperature, deltaH;
            double prob, d;
            int pos, deltaHtmp, deltaHq,
                index = 0, stepCount = 0, n = queens.Count();
            Random rnd = new Random();
            while(true)
            {
                if (T == 0)
                {
                    info.Text = "Algorithm ended with no solution.";
                    break; //ko pridemo do T == 0, konec algota
                }
                
                index = rnd.Next(n); //random move generator
                pos = rnd.Next(n);

                tmpState = Methods.cloneList(queens); //trenutno stanje shranimo v tmp, naključni naslednik
                tmpState[index].setY(pos); //naključni naslednik od queens stanja

                deltaHtmp = heuristicCalc(tmpState);
                deltaHq = heuristicCalc(queens);
                deltaH = deltaHtmp - deltaHq;

                if (deltaH < 0)
                {
                    queens = Methods.cloneList(tmpState);
                }
                else
                {
                    prob = Math.Pow(Math.E, -deltaH / T); //računanje verjetnosti
                    d = rnd.NextDouble();
                    
                    if (d > prob)
                    {
                        Debug.WriteLine(prob);
                        queens = Methods.cloneList(tmpState);
                    }
                }
                if(heuristicCalc(queens) == 0)
                {
                    info.Text = "Solution found.";
                    break;
                }
                T -= delta;
                stepCount++;
                index++;
            }
            drawBoardOnly(n);
            heuCount.Text = heuristicCalc(queens).ToString();
            stepCounter.Text = stepCount.ToString();
        }

        public void localBeamAlgo(int stCount, int maxIt) //stCount = število stanj v eni zaključeni množici, maxIt = število iteracij
        {
            int n = queens.Count(), heu, tmp, bound,
                steps = 1;
            bool shooting = true;
            ListWithDuplicates P = new ListWithDuplicates();
            int heuristic = heuristicCalc(queens);
            P.Add(heuristic, Methods.cloneList(queens));
            for (int i = 0; i < stCount-1; i++) //generiranje prvih k stanj
            {
                queens.Clear();
                boardGen(n);
                heu = heuristicCalc(queens);
                tmpState = Methods.cloneList(queens);
                P.Add(heu, tmpState);
            }
            //do tu imam množico P s stCount stanji in izračunanimi hevristikami

            while(shooting)
            {
                P.sortList(); //prvo sortiranje lista
                /*foreach (var x in P)
                {
                    Debug.WriteLine(x.Key + ":");
                    printPos(x.Value);
                }*/
                var result = P.Find(x => x.Key == 0);
                if (result.Key == 0 && result.Value != null) //če obstaja stanje s h = 0 prekini algo
                {
                    queens = result.Value;
                    //shooting = false;
                    break;
                }
                else //če ne, množica Q = k najbolje ocenjenih stanj v P
                {
                    if(P.Count() > stCount) P.RemoveRange(stCount, P.Count() - stCount);
                    foreach (var x in P)
                    {
                        Debug.WriteLine(x.Key + ":");
                        printPos(x.Value);
                    }
                } //do tu ostane le še stCount elementov v listu

                //razvij P
                bound = P.Count();
                for (int count = 0; count < bound; count++)
                {
                    queens = P[count].Value;
                    for (int i = 0; i < n; i++)
                    {
                        //skozi vsako vrstico
                        for (int j = 0; j < n; j++)
                        {
                            //preskoči trenutno kraljico
                            if (j != queens[i].getY())
                            {
                                //move q in shrani stanje
                                tmp = queens[i].getY();
                                queens[i].MoveQueen(j);
                                P.Add(heuristicCalc(queens), Methods.cloneList(queens));
                                queens[i].MoveQueen(tmp);
                            }
                        }
                    }
                }
                steps++;
                if (steps == maxIt)
                {
                    info.Text = "Algorithm ended with no solution.";
                    break;
                }
            }
            info.Text = "Solution found.";
            drawBoardOnly(n);
            heuCount.Text = heuristicCalc(queens).ToString();
            stepCounter.Text = (steps).ToString();
        }

        public void geneticAlgo(int pop, float elite, float cross, float mut, int gen)
        {
            //TODO: Genetic Algo implementation
            ListWithDuplicates P = new ListWithDuplicates();
            ListWithDuplicates Q = new ListWithDuplicates();
            int heuristic = heuristicCalc(queens), n = queens.Count(), steps = 0,
                heu;
            P.Add(heuristic, Methods.cloneList(queens));
            for (int i = 0; i < pop - 1; i++) //generiranje k kromosomov
            {
                queens.Clear();
                boardGen(n);
                heu = heuristicCalc(queens);
                tmpState = Methods.cloneList(queens);
                P.Add(heu, tmpState);
            } //do tu imam množico P s kromosomi

            while(true)
            {
                //zračunaj h vseh kromosomov v P
                P.sortList(); //uredi P po ocenah
                tmpState = P[0].Value; //najbolje ocenjen kromosom, ker je sortiran seznam, je na indexu 0
                if(P[0].Key == 0)
                {
                    queens = tmpState;
                    break;
                }
                Q.Clear(); //Q postane prazna množica
                for(int i = 0; i < pop/2; i++)
                {
                    //izberi dva starša iz P
                    C1 = rouletteSel(P);
                    C2 = rouletteSel(P);
                    //starša izbrana z roulette selection algotom

                    //križanje
                    if(rand.NextDouble() < cross)
                    {
                        TwoPointCross(C1, C2); //križaj C1 in C2
                    }
                    //else greta dalje C1 in C2 brez križanja

                    //mutacija
                    if(rand.NextDouble() < mut)
                    {
                        //mutiraj C1 in C2
                        C1[rand.Next(C1.Count())].y = rand.Next(C1.Count());
                        C2[rand.Next(C2.Count())].y = rand.Next(C2.Count());
                    }
                    //else C1 in C2 greta dalje
                    //dodaj C1 in C2 v Q
                    Q.Add(heuristicCalc(C1), Methods.cloneList(C1));
                    Q.Add(heuristicCalc(C2), Methods.cloneList(C2));
                }
                //P = Q
                //TODO: clone Q to P
                P = Methods.cloneListDuplicates(Q);
                if (steps == gen)
                {
                    info.Text = "Algorithm has ended with no solution.";
                    break;
                }
                steps++;
            }
            info.Text = "Solution found.";
            drawBoardOnly(n);
            heuCount.Text = heuristicCalc(queens).ToString();
            stepCounter.Text = (steps).ToString();
        }

        public void TwoPointCross(List<Queen> C1, List<Queen> C2)
        {
            int point1, point2, tmp;
            point1 = rand.Next(C1.Count());
            point2 = rand.Next(C1.Count());
            while(point1 == point2)
            {
                point1 = rand.Next(C1.Count());
                point2 = rand.Next(C1.Count());
            }
            if(point2 < point1)
            {
                tmp = point1;
                point1 = point2;
                point2 = tmp;
            }
            tmpState = Methods.cloneList(C1); //shrani prvega starša v tmp da lahko križamo
            for (int i = point1; i <= point2; i++)//za vsako kraljico v pravem C1 od point1 do point2 menjaj gena
            {
                tmp = C1[i].y;
                C1[i].y = C2[i].y;
                C2[i].y = tmp; //križa vse med p1 in p2
            }
        }

        public List<Queen> rouletteSel(ListWithDuplicates lwd)
        {
            int i = 0;
            int s = sigmaSum(lwd.Count(), lwd); //hevristika vseh kromosomov
            int a = rand.Next(s);
            while(sigmaSum(i, lwd) < a)
            {
                i++;
            }

            if (i >= 100)
            {
                i = rand.Next(100);
            }

            return lwd[i].Value;
        }

        public int sigmaSum(int k, ListWithDuplicates lwd)
        {
            int ret = 0;
            for(int j = 0; j < k; j++)
            {
                ret += lwd[j].Key;
                if (ret >= 100)
                {
                    ret = 0;
                }
            }
            return ret;
        }

        public int heuristicCalc(List<Queen> q)
        {
            int heu = 0;
            int remaining = 1; //število
            int tilesNum = 1;
            foreach(var x in q)
            {
                tilesNum = 1;
                for (int j = remaining; j < q.Count(); j++)
                {
                    int tmp;
                    tmp = q[j].y;
                    if (x.y - tilesNum == tmp ||
                        x.y == tmp ||
                        x.y + tilesNum == tmp)
                    {
                        heu++;
                    }
                    tilesNum++;
                }
                remaining++;
            }
            return heu;
        }

        public void printPos(List<Queen> q)
        {
            int i = 1;
            foreach(var x in q)
            {
                Debug.WriteLine("Queen " + i + " : {" + x.x + ", " + x.y + "}");
                i++;
            }
            Debug.WriteLine("\n");
        }

        public void boardGen(float n)
        {
            //float absSize = SIZE / n;

            
            int posY;
            for (int i = 0; i < n; i++)
            {
                posY = rand.Next((int)n);
                //q.x = i;
                //q.y = posY;
                queens.Add(new Queen(i, posY));
            }
            info.Text = "Board generated.";
        }

        public void drawBoardOnly(float n)
        {
            float absSize = SIZE / n;

            Bitmap bm = new Bitmap(SIZE, SIZE);
            Bitmap im = new Bitmap("queen.png");
            Bitmap adapted = new Bitmap(im, (int)absSize, (int)absSize);

            TextureBrush tb = new TextureBrush(adapted);
            using (Graphics g = Graphics.FromImage(bm))
            using (SolidBrush blackBrush = new SolidBrush(Color.FromArgb(61, 61, 61)))
            using (SolidBrush whiteBrush = new SolidBrush(Color.FromArgb(205, 150, 50)))
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if ((j % 2 == 0 && i % 2 == 0) || (j % 2 != 0 && i % 2 != 0))
                        {
                            g.FillRectangle(blackBrush, i * absSize, j * absSize, absSize, absSize);
                        }
                        else if ((j % 2 == 0 && i % 2 != 0) || (j % 2 != 0 && i % 2 == 0))
                        {
                            g.FillRectangle(whiteBrush, i * absSize, j * absSize, absSize, absSize);
                        }

                        foreach(var x in queens)
                        {
                            if(x.x == i && x.y == j) g.DrawImage(adapted, i * absSize, j * absSize, absSize, absSize);
                        }

                    }
                }
                chessCanvas.BackgroundImage = bm;
            }
        }

        public Form1()
        {
            InitializeComponent();
            sizeBox.SelectedIndex = 0;
            hillClimb.Checked = true;
            label1.Text = "Moves to equally good state:";
            queens = new List<Queen>();
            //default init for n = 4;
            boardGen(4);
            drawBoardOnly(4);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void startState_Click(object sender, EventArgs e)
        {
            queens.Clear(); //pobriše stanja kraljic
            printPos(queens);
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

            boardGen(n);
            drawBoardOnly(n);
            heuCount.Text = heuristicCalc(queens).ToString();
            printPos(queens);
            stepCounter.Text = "0";
            //info.Text = "Board generated.";
        }

        private void startAlgo_Click(object sender, EventArgs e)
        {
            if(hillClimb.Checked)
            {
                if (!int.TryParse(textBox1.Text, out int steps))
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                HillClimbAlgo(steps);
            }

            else if(simAnneal.Checked)
            {
                if (!int.TryParse(textBox1.Text, out int temp))
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                if (!int.TryParse(textBox2.Text, out int deltaTemp))
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                SimAnnealAlgo(temp, deltaTemp);
            }
            else if(beamSearch.Checked)
            {
                if (!int.TryParse(textBox1.Text, out int stateCount))
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                if (!int.TryParse(textBox2.Text, out int maxIt))
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                localBeamAlgo(stateCount, maxIt);
            }
            else if (genAlgo.Checked)
            {
                if (!int.TryParse(textBox1.Text, out int population)) //population size
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                /*elitism = (float)Convert.ToDouble(textBox2.Text);
                cross = (float)Convert.ToDouble(textBox3.Text);
                mutate = (float)Convert.ToDouble(textBox4.Text);*/

                if (!float.TryParse(textBox2.Text, out float elitism)) //elite percentage
                {
                    info.Text = "Invalid parameters";
                    return;
                }

                if (!float.TryParse(textBox3.Text, out float cross)) //cross
                {
                    info.Text = "Invalid parameters";
                    return;
                }

                if (!float.TryParse(textBox4.Text, out float mutate)) //mutate
                {
                    info.Text = "Invalid parameters";
                    return;
                }

                if (!int.TryParse(textBox5.Text, out int gens)) //generation count
                {
                    info.Text = "Invalid parameters";
                    return;
                }
                geneticAlgo(population, elitism, cross, mutate, gens);
                //geneticAlgo(100, 0.2F, 0.35F, 0.05F, 1000);
            }
        }

        private void hillClimb_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = "Moves to equally good state:";

            label2.Visible = false;
            textBox2.Visible = false;

            label3.Visible = false;
            textBox3.Visible = false;

            label4.Visible = false;
            textBox4.Visible = false;

            label5.Visible = false;
            textBox5.Visible = false;
        }

        private void simAnneal_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = "Starting temp:";

            label2.Text = "Temp change factor:";
            label2.Visible = true;
            textBox2.Visible = true;

            label3.Visible = false;
            textBox3.Visible = false;

            label4.Visible = false;
            textBox4.Visible = false;

            label5.Visible = false;
            textBox5.Visible = false;
        }

        private void beamSearch_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = "State count:";

            label2.Text = "Max iterations:";
            label2.Visible = true;
            textBox2.Visible = true;

            label3.Visible = false;
            textBox3.Visible = false;

            label4.Visible = false;
            textBox4.Visible = false;

            label5.Visible = false;
            textBox5.Visible = false;
        }

        private void genAlgo_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = "Population size:";

            label2.Text = "Elite percentage:";
            label2.Visible = true;
            textBox2.Visible = true;

            label3.Text = "Cross-breed probability:";
            label3.Visible = true;
            textBox3.Visible = true;

            label4.Text = "Mutation probability:";
            label4.Visible = true;
            textBox4.Visible = true;

            label5.Text = "Generation count:";
            label5.Visible = true;
            textBox5.Visible = true;
        }

        private void heuristic_Click(object sender, EventArgs e)
        {

        }

        private void sizeBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chessCanvas_Click(object sender, EventArgs e)
        {

        }

        private void Info_Click(object sender, EventArgs e)
        {

        }
    }

    public class Methods
    {
        public static List<Queen> cloneList(List<Queen> oldList)
        {
            List<Queen> newList = new List<Queen>(oldList.Count);

            oldList.ForEach((item) =>
            {
                newList.Add(new Queen(item));
            });

            return newList;
        }

        public static ListWithDuplicates cloneListDuplicates(ListWithDuplicates oldList)
        {
            ListWithDuplicates newList = new ListWithDuplicates();

            oldList.ForEach((item) =>
            {
                newList.Add(new KeyValuePair<int, List<Queen>>(item.Key, item.Value));
            });

            return newList;
        }
    }

    public class Queen
    {
        public int x; //stolpec
        public int y; //vrstica

        public int getX() { return x; }
        public int getY() { return y; }
        public Queen(int x1, int y1) { x = x1; y = y1; }
        public Queen(Queen queen) { x = queen.x; y = queen.y; }
        public void setY(int y1) { y = y1; }
        public void MoveQueen(int y1) { y = y1; }
        public void ResetQueen(int x1, int y1) { x = x1; y = y1; }
    }

    public class ListWithDuplicates : List<KeyValuePair<int, List<Queen>>>
    {
        public void Add(int key, List<Queen> value)
        {
            var element = new KeyValuePair<int, List<Queen>>(key, value);
            this.Add(element);
        }

        public void sortList()
        {
            this.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
    }
}
