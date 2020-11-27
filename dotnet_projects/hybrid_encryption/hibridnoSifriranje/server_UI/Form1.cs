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

namespace server_UI
{
    public partial class Form1 : Form
    {
        const int msg_size = 1024;
        const int BlockBitSize = 128;
        const int KeyBitSize = 256;

        public static byte[] serverPublic;
        public static byte[] clientPublic;
        public static byte[] simKey;
        public static string fileName = null;
        public static byte[] IV = Encoding.UTF8.GetBytes("vednovsepovejres");
        public static byte[] tujIV = null;
        public static NetworkStream netst = null;

        TcpListener socket = null;
        TcpClient client = null;

        string ipaddress = "127.0.0.1";


        public Form1()
        {
            InitializeComponent();
            info.AppendText("Application running. Start starts a server, connect starts a client.");
            info.AppendText(Environment.NewLine);
        }
                
        

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

            info.AppendText(value);
            info.AppendText(Environment.NewLine);
        }

        private void start_Click(object sender, EventArgs e)  //vede se kot server
        {
            //init socketa
            clientButton.Visible = false;
            fileChoose.Visible = false;
            start.Text = "Started";
            AppendTextBox("SERVER STARTED.");
            socket = new TcpListener(IPAddress.Parse(ipaddress), 5000);
            
            socket.Start();
            AppendTextBox("\nServer up and running on: " + ipaddress + ":5000 ...");

            Thread t = new Thread(new ThreadStart(ServerKeyExchange));
            t.Start();
        }

        private void clientButton_Click(object sender, EventArgs e) //vede se kot client
        {
            //init tcpclienta
            start.Visible = false;
            clientButton.Text = "Connected";
            AppendTextBox("CLIENT STARTED.");
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            client = new TcpClient();
            client.Connect(ip, 5000);
            AppendTextBox("\nConnection established!");


            Thread t = new Thread(new ThreadStart(ClientKeyExchange));
            t.Start();
        }

        private void ClientKeyExchange() //funkcija se kliče ko se starta client
        {
            //init private keya za client
            netst = client.GetStream();
            ECDiffieHellmanCng clientPrivate = new ECDiffieHellmanCng();
            clientPrivate.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            clientPrivate.HashAlgorithm = CngAlgorithm.Sha256;
            clientPublic = clientPrivate.PublicKey.ToByteArray();
            string publicKeyString = Convert.ToBase64String(clientPublic);

            AppendTextBox("Private and public key generated, waiting to send public key: " + publicKeyString + " ...");

            //poslan public key
            Send(netst, publicKeyString);
            AppendTextBox("Public key sent.");

            //prejemanje server public keya

            string data = ClientReceive(netst);

            AppendTextBox("Server public key received: " + data);
            byte[] bServerPublic = Convert.FromBase64String(data); //will use u in future

            //NA TEJ TOČKI IMATA IZMENJANA PUBLIC KEYA

            //pošiljanje in prejem IV
            byte[] IV = Encoding.UTF8.GetBytes("nikolinepovejres");
            Send(netst, "nikolinepovejres");
            string tujIVS = ClientReceive(netst);
            AppendTextBox("IV sent and received: " + tujIVS);
            byte[] tujIV = Encoding.UTF8.GetBytes(tujIVS);

            //izračun simetričnega ključa iz serverjevega public keya
            simKey = clientPrivate.DeriveKeyMaterial(ECDiffieHellmanCngPublicKey.FromByteArray(bServerPublic, CngKeyBlobFormat.EccPublicBlob));
            AppendTextBox("SIMETRIČEN KLJUČ JE: " + Convert.ToBase64String(simKey));

            //NA TEJ TOČKI IMATA OBA ENAK SIMETRIČNI KLJUČ
        }

        private void ServerKeyExchange() //funkcija se kliče ko se starta server
        {
            TcpClient client;
            ECDiffieHellmanCng serverPrivate = new ECDiffieHellmanCng();
            serverPrivate.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            serverPrivate.HashAlgorithm = CngAlgorithm.Sha256;
            serverPublic = serverPrivate.PublicKey.ToByteArray();
            string publicKeyString = Convert.ToBase64String(serverPublic);

            client = socket.AcceptTcpClient();

            AppendTextBox("Client connected. Waiting for public key ...");
            netst = client.GetStream();


            //prejem client public keya
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int readBytes = netst.Read(buffer, 0, client.ReceiveBufferSize);
            string data = Encoding.UTF8.GetString(buffer, 0, readBytes);
            AppendTextBox("Client public key received: " + data);
            byte[] bClientPublic = Convert.FromBase64String(data);

            //poslan server public key clientu
            Send(netst, publicKeyString);
            AppendTextBox("Server public key sent: " + publicKeyString);

            //NA TEJ TOČKI IMATA IZMENJANA PUBLIC KEYA

            //prejem IV in pošiljanje IV
            IV = Encoding.UTF8.GetBytes("vednoooopovejres");
            tujIV = Receive(netst);
            AppendTextBox("Client IV: " + Encoding.UTF8.GetString(tujIV));
            Send(netst, "vednoooopovejres");

            //izračun
            simKey = serverPrivate.DeriveKeyMaterial(ECDiffieHellmanCngPublicKey.FromByteArray(bClientPublic, CngKeyBlobFormat.EccPublicBlob));
            AppendTextBox("SIMETRIČEN KLJUČ JE: " + Convert.ToBase64String(simKey));

            //NA TEJ TOČKI IMATA OBA ENAK KLJUČ

            byte[] receivedData = new byte[1000000];
            int receivedBytes = netst.Read(receivedData, 0, receivedData.Length);

            //getting decrypted data
            byte[] decrypted = Decrypt(receivedData, receivedBytes);

            //razpoznavanje filea po protokolu - prvi 4 byti dolžina imena
            //Copy (Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length);
            byte[] nameLength = new byte[4];
            Buffer.BlockCopy(decrypted, 0, nameLength, 0, nameLength.Length);
            int iNameLength = BitConverter.ToInt32(nameLength, 0);

            //naslednji byti - ime
            byte[] fileNameLocal = new byte[iNameLength];
            Buffer.BlockCopy(decrypted, nameLength.Length, fileNameLocal, 0, iNameLength);
            string sFileName = Encoding.UTF8.GetString(fileNameLocal);

            //naslednji byti - datoteka
            byte[] finalFile = new byte[receivedBytes - nameLength.Length - fileNameLocal.Length];
            Buffer.BlockCopy(decrypted, nameLength.Length + fileNameLocal.Length, finalFile, 0, decrypted.Length - iNameLength - nameLength.Length);

            //zapis v datoteko še
            string path = @"C:\Users\blaz\Desktop\PREJETO\decrypted\" + sFileName;
            File.WriteAllBytes(path, finalFile);
            AppendTextBox("File received and decrypted.");

        }

        void fileChoose_Click(object sender, EventArgs e) //choose file gumb
        {
            OpenFileDialog chooseFileD = new OpenFileDialog();
            chooseFileD.Filter = "Txt file (*.txt)|*.txt|Image file (*.png)|*.png|Sound file (*.mp3)|*.mp3";

            //prvi 4 byti nameLength, dalje name, dalje pa celi file
            if(chooseFileD.ShowDialog() == DialogResult.OK)
            {
                fileName = chooseFileD.FileName;
                string fileNameLocal = fileName.Split('\\').Last();
                int nameLength = fileNameLocal.Length;
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);     //
                byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);  // to troje se bo pošiljalo naprej
                byte[] output = File.ReadAllBytes(fileName);                 //

                //array to send velikost = velikost output arraya + velikost ime arraya + velikost dolžina imena arraya
                int size = output.Length + fileNameBytes.Length + nameLengthBytes.Length;
                byte[] toSend = new byte[size];

                //spodnja funkcija omogoča, da želene podatke spravim na želeno mesto
                Buffer.BlockCopy(nameLengthBytes, 0, toSend, 0, nameLengthBytes.Length);
                Buffer.BlockCopy(fileNameBytes, 0, toSend, nameLengthBytes.Length, fileNameBytes.Length);
                Buffer.BlockCopy(output, 0, toSend, nameLengthBytes.Length + fileNameBytes.Length, output.Length);

                //začetek enkripcije
                Encrypt(toSend);
                AppendTextBox("File encrypted and sent.");
            }
            else
            {
                AppendTextBox("Unknown error.");
            }
            
        }

        static void Encrypt(byte[] toSend)
        {
            byte[] encrypted = null;
            RijndaelManaged crypt = new RijndaelManaged();
            crypt.Key = simKey;
            crypt.IV = IV;
            crypt.BlockSize = BlockBitSize;
            crypt.KeySize = KeyBitSize;

            ICryptoTransform encrypt = crypt.CreateEncryptor(crypt.Key, crypt.IV);

            using (MemoryStream memst = new MemoryStream())
            {
                using (CryptoStream cryst = new CryptoStream(memst, encrypt, CryptoStreamMode.Write))
                {
                    cryst.Write(toSend, 0, toSend.Length);
                    encrypted = memst.ToArray();
                }
            }
            netst.Write(encrypted, 0, encrypted.Length);
            string path = @"C:\Users\blaz\Desktop\PREJETO\encrypted\encFile";
            File.WriteAllBytes(path, encrypted);
        }

        static byte[] Decrypt(byte[] data, int size)
        {
            byte[] decrypted = null;
            RijndaelManaged crypt = new RijndaelManaged();
            crypt.Key = simKey;
            crypt.IV = tujIV;
            crypt.BlockSize = BlockBitSize;
            crypt.KeySize = KeyBitSize;

            ICryptoTransform decrypt = crypt.CreateDecryptor(crypt.Key, crypt.IV);

            using (MemoryStream memst = new MemoryStream(data))
            {
                using (CryptoStream cryst = new CryptoStream(memst, decrypt, CryptoStreamMode.Write))
                {
                    cryst.Write(data, 0, size);
                    decrypted = memst.ToArray();
                }
            }

            return decrypted;
        }

        static void Send(NetworkStream netst, string message)
        {
            byte[] send = Encoding.UTF8.GetBytes(message.ToCharArray(), 0, message.Length);
            netst.Write(send, 0, send.Length);
        }

        static byte[] Receive(NetworkStream netst)
        {
            byte[] receivedBytes = new byte[msg_size];
            int bytesToRead = netst.Read(receivedBytes, 0, receivedBytes.Length);
            string toConvert = Encoding.UTF8.GetString(receivedBytes, 0, bytesToRead);
            byte[] toReturn = Encoding.UTF8.GetBytes(toConvert);
            return toReturn;
        }

        static string ClientReceive(NetworkStream netst)
        {
            byte[] receivedBytes = new byte[msg_size];
            int bytesToRead = netst.Read(receivedBytes, 0, receivedBytes.Length);
            string toReturn = Encoding.UTF8.GetString(receivedBytes, 0, bytesToRead);
            return toReturn;
        }

        private void info_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
