using System;
using System.Collections.Generic;
using System.IO;

namespace arithmetic_coding
{
    class Program
    {
	    static int Encode(string fileNameIn, string fileNameOut)
	    {
		    Console.WriteLine("Encoding message...");
		    int bitMode = 32;
		    Table table = new Table();
		    byte[] bytes = File.ReadAllBytes(fileNameIn);
		    foreach (var b in bytes)
		    {
			    bool added = false;
			    int location = table.Contains(b);
			    if (location >= 0)
			    {
				    table.Elements[location].Freq++;
			    }
			    else
			    {
				    FileChars tmp = new FileChars(b);
				    table.Elements.Add(tmp);
			    }
		    }
		    
		    for (int i = 0; i < table.Elements.Count; i++) {
			    if (i == 0) {
				    table.Elements[i].High = table.Elements[i].Freq;
			    }
			    else {
				    table.Elements[i].Low = table.Elements[i - 1].High;
				    table.Elements[i].High = table.Elements[i].Low + table.Elements[i].Freq;
			    }
		    }

		    ulong LBC = 0;
		    ulong UBC = (ulong)Math.Pow(2, bitMode - 1) - 1;
		    ulong sQ = (UBC + 1) / 2;
		    ulong fQ = sQ / 2;
		    ulong tQ = fQ * 3;
		    ulong cFreq = (ulong)table.Elements[^1].High;
		    List<bool> o = new List<bool>();
		    int e3Counter = 0;

		    foreach (var b in bytes)
		    {
			    var step = (UBC - LBC + 1) / cFreq;
			    UBC = LBC + step * (ulong)table.Elements[table.Contains(b)].High - 1;
			    LBC = LBC + step * (ulong)table.Elements[table.Contains(b)].Low;

			    while (UBC < sQ || LBC >= sQ)
			    {
				    if (UBC < sQ)
				    {
					    LBC = LBC * 2;
					    UBC = (UBC * 2) + 1;
					    o.Add(false);
					    for (var j = 0; j < e3Counter; j++) {
						    o.Add(true);
					    }
					    e3Counter = 0;
				    }
				    if (LBC >= sQ)
				    {
					    LBC = 2 * (LBC - sQ);
					    UBC = 2 * (UBC - sQ) + 1;
					    o.Add(true);
					    for (int j = 0; j < e3Counter; j++) {
						    o.Add(false);
					    }
					    e3Counter = 0;
				    }
			    }
			    while (LBC >= fQ && UBC < tQ) {
				    LBC = 2 * (LBC - fQ);
				    UBC = 2 * (UBC - fQ) + 1;
				    e3Counter++;
			    }
		    }
		    
		    if (LBC < fQ)
		    {
			    o.Add(false);
			    o.Add(true);
			    for (var i = 0; i < e3Counter; i++) {
				    o.Add(true);
			    }
		    }
		    else
		    {
			    o.Add(true);
			    o.Add(false);
			    for (int i = 0; i < e3Counter; i++) {
				    o.Add(false);
			    }
		    }
		    
		    // Writing to file
		    int charNum = table.Elements.Count;

		    byte remainder = 0;
		    while (o.Count % 8 != 0) {
			    o.Add(false);
			    remainder++;
		    }

		    BinaryWriter binWrite = new BinaryWriter(File.Open(fileNameOut, FileMode.Create));
		    binWrite.Write(bitMode); // Write 4 bytes
		    binWrite.Write(charNum); // Write 4 bytes
		    binWrite.Write(remainder); // Write 1 byte
		    
		    for (int i = 0; i < table.Elements.Count; i++) {
			    binWrite.Write(table.Elements[i].Symbol); // Write 1 byte
			    binWrite.Write(table.Elements[i].Freq); // Write 4 bytes
		    }
		    
		    byte wr = 0;
		    for (int i = 0; i < o.Count; i++) {
			    if (o[i]) {
				    wr |= 1 << 0;
			    }
			    if ((i + 1) % 8 == 0 && i != 0) {
				    binWrite.Write(wr); // Write 1 byte
				    wr = 0;
			    }
			    else {
				    wr = (byte) (wr << 1);
			    }
		    }

		    return 0;
	    }

	    static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid argument count.");
                Environment.Exit(1);
            }

            String option = args[0];
            String fileIn = args[1];
            String fileOut = args[2];
            
            // TODO: Encode
            if (option == "c")
            {
	            Encode(fileIn, fileOut);
            }
            // TODO: Decode
            else if (option == "d")
            {
                
            }
            else
            {
                Console.WriteLine("Invalid option selected.");
                Environment.Exit(1);
            }
        }
    }
}