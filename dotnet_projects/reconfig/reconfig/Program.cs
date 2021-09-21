using System;

namespace reconfig
{
    internal static class Program
    {
        private static void PrintHelp()
        {
            Console.WriteLine("./reconfig <inFile.txt> <(S)imple|(I)mproved|(C)ustom> <(v)erbose|(s)ilent>");
        }
        
        static void Main(string[] args)
        {
            string file;
            string method;
            bool verb;
            switch (args.Length)
            {
                case 1 when args[0] == "help":
                    PrintHelp();
                    return;
                case 3:
                {
                    file = args[0];
                    method = args[1];
                    switch (args[2])
                    {
                        case "s":
                            verb = false;
                            break;
                        case "v":
                            verb = true;
                            break;
                        default:
                            PrintHelp();
                            return;
                    }

                    if (!file.Contains(".txt"))
                    {
                        Console.WriteLine("File not type .txt");
                        return;
                    }

                    break;
                }
                default:
                    PrintHelp();
                    return;
            }

            var r = new Reconfig(file);
            r.Read();

            switch (method)
            {
                case "S":
                    r.SimpleReversalSort(verb);
                    break;
                case "I":
                    r.SortImprovedBreakpoint(verb);
                    break;
                case "C":
                    r.CustomSort(verb);
                    break;
                default:
                    Console.WriteLine("Unknown method!");
                    PrintHelp();
                    break;
            }
        }
    }
}