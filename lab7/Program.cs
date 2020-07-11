using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab7
{
    class Program
    {
        // Бинаризация изображения
        static Bitmap Binarization(Bitmap img)
        {
            int width = img.Width, height = img.Height;
            Bitmap binImg = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++) 
                    binImg.SetPixel(i, j, img.GetPixel(i, j).R > 128 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(0, 0, 0));

            return binImg; 
        }

        // Размечаем 4-х связные области
        static int[,] CCL(Bitmap img)
        {
            int km, kn;
            int cur = 1;
            int A, B, C;
            int width = img.Width, height = img.Height;
            int[,] im = new int[width, height];
            
            // Приводим изображение к матрице из 0 и 1
              for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    im[i, j] = img.GetPixel(i, j).R / 255;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    kn = j - 1;
                    if (kn <= 0)
                    {
                        kn = 1;
                        B = 0;
                    }
                    else
                        B = im[i, kn];

                    km = i - 1;

                    if (km <= 0)
                    {
                        km = 1;
                        C = 0;
                    }
                    else
                        C = im[km, j];

                    A = im[i, j];

                    if (A == 0)
                        continue;
                    else
                    {
                        if ((B == 0) && (C == 0))
                        {
                            cur++;
                            im[i, j] = cur;
                        }

                        if ((B != 0) && (C == 0))
                            im[i, j] = B;

                        if ((B == 0) && (C != 0))
                            im[i, j] = C;

                        if ((B != 0) && (C != 0))
                            if (B == C)
                                im[i, j] = B;
                            else
                            {
                                im[i, j] = B;
                                im = ReNumber(im, C, B);
                            }
                    }
                }
            }

            return im;
        }

        // Перенумируем все A на B
        static int[,] ReNumber(int[,] img, int A, int B)
        {
            for (int i = 0; i < img.GetLength(0); i++)
                for (int j = 0; j < img.GetLength(1); j++)
                    if (img[i, j] == A)
                        img[i, j] = B;
            return img;
        }

        // Раскрасим изображение изображение в соответствии с разметкой
        static Bitmap Painting(int[,] map)
        {
            double maxIndex = 0;
            Bitmap colorImage = new Bitmap(map.GetLength(0), map.GetLength(1));
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] > maxIndex)
                        maxIndex = map[i, j];

            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++) {
                    if (map[i, j] <= maxIndex / 4)
                        colorImage.SetPixel(i, j, Color.FromArgb(255, 0, 0)); // закраска красным цветом
                    else
                        if (map[i, j] <= maxIndex / 2)
                            colorImage.SetPixel(i, j, Color.FromArgb(255, 255, 0)); // закраска желтым цветом
                        else
                            if (map[i, j] <= 3 * maxIndex / 4)
                                colorImage.SetPixel(i, j, Color.FromArgb(0, 255, 0)); // закраска зеленым цветом
                            else
                                colorImage.SetPixel(i, j, Color.FromArgb(0, 0, 255)); // закраска синим цветом
                }

            return colorImage;
        }

        // Записываем номера уникальных областей
        static ISet<int> Regions(int[,] map)
        {
            ISet<int> regions = new HashSet<int> { };

            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (!regions.Contains(map[i, j]))
                        regions.Add(map[i, j]);
            return regions;
        }

        // Нулевой геометрический момент
        static double[] ZeroMoment(int[,] map, Bitmap img)
        {
            int[] regions = Regions(map).ToArray<int>();
            double[] moments = new double[regions.Length];

            for (int k = 0; k < regions.Length; k++)
                for (int i = 0; i < map.GetLength(0); i++)
                    for (int j = 0; j < map.GetLength(1); j++)
                        if (regions[k] == map[i, j])
                            moments[k] += img.GetPixel(i, j).R;
            return moments;
        }

        // Первый геометрический момент
        static double[,] FirstMoment(int[,] map, Bitmap img)
        {
            int[] regions = Regions(map).ToArray<int>();
            double[,] moments = new double[regions.Length, 2];

            for (int k = 0; k < regions.Length; k++)
                for (int i = 0; i < map.GetLength(0); i++)
                    for (int j = 0; j < map.GetLength(1); j++)
                        if (regions[k] == map[i, j])
                        {
                            moments[k, 0] += i * img.GetPixel(i, j).R;
                            moments[k, 1] += j * img.GetPixel(i, j).R;
                        }
            return moments;
        }

        // Центральный геометрический момент 2-го порядка
        static double[,] CentralSecondMoment(int[,] map, Bitmap img)
        {
            int[] regions = Regions(map).ToArray<int>();
            double[,] moments = new double[regions.Length, 2];
            double[,] firstMoment = FirstMoment(map, img);
            double[] zeroMoment = ZeroMoment(map, img);
            double x_c, y_c;

            for (int k = 0; k < regions.Length; k++) {
                x_c = firstMoment[k, 0] / zeroMoment[k];
                y_c = firstMoment[k, 1] / zeroMoment[k];
                for (int i = 0; i < map.GetLength(0); i++) {
                    for (int j = 0; j < map.GetLength(1); j++)
                        if (regions[k] == map[i, j]) {
                            moments[k, 0] += Math.Pow(i - x_c, 2) * img.GetPixel(i, j).R;
                            moments[k, 1] += Math.Pow(j - y_c, 2) * img.GetPixel(i, j).R;
                        }
                }
            }
            return moments;
        }

        static void Main(string[]  args)
        {
            try
            {
                Console.Write("Введите путь к папке: ");
                string path = Console.ReadLine();
                Console.Write("Введите имя файла: ");
                string imgName = Console.ReadLine();
                Console.WriteLine("--------------------------\n");
                Bitmap image = new Bitmap(path + @"\" + imgName);
                Bitmap bin;
                Console.WriteLine("Binarization started");
                bin = Binarization(image);
                bin.Save(path + @"\binImg.png");
                Console.WriteLine("Binarization completed\n--------------------------\n");
                Console.WriteLine("Coloring started");
                Painting(CCL(bin)).Save(path + @"\colorAreas.png");
                Console.WriteLine("Coloring completed\n--------------------------\n");
                Console.WriteLine("Writing moments to a file has begun");

                double[] zero = ZeroMoment(CCL(bin), image);
                double[,] first = FirstMoment(CCL(bin), image);
                double[,] second = CentralSecondMoment(CCL(bin), image);
                using (StreamWriter sw = new StreamWriter(path + @"\moments.txt", false, System.Text.Encoding.Default))
                {
                    for (int k = 0; k < zero.Length; k++)
                    {
                        sw.WriteLine("Region # " + k);
                        sw.WriteLine("m_00 = {0:f2}", zero[k]);
                        sw.WriteLine("m_10 = {0:f2}", first[k, 0]);
                        sw.WriteLine("m_01 = {0:f2}", first[k, 1]);
                        sw.WriteLine("M_20 = {0:f2}", second[k, 0]);
                        sw.WriteLine("M_02 = {0:f2}", second[k, 1]);

                        sw.WriteLine("---------------------------\n");
                    }
                }
                Console.WriteLine("Writing moments to a file has completed");

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
