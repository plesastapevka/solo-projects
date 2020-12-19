using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
class Program
{
    static object lck = new object();
    static Dictionary<int, TcpClient> seznam = new Dictionary<int, TcpClient>();

    static void Main(string[] args) {
        //init socketa
        string ipaddress = "127.0.0.1";
        TcpListener socket = new TcpListener(IPAddress.Parse(ipaddress), 5000);
        TcpClient client;
        socket.Start();
        Console.WriteLine("Server up and running on: " + ipaddress + ":5000 ...");
        int st = 1;

        while(true) {
            try {
            //ustvarjanje niti
            client = socket.AcceptTcpClient();
            lock (lck) seznam.Add(st, client);
            Thread nit = new Thread(threadClients);
            nit.Start(st);
            st++;
            } catch (Exception x) {
                Console.WriteLine("ERROR: " + x.Message);
            }
        }
    }

    public static void threadClients(object o) {
        int id = (int)o;
        TcpClient client;
        string head = ""; //protocol crucial head
        string user = ""; //username
        string messRaw = ""; //raw message
        string addData = ""; //additional data
        string searchWord = ""; //beseda za igro
        string searchWordH = "";
        string[] gameWords = {"connection", "declaration", "webpage", "looking", "walking", "desire"};
        string[] gameWords_hidden = {"c_n_ect_on", "de_la_a_i_n", "_ebp_ge", "l_ok__g", "w_l_in_", "de_i_e"};
        lock (lck) client = seznam[id];
        bool game_start = false;

        while (true) {
            NetworkStream netst = client.GetStream();
            byte[] buffer = new byte[1024];
            int byte_count = netst.Read(buffer, 0, buffer.Length);

            if(byte_count == 0) {
                break;
            }
            try {
                string data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                string finalData = "";
                string[] words = data.Split("|");
                head = words[0]; //i.e J, M ...
                user = words[1]; //i.e bunny, nejc, plespev ...
                //Console.Write(words[0] + " " + words[1] + "\n");

                addData = DateTime.Now.ToString("hh:mm:ss tt");

                #region protocol

                switch(head) {
                    case "#J":
                        finalData = addData + " - " + user + " has joined. Hooray!";
                    break;

                    case "#M":
                    messRaw = words[2];
                    if(game_start == true && messRaw == searchWord) {
                        finalData = addData + " - " + user + " won. Congratulations. The word was: " + searchWord;
                        game_start = false;
                    }
                    else if(game_start == true) {
                        finalData = addData + " - " + user + "'s guess: " + messRaw + ". Word: " + searchWordH;
                    }
                    else {
                        finalData = addData + " - " + user + ": " + messRaw;
                    }
                    break;

                    case "#L":
                        finalData = addData + " - " + user + " has left. Booo!";
                    break;

                    case "#G":
                        Random rand = new Random();
                        int ind = rand.Next(0, 5);
                        game_start = true;
                        searchWord = gameWords[ind];
                        searchWordH = gameWords_hidden[ind];
                        finalData = addData + " - " + user + " has started a game. " + searchWordH;
                    break;

                    default:
                        finalData = "Unknown error.";
                    break;
                }

                #endregion

                broadcast(finalData);
                Console.WriteLine(finalData);
                
            } catch(Exception x) {
                Console.WriteLine("ERROR: " + x.Message);
                broadcast("SERVER ERROR: " + x.Message);
            }
        }
        lock (lck) seznam.Remove(id);
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    public static void broadcast(string data) { //pisanje na podatkovni tok posameznim clientom
        byte[] buffer = Encoding.UTF8.GetBytes(data + "\n");
        foreach (TcpClient client in seznam.Values) {
            writeToStream(client, buffer);
        }
    }
    public static void writeToStream(TcpClient client, byte[] buffer) { //pisanje na podatkovni tok
        NetworkStream netst = client.GetStream();
        netst.Write(buffer, 0, buffer.Length);
    }

    public static string readFromStream(NetworkStream netst) { //branje s podatkovnega toka
        byte[] buffer = new byte[1024];
        int bytesRead = netst.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}