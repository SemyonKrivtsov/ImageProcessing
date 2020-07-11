using System;
using System.Drawing;

namespace lab3
{
    class Program
    {
        static readonly string path = @""; // !!!Указать адрес корневого каталога
        static void Rotation(string imageName, double angle)
        {
            angle = angle * Math.PI / 180.0;
            Bitmap img = new Bitmap(path + imageName);
            int x, y; //Декартовые координаты
            double r_vector, teta; //Полярные координаты
            int floorX, ceilingX, floorY, ceilingY; //
            double realX, realY; //Неокругленные значения декартовых координат
            double deltaX, deltaY; //Интерполяционные коориднаты
            Color[,] clr = new Color[4,4];
            int Red, Green, Blue;
            int centreX, centreY; 
            int width = img.Width, height = img.Height;
            centreX = width / 2;
            centreY = height / 2;

            Bitmap toDestination = new Bitmap(width, height);
            Bitmap fromDestination = new Bitmap(width, height);
            Bitmap interpolation = new Bitmap(width, height);

            //Накладываем черный задний фон
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                {
                    toDestination.SetPixel(j, i, Color.Black);
                    fromDestination.SetPixel(j, i, Color.Black);
                    interpolation.SetPixel(j, i, Color.Black);
                }

            //Определение значений пикселей из исходного изображения в новое
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    //Переход в декартовы координаты
                    x = j - centreX;
                    y = centreY - i;

                    //Переход в полярные координаты
                    r_vector = Math.Sqrt(x * x + y * y);
                    if (x == 0)
                        if (y == 0)
                        {
                            //Центр не вращается
                            toDestination.SetPixel(j, i, img.GetPixel(j, i));
                            continue;
                        }
                        else if (y < 0)
                            teta = 1.5 * Math.PI;
                        else
                            teta = 0.5 * Math.PI;
                    else
                        teta = Math.Atan2((double)y, (double)x);

                    //Полярный угол "положительного" направления
                    teta += angle;

                    //Переход в декартовые координаты
                    x = (int)(Math.Round(r_vector * Math.Cos(teta)));
                    y = (int)(Math.Round(r_vector * Math.Sin(teta)));

                    //Переход в растровые координаты
                    x += centreX;
                    y = centreY - y;

                    //Проверка границ
                    if (x < 0 || x >= width || y < 0 || y >= height) continue;

                    toDestination.SetPixel(x, y, img.GetPixel(j, i));
                }

            //Определяем пиксели нового изображения из исходного
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    //Переход в декартовые координаты
                    x = j - centreX;
                    y = centreY - i;

                    //Переход в полярные координаты
                    r_vector = Math.Sqrt(x * x + y * y);
                    if (x == 0)
                        if (y == 0)
                        {
                            //Центр неподвижен
                            fromDestination.SetPixel(j, i, img.GetPixel(j, i));
                            continue;
                        }
                        else if (y < 0)
                            teta = 1.5 * Math.PI;
                        else
                            teta = 0.5 * Math.PI;
                    else
                        teta = Math.Atan2((double)y, (double)x);

                    //Полярный угол "отрицательного" направления
                    teta -= angle;

                    //Переход в декартовые координаты
                    x = (int)(Math.Round(r_vector * Math.Cos(teta)));
                    y = (int)(Math.Round(r_vector * Math.Sin(teta)));

                    //Переход в растровые координаты
                    x += centreX;
                    y = centreY - y;

                    //Проверка границ
                    if (x < 0 || x >= width || y < 0 || y >= height) continue;

                    fromDestination.SetPixel(j, i, img.GetPixel(x, y));
                }

            //Интерполяция
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    //Переход в декартовые координаты
                    x = j - centreX;
                    y = centreY - i;

                    //Переход в полярные координаты
                    r_vector = Math.Sqrt(x * x + y * y);
                    if (x == 0)
                        if (y == 0)
                        {
                            //Центр неподвижен
                            interpolation.SetPixel(j, i, img.GetPixel(j, i));
                            continue;
                        }
                        else if (y < 0)
                            teta = 1.5 * Math.PI;
                        else
                            teta = 0.5 * Math.PI;
                    else
                        teta = Math.Atan2((double)y, (double)x);

                   teta -= angle;

                   realX = r_vector * Math.Cos(teta);
                   realY = r_vector * Math.Sin(teta);

                   realX += (double)centreX;
                   realY = (double)centreY - realY;

                   floorX = (int)(Math.Floor(realX));
                   floorY = (int)(Math.Floor(realY));
                   ceilingX = (int)(Math.Ceiling(realX));
                   ceilingY = (int)(Math.Ceiling(realY));

                    //Проверка границ
                    if (floorX < 0 ||ceilingX < 0 || floorX >= width ||ceilingX >= width || floorY < 0 ||ceilingY < 0 || floorY >= height ||ceilingY >= height) continue;

                    deltaX =realX - (double)floorX;
                    deltaY =realY - (double)floorY;

             
                    double[] b = new double[16];
                    //Заполняем сетку 4 х 4 
                    for (int k = 0; k < 4; k++)
                        for (int m = 0; m < 4; m++)
                            if (ceilingX + k <= height &&ceilingY + m <= width)
                                clr[k, m] = img.GetPixel(floorX + k, floorY + m);

                    //Расчитываем вспомогательные коэффиценты
                    b[0] = (deltaX - 1) * (deltaX - 2) * (deltaX + 1) * (deltaY - 1) * (deltaY - 2) * (deltaY + 1) / 4; //b1
                    b[1] = -deltaX * (deltaX - 2) * (deltaX + 1) * (deltaY - 1) * (deltaY - 2) * (deltaY + 1) / 4;      //b2
                    b[2] = -deltaY * (deltaX - 1) * (deltaX - 2) * (deltaX + 1) * (deltaY - 2) * (deltaY + 1) / 4;      //b3
                    b[3] =  deltaX * (deltaX - 2) * (deltaX + 1) * deltaY * (deltaY - 2) * (deltaY + 1) / 4;            //b4
                    b[4] = -deltaX * (deltaX - 1) * (deltaX - 2) * (deltaY - 1) * (deltaY - 2) * (deltaY + 1) / 12;     //b5
                    b[5] =  -(deltaX-1) * (deltaX - 2) * (deltaX + 1) * deltaY * (deltaY - 1) * (deltaY - 2) / 12;      //b6
                    b[6] =  deltaX * (deltaX - 1) * (deltaX - 2) * deltaY * (deltaY - 2) * (deltaY + 1) / 12;           //b7
                    b[7] =  deltaX * (deltaX - 2) * (deltaX + 1) * deltaY * (deltaY - 1) * (deltaY - 2) / 12;           //b8
                    b[8] =  deltaX * (deltaX - 1) * (deltaX + 1) * (deltaY - 1) * (deltaY - 2) * (deltaY + 1) / 12;     //b9
                    b[9] =  (deltaX - 1) * (deltaX - 2) * (deltaX + 1) * deltaY * (deltaY - 1) * (deltaY + 1) / 12;     //b10
                    b[10] = deltaX * (deltaX - 1) * (deltaX -2) * deltaY * (deltaY - 1) * (deltaY - 2) / 36;            //b11
                    b[11] = -deltaX * (deltaX - 1) * (deltaX + 1) * deltaY * (deltaY - 2) * (deltaY + 1) / 12;          //b12
                    b[12] = -deltaX * (deltaX - 2) * (deltaX + 1) * deltaY * (deltaY - 1) * (deltaY + 1) / 12;          //b13
                    b[13] = -deltaX * (deltaX - 1) * (deltaX + 1) * deltaY * (deltaY - 1) * (deltaY - 2) / 36;          //b14
                    b[14] = -deltaX * (deltaX - 1) * (deltaX - 2) * deltaY * (deltaY - 1) * (deltaY + 1) / 36;          //b15
                    b[15] = deltaX * (deltaX - 1) * (deltaX + 1) * deltaY * (deltaY - 1) * (deltaY + 1) / 36;           //b16
                    
                    
                    //Интерполируем каждый цвет
                    Red = (int)(Math.Round(b[0] * clr[1, 1].R + b[1] * clr[2, 1].R + b[2] * clr[1, 2].R + b[3] * clr[2, 2].R + b[4] * clr[0, 1].R +
                        b[5] * clr[1, 0].R + b[6] * clr[0, 2].R + b[7] * clr[2, 0].R + b[8] * clr[3, 1].R + b[9] * clr[1, 3].R +
                        b[10] * clr[0, 0].R + b[11] * clr[3, 2].R + b[12] * clr[2, 3].R + b[13] * clr[3, 0].R + b[14] * clr[0, 3].R + b[15] * clr[3, 3].R));

                    Green = (int)(Math.Round(b[0] * clr[1, 1].G + b[1] * clr[2, 1].G + b[2] * clr[1, 2].G + b[3] * clr[2, 2].G + b[4] * clr[0, 1].G +
                        b[5] * clr[1, 0].G + b[6] * clr[0, 2].G + b[7] * clr[2, 0].G + b[8] * clr[3, 1].G + b[9] * clr[1, 3].G +
                        b[10] * clr[0, 0].G + b[11] * clr[3, 2].G + b[12] * clr[2, 3].G + b[13] * clr[3, 0].G + b[14] * clr[0, 3].G + b[15] * clr[3, 3].G));

                    Blue = (int)(Math.Round(b[0] * clr[1, 1].B + b[1] * clr[2, 1].B + b[2] * clr[1, 2].B + b[3] * clr[2, 2].B + b[4] * clr[0, 1].B +
                        b[5] * clr[1, 0].B + b[6] * clr[0, 2].B + b[7] * clr[2, 0].B + b[8] * clr[3, 1].B + b[9] * clr[1, 3].B +
                        b[10] * clr[0, 0].B + b[11] * clr[3, 2].B + b[12] * clr[2, 3].B + b[13] * clr[3, 0].B + b[14] * clr[0, 3].B + b[15] * clr[3, 3].B));

                    //Нормировка цветов
                    if (Red < 0) Red = 0;
                    if (Red > 255) Red = 255;
                    if (Green < 0) Green = 0;
                    if (Green > 255) Green = 255;
                    if (Blue < 0) Blue = 0;
                    if (Blue > 255) Blue = 255;

                    interpolation.SetPixel(j, i, Color.FromArgb(Red, Green, Blue));
                }
            interpolation.Save(path + @"tests\result.png"); // !!!Создать папку tests внутри корневого каталога
        }

        static void Main()
        {
            Console.Write("Enter rotation angle: ");
            double arg = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Start!");
            Rotation("Lena.png", arg); //Имя файла и угол поворота
            Console.WriteLine("Ready!");
        }
    }
}