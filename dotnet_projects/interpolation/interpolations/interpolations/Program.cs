using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Data;
using System.Diagnostics;
using DotSpatial.Topology;
using System.Data;
using System.IO;

namespace interpolations
{
    class Program
    {
        struct Point
        {
            public int row;
            public int col;
            public double x;
            public double y;
            public double visina;

            public Point(int r, int c, double x, double y, double h)
            {
                this.row = r;
                this.col = c;
                this.x = x;
                this.y = y;
                this.visina = h;
            }
        }
        static string globalPath = @"C:\Users\urban\OneDrive - Univerza v Mariboru\3_letnik\2_semester\GIS\vaja02_interpolacija\";
        static void Main(string[] args)
        {
            string path = @"C:\Users\urban\OneDrive - Univerza v Mariboru\3_letnik\2_semester\GIS\vaja02_interpolacija\DatotekePolnaMreza\Maribor_2_5_All.shp";
            Console.Write("Scaling factor: ");
            int factor = Convert.ToInt32(Console.ReadLine());
            Console.Write("Interpolation (simple/bilinear/inverse): ");
            string interpol = Console.ReadLine();
            Console.WriteLine("Starting " + interpol + " ...");
            double zacetnaLocljivost = 2.5;
            FeatureSet shpFile = (FeatureSet)FeatureSet.Open(path);

            int sirinaGrida = (int)((shpFile.Extent.MaxX - shpFile.Extent.MinX) / (double)zacetnaLocljivost) + 1;
            int visinaGrida = (int)((shpFile.Extent.MaxY - shpFile.Extent.MinY) / (double)zacetnaLocljivost) + 1;
            List<Point> queue = new List<Point>();
            Point?[,] newGrid = new Point?[visinaGrida * factor, sirinaGrida * factor];
           
            int row = 0, col = 0;
            for (int i = 0; i < shpFile.Vertex.Length / 2; i++)
            {
                Point p;
                p.x = shpFile.Vertex[i * 2];
                p.y = shpFile.Vertex[i * 2 + 1];
                p.visina = shpFile.Z[i];
                p.row = row;
                p.col = col;
                queue.Add(p);
                row++;
                if (row == visinaGrida)
                {
                    row = 0;
                    col++;
                }
            }

            for(int i = 0; i < queue.Count; i++)
            {
                Point v = queue[i];
                queue[i] = new Point(v.row * factor, v.col * factor, v.x, v.y, v.visina);
                newGrid[v.row * factor, v.col * factor] = queue[i];
            }

            switch(interpol)
            {
                case "simple":
                    interpolation(newGrid, queue);
                    break;

                case "bilinear":
                    bilinear(newGrid, factor);
                    break;

                case "inverse":
                    break;

                default:
                    Console.WriteLine("Invalid interpolation.");
                    break;
            }
            serializeShapeFile(newGrid, interpol);
            Console.WriteLine("Press enter to exit ...");
            Console.Read();
        }

        static void bilinear(Point?[,] grid, int factor)
        {
            double s00, s10, s01, s11;
            double x = 0, y = 0;
            int col = 0, row = 0;
            while(col + factor < grid.GetLength(1) && row + factor < grid.GetLength(0))
            {
                s00 = grid[0 + row, 0 + col].Value.visina;
                s10 = grid[row + factor, 0 + col].Value.visina;
                s01 = grid[0 + row, col + factor].Value.visina;
                s11 = grid[row + factor, col + factor].Value.visina;
                for (int i = 0; i < factor; i++)
                {
                    for (int j = 0; j < factor; j++)
                    {
                        if (grid[i + row, j + col] == null)
                        {
                            x = j / factor;
                            y = i / factor;
                            double s = (1 - x) * (1 - y) * s00 + x * (1 - y) * s10 + (1 - x) * y * s01 + x * y * s11;
                            grid[i + row, j + col] = new Point(i + row, j + col, x, y, s);
                        }
                    }
                }
                col += factor;
                if (col >= grid.GetLength(1) - factor)
                {
                    col = 0;
                    row += factor;
                }
            }
            //Console.WriteLine(grid[444, 123] == null);
            //Console.WriteLine(grid[1005, 1504] == null);
        }

        static void interpolation(Point?[,] grid, List<Point> queue)
        {
            int count = queue.Count;
            int length = grid.GetLength(0);
            int width = grid.GetLength(1);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (queue.Count > 0 && count < width * length) {
                Point p = queue[0];
                for (int colNum = p.col - 1; colNum <= (p.col + 1); colNum += 1)
                {
                    for (int rowNum = p.row - 1; rowNum <= (p.row + 1); rowNum += 1)
                    {
                        if (!((colNum == p.col) && (rowNum == p.row)))
                        {
                            if (isInBounds(colNum, rowNum, length, width))
                            {
                                //TODO: do smth with found element
                                if(grid[rowNum, colNum] == null)
                                {
                                    grid[rowNum, colNum] = p;
                                    count++;
                                    queue.Add(new Point(rowNum, colNum, p.x, p.y, p.visina));
                                }
                            }
                        }
                    }
                }
                queue.RemoveAt(0);
            }
            sw.Stop();
            Console.WriteLine("Elapsed: {0}", sw.Elapsed.TotalSeconds);
            //Console.ReadLine();
            //printArray(grid);
        }

        static void serializeShapeFile(Point?[,] grid, string type)
        {
            Console.WriteLine("Saving grid ...");
            StringBuilder sb = new StringBuilder();
            var fs = new FeatureSet(FeatureType.Point);
            var dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn("i", typeof(int)));
            dataTable.Columns.Add(new DataColumn("j", typeof(int)));
            dataTable.Columns.Add(new DataColumn("X", typeof(double)));
            dataTable.Columns.Add(new DataColumn("Y", typeof(double)));
            dataTable.Columns.Add(new DataColumn("Z", typeof(double)));

            for (int i = 0; i < grid.GetLength(0); i += 1)
            {
                for (int j = 0; j < grid.GetLength(1); j += 1)
                {
                    if (grid[i, j] == null)
                    {

                    }
                    else
                    {
                        var i1 = grid[i, j].Value.col;
                        var j1 = grid[i, j].Value.row;
                        var X = grid[i, j].Value.x;
                        var Y = grid[i, j].Value.y;
                        var Z = grid[i, j].Value.visina;

                        fs.Features.Add(new Coordinate(X, Y, Z));
                        dataTable.Rows.Add(i, j, X, Y, Z);
                    }
                }
            }

            if (type == "simple")
            {
                dataTable.TableName = "testTable";
                fs.DataTable = dataTable;
                fs.SaveAs(Path.Combine(globalPath, "test0.shp"), true);
                Console.WriteLine("Grid saved to: " + globalPath + "test0.shp");
            }
            else if (type == "bilinear")
            {
                dataTable.TableName = "testBilinear";
                fs.DataTable = dataTable;
                fs.SaveAs(Path.Combine(globalPath, "testBilinear0.shp"), true);
                Console.WriteLine("Grid saved to: " + globalPath + "testBilinear0.shp");
            }
        }

        static void printArray(Point?[,] arr)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    if(arr[i, j] == null)
                    {
                        //Console.Write("NULL");
                    }
                    //Console.Write(string.Format("{0} ", arr[i, j].Value.visina));
                }
                //Console.Write(Environment.NewLine + Environment.NewLine);
            }
            //Console.WriteLine("DONE");
            //Console.ReadLine();   
        }

        static bool isInBounds(int colNum, int rowNum, int ROWS, int COLS)
        {
            if ((colNum < 0) || (rowNum < 0))
            {
                return false;
            }
            if ((colNum >= COLS) || (rowNum >= ROWS))
            {
                return false;
            }
            return true;
        }
    }
}
