using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace steganography
{
    class Program
    {
	    static void Main(string[] args)
		{
			Console.WriteLine("___F5 Steganography___");

			//String compress_image = "lena.png";
			//String compress_out = "out.bin";

			//String decompress_file = "out.bin";
			//String decompress_image = "OUT.png";
			Console.WriteLine("Your arguments:");
			foreach (var t in args)
			{
				Console.Write(t + " ");
			}
			Console.WriteLine("");

			String compressImage = "";
            String compressOut = "OUT.bin";
            String decompressFile = "";
            String decompressImage = "OUT_IMG.jpg";
			//String messageIn = "";
			//String messageOut = "";
			String txtFile = "";
			String MSG = "";
            bool mode;
			int N = -1;
			int M = -1;


            string HELP = "Usage: Steganography.exe <inFile> <h/e>(hide/extract) <in/out.txt> <N-threshold?> <M-unique threes?>";

            if (args.Length != 3 && args.Length != 5)
            {
                System.Console.WriteLine("!0 "+HELP);
                return;
            }

            if (args[1] == "h")
            {
				int n = -1;
				int m = -1;
                compressImage = args[0];
                txtFile = args[2];
                mode = true;
                if (!Int32.TryParse(args[3], out n) && !Int32.TryParse(args[4], out m))
                {
                    System.Console.WriteLine("N/M argument is not a number!");
					System.Console.WriteLine("!1 " + HELP);
					return;
                }
                if (args.Length != 5)
                {
                    System.Console.WriteLine("There should be 5 arguments in message-hiding mode");
					System.Console.WriteLine("!2 " + HELP);
					return;
                }
				N = Int32.Parse(args[3]);
				M = Int32.Parse(args[4]);
                if ((N < 0) || (M < 0))
                {
                    System.Console.WriteLine("N/M should be greater than 0!");
					System.Console.WriteLine("!3 " + HELP);
					return;
                }
				if(N > 60)
                {
					System.Console.WriteLine("Argument N must be <= 60!");
					return;
				}
				if(M > 5)
                {
					System.Console.WriteLine("Argument M must be <= 5!");
					return;
				}
				MSG = Utils.ReadTxtFile(txtFile);
            }
            else if (args[1] == "e")
            {
                decompressFile = args[0];
                txtFile = args[2];
                mode = false;
                if (args.Length != 3)
                {
                    System.Console.WriteLine("There should be 3 arguments in decompression mode");
					System.Console.WriteLine("!4 " + HELP);
					return;
                }
            }
            else
            {
                System.Console.WriteLine("!5 " + HELP);
                return;
            }

			//string message = "This is a hidden message";
			//int N = 10;
			//int M = 3;
			



			//MAKE
			if (mode)
			{
				Console.WriteLine("Selected option: compression");
				Console.WriteLine("		-> Reading image");
				Bitmap img = Utils.ReadImage(compressImage);
				if(img == null)
                {
					Console.WriteLine("No image with given name found: " + compressImage);
                }
				Console.WriteLine("		-> Getting image parameters");
				(int original_W, int original_H) = Utils.SaveOriginalParams(img);
				Console.WriteLine("		-> Correcting image parameters (divisable by 8)");
				(int w, int h) = Utils.CorrectImageDimensions(img);
				Console.WriteLine("		-> Getting block width and height");
				int blockW = w / 8;
				int blockH = h / 8;
				Console.WriteLine("		-> Checking message length");
				bool proceed = Utils.LimitMsgLength(MSG, blockW, blockH, N, M);
                if (!proceed)
                {
					Console.WriteLine("Message length is too big for the image you are trying to use!");
					return;
                }

				Console.WriteLine("		-> Getting RGB channels");
				(int[,] R, int[,] G, int[,] B) = Utils.GetChannels(img, w, h);
				Console.WriteLine("		-> Initializing 8x8 matrice arrays");
				(float[,,] R_matrices, float[,,] G_matrices, float[,,] B_matrices) = Utils.InitMatrices(blockW, blockH);
				Console.WriteLine("		-> Blockifying channels");
				Utils.Blockify(R, G, B, R_matrices, G_matrices, B_matrices, blockW, blockH);
				Console.WriteLine("		-> Applying haar-transformation");
				(float[,,] R_multiplied, float[,,] G_multiplied, float[,,] B_multiplied) = Utils.MultiplyHandle(R_matrices, G_matrices, B_matrices, blockW, blockH);
				Console.WriteLine("		-> Zig-zag decomposition");
				List<List<float>> lst = Utils.ZigZagManage(R_multiplied, G_multiplied, B_multiplied, blockW * blockH);
				Console.WriteLine("		-> Applying threshold");
				Utils.ThresholdQuantization(lst, N);
				int[] nums = Utils.F5(lst, N, M, MSG);


				Console.WriteLine("		-> Converting ints to bytes");
				byte[] bytes = Utils.ToBytes(nums);
				Console.WriteLine("		-> Huffman compression");
				uint originalDataSize = (uint)bytes.Length;
				byte[] compressedData = new byte[originalDataSize * (101 / 100) + 320];
				int compressedDataSize = HuffmanCompress.Compress(bytes, compressedData, originalDataSize);
				Console.WriteLine("		-> Trimming compressed data");
				byte[] CDATA = Utils.Trim(compressedDataSize, compressedData);
				Console.WriteLine("		-> Preparing metadata and image data");
				bytes = Utils.PrepareToWrite(w, h, original_W, original_H, blockW, blockH, CDATA, originalDataSize, M, N);
				Console.WriteLine("		-> Writing data to file");
				Utils.ByteArrayToFile("out.bin", bytes);
				Console.WriteLine("		-> DONE! Result in file: " + compressOut);
			}
			else
			{
				//UN-make
				Console.WriteLine("Selected option: decompression");
				Console.WriteLine("		-> Reading file");
				byte[] fromFile = File.ReadAllBytes(decompressFile);
				Console.WriteLine("		-> Getting image metadata and byte array");
				(int nH, int nW, int orH, int orW, int BH, int BW, int originalSize, int m, int n, byte[] newData) = Utils.RetrieveData(fromFile);//TODO tu še M in N preberi ven...
				byte[] decompressed = new byte[originalSize];
				Console.WriteLine("		-> Decompression");
				HuffmanDecompress.Decompress(newData, decompressed, (uint)newData.Length, (uint)originalSize);
				Console.WriteLine("		-> Converting to float array");
				int[] ints = Utils.ToInts(decompressed);
				Console.WriteLine("		-> Retrieving hidden message");
				string hiddenMSG = Utils.ReverseF5(ints, m, n);

				Console.WriteLine("		-> Splitting channels");
				(List<float> rev_R, List<float> rev_G, List<float> rev_B) = Utils.SplitChannels(ints);
				Console.WriteLine("		-> Reverse zig-zag process");
				(float[,,] RR, float[,,] RG, float[,,] RB, int usedSize) = Utils.ReverseZigZagManage(rev_R, rev_G, rev_B);
				Console.WriteLine("		-> Reverse haar transformation");
				(float[,,] RT, float[,,] GT, float[,,] BT) = Utils.ReverseMultiplyHandle(RR, RG, RB, usedSize);
				Console.WriteLine("		-> Assembling blocks into channels");
				(int[,] R_f, int[,] G_f, int[,] B_f) = Utils.AssembleChannels(RT, GT, BT, orW, orH, BW, BH);//TODO GET THOSE BLOCK ARGS
				Console.WriteLine("		-> Generating image");
				Bitmap outImg = Utils.GenerateImage(orW, orH, R_f, G_f, B_f);
				Console.WriteLine("		-> Saving image");
				Utils.SaveImage(outImg, decompressImage);
				Console.WriteLine("		-> DONE! Result image in file: " + decompressImage);
				Console.WriteLine("		-> RETRIEVED MESSAGE: " + hiddenMSG + " >>> SAVED TO " + txtFile);
				Utils.WriteTxtFile(txtFile, hiddenMSG);
			}
        }
    }
}