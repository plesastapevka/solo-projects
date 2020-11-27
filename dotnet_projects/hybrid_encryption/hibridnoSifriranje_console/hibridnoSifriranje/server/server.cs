using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security;
using System.IO;

class Program
{
    const int msg_size = 1024;
    const int BlockBitSize = 128;
    const int KeyBitSize = 256;
    public static byte[] serverPublic;

    static void Main(string[] args)
    {
        //init socketa
        string ipaddress = "127.0.0.1";
        TcpListener socket = new TcpListener(IPAddress.Parse(ipaddress), 5000);
        TcpClient client;
        socket.Start();
        Console.WriteLine("Server up and running on: " + ipaddress + ":5000 ...");

        //init private keya za server
        ECDiffieHellmanCng serverPrivate = new ECDiffieHellmanCng();
        serverPrivate.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        serverPrivate.HashAlgorithm = CngAlgorithm.Sha256;
        serverPublic = serverPrivate.PublicKey.ToByteArray();
        string publicKeyString = Convert.ToBase64String(serverPublic);

        client = socket.AcceptTcpClient();

        Console.WriteLine("Client connected. Waiting for public key ...");
        NetworkStream netst = client.GetStream();


        //prejem client public keya
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int readBytes = netst.Read(buffer, 0, client.ReceiveBufferSize);
        string data = Encoding.UTF8.GetString(buffer, 0, readBytes);
        Console.WriteLine("Client public key received: " + data);
        byte[] bClientPublic = Convert.FromBase64String(data);

        //poslan server public key clientu
        Send(netst, publicKeyString);
        Console.WriteLine("Server public key sent: " + publicKeyString);

        //NA TEJ TOČKI IMATA IZMENJANA PUBLIC KEYA

        //prejem IV in pošiljanje IV
        byte[] IV = Encoding.UTF8.GetBytes("vednoooopovejres");
        byte[] tujIV = Receive(netst);
        Console.WriteLine("Client IV: " + Encoding.UTF8.GetString(tujIV));
        Send(netst, "vednoooopovejres");

        //izračun
        byte[] simKey = serverPrivate.DeriveKeyMaterial(ECDiffieHellmanCngPublicKey.FromByteArray(bClientPublic, CngKeyBlobFormat.EccPublicBlob));
        Console.WriteLine("SIMETRIČEN KLJUČ JE: " + Convert.ToBase64String(simKey));

        //NA TEJ TOČKI IMATA OBA ENAK SIMETRIČNI KLJUČ
        //DO TU JE VSE OK

        int read = netst.Read(new byte[msg_size], 0, new byte[msg_size].Length);
        string kodirano = Encoding.UTF8.GetString(new byte[msg_size], 0, 32);
        byte[] encoded = Encoding.UTF8.GetBytes(kodirano);

        Console.WriteLine("Encoded message: " + kodirano);

        string decoded = Decrypt(encoded, simKey, tujIV);

        Console.WriteLine("Decoded message: " + decoded);

        Console.ReadKey();

        
        client.Client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    static byte[] Encrypt(string data, byte[] simKey, byte[] IV)
    {
        byte[] encrypted = null;
        RijndaelManaged crypt = new RijndaelManaged();
        crypt.Key = simKey;
        crypt.IV = IV;

        ICryptoTransform encrypt = crypt.CreateEncryptor(crypt.Key, crypt.IV);

        using (MemoryStream memst = new MemoryStream())
        {
            using (CryptoStream cryst = new CryptoStream(memst, encrypt, CryptoStreamMode.Write))
            {
                using (StreamWriter strW = new StreamWriter(cryst))
                {
                    strW.Write(data);
                }
                encrypted = memst.ToArray();
            }
        }

        return encrypted;
    }

    static string Decrypt(byte[] data, byte[] simKey, byte[] IV)
    {
        string output = null;
        RijndaelManaged crypt = new RijndaelManaged();
        crypt.Key = simKey;
        crypt.IV = IV;

        ICryptoTransform decrypt = crypt.CreateDecryptor(crypt.Key, crypt.IV);

        using (MemoryStream memst = new MemoryStream(data))
        {
            using (CryptoStream cryst = new CryptoStream(memst, decrypt, CryptoStreamMode.Read))
            {
                using (StreamReader strR = new StreamReader(cryst))
                {
                    output = strR.ReadToEnd();
                }
            }
        }

        return output;
    }

    static void Send(NetworkStream netst, string message)
    {
        try
        {
            byte[] send = Encoding.UTF8.GetBytes(message.ToCharArray(), 0, message.Length);
            netst.Write(send, 0, send.Length);
        }
        catch (Exception x)
        {
            Console.WriteLine("ERROR: " + x.Message);
        }
    }

    static byte[] Receive(NetworkStream netst)
    {
        byte[] receivedBytes = new byte[msg_size];
        int bytesToRead = netst.Read(receivedBytes, 0, receivedBytes.Length);
        string toConvert = Encoding.UTF8.GetString(receivedBytes, 0, bytesToRead);
        byte[] toReturn = Encoding.UTF8.GetBytes(toConvert);
        return toReturn;
    }
}
