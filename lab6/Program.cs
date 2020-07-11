using System;
using System.Drawing;

namespace Lab_6
{
    class Program
    {
        static Bitmap orderStat(Bitmap image, int rang, int[,] domain)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap new_image = new Bitmap(width, height);
            int wDom = domain.GetLength(0);
            int hDom = domain.GetLength(1);
            int r, g, b;
            int units = 0;
            Color col;
            // Подсчитываем кол-во единиц в матрице
            for (int i = 0; i < wDom; i++)
                for (int j = 0; j < hDom; j++)
                    units += domain[i, j];

            if (rang > units)
                throw new Exception();

            // Окно изображения
            for (int x = 0; x < width - wDom; x++)
                for (int y = 0; y < height - hDom; y++)
                {
                    int[] rangeR = new int[units];
                    int[] rangeG = new int[units];
                    int[] rangeB = new int[units];
                    int idR = 0, idG = 0, idB = 0;
                    int cen_X = 0, cen_Y = 0;
                    cen_X = x + wDom / 2;
                    cen_Y = y + hDom / 2;

                    // Окно фильтра
                    for (int i = 0; i < wDom; i++)
                        for (int j = 0; j < hDom; j++)
                        {
                            if ((cen_X > width) || (cen_Y > height) || (cen_X < 0) || (cen_Y < 0)) continue;

                            col = Color.FromArgb(image.GetPixel(x + i, y + j).ToArgb());
                            r = col.R;
                            g = col.G;
                            b = col.B;

                            if (domain[i, j] == 1)
                            {
                                rangeR[idR] = r; idR++;
                                rangeG[idG] = g; idG++;
                                rangeB[idB] = b; idB++;
                            }
                           
                        }

                    Array.Sort(rangeR);
                    Array.Sort(rangeG);
                    Array.Sort(rangeB);

                    new_image.SetPixel(cen_X, cen_Y, Color.FromArgb(rangeR[rang-1], rangeG[rang-1], rangeB[rang-1]));
                }

                return new_image; 
        }

        static void Main(string[] args)
        {
               try
               {
                   Console.Write("Введите путь к папке: ");
                   string path = Console.ReadLine();
                   Console.Write("Введите имя файла: ");
                   string img_name = Console.ReadLine();
                   int n1, n2, k;
                   Bitmap img = new Bitmap(path + @"\" + img_name);

                   Console.Write("Введите ранг: ");
                   k = int.Parse(Console.ReadLine());
                   Console.Write("Заполните апертуры фильтра:\nКол-во строк: ");
                   n1 = int.Parse(Console.ReadLine());
                   Console.Write("Заполните апертуры фильтра:\nКол-во столбцов: ");
                   n2 = int.Parse(Console.ReadLine());

                    if (n1 * n2 < k)
                    {
                        throw new Exception();
                    }

                    Console.WriteLine("Введите элементы:");
                    int[,] mtx = new int[n1, n2];
                    for (int i = 0; i < n1; i++)
                        for (int j = 0; j < n2; j++)
                        {
                            Console.Write("matrix[" + i + "," + j + "]: ");
                            mtx[i, j] = int.Parse(Console.ReadLine());
                        }

                    Console.WriteLine("Begin!");
                        orderStat(img, k, mtx).Save(path + @"\result.png");
                    Console.WriteLine("Ready!");
                }

                catch (Exception)
                {
                    Console.WriteLine("Ошибка!");
                }

            Console.ReadKey();
        }
    
    }
}
