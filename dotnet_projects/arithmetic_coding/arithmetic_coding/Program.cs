using System;
using System.Diagnostics;
using System.IO;

namespace arithmetic_coding
{
    class Program
    {
	    static void Main(string[] args)
        {
            if (args.Length != 3 )
            {
                Console.WriteLine("Invalid argument count.");
                Environment.Exit(1);
            }

            String option = args[0];
            String fileIn = args[1];
            String fileOut = args[2];

            Encoder encoder = new Encoder(fileIn, fileOut);
            Stopwatch sw = new Stopwatch();
            long fileInSize, fileOutSize, compressionRatio;
            
            switch (option)
            {
                // Encode
                case "c":
                    sw.Start();
                    encoder.Encode();
                    sw.Stop();
                    Console.WriteLine($"Elapsed = {sw.Elapsed}");

                    fileInSize = new FileInfo(fileIn).Length;
                    fileOutSize = new FileInfo(fileOut).Length;
                    compressionRatio = (fileOutSize * 100) / fileInSize;
                    Console.WriteLine($"Input file size: {fileInSize}\n" +
                                      $"Output file size: {fileOutSize}\n" +
                                      $"Compression ratio: {compressionRatio}%");
                    break;
                
                // Decode
                case "d":
                    sw.Start();
                    encoder.Decode();
                    sw.Stop();
                    Console.WriteLine($"Elapsed = {sw.Elapsed}");
                    fileInSize = new FileInfo(fileIn).Length;
                    fileOutSize = new FileInfo(fileOut).Length;
                    compressionRatio = (fileOutSize * 100) / fileInSize;
                    Console.WriteLine($"Input file size: {fileInSize}\n" +
                                      $"Output file size: {fileOutSize}\n" +
                                      $"Compression ratio: {compressionRatio}%");
                    break;
                
                // Testing purposes, b for both (encode/decode)
                case "b":
                    encoder.TestEncoding();
                    break;
                
                default:
                    Console.WriteLine("Invalid option selected.");
                    Environment.Exit(1);
                    break;
            }
        }
    }
}