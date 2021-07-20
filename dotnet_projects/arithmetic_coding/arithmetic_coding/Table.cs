using System.Collections.Generic;
using System.Windows;

namespace arithmetic_coding
{
    public class FileChars
    {
        public byte Symbol { get; set; }
        public int Freq { get; set; }
        public int Low { get; set; }
        public int High { get; set; }
        
        public FileChars(byte s) {
            Symbol = s;
            Freq = 1;
            Low = 0;
            High = 0;
        }
    }
    public class Table
    {
        public List<FileChars> Elements { get; }

        public Table()
        {
            Elements = new List<FileChars>();
        }
        public int Contains(byte c) {
            for (var i = 0; i < Elements.Count; i++) {
                if (Elements[i].Symbol == c) {
                    return i;
                }
            }
            return -1;
        }

        public byte Find(double d)
        {
            foreach (var t in Elements)
            {
                if (d >= t.Low && d < t.High) {
                    return t.Symbol;
                }
            }

            return 0;
        }
    }
}