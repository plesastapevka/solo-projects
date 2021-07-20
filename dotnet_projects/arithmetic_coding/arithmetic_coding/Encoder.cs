using System;
using System.Collections.Generic;
using System.IO;

namespace arithmetic_coding
{
    public class Encoder
    {
        private Table _table;
        private string _filePathIn;
        private string _filePathOut;

        public Encoder(string filePathIn, string filePathOut)
        {
            this._filePathIn = filePathIn;
            this._filePathOut = filePathOut;
            this._table = new Table();
        }

        public int Encode()
        {
            Console.WriteLine($"Encoding file {_filePathIn} ...");
            int bitMode = 32;
            byte[] bytes = File.ReadAllBytes(_filePathIn);
            foreach (var b in bytes)
            {
                bool added = false;
                int location = _table.Contains(b);
                if (location >= 0)
                {
                    _table.Elements[location].Freq++;
                }
                else
                {
                    FileChars tmp = new FileChars(b);
                    _table.Elements.Add(tmp);
                }
            }

            for (int i = 0; i < _table.Elements.Count; i++)
            {
                if (i == 0)
                {
                    _table.Elements[i].High = _table.Elements[i].Freq;
                }
                else
                {
                    _table.Elements[i].Low = _table.Elements[i - 1].High;
                    _table.Elements[i].High = _table.Elements[i].Low + _table.Elements[i].Freq;
                }
            }

            ulong LBC = 0;
            ulong UBC = (ulong) Math.Pow(2, bitMode - 1) - 1;
            ulong sQ = (UBC + 1) / 2;
            ulong fQ = sQ / 2;
            ulong tQ = fQ * 3;
            ulong cFreq = (ulong) _table.Elements[^1].High;
            List<bool> o = new List<bool>();
            int e3Counter = 0;

            foreach (var b in bytes)
            {
                var step = (UBC - LBC + 1) / cFreq;
                UBC = LBC + step * (ulong) _table.Elements[_table.Contains(b)].High - 1;
                LBC = LBC + step * (ulong) _table.Elements[_table.Contains(b)].Low;

                while (UBC < sQ || LBC >= sQ)
                {
                    if (UBC < sQ)
                    {
                        LBC = LBC * 2;
                        UBC = (UBC * 2) + 1;
                        o.Add(false);
                        for (var j = 0; j < e3Counter; j++)
                        {
                            o.Add(true);
                        }

                        e3Counter = 0;
                    }

                    if (LBC >= sQ)
                    {
                        LBC = 2 * (LBC - sQ);
                        UBC = 2 * (UBC - sQ) + 1;
                        o.Add(true);
                        for (int j = 0; j < e3Counter; j++)
                        {
                            o.Add(false);
                        }

                        e3Counter = 0;
                    }
                }

                while (LBC >= fQ && UBC < tQ)
                {
                    LBC = 2 * (LBC - fQ);
                    UBC = 2 * (UBC - fQ) + 1;
                    e3Counter++;
                }
            }

            if (LBC < fQ)
            {
                o.Add(false);
                o.Add(true);
                for (var i = 0; i < e3Counter; i++)
                {
                    o.Add(true);
                }
            }
            else
            {
                o.Add(true);
                o.Add(false);
                for (int i = 0; i < e3Counter; i++)
                {
                    o.Add(false);
                }
            }

            // Writing to file
            int charNum = _table.Elements.Count;

            byte remainder = 0;
            while (o.Count % 8 != 0)
            {
                o.Add(false);
                remainder++;
            }

            BinaryWriter binWrite = new BinaryWriter(File.Open(_filePathOut, FileMode.Create));
            binWrite.Write(bitMode); // Write 4 bytes
            binWrite.Write(charNum); // Write 4 bytes
            binWrite.Write(remainder); // Write 1 byte

            for (int i = 0; i < _table.Elements.Count; i++)
            {
                binWrite.Write(_table.Elements[i].Symbol); // Write 1 byte
                binWrite.Write(_table.Elements[i].Freq); // Write 4 bytes
            }

            byte wr = 0;
            for (int i = 0; i < o.Count; i++)
            {
                if (o[i])
                {
                    wr |= 1 << 0;
                }

                if ((i + 1) % 8 == 0 && i != 0)
                {
                    binWrite.Write(wr); // Write 1 byte
                    wr = 0;
                }
                else
                {
                    wr = (byte) (wr << 1);
                }
            }

            binWrite.Close();
            Console.WriteLine($"File encoded to {_filePathOut}.");

            return 0;
        }

        public int Decode()
        {
            Console.WriteLine($"Decoding file {_filePathIn} ...");
            // POTENTIAL MISSING LOCAL DECLARATION OF Table table;
            List<bool> ins = new List<bool>();
            BinaryReader binRead = new BinaryReader(File.Open(_filePathIn, FileMode.Open));
            binRead.BaseStream.Position = 0;
            int bitMode, charNum;
            byte remainder;
            try
            {
                bitMode = binRead.ReadInt32();
                charNum = binRead.ReadInt32();
                remainder = binRead.ReadByte();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading data: {e.Message}");
                return 1;
            }

            // Read the rest of the data
            for (var i = 0; i < charNum; i++)
            {
                FileChars tmp = new FileChars(0) {Symbol = binRead.ReadByte(), Freq = binRead.ReadInt32()};
                _table.Elements.Add(tmp);
            }

            for (int i = 0; i < _table.Elements.Count; i++)
            {
                if (i == 0)
                {
                    _table.Elements[i].High = _table.Elements[i].Freq;
                }
                else
                {
                    _table.Elements[i].Low = _table.Elements[i - 1].High;
                    _table.Elements[i].High = _table.Elements[i].Low + _table.Elements[i].Freq;
                }
            }

            var remainingBytes = (int) (binRead.BaseStream.Length - binRead.BaseStream.Position);
            byte[] bytes = binRead.ReadBytes(remainingBytes);

            foreach (var b in bytes)
            {
                for (int i = 7; i >= 0; i--)
                {
                    int tmp = (b >> i & 1);
                    ins.Add(Convert.ToBoolean(tmp));
                }
            }

            for (int i = 0; i < remainder; i++)
            {
                ins.RemoveAt(ins.Count - 1);
            }

            ulong LBC = 0;
            ulong UBC = (ulong) Math.Pow(2, bitMode - 1) - 1;
            ulong sQ = (UBC + 1) / 2;
            ulong fQ = sQ / 2;
            ulong tQ = fQ * 3;
            ulong cFreq = (ulong) _table.Elements[^1].High;
            ulong step;

            ulong array = 0;
            int nextBitCounter = 0; // POTENTIAL BUG
            int nextBit = 0;

            for (int i = 0; i < bitMode - 1; i++)
            {
                if (nextBitCounter < ins.Count)
                {
                    if (ins[nextBitCounter])
                    {
                        array |= 1 << 0;
                    }
                }

                nextBitCounter++;

                if (i % (bitMode - 2) != 0 || i == 0)
                {
                    array = array << 1;
                }
            }

            ulong cumulative = (ulong) _table.Elements[^1].High;
            BinaryWriter binWrite = new BinaryWriter(File.Open(_filePathOut, FileMode.Create));

            for (var i = (ulong) 0; i < cumulative; i++)
            {
                step = (UBC - LBC + 1) / cFreq;
                double v = (array - LBC) / (double) step;
                byte outChar = _table.Find(v);
                binWrite.Write(outChar);

                ulong index = (ulong) _table.Contains(outChar);

                UBC = LBC + step * (ulong) _table.Elements[(int)index].High - 1;
                LBC = LBC + step * (ulong) _table.Elements[(int)index].Low;
                while (UBC < sQ || LBC >= sQ)
                {
                    if (UBC < sQ)
                    {
                        LBC = LBC * 2;
                        UBC = (UBC * 2) + 1;

                        nextBit = nextBitCounter >= ins.Count ? 0 : Convert.ToInt32(ins[nextBitCounter]);

                        nextBitCounter++;

                        array = 2 * array + (ulong) nextBit;
                    }

                    if (LBC >= sQ)
                    {
                        LBC = 2 * (LBC - sQ);
                        UBC = 2 * (UBC - sQ) + 1;

                        nextBit = nextBitCounter >= ins.Count ? 0 : Convert.ToInt32(ins[nextBitCounter]);
                        nextBitCounter++;

                        array = 2 * (array - sQ) + (ulong) nextBit;
                    }
                }

                while (LBC >= fQ && UBC < tQ)
                {
                    LBC = 2 * (LBC - fQ);
                    UBC = 2 * (UBC - fQ) + 1;

                    if (nextBitCounter >= ins.Count)
                    {
                        nextBit = 0;
                    }
                    else
                    {
                        nextBit = Convert.ToInt32(ins[nextBitCounter]);
                    }

                    nextBitCounter++;

                    array = 2 * (array - fQ) + (ulong) nextBit;
                }
            }
            
            binRead.Close();
            binWrite.Close();

            Console.WriteLine($"File decoded to {_filePathOut}.");

            return 0;
        }

        public int TestEncoding()
        {
            Console.WriteLine("Running test ...");
            _filePathOut = "custom_encoded.txt";
            Encode();
            _table = new Table();
            _filePathIn = "custom_encoded.txt";
            _filePathOut = "custom_original.txt";
            Decode();
            
            return 0;
        }
    }
}