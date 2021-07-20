using System;

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

            switch (option)
            {
                // Encode
                case "c":
                    encoder.Encode();
                    break;
                
                // Decode
                case "d":
                    encoder.Decode();
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