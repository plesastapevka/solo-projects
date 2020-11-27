using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;

class Program
{

    static void Main(string[] args) {
        //neede cons
        bool first_login = true;
        string username = "";
        string msg = "";
        string finalData = "";
        byte[] buffer;
        //init tcpclienta
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        TcpClient client = new TcpClient();
        client.Connect(ip, 5000);
        Console.WriteLine("Connection established!");
        NetworkStream netst = client.GetStream();

        //nitenje
        Thread nit = new Thread(o=>recv((TcpClient)o));
        nit.Start(client);
        try {
            while(true) {
                if(first_login) {
                    Console.Write("First time here. Enter username: ");
                    username = Console.ReadLine();
                    finalData = "#J|" + username;
                    first_login = false;
                }
                else {
                    //Console.Write("Message: ");
                    msg = Console.ReadLine();
                    if(msg != "#exit" && msg != "#game") finalData = "#M|" + username + "|" + msg;
                    else if(msg == "#game") finalData = "#G|" + username;
                    else {
                        finalData = "#L|" + username;
                        buffer = Encoding.UTF8.GetBytes(finalData);
                        netst.Write(buffer, 0, buffer.Length);
                        break;
                    }
                }

            buffer = Encoding.UTF8.GetBytes(finalData);
            netst.Write(buffer, 0, buffer.Length);
        }
    } catch(Exception x) {
        Console.WriteLine("Error: " + x.Message);
    }
    Console.WriteLine("You've left the chat.");
    client.Client.Shutdown(SocketShutdown.Send);
    nit.Join();
    netst.Close();
    client.Close();
    }

    static void recv(TcpClient client) {
        NetworkStream netst = client.GetStream();
        byte[] receivedBytes = new byte[1024];
        int bytesToRead;

        while ((bytesToRead = netst.Read(receivedBytes, 0, receivedBytes.Length)) > 0){
            Console.Write(Encoding.UTF8.GetString(receivedBytes, 0, bytesToRead));
        }
    }
}
