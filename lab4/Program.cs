using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace lab
{
    class Program
    {
        static void processing(string path)
        {
            Bitmap image = new Bitmap(path);
            for (int i = 1; i < image.Width - 1; i++)
                for (int j = 0; j < image.Height - 1; j++)
                    algorithm(image, i, j);

            Regex regex = new Regex(@"\w*.png$");
            string new_path = regex.Replace(path, "result.png");
            image.Save(new_path);
        }

        static void algorithm(Bitmap image, int x, int y)
        {
            Color pix = image.GetPixel(x, y);
            int oldR = pix.R, oldG = pix.G, oldB = pix.B;
            int newR = roundCol(pix)[0];
            int newG = roundCol(pix)[1];
            int newB = roundCol(pix)[2];
            image.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
            int[] err = { oldR - newR, oldG - newG, oldB - newB };
           
            image.SetPixel(x + 1, y, dithering(image.GetPixel(x + 1, y), err, 7.0 / 16.0));
            image.SetPixel(x - 1, y + 1, dithering(image.GetPixel(x - 1, y + 1), err, 3.0 / 16.0));
            image.SetPixel(x, y + 1, dithering(image.GetPixel(x, y + 1), err, 5.0 / 16.0));
            image.SetPixel(x + 1, y + 1, dithering(image.GetPixel(x + 1, y + 1), err, 1.0 / 16.0));
            
        }

        static int[] roundCol(Color col)
        {
            int r = (int)(Math.Round(col.R / 255.0) * 255);
            int g = (int)(Math.Round(col.G / 255.0) * 255);
            int b = (int)(Math.Round(col.B / 255.0) * 255);
            int[] rgb = { r, g, b };
            return rgb;
        }

        static Color dithering(Color col, int[] err, double coefficient)
        {
            int r = col.R, g = col.G, b = col.B;
            r = Math.Max(0, Math.Min(255, (int)(r + err[0] * coefficient)));
            g = Math.Max(0, Math.Min(255, (int)(g + err[1] * coefficient)));
            b = Math.Max(0, Math.Min(255, (int)(b + err[2] * coefficient)));

            return Color.FromArgb(r, g, b);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the full path to the image (and press the \"Enter\" key): ");
            string path = Console.ReadLine();      
            Console.WriteLine("Start!");
            processing(path);
            Console.WriteLine("Ready! Results saved to file: result.png");
        }
    }
}
