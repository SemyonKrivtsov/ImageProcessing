using System;
using System.Drawing;
using System.IO;

namespace lab1
{
    class Program
    {
        static string path = @""; // !!!Указать адрес корневого каталога

        public static void rotTo90(string filename)
        {
            Bitmap image = new Bitmap(path + filename + ".png");
            Bitmap image1 = new Bitmap(image.Height, image.Width);
            Color a;
            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                {
                    a = image.GetPixel(i, j);
                    image1.SetPixel(j, i, a);
                }

            image1.Save(path + @"tests\rotationTo90Result.png"); // !!!Создать папку tests внутри корневого каталога
        }

        public static void mirrorHorizontal (string filename)
        {
            Bitmap image = new Bitmap(path + filename + ".png");
            int width = image.Width, height = image.Height;
            Color temp;
            
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height / 2; j++)
                    {
                        temp = image.GetPixel(i, j);
                        image.SetPixel(i, j, image.GetPixel(i, height - j - 1));
                        image.SetPixel(i, height - j - 1, temp);
                    }

                image.Save(path + @"tests\mirrorHorizontalResult.png");         
        }

        public static void mirrorVertical(string filename)
        {
            Bitmap image = new Bitmap(path + filename + ".png");
            int width = image.Width, height = image.Height;
            Color temp;

            for (int i = 0; i < width / 2; i++)
                for (int j = 0; j < height; j++)
                {
                    temp = image.GetPixel(i, j);
                    image.SetPixel(i, j, image.GetPixel(width - i - 1, j));
                    image.SetPixel(width - i - 1, j, temp);
                }

            image.Save(path + @"tests\mirrorVerticalResult.png");
        }

        public static void blending (string filename_original, string filename_background, string file_name_alpha_sample)
        {
            Bitmap original, background, alpha_sample, result;
            Color col_original, col_background, col_alpha_sample;
            int col_result_r, col_result_g, col_result_b;
            original = new Bitmap(path + filename_original + ".png", true);
            background = new Bitmap(path + filename_background + ".png", true);
            alpha_sample = new Bitmap(path + file_name_alpha_sample + ".png", true);
            result = new Bitmap(original.Width, original.Height);
            if ((original.Size == background.Size) && (background.Size == alpha_sample.Size))
            {
                for (int i = 0; i < original.Width; i++)
                    for (int j = 0; j < original.Height; j++)
                    {
                        col_original = original.GetPixel(i, j);
                        col_background = background.GetPixel(i, j);
                        col_alpha_sample = alpha_sample.GetPixel(i, j);
                        col_result_r = col_original.R * col_alpha_sample.R / 255 + col_background.R * (255 - col_alpha_sample.R) / 255;
                        col_result_g = col_original.G * col_alpha_sample.G / 255 + col_background.G * (255 - col_alpha_sample.G) / 255;
                        col_result_b = col_original.B * col_alpha_sample.B / 255 + col_background.B * (255 - col_alpha_sample.B) / 255;
                        result.SetPixel(i, j, Color.FromArgb(col_alpha_sample.A, col_result_r, col_result_g, col_result_b));
                    }

                result.Save(path + @"tests\blending_result.png");
            }
            else
                Console.WriteLine("ERROR: Размеры изображений не совпадают!");
        }

        static void Main(string[] args)
        {
            try
            {
                Directory.Delete(path + @"tests\", true);
            }
            catch (Exception)
            {
                Console.WriteLine("New directory created");
            }
            Directory.CreateDirectory(path + @"tests\");

            blending("Lena", "baboon", "boat"); //Смешивает два изображения с использованием третьего (в качестве альфа-канала)
            rotTo90("Lena");                    //Транспонирование изображения
            mirrorHorizontal("boat");           //Зеркальное отражение изображение относительно горизонтальной оси
            mirrorVertical("boat");             //Зеркальное отражение изображение относительно вертикальной оси

            Console.WriteLine("Done!");         
        }
    }
}