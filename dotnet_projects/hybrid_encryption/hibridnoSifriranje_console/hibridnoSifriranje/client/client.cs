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

    public static byte[] clientPublic;
    const int msg_size = 1024;
    const int BlockBitSize = 128;
    const int KeyBitSize = 256;

    static void Main(string[] args)
    {

        //init tcpclienta
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        TcpClient client = new TcpClient();
        client.Connect(ip, 5000);
        Console.WriteLine("Connection established!");
        NetworkStream netst = client.GetStream();

        //init private keya za client
        ECDiffieHellmanCng clientPrivate = new ECDiffieHellmanCng();
        clientPrivate.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        clientPrivate.HashAlgorithm = CngAlgorithm.Sha256;
        clientPublic = clientPrivate.PublicKey.ToByteArray();
        string publicKeyString = Convert.ToBase64String(clientPublic);

        Console.WriteLine("Private and public key generated, waiting to send public key: " + publicKeyString + " ...");

        //poslan public key
        Send(netst, publicKeyString);
        Console.WriteLine("Public key sent.");

        //prejemanje server public keya

        string data = Receive(netst);

        Console.WriteLine("Server public key received: " + data);
        byte[] bServerPublic = Convert.FromBase64String(data); //will use u in future

        //NA TEJ TOČKI IMATA IZMENJANA PUBLIC KEYA

        //pošiljanje in prejem IV
        byte[] IV = Encoding.UTF8.GetBytes("nikolinepovejres");
        Send(netst, "nikolinepovejres");
        string tujIVS = Receive(netst);
        Console.WriteLine("IV sent and received: " + tujIVS);
        byte[] tujIV = Encoding.UTF8.GetBytes(tujIVS);

        //izračun simetričnega ključa iz serverjevega public keya
        byte[] simKey = clientPrivate.DeriveKeyMaterial(ECDiffieHellmanCngPublicKey.FromByteArray(bServerPublic, CngKeyBlobFormat.EccPublicBlob));
        Console.WriteLine("SIMETRIČEN KLJUČ JE: " + Convert.ToBase64String(simKey));

        //NA TEJ TOČKI IMATA OBA ENAK SIMETRIČNI KLJUČ
        //DO TU JE VSE OK

        string message = "TO JE TEST ENCRYPTIONA.";

        //enkripcija
        byte[] encrypted = Encrypt(message, simKey, IV); //32 bytes
        string outgoing = Encoding.UTF8.GetString(encrypted);
        byte[] toSend = Encoding.UTF8.GetBytes(outgoing);
        netst.Write(encrypted, 0, toSend.Length);
        //pošlje message
        Console.Write("Encrypted message: " + Encoding.UTF8.GetString(encrypted));
            
        Console.ReadKey();

        
        client.Client.Shutdown(SocketShutdown.Send);
        netst.Close();
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

    static string Decrypt(byte[] data, byte[] simKey, byte[]IV)
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
        } catch (Exception x)
        {
            Console.WriteLine("ERROR: " + x.Message);
        }
    }

    static string Receive(NetworkStream netst)
    {
        byte[] receivedBytes = new byte[msg_size];
        int bytesToRead = netst.Read(receivedBytes, 0, receivedBytes.Length);
        string toReturn = Encoding.UTF8.GetString(receivedBytes, 0, bytesToRead);
        return toReturn;
    }
}
