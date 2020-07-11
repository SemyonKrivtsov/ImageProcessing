using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2
{
    public partial class Lab2 : Form
    {
        static int width, height;
        Bitmap image;
        public Lab2()
        {
            InitializeComponent();
        }

        private void Lab2_Load(object sender, EventArgs e)
        {
            string path = @""; // // !!!Указать адрес корневого каталога
            image = new Bitmap(path + "Lena.png");      
            int[] brightness = new int[256];
            width = image.Width; height = image.Height;
           
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < width; j++)
                    for (int k = 0; k < height; k++)
                        if (Convert.ToInt32(image.GetPixel(j, k).GetBrightness() * 255) == i)
                            brightness[i]++;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 255;
            for (int i = 0; i < 256; i++)
                chart1.Series["Яркость"].Points.Add(brightness[i]);

            double[] p = new double[256];
            double mean = 0.0, variance = 0.0, entropy = 0.0, uniformity = 0.0, asym = 0.0, excess = 0.0;

            //Находим вероятность нахождения пикселя с яркостью i и среднее, энергию, энтропию
            for (int i = 0; i < 256; i++)
            {
                p[i] = Convert.ToDouble(brightness[i]) / Convert.ToDouble(width * height);
                mean += p[i] * i;
                uniformity += Math.Pow(p[i], 2);

                if (p[i] != 0)
                    entropy -= Math.Log(p[i], 2) * p[i];
            }

            //Находим дисперсию
            for (int i = 0; i < 256; i++)
                variance += Math.Pow(i - mean, 2) * p[i];

            //Находим коэффициенты асимметрии и эксцесса
            for (int i = 0; i < 256; i++) {
                asym += Math.Pow(i - mean, 3) * p[i] / Math.Pow(Math.Sqrt(variance), 3);
                excess += (Math.Pow(i - mean, 4) * p[i] - 3) / Math.Pow(variance, 2);
            }

            textBox2.Text = String.Format("{0:0.0000}", mean);
            textBox3.Text = String.Format("{0:0.0000}", variance);
            textBox4.Text = String.Format("{0:0.0000}", entropy);
            textBox5.Text = String.Format("{0:0.0000}", uniformity);
            textBox6.Text = String.Format("{0:0.0000}", asym); 
            textBox7.Text = String.Format("{0:0.0000}", excess);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int Nt = 0, r, c;
            int[,] Ns = new int[width, height];
            r = Convert.ToInt32(textBox1.Text);
            c = Convert.ToInt32(textBox8.Text);

            //Подсчет всевозможных вариантов, с заданными r и c
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if ((i + c < width) && (j + r < height))
                    {
                        Nt++;
                        Ns[Convert.ToInt32(image.GetPixel(i, j).GetBrightness() * 255), Convert.ToInt32(image.GetPixel(i + c, j + r).GetBrightness() * 255)]++;
                    }
                    else
                        break;
                }

            textBox9.Text = Nt.ToString();

            //Вычисляем энергию совместной встречаемости
            double energy = 0.0;

            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 255;
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                {
                    chart2.Series["Встречаемость"].Points.AddXY(i, Ns[i, j]);
                    energy += Math.Pow(Convert.ToDouble(Ns[i, j]) / Convert.ToDouble(Nt), 2);
                }

            textBox11.Text = String.Format("{0:0.0000}", energy);
            
        }
     
    }
}
