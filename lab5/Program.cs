using System;
using System.Drawing;

namespace lab5
{
    class Program
    {
        static Bitmap Convolution (Bitmap img, double[,] kernel)
        {
            Bitmap reimg = new Bitmap(img);
            int width = img.Width;
            int height = img.Height;
            int K = kernel.GetLength(0); //kernel_size
            int half = K / 2;
            int pix_X, pix_Y;
            double ker, r, g, b;

            Color col;
            for (int x = half; x < width - half; x++)
                for (int y = half; y < height - half; y++)
                {
                    double rSum = 0, gSum = 0, bSum = 0;
                    for (int i = 0; i < K; i++)
                        for (int j = 0; j < K; j++)
                        {
                            pix_X = x + i - half;
                            pix_Y = y + j - half;

                            col = Color.FromArgb(img.GetPixel(pix_X, pix_Y).ToArgb());
                            r = col.R;
                            g = col.G;
                            b = col.B;

                            ker = kernel[i, j];

                            rSum += r * ker;
                            gSum += g * ker;
                            bSum += b * ker;
                        }

                    if (rSum < 0) rSum = 0;
                    if (rSum > 255) rSum = 255;

                    if (gSum < 0) gSum = 0;
                    if (gSum > 255) gSum = 255;

                    if (bSum < 0) bSum = 0;
                    if (bSum > 255) bSum = 255;

                    reimg.SetPixel(x, y, Color.FromArgb((int)rSum, (int)gSum, (int)bSum));
                }
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    reimg.SetPixel(0, j, img.GetPixel(0, j));
                    reimg.SetPixel(width - 1, j, img.GetPixel(width - 1, j));
                    reimg.SetPixel(i, 0, img.GetPixel(i, 0));
                    reimg.SetPixel(i, height - 1, img.GetPixel(i, height - 1));
                }

            return reimg;
        }

        static Bitmap LPF(Bitmap img, int size)
        {
            double[,] kernel = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] = 1.0 / (double)(size * size);
            return Convolution(img, kernel);
        }

        static Bitmap HPF(Bitmap img, int size)
        {
            double[,] kernel = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if ((i == size / 2) && (j == size / 2))
                        kernel[i, j] = (double)(size * size - 1) / (double)(size * size);
                    else
                        kernel[i, j] = -1.0 / (double)(size * size);
            return Convolution(img, kernel);
        }

        static Bitmap Sharpness(Bitmap img, int size)
        {
            Bitmap sharp_img = new Bitmap(img); 
            Bitmap blur = LPF(img, size);
            double r, g, b;
            // Находим детали (вычитаем из оригинала размытое изображение)
            // и накладываем детали на оригинал с коэф. = 0,1
            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                {
                    r = img.GetPixel(i, j).R + 0.1 * (img.GetPixel(i, j).R - blur.GetPixel(i, j).R);
                    g = img.GetPixel(i, j).R + 0.1 * (img.GetPixel(i, j).G - blur.GetPixel(i, j).G);
                    b = img.GetPixel(i, j).R + 0.1 * (img.GetPixel(i, j).B - blur.GetPixel(i, j).B);

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;

                    if (g < 0) g = 0;
                    if (g > 255) g = 255;

                    if (b < 0) b = 0;
                    if (b > 255) b = 255;
                   sharp_img.SetPixel(i, j, Color.FromArgb((int)r, (int)g, (int)b));
                }
            return sharp_img;
        }

        static void Main(string[] args)
        {
            
              Console.Write("Введите адрес папки: ");
              string path = Console.ReadLine();
              Console.Write("Введите имя изображения: ");
              string img_name = Console.ReadLine();
              Bitmap image = new Bitmap(path + @"\" + img_name);
              int choice;
              int size;
              bool ex = true;
            while (ex)
            {
                try
                {
                    Console.WriteLine("1) Задать свое ядро свертки\n2) Использовать ФНЧ\n3) Использовать ФВЧ\n4) Использовать фильтр, повышающий резкость\n0) Выйти");
                    Console.Write("Выбириите опцию: ");
                    choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Требуется задать матрицу ядра");
                            Console.Write("Введите размерность ядра: ");
                            size = int.Parse(Console.ReadLine());

                            double[,] kernel = new double[size, size];
                            Console.WriteLine();

                            Console.WriteLine("Введите элементы:");

                            for (int i = 0; i < size; i++)
                                for (int j = 0; j < size; j++)
                                {
                                    Console.Write("matrix[" + i + "," + j + "]: ");
                                    kernel[i, j] = double.Parse(Console.ReadLine());
                                }
                            Convolution(image, kernel).Save(path + @"\convolution_result.png");
                            Console.WriteLine("Done!");
                            break;
                        case 2:
                            Console.Write("Укажите размер окна фильтр: ");
                            size = int.Parse(Console.ReadLine());
                            LPF(image, size).Save(path + @"\lpf_result.png");
                            Console.WriteLine("Done!");
                            break;
                        case 3:
                            Console.Write("Укажите размер окна фильтр: ");
                            size = int.Parse(Console.ReadLine());
                            HPF(image, size).Save(path + @"\hpf_result.png");
                            Console.WriteLine("Done!");
                            break;
                        case 4:
                            Console.Write("Укажите размер окна фильтра размытия: ");
                            size = int.Parse(Console.ReadLine());
                            Sharpness(image, size).Save(path + @"\sharp_result.png");
                            Console.WriteLine("Done!");
                            break;
                        case 0:
                            ex = false;
                            break;

                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный формат");
                }
                Console.WriteLine("----------------------------------");
            }
        }
    }
}