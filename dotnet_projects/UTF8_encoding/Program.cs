using System;
using System.Text;
using System.Linq;

namespace vaja03_UTF8
{

    class Program
    {

        static byte[] string2HexByte(String toByte) {
            int stChar = toByte.Length;
            byte[] bytes = new byte[stChar / 2];
            for(int i = 0; i < stChar; i += 2) {
                bytes[i / 2] = Convert.ToByte(toByte.Substring(i, 2), 16);
            }
            return bytes;
        }

        static bool checkInvalid(String toByte) {
            if(toByte == "ff" || toByte == "fe") return true;
            else return false;
        }

        #region variables
            static private char opt;
            static private String toCode;
        #endregion
        static void Main(string[] args)
        {
            Console.WriteLine("1) Decoding\n2) Encoding");
            opt = Console.ReadKey().KeyChar;
            //opt = '2';
            Console.WriteLine("\n");

            switch(opt) {
                #region decoding-case1
                case '1': //iz utf8 code unit v unicode code point

                Console.WriteLine("DECODING\nEnter UTF-8 input: ");
                toCode = Console.ReadLine();
                toCode = toCode.Replace(" ", string.Empty); //ob morebitnih presledkih jih odstrani in združi v en string
                
                if(checkInvalid(toCode)) break;
                
                byte[] hexBytes = string2HexByte(toCode); //pretvori string v byte hex zapis
                String utf8Encoded = Encoding.UTF8.GetString(hexBytes); //kodira po UTF-8
                char[] fragmented = utf8Encoded.ToCharArray(); //iz prejšnjega stringa pretvori v char
                Console.WriteLine(utf8Encoded); //zapiše na konzolo pravi znak
                
                int st = fragmented[0]; //pretvori char v int
                byte[] bytesToOutput = BitConverter.GetBytes(st);

                if(BitConverter.IsLittleEndian) {
                    Array.Reverse(bytesToOutput);
                }

                for(int i = 0; i < bytesToOutput.Length; i++) { //izpisovanje hex bytov
                    Console.Write("{0:X2} ", bytesToOutput[i]);
                }
                Console.WriteLine();

                break;
                #endregion

                #region encoding-case2
                case '2': //iz unicode code point v utf8 code unit

                Console.WriteLine("ENCODING\nEnter Unicode input: ");
                toCode = Console.ReadLine();
                toCode = toCode.Replace(" ", string.Empty); //pobriše presledke med števili
                int intVal = 0;

                if(toCode.Length > 4) break;

                intVal = Convert.ToInt32(toCode.Substring(0, toCode.Length), 16); //branje bytov iz hex stringa
            
                char outtt = (char)intVal; //pretvorba int v char
                //spodaj ----- pretvori int nazaj v string v hex zapisu
                char[] charToBytes = {outtt}; //shrani char v array da ga lahko izpiše po bytih
                byte[] bytes = Encoding.UTF8.GetBytes(charToBytes);

                for(int i = 0; i < bytes.Length; i++) { //izpisovanje rezultata
                    Console.Write("{0:X2} ", bytes[i]);
                }
                Console.WriteLine();
                
                break;
                #endregion

                default:
                Console.WriteLine("Invalid option.");
                break;
            }
        }
    }
}
