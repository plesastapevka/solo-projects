using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.IO;

namespace blockchain
{
    public partial class Form1 : Form
    {

        public bool alive, first = true;

        public void AppendTextBox(string value) //IZPISOVANJE V RICHTXTBOX
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

            info.AppendText(value);
            info.AppendText(Environment.NewLine);
        }        

        public class Block //block class, ki se potem dodaja v seznam v blockchain classu
        {
            public int Index { get; set; }
            public int Diff { get; set; }
            public int Nonce { get; set; }
            public DateTime TimeStamp { get; set; }
            public string PreviousHash { get; set; }
            public string Hash { get; set; }
            public string Data { get; set; }
            Random rndm = new Random();


            public Block(DateTime timeStamp, string previousHash, string data)
            {
                Index = 0;
                Diff = 2;
                TimeStamp = timeStamp;
                PreviousHash = previousHash;
                Data = data;
                //Hash = CalculateHash();
                Nonce = 0;
            }

            public string CalculateHash()
            {
                SHA256 sha256 = SHA256.Create();

                byte[] inputBytes = Encoding.UTF8.GetBytes(Index.ToString() + TimeStamp.ToString() + Data + PreviousHash + Diff.ToString() + Nonce.ToString());
                byte[] outputBytes = sha256.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();

                string pre = sb.Append('0', Diff).ToString();
                return pre + Convert.ToBase64String(outputBytes);
            }

        }

        public class Blockchain //driver za list z vsemi funkcijami
        {
            public IList<Block> chain { set; get; }

            public Blockchain()
            {
                StartChain();
                AddFirst();
            }

            public void StartChain()
            {
                chain = new List<Block>();
            }

            public void AddFirst()
            {
                Block first = new Block(DateTime.Now, "0", "{TEST:GENESIS}");
                first.Hash = first.CalculateHash();
                first.Nonce = 0;
                first.Index = 0;
                chain.Add(first);
            }

            public Block GetLatestBlock()
            {
                return chain[chain.Count - 1];
            }

            public void AddBlock(Block block)
            {
                Block latestBlock = GetLatestBlock();
                block.Index = latestBlock.Index + 1;
                block.PreviousHash = latestBlock.Hash;
                //block.Hash = block.CalculateHash();
                chain.Add(block);
            }
        }

        public bool IsValid(IList<Block> chain)
        {
            for (int i = 1; i < chain.Count; i++)
            {
                Block currentBlock = chain[i];
                Block previousBlock = chain[i - 1];

                if (currentBlock.Index - previousBlock.Index != 1) //index za 1 večji od prejšnjega in novi blok prevhash == prevblock hash
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }

        public Form1()
        {
            InitializeComponent();
            info.AppendText("Application running.");
            info.AppendText(Environment.NewLine);
        }

        public void ProofOfWork(Blockchain blockchain)
        {
            int nonce = 0;
            string calculatedHash, substr, pre;
            byte[] inputBytes;
            byte[] outputBytes;
            SHA256 sha256 = SHA256.Create(); //class za calculating hasha
            StringBuilder sb = new StringBuilder();
            Block b1 = new Block(DateTime.Now, null, "{sender:urbn,receiver:feri,amount:1000}");
            pre = sb.Append('0', b1.Diff).ToString();
            while (true)
            {
                //enačba za sestavljanje hasha
                inputBytes = Encoding.UTF8.GetBytes(b1.Index.ToString() + b1.TimeStamp.ToString() + b1.Data + b1.PreviousHash + b1.Diff.ToString() + nonce.ToString());
                outputBytes = sha256.ComputeHash(inputBytes);

                calculatedHash = Convert.ToBase64String(outputBytes); //npr.: 00000dasjaA(hdHSh ...
                //AppendTextBox("Calculated hash: " + calculatedHash);
                Console.WriteLine("Calculated hash: " + calculatedHash);

                substr = calculatedHash.Substring(0, b1.Diff); //prvih 5 znakov iz calculated hasha
                Console.WriteLine(substr);
                Console.WriteLine(pre);
                Console.WriteLine(nonce);

                if (pre == substr)
                {
                    b1.Nonce = nonce;
                    b1.Hash = calculatedHash;
                    AppendTextBox("Block created.");
                    blockchain.AddBlock(b1);
                    break;
                }
                else
                {
                    //AppendTextBox("Invalid difficulty.");
                    nonce++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)  //začetek ustvarjanja blokov
        {
            alive = true;
            Blockchain blockchain = new Blockchain();
            AppendTextBox("Genesis block created. Mining ...");
            for (int i = 0; i < 3; i++)
            {
                if (IsValid(blockchain.chain))
                {
                    AppendTextBox("Chain valid.");
                }
                else
                {
                    AppendTextBox("Chain invalid.");
                    return;
                }

                ProofOfWork(blockchain);

                foreach (Block b in blockchain.chain)
                {
                    AppendTextBox(b.Hash + " " + b.PreviousHash);
                }

                if (IsValid(blockchain.chain))
                {
                    AppendTextBox("Chain valid.");
                }
                else
                {
                    AppendTextBox("Chain invalid.");
                    return;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            alive = false;
            AppendTextBox("Mining stopped.");
        }
    }
}