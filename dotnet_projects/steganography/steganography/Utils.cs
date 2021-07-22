using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using steganography;

namespace steganography
{
    class Utils
    {
        public struct DataStruct
        {
            public int AC1 { get; set; }
            public int AC2 { get; set; }
            public int AC3 { get; set; }
            public bool C1 { get; set; }
            public bool C2 { get; set; }
            public bool C3 { get; set; }
            public bool x1 { get; set; }
            public bool x2 { get; set; }
        }

        static float[,] HAAR = new float[8, 8]
        {
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) Math.Sqrt(8.0 / 64.0), (float) (1.0 / 2.0), 0, (float) Math.Sqrt(
                    2.0 / 4.0),
                0, 0, 0
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) Math.Sqrt(8.0 / 64.0), (float) (1.0 / 2.0), 0,
                (float) -Math.Sqrt(
                    2.0 / 4.0),
                0, 0, 0
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) Math.Sqrt(8.0 / 64.0), (float) (-1.0 / 2.0), 0, 0,
                (float) Math.Sqrt(
                    2.0 / 4.0),
                0, 0
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) Math.Sqrt(8.0 / 64.0), (float) (-1.0 / 2.0), 0, 0,
                (float) -Math.Sqrt(
                    2.0 / 4.0),
                0, 0
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) -Math.Sqrt(8.0 / 64.0), 0, (float) (1.0 / 2.0), 0, 0,
                (float) Math.Sqrt(
                    2.0 / 4.0),
                0
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) -Math.Sqrt(8.0 / 64.0), 0, (float) (1.0 / 2.0), 0, 0,
                (float) -Math.Sqrt(
                    2.0 / 4.0),
                0
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) -Math.Sqrt(8.0 / 64.0), 0, (float) (-1.0 / 2.0), 0, 0, 0,
                (float) Math
                    .Sqrt(2.0 / 4.0)
            },
            {
                (float) Math.Sqrt(8.0 / 64.0), (float) -Math.Sqrt(8.0 / 64.0), 0, (float) (-1.0 / 2.0), 0, 0, 0,
                (float) -Math
                    .Sqrt(2.0 / 4.0)
            }
        };

        static float[,] HAAR_T = Transpose(HAAR);


        static float[,] Transpose(float[,] m)
        {
            int w = m.GetLength(0);
            int h = m.GetLength(1);

            float[,] res = new float[h, w];
            for (int i = 0;
                i < w;
                i++)
            {
                for (int j = 0; j < h; j++)
                {
                    res[j, i] = m[i, j];
                }
            }

            return res;
        }

        static Bitmap ReadImage(string path)
        {
            try
            {
                var image = new Bitmap(path);
                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        static (int, int) CorrectImageDimensions(Bitmap img) //need divisable by 8
        {
            int nW = img.Width;
            int nH = img.Height;
            while (nW % 8 != 0)
            {
                nW++;
            }

            while (nH % 8 != 0)
            {
                nH++;
            }

            return (nW, nH);
        }

        static (int[,], int[,], int[,]) GetChannels(Bitmap img, int w, int h)
        {
            int[,] R = new int[w, h];
            int[,] G = new int[w, h];
            int[,] B = new int[w, h];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (j >= img.Width || i >= img.Height)
                    {
                        R[j, i] = 0;
                        G[j, i] = 0;
                        B[j, i] = 0;
                        continue;
                    }

                    Color c = img.GetPixel(j, i);
                    R[j, i] = c.R;
                    G[j, i] = c.G;
                    B[j, i] = c.B;
                }
            }

            //printChannel(R, w, h);
            //Console.WriteLine("");
            //printChannel(G, w, h);
            //Console.WriteLine("");
            //printChannel(B, w, h);
            return (R, G, B);
        }

        static (int, int) SaveOriginalParams(Bitmap image)
        {
            return (image.Width, image.Height);
        }

        static (float[,,], float[,,], float[,,]) InitMatrices(int blockW, int blockH)
        {
            float[,,] rBlocks = new float[8, 8, blockW * blockH];
            float[,,] gBlocks = new float[8, 8, blockW * blockH];
            float[,,] bBlocks = new float[8, 8, blockW * blockH];
            return (rBlocks, gBlocks, bBlocks);
        }

        static (int, int) GetBlockNum(int w, int h)
        {
            return (w / 8, h / 8);
        }

        static void Blockify(int[,] R, int[,] G, int[,] B, float[,,] rMatrices, float[,,] gMatrices,
            float[,,] bMatrices,
            int blockW, int blockH)
        {
            int count = 0;
            for (int x = 0; x < blockW; x++)
            {
                for (int y = 0; y < blockH; y++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            rMatrices[i, j, count] = R[x * 8 + i, y * 8 + j];
                            gMatrices[i, j, count] = G[x * 8 + i, y * 8 + j];
                            bMatrices[i, j, count] = B[x * 8 + i, y * 8 + j];
                        }
                    }

                    count++;
                }
            }
        }

        static float[,] Multiply(float[,] a, float[,] b)
        {
            int M = 8;
            int N = 8;
            float[,] C = new float[M, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    C[i, j] = 0;
                    for (int k = 0; k < M; k++)
                    {
                        C[i, j] += a[i, k] * b[k, j];
                    }
                }
            }

            return C;
        }

        static float[,] Multiply(float[,] a, float[,,] b, int b_index)
        {
            int M = 8;
            int N = 8;
            float[,] C = new float[M, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    C[i, j] = 0;
                    for (int k = 0; k < M; k++)
                    {
                        C[i, j] += a[i, k] * b[k, j, b_index];
                    }
                }
            }

            return C;
        }

        static void Emplace(float[,,] rMatrices, float[,,] gMatrices, float[,,] bMatrices, float[,] TR, float[,] TG,
            float[,] TB, int index)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    rMatrices[i, j, index] = TR[i, j];
                    gMatrices[i, j, index] = TG[i, j];
                    bMatrices[i, j, index] = TB[i, j];
                }
            }
        }

        static (float[,,], float[,,], float[,,]) MultiplyHandle(float[,,] rMatrices, float[,,] gMatrices,
            float[,,] bMatrices, int blockW, int blockH)
        {
            int count = blockH * blockW;
            float[,,] rBlocks = new float[8, 8, blockW * blockH];
            float[,,] gBlocks = new float[8, 8, blockW * blockH];
            float[,,] bBlocks = new float[8, 8, blockW * blockH];

            for (int l = 0; l < count; l++)
            {
                //int m = 8, n = 8, q = 8;
                float[,] C_R = new float[8, 8];
                float[,] C_G = new float[8, 8];
                float[,] C_B = new float[8, 8];

                C_R = Multiply(HAAR_T, rMatrices, l);
                C_G = Multiply(HAAR_T, gMatrices, l);
                C_B = Multiply(HAAR_T, bMatrices, l);

                float[,] transformedR = Multiply(C_R, HAAR);
                float[,] transformedG = Multiply(C_G, HAAR);
                float[,] transformedB = Multiply(C_B, HAAR);

                Emplace(rBlocks, gBlocks, bBlocks, transformedR, transformedG, transformedB, l);
            }

            return (rBlocks, gBlocks, bBlocks);
        }

        static List<float> ZigZag(float[,,] block, int count)
        {
            List<float> lst = new List<float>();
            lst.Add(block[0, 0, count]);

            lst.Add(block[0, 1, count]);
            lst.Add(block[1, 0, count]);

            lst.Add(block[2, 0, count]);
            lst.Add(block[1, 1, count]);
            lst.Add(block[0, 2, count]);

            lst.Add(block[0, 3, count]);
            lst.Add(block[1, 2, count]);
            lst.Add(block[2, 1, count]);
            lst.Add(block[3, 0, count]);

            lst.Add(block[4, 0, count]);
            lst.Add(block[3, 1, count]);
            lst.Add(block[2, 2, count]);
            lst.Add(block[1, 3, count]);
            lst.Add(block[0, 4, count]);

            lst.Add(block[0, 5, count]);
            lst.Add(block[1, 4, count]);
            lst.Add(block[2, 3, count]);
            lst.Add(block[3, 2, count]);
            lst.Add(block[4, 1, count]);
            lst.Add(block[5, 0, count]);

            lst.Add(block[6, 0, count]);
            lst.Add(block[5, 1, count]);
            lst.Add(block[4, 2, count]);
            lst.Add(block[3, 3, count]);
            lst.Add(block[2, 4, count]);
            lst.Add(block[1, 5, count]);
            lst.Add(block[0, 6, count]);

            lst.Add(block[0, 7, count]);
            lst.Add(block[1, 6, count]);
            lst.Add(block[2, 5, count]);
            lst.Add(block[3, 4, count]);
            lst.Add(block[4, 3, count]);
            lst.Add(block[5, 2, count]);
            lst.Add(block[6, 1, count]);
            lst.Add(block[7, 0, count]);

            lst.Add(block[7, 1, count]);
            lst.Add(block[6, 1, count]);
            lst.Add(block[5, 3, count]);
            lst.Add(block[4, 4, count]);
            lst.Add(block[3, 5, count]);
            lst.Add(block[2, 6, count]);
            lst.Add(block[1, 7, count]);

            lst.Add(block[2, 7, count]);
            lst.Add(block[3, 6, count]);
            lst.Add(block[4, 5, count]);
            lst.Add(block[5, 4, count]);
            lst.Add(block[6, 3, count]);
            lst.Add(block[7, 2, count]);

            lst.Add(block[7, 3, count]);
            lst.Add(block[6, 4, count]);
            lst.Add(block[5, 5, count]);
            lst.Add(block[4, 6, count]);
            lst.Add(block[3, 7, count]);

            lst.Add(block[4, 7, count]);
            lst.Add(block[5, 6, count]);
            lst.Add(block[6, 5, count]);
            lst.Add(block[7, 4, count]);

            lst.Add(block[7, 5, count]);
            lst.Add(block[6, 6, count]);
            lst.Add(block[5, 7, count]);

            lst.Add(block[6, 7, count]);
            lst.Add(block[7, 6, count]);

            lst.Add(block[7, 7, count]);
            return lst;
        }

        static List<List<float>> ZigZagManage(float[,,] rMatrices, float[,,] gMatrices, float[,,] bMatrices,
            int count)
        {
            List<List<float>> res = new List<List<float>>();
            //List<float> temp = new List<float>();
            //float[] res = new float[count * 3];
            for (int i = 0; i < count; i++)
            {
                res.Add(ZigZag(rMatrices, i));
            }

            for (int i = 0; i < count; i++)
            {
                res.Add(ZigZag(gMatrices, i));
            }

            for (int i = 0; i < count; i++)
            {
                res.Add(ZigZag(bMatrices, i));
            }

            return res;
        }

        static float[] Threshold(List<float> l, int threshold)
        {
            float[] list = new float[l.Count];
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i] < threshold)
                {
                    list[i] = 0;
                }
                else
                {
                    list[i] = l[i];
                }
            }

            return list;
        }

        static void ThresholdQuantization(List<List<float>> l, int threshold)
        {
            int border = 64 - threshold - 1;
            for (int i = 0; i < l.Count; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (j <= border)
                    {
                        l[i][j] = (int) Math.Round(l[i][j]);
                    }
                    else
                    {
                        l[i][j] = 0;
                    }
                }
            }
        }

        static byte[] ToBytes(float[] nums)
        {
            byte[] res = new byte[nums.Length * sizeof(float)];
            Buffer.BlockCopy(nums, 0, res, 0, res.Length);
            return res;
        }

        static byte[] ToBytes(int[] nums)
        {
            byte[] res = new byte[nums.Length * sizeof(int)];
            Buffer.BlockCopy(nums, 0, res, 0, res.Length);
            return res;
        }

        public static byte[] Trim(int size, byte[] data)
        {
            byte[] res = new byte[size];
            Array.Copy(data, 0, res, 0, size);
            return res;
        }

        public static byte[] PrepareToWrite(int w, int h, int originalW, int originalH, int blockW, int blockH,
            byte[] data,
            uint originalSize, int M, int N)
        {
            byte[] H = BitConverter.GetBytes(h); //block height
            byte[] W = BitConverter.GetBytes(w); //block width
            byte[] oH = BitConverter.GetBytes(originalH);
            byte[] oW = BitConverter.GetBytes(originalW);
            byte[] bH = BitConverter.GetBytes(blockH);
            byte[] bW = BitConverter.GetBytes(blockW);
            byte[] ods = BitConverter.GetBytes(originalSize); //original data size
            byte[] MD = BitConverter.GetBytes(M);
            byte[] ND = BitConverter.GetBytes(N);

            byte[] rv = new byte[H.Length + W.Length + oH.Length + oW.Length + bH.Length + bW.Length + ods.Length +
                                 MD.Length +
                                 ND.Length + data.Length]; //data ALL


            System.Buffer.BlockCopy(H, 0, rv, 0, H.Length);
            System.Buffer.BlockCopy(W, 0, rv, H.Length, W.Length);
            System.Buffer.BlockCopy(oH, 0, rv, H.Length + W.Length, oH.Length);
            System.Buffer.BlockCopy(oW, 0, rv, H.Length + W.Length + oH.Length, oW.Length);
            System.Buffer.BlockCopy(bH, 0, rv, H.Length + W.Length + oH.Length + oW.Length, bH.Length);
            System.Buffer.BlockCopy(bW, 0, rv, H.Length + W.Length + oH.Length + oW.Length + bH.Length, bW.Length);
            System.Buffer.BlockCopy(ods, 0, rv, H.Length + W.Length + oH.Length + oW.Length + bH.Length + bW.Length,
                ods.Length);
            System.Buffer.BlockCopy(MD, 0, rv,
                H.Length + W.Length + oH.Length + oW.Length + bH.Length + bW.Length + ods.Length,
                MD.Length);
            System.Buffer.BlockCopy(ND, 0, rv,
                H.Length + W.Length + oH.Length + oW.Length + bH.Length + bW.Length + ods.Length + MD.Length,
                ND.Length);
            System.Buffer.BlockCopy(data, 0, rv,
                H.Length + W.Length + oH.Length + oW.Length + bH.Length + bW.Length + ods.Length + MD.Length +
                ND.Length,
                data.Length);
            return rv;
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        static (int, int) Expand(int width, int height)
        {
            int block1;
            if (width % 8 == 0)
            {
                block1 = width / 8;
            }
            else
            {
                block1 = width / 8;
                block1++;
                block1++;
            }

            int block2;
            if (height % 8 == 0)
            {
                block2 = height / 8;
            }
            else
            {
                block2 = height / 8;
                block2++;
                block2++;
            }

            return (block1, block2);
        }

        static (int, int, int, int, int, int, int, int, int, byte[]) RetrieveData(byte[] data)
        {
            byte[] h = new byte[4];
            byte[] w = new byte[4];
            byte[] originalH = new byte[4];
            byte[] originalW = new byte[4];
            byte[] bH = new byte[4];
            byte[] bW = new byte[4];
            byte[] size = new byte[4];
            byte[] m = new byte[4];
            byte[] n = new byte[4];

            int sz = data.Length - (9 * 4); //9 ints of data
            byte[] res = new byte[sz];
            Array.Copy(data, 0, h, 0, 4);
            Array.Copy(data, 4, w, 0, 4);

            Array.Copy(data, 8, originalH, 0, 4);
            Array.Copy(data, 12, originalW, 0, 4);

            Array.Copy(data, 16, bH, 0, 4);
            Array.Copy(data, 20, bW, 0, 4);

            Array.Copy(data, 24, size, 0, 4);

            Array.Copy(data, 28, m, 0, 4);
            Array.Copy(data, 32, n, 0, 4);

            Array.Copy(data, 36, res, 0, sz);

            int H = BitConverter.ToInt32(h, 0);
            int W = BitConverter.ToInt32(w, 0);
            int oH = BitConverter.ToInt32(originalH, 0);
            int oW = BitConverter.ToInt32(originalW, 0);
            int blockH = BitConverter.ToInt32(bH, 0);
            int blockW = BitConverter.ToInt32(bW, 0);
            int SIZE = BitConverter.ToInt32(size, 0);
            int M = BitConverter.ToInt32(m, 0);
            int N = BitConverter.ToInt32(n, 0);

            return (H, W, oH, oW, blockH, blockW, SIZE, M, N, res);
        }

        static float[] ToFloats(byte[] data)
        {
            int counter = 0;
            float[] res = new float[data.Length / 4]; //float = 4 bytes
            byte[] buffer = new byte[4];
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 4 == 0 && i != 0)
                {
                    res[counter] = BitConverter.ToSingle(buffer, 0);
                    counter++;
                }

                buffer[i % 4] = data[i];
            }

            res[counter] = BitConverter.ToSingle(buffer, 0); //last element
            return res;
        }

        static int[] ToInts(byte[] data)
        {
            int counter = 0;
            int[] res = new int[data.Length / 4]; //float = 4 bytes
            byte[] buffer = new byte[4];
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 4 == 0 && i != 0)
                {
                    res[counter] = BitConverter.ToInt32(buffer, 0);
                    counter++;
                }

                buffer[i % 4] = data[i];
            }

            res[counter] = BitConverter.ToInt32(buffer, 0); //last element
            return res;
        }

        static (List<float>, List<float>, List<float>) split_channels(int[] data)
        {
            List<float> R = new List<float>();
            List<float> G = new List<float>();
            List<float> B = new List<float>();
            int size = data.Length;
            for (int i = 0; i < size; i++)
            {
                if (i < size / 3)
                {
                    R.Add(data[i]);
                    //finalArrayR.Add(finalArray[i]);
                }
                else if (i < (size / 3) * 2)
                {
                    G.Add(data[i]);
                }
                else
                {
                    B.Add(data[i]);
                }
            }

            return (R, G, B);
        }

        static void zigzag_reverse(float[,,] matrix, List<float> list, int size)
        {
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                matrix[0, 0, i] = list[counter];
                counter++;

                matrix[0, 1, i] = list[counter];
                counter++;
                matrix[1, 0, i] = list[counter];
                counter++;

                matrix[2, 0, i] = list[counter];
                counter++;
                matrix[1, 1, i] = list[counter];
                counter++;
                matrix[0, 2, i] = list[counter];
                counter++;

                matrix[0, 3, i] = list[counter];
                counter++;
                matrix[1, 2, i] = list[counter];
                counter++;
                matrix[2, 1, i] = list[counter];
                counter++;
                matrix[3, 0, i] = list[counter];
                counter++;

                matrix[4, 0, i] = list[counter];
                counter++;
                matrix[3, 1, i] = list[counter];
                counter++;
                matrix[2, 2, i] = list[counter];
                counter++;
                matrix[1, 3, i] = list[counter];
                counter++;
                matrix[0, 4, i] = list[counter];
                counter++;

                matrix[0, 5, i] = list[counter];
                counter++;
                matrix[1, 4, i] = list[counter];
                counter++;
                matrix[2, 3, i] = list[counter];
                counter++;
                matrix[3, 2, i] = list[counter];
                counter++;
                matrix[4, 1, i] = list[counter];
                counter++;
                matrix[5, 0, i] = list[counter];
                counter++;

                matrix[6, 0, i] = list[counter];
                counter++;
                matrix[5, 1, i] = list[counter];
                counter++;
                matrix[4, 2, i] = list[counter];
                counter++;
                matrix[3, 3, i] = list[counter];
                counter++;
                matrix[2, 4, i] = list[counter];
                counter++;
                matrix[1, 5, i] = list[counter];
                counter++;
                matrix[0, 6, i] = list[counter];
                counter++;

                matrix[0, 7, i] = list[counter];
                counter++;
                matrix[1, 6, i] = list[counter];
                counter++;
                matrix[2, 5, i] = list[counter];
                counter++;
                matrix[3, 4, i] = list[counter];
                counter++;
                matrix[4, 3, i] = list[counter];
                counter++;
                matrix[5, 2, i] = list[counter];
                counter++;
                matrix[6, 1, i] = list[counter];
                counter++;
                matrix[7, 0, i] = list[counter];
                counter++;

                matrix[7, 1, i] = list[counter];
                counter++;
                matrix[6, 1, i] = list[counter];
                counter++;
                matrix[5, 3, i] = list[counter];
                counter++;
                matrix[4, 4, i] = list[counter];
                counter++;
                matrix[3, 5, i] = list[counter];
                counter++;
                matrix[2, 6, i] = list[counter];
                counter++;
                matrix[1, 7, i] = list[counter];
                counter++;

                matrix[2, 7, i] = list[counter];
                counter++;
                matrix[3, 6, i] = list[counter];
                counter++;
                matrix[4, 5, i] = list[counter];
                counter++;
                matrix[5, 4, i] = list[counter];
                counter++;
                matrix[6, 3, i] = list[counter];
                counter++;
                matrix[7, 2, i] = list[counter];
                counter++;

                matrix[7, 3, i] = list[counter];
                counter++;
                matrix[6, 4, i] = list[counter];
                counter++;
                matrix[5, 5, i] = list[counter];
                counter++;
                matrix[4, 6, i] = list[counter];
                counter++;
                matrix[3, 7, i] = list[counter];
                counter++;

                matrix[4, 7, i] = list[counter];
                counter++;
                matrix[5, 6, i] = list[counter];
                counter++;
                matrix[6, 5, i] = list[counter];
                counter++;
                matrix[7, 4, i] = list[counter];
                counter++;

                matrix[7, 5, i] = list[counter];
                counter++;
                matrix[6, 6, i] = list[counter];
                counter++;
                matrix[5, 7, i] = list[counter];
                counter++;

                matrix[6, 7, i] = list[counter];
                counter++;
                matrix[7, 6, i] = list[counter];
                counter++;

                matrix[7, 7, i] = list[counter];
                counter++;
            }
        }

        static (float[,,], float[,,], float[,,], int usedSize) ReverseZigZagManage(List<float> R, List<float> G,
            List<float> B)
        {
            float[,,] rMatrices = new float[8, 8, R.Count / 64];
            float[,,] gMatrices = new float[8, 8, G.Count / 64];
            float[,,] bMatrices = new float[8, 8, B.Count / 64];
            zigzag_reverse(rMatrices, R, R.Count / 64);
            zigzag_reverse(gMatrices, G, G.Count / 64);
            zigzag_reverse(bMatrices, B, B.Count / 64);
            return (rMatrices, gMatrices, bMatrices, R.Count / 64);
        }

        static (float[,,], float[,,], float[,,]) reverse_multiply_handle(float[,,] rMatrices, float[,,] gMatrices,
            float[,,] bMatrices, int count)
        {
            float[,,] rBlocks = new float[8, 8, count];
            float[,,] gBlocks = new float[8, 8, count];
            float[,,] bBlocks = new float[8, 8, count];

            for (int l = 0; l < count; l++)
            {
                //int m = 8, n = 8, q = 8;
                float[,] C_R = new float[8, 8];
                float[,] C_G = new float[8, 8];
                float[,] C_B = new float[8, 8];

                C_R = Multiply(HAAR, rMatrices, l);
                C_G = Multiply(HAAR, gMatrices, l);
                C_B = Multiply(HAAR, bMatrices, l);

                float[,] transformedR = Multiply(C_R, HAAR_T);
                float[,] transformedG = Multiply(C_G, HAAR_T);
                float[,] transformedB = Multiply(C_B, HAAR_T);

                Emplace(rBlocks, gBlocks, bBlocks, transformedR, transformedG, transformedB, l);
            }

            return (rBlocks, gBlocks, bBlocks);
        }

        static (int[,], int[,], int[,]) AssembleChannels(float[,,] matricesR, float[,,] matricesG,
            float[,,] matricesB,
            int originalW, int originalH, int blockW, int blockH)
        {
            int[,] R = new int[originalW, originalH];
            int[,] G = new int[originalW, originalH];
            int[,] B = new int[originalW, originalH];
            int count = 0;
            for (int x = 0; x < blockW; x++)
            {
                for (int y = 0; y < blockH; y++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (x * 8 + i < originalW && y * 8 + j < originalH)
                            {
                                R[x * 8 + i, y * 8 + j] = (int) Math.Round(matricesR[i, j, count]);
                                G[x * 8 + i, y * 8 + j] = (int) Math.Round(matricesG[i, j, count]);
                                B[x * 8 + i, y * 8 + j] = (int) Math.Round(matricesB[i, j, count]);
                            }
                        }
                    }

                    count++;
                }
            }

            return (R, G, B);
        }

        static Bitmap GenerateImage(int w, int h, int[,] R, int[,] G, int[,] B)
        {
            Bitmap bmp = new Bitmap(w, h);
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    int r = R[y, x];
                    int g = G[y, x];
                    int b = B[y, x];
                    r = (r > 255) ? 255 : r;
                    g = (g > 255) ? 255 : g;
                    b = (b > 255) ? 255 : b;

                    r = (r < 0) ? 0 : r;
                    g = (g < 0) ? 0 : g;
                    b = (b < 0) ? 0 : b;

                    bmp.SetPixel(y, x, Color.FromArgb((byte) r, (byte) g, (byte) b));
                }
            }

            return bmp;
        }

        static void SaveImage(Bitmap image, string name)
        {
            image.Save(name);
        }

        static bool GetLSB(int intValue)
        {
            var bit = ((intValue >> 0) & 1);

            return bit == 1 ? true : false;
        }

        static int NegLSB(int intValue)
        {
            intValue ^= 1 << 0;
            return intValue;
        }

        static List<bool> Binarize(int num, string message)
        {
            BitArray b = new BitArray(new int[] {num});
            int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();

            byte[] btText;
            btText = Encoding.UTF8.GetBytes(message);
            Array.Reverse(btText);
            BitArray bit = new BitArray(btText);
            List<bool> bitsList = new List<bool>();

            for (int i = bits.Length - 1; i >= 0; i--) //4 bytes of message length (length of characters)
            {
                bitsList.Add(bits[i] != 0);
            }

            for (int i = bit.Length - 1; i >= 0; i--)
            {
                bitsList.Add(bit[i] == true);
            }

            return bitsList;
        }

        static int[] F5(List<List<float>> l, int N, int M, string message)
        {
            int msgLen = message.Length;
            Random rand = new Random(M * N);
            List<DataStruct> data = new List<DataStruct>();
            List<bool> bits = Binarize(msgLen, message);
            int[] res = new int[l.Count * 64];

            int low = 4;
            int high = 32;
            if (N > 32)
            {
                high = 64 - N;
            }

            foreach (var t in l)
            {
                var indexes = new List<int>();
                for (int threes = 0; threes < M; threes++)
                {
                    int rnd = rand.Next(low, high + 1);
                    if (!indexes.Contains(rnd) && !indexes.Contains(rnd + 1) && !indexes.Contains(rnd + 2))
                    {
                        if (rnd + 2 < 64)
                        {
                            indexes.Add(rnd);
                            indexes.Add(rnd + 1);
                            indexes.Add(rnd + 2);
                            DataStruct temp = new DataStruct
                            {
                                AC1 = (int) t[rnd], AC2 = (int) t[rnd + 1], AC3 = (int) t[rnd + 2]
                            };
                            temp.C1 = GetLSB(temp.AC1);
                            temp.C2 = GetLSB(temp.AC2);
                            temp.C3 = GetLSB(temp.AC3);
                            if (bits.Count != 0)
                            {
                                temp.x1 = bits[0];
                                bits.RemoveAt(0);
                                if (bits.Count != 0)
                                {
                                    temp.x2 = bits[0];
                                    bits.RemoveAt(0);
                                }
                            }
                            else continue;

                            if ((temp.x1 == (temp.C1 ^ temp.C2)) && (temp.x2 == (temp.C2 ^ temp.C3)))
                            {
                                //na
                            }

                            if ((temp.x1 != (temp.C1 ^ temp.C2)) && (temp.x2 == (temp.C2 ^ temp.C3)))
                            {
                                // negate lsb AC1
                                temp.AC1 = NegLSB(temp.AC1);
                            }

                            if ((temp.x1 == (temp.C1 ^ temp.C2)) && (temp.x2 != (temp.C2 ^ temp.C3)))
                            {
                                // negate lsb AC3
                                temp.AC3 = NegLSB(temp.AC3);
                            }

                            if ((temp.x1 != (temp.C1 ^ temp.C2)) && (temp.x2 != (temp.C2 ^ temp.C3)))
                            {
                                // negate lsb AC2
                                temp.AC2 = NegLSB(temp.AC2);
                            }

                            t[rnd] = temp.AC1;
                            t[rnd + 1] = temp.AC2;
                            t[rnd + 2] = temp.AC3;
                        }
                        else
                        {
                            threes--;
                        }
                    }
                    else
                    {
                        threes--;
                    }
                }
            }

            int c = 0;
            foreach (var t in l)
            {
                for (int j = 0; j < 64; j++)
                {
                    res[c] = (int) t[j];
                    c++;
                }
            }

            return res;
        }

        static string ReverseF5(int[] nums, int M, int N)
        {
            Random rand = new Random(M * N);
            List<List<int>> l = new List<List<int>>();
            List<int> temp = new List<int>();
            List<bool> bits = new List<bool>();

            int low = 4;
            int high = 32;
            if (N > 32)
            {
                high = 64 - N;
            }

            foreach (var t in nums)
            {
                temp.Add(t);
                if (temp.Count != 64) continue;
                l.Add(temp);
                temp = new List<int>();
            }

            foreach (var t in l)
            {
                var indexes = new List<int>();
                for (int threes = 0; threes < M; threes++)
                {
                    int rnd = rand.Next(low, high + 1);
                    if (!indexes.Contains(rnd) && !indexes.Contains(rnd + 1) && !indexes.Contains(rnd + 2))
                    {
                        if (rnd + 2 < 64)
                        {
                            indexes.Add(rnd);
                            indexes.Add(rnd + 1);
                            indexes.Add(rnd + 2);
                            DataStruct tmp = new DataStruct();
                            tmp.AC1 = (int) t[rnd];
                            tmp.AC2 = (int) t[rnd + 1];
                            tmp.AC3 = (int) t[rnd + 2];
                            tmp.C1 = GetLSB(tmp.AC1);
                            tmp.C2 = GetLSB(tmp.AC2);
                            tmp.C3 = GetLSB(tmp.AC3);
                            tmp.x1 = tmp.C1 ^ tmp.C2;
                            tmp.x2 = tmp.C2 ^ tmp.C3;
                            bits.Add(tmp.x1);
                            bits.Add(tmp.x2);
                        }
                        else
                        {
                            threes--;
                        }
                    }
                    else
                    {
                        threes--;
                    }
                }
            }

            BitArray bitsToBytes = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                bitsToBytes[32 - i - 1] = bits[i];
            }

            int[] lenArray = new int[1];
            bitsToBytes.CopyTo(lenArray, 0);
            int msgLen = lenArray[0];

            List<bool> bitMessage = new List<bool>();
            for (int i = 32; i < ((msgLen * 8) + 32); i++)
            {
                bitMessage.Add(bits[i]);
            }
            bool[] messageReversedBits = new bool[bitMessage.Count];
            for (int i = 0; i < bitMessage.Count; i++)
            {
                messageReversedBits[i] = bitMessage[i];
            }

            byte[] messageReversed = ToByteArray(messageReversedBits);

            string res = Encoding.Default.GetString(messageReversed);
            return res;
        }

        static void PrintBits(bool[] bits)
        {
            foreach (var t in bits)
            {
                Console.Write(t ? 1 : 0);
            }
        }

        static byte[] ToByteArray(bool[] input)
        {
            if (input.Length % 8 != 0)
            {
                throw new ArgumentException("input");
            }

            byte[] ret = new byte[input.Length / 8];
            for (int i = 0; i < input.Length; i += 8)
            {
                int value = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (input[i + j])
                    {
                        value += 1 << (7 - j);
                    }
                }

                ret[i / 8] = (byte) value;
            }

            return ret;
        }

        static string ReadTxtFile(string name)
        {
            string text = File.ReadAllText(name);
            return text;
        }

        static void WriteTxtFile(string file, string text)
        {
            File.WriteAllText(file, text);
        }

        static bool LimitMsGlength(String message, int wBlock, int hBlock, int N, int M)
        {
            //v vsakem blocku M trojic, vsaki ma na voljo 1 bit
            int len = message.Length;
            int blocks = wBlock * hBlock; // kolko mamo blokov
            int blocksPlaces = blocks * M; // kolko bitov lahko koristimo v vsakem bloku (skupaj v celi sliki)
            if (len >= blocksPlaces) // ce je bitov v sporocilu vec kot bitov ki jih koristimo v celi sliki -> FALSE
            {
                return false;
            }

            return true;
        }
        
        static void Blockyness(Bitmap g)
        {
			int M = g.Width;
			int N = g.Height;

			int Br1 = 0;
			int Bg1 = 0;
			int Bb1 = 0;
			for(int i = 0; i < ((M)/8); i++)
            {
				for(int j = 0; j < N; j++)
                {
					var c1 = g.GetPixel(8 * i, j);
					var c2 = g.GetPixel(8 * i + 1, j);
					Br1 += Math.Abs(c1.R - c2.R);
					Bg1 += Math.Abs(c1.G - c2.G);
					Bb1 += Math.Abs(c1.B - c2.B);
                }
            }
			int Br2 = 0;
			int Bg2 = 0;
			int Bb2 = 0;

			for(int j = 0; j < ((N)/8); j++)
            {
				for(int i = 0; i < M; i++)
                {
					var c1 = g.GetPixel(i, 8 * j);
					var c2 = g.GetPixel(i, 8 * j + 1);
					Br2 += Math.Abs(c1.R - c2.R);
					Bg2 += Math.Abs(c1.G - c2.G);
					Bb2 += Math.Abs(c1.B - c2.B);
				}
            }
			int Br = Br1 + Br2;
			int Bg = Bg1 + Bg2;
			int Bb = Bb1 + Bb2;

			double B = (Br + Bg + Bb) / 3;
			Console.WriteLine("Blockyness: " + B);
		}

		static void Psnr(Bitmap before, Bitmap after)
        {
			double MSE = 0;
			int M = before.Width;
			int N = before.Height;
			double temp = 0;
			double MAX = 0;

			double k = (double)1 / (M * N);
			for(int i = 0; i < M; i++)
            {
				for(int j = 0; j < N; j++)
                {
					var c1 = before.GetPixel(i, j);
					var c2 = after.GetPixel(i, j);
					temp += Math.Abs(c1.R - c2.R);
					temp += Math.Abs(c1.G - c2.G);
					temp += Math.Abs(c1.B - c2.B);
					temp /= 3;
					if(temp > MAX)
                    {
						//temp = MAX;
						MAX = temp;
                    }
					temp = Math.Pow(temp, 2);
					MSE += temp;
					temp = 0;
				}
            }

			MSE = MSE * k;
			double PSNR = 20 * Math.Log10(MAX / Math.Sqrt(MSE));
			Console.WriteLine("PSNR: " + PSNR);

		}
		static void Intensity(Bitmap img)
		{
			Console.WriteLine("Intensity:");
			double space = 255 / 15;
			int M = img.Width;
			int N = img.Height;
			List<int> intensitiesR = new List<int>();
			List<int> intensitiesG = new List<int>();
			List<int> intensitiesB = new List<int>();

			int[] valsR = new int[(int)space];
			int[] valsG = new int[(int)space];
			int[] valsB = new int[(int)space];

			for (int i = 0; i < M; i++)
			{
				for (int j = 0; j < N; j++)
				{
					var c1 = img.GetPixel(i, j);
					intensitiesR.Add(c1.R);
					intensitiesG.Add(c1.G);
					intensitiesB.Add(c1.B);
				}
			}
			for(int i = 0; i < intensitiesR.Count; i++)
            {
				if (intensitiesR[i] >= 0 && intensitiesR[i] < 17) valsR[0]++;
				else if (intensitiesG[i] >= 0 && intensitiesG[i] < 17) valsG[0]++;
				else if (intensitiesB[i] >= 0 && intensitiesB[i] < 17) valsB[0]++;

				else if (intensitiesR[i] >= 17 && intensitiesR[i] < 17 * 2) valsR[1]++;
				else if (intensitiesG[i] >= 17 && intensitiesG[i] < 17 * 2) valsG[1]++;
				else if (intensitiesB[i] >= 17 && intensitiesB[i] < 17 * 2) valsB[1]++;

				else if (intensitiesR[i] >= 17 * 2 && intensitiesR[i] < 17 * 3) valsR[2]++;
				else if (intensitiesG[i] >= 17 * 2 && intensitiesG[i] < 17 * 3) valsG[2]++;
				else if (intensitiesB[i] >= 17 * 2 && intensitiesB[i] < 17 * 3) valsB[2]++;

				else if (intensitiesR[i] >= 17 * 3 && intensitiesR[i] < 17 * 4) valsR[3]++;
				else if (intensitiesG[i] >= 17 * 3 && intensitiesG[i] < 17 * 4) valsG[3]++;
				else if (intensitiesB[i] >= 17 * 3 && intensitiesB[i] < 17 * 4) valsB[3]++;

				else if (intensitiesR[i] >= 17 * 4 && intensitiesR[i] < 17 * 5) valsR[4]++;
				else if (intensitiesG[i] >= 17 * 4 && intensitiesG[i] < 17 * 5) valsG[4]++;
				else if (intensitiesB[i] >= 17 * 4 && intensitiesB[i] < 17 * 5) valsB[4]++;

				else if (intensitiesR[i] >= 17 * 5 && intensitiesR[i] < 17 * 6) valsR[5]++;
				else if (intensitiesG[i] >= 17 * 5 && intensitiesG[i] < 17 * 6) valsG[5]++;
				else if (intensitiesB[i] >= 17 * 5 && intensitiesB[i] < 17 * 6) valsB[5]++;

				else if (intensitiesR[i] >= 17 * 6 && intensitiesR[i] < 17 * 7) valsR[6]++;
				else if (intensitiesG[i] >= 17 * 6 && intensitiesG[i] < 17 * 7) valsG[6]++;
				else if (intensitiesB[i] >= 17 * 6 && intensitiesB[i] < 17 * 7) valsB[6]++;

				else if (intensitiesR[i] >= 17 * 7 && intensitiesR[i] < 17 * 8) valsR[7]++;
				else if (intensitiesG[i] >= 17 * 7 && intensitiesG[i] < 17 * 8) valsG[7]++;
				else if (intensitiesB[i] >= 17 * 7 && intensitiesB[i] < 17 * 8) valsB[7]++;

				else if (intensitiesR[i] >= 17 * 8 && intensitiesR[i] < 17 * 9) valsR[8]++;
				else if (intensitiesG[i] >= 17 * 8 && intensitiesG[i] < 17 * 9) valsG[8]++;
				else if (intensitiesB[i] >= 17 * 8 && intensitiesB[i] < 17 * 9) valsB[8]++;

				else if (intensitiesR[i] >= 17 * 9 && intensitiesR[i] < 17 * 10) valsR[9]++;
				else if (intensitiesG[i] >= 17 * 9 && intensitiesG[i] < 17 * 10) valsG[9]++;
				else if (intensitiesB[i] >= 17 * 9 && intensitiesB[i] < 17 * 10) valsB[9]++;

				else if (intensitiesR[i] >= 17 * 10 && intensitiesR[i] < 17 * 11) valsR[10]++;
				else if (intensitiesG[i] >= 17 * 10 && intensitiesG[i] < 17 * 11) valsG[10]++;
				else if (intensitiesB[i] >= 17 * 10 && intensitiesB[i] < 17 * 11) valsB[10]++;

				else if (intensitiesR[i] >= 17 * 11 && intensitiesR[i] < 17 * 12) valsR[11]++;
				else if (intensitiesG[i] >= 17 * 11 && intensitiesG[i] < 17 * 12) valsG[11]++;
				else if (intensitiesB[i] >= 17 * 11 && intensitiesB[i] < 17 * 12) valsB[11]++;

				else if (intensitiesR[i] >= 17 * 12 && intensitiesR[i] < 17 * 13) valsR[12]++;
				else if (intensitiesG[i] >= 17 * 12 && intensitiesG[i] < 17 * 13) valsG[12]++;
				else if (intensitiesB[i] >= 17 * 12 && intensitiesB[i] < 17 * 13) valsB[12]++;

				else if (intensitiesR[i] >= 17 * 13 && intensitiesR[i] < 17 * 14) valsR[13]++;
				else if (intensitiesG[i] >= 17 * 13 && intensitiesG[i] < 17 * 14) valsG[13]++;
				else if (intensitiesB[i] >= 17 * 13 && intensitiesB[i] < 17 * 14) valsB[13]++;

				else if (intensitiesR[i] >= 17 * 14 && intensitiesR[i] <= 17 * 15) valsR[14]++;
				else if (intensitiesG[i] >= 17 * 14 && intensitiesG[i] <= 17 * 15) valsG[14]++;
				else if (intensitiesB[i] >= 17 * 14 && intensitiesB[i] <= 17 * 15) valsB[14]++;
			}

			for(int i = 0; i < valsR.Length; i++)
            {
				Console.WriteLine(i + " R:" + valsR[i]);
				Console.WriteLine(i + " G:" + valsG[i]);
				Console.WriteLine(i + " B:" + valsB[i]);
				Console.WriteLine("");
			}
		}
    }
}