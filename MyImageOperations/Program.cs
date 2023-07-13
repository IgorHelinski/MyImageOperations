using System;
using System.Drawing;
using System.Numerics;
using System.Collections;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello! Specify path of the image!");
        string filePath = Console.ReadLine();
        
        Bitmap bmpOriginal = new Bitmap(filePath);
        Bitmap bmp = bmpOriginal;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                Color pixelColor = bmp.GetPixel(i, j);
                int red = pixelColor.R;
                int green = pixelColor.G;
                int blue = pixelColor.B;
                int gray = (byte)(.299 * red + .587 * green + .114 * blue);

                float _r = (float)red;
                float _g = (float)green;
                float _b = (float)blue; 

                decimal r = Math.Round((decimal)(2 * _r / 255)) * (255 / 2);
                decimal g = Math.Round((decimal)(2 * _g / 255)) * (255 / 2);
                decimal b = Math.Round((decimal)(2 * _b / 255)) * (255 / 2);

                Color ditherPixel = Color.FromArgb((int)r, (int)g, (int)b);
                bmp.SetPixel(i, j, ditherPixel);

                // INVERSE
                //Color inversePixel = Color.FromArgb(255 - red, 255 - green, 255 - blue);
                //bmp.SetPixel(i, j, inversePixel);

                // BLACK OR WHITE
                //if(gray < 128)
                //{
                //    bmp.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                //}
                //else if(gray >= 128)
                //{
                //    bmp.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                //}

                // GRAYSCALE
                //red = gray;
                //green = gray;
                //blue = gray;
                //bmp.SetPixel(i, j, Color.FromArgb(red, green, blue));
            }
        }

        stopwatch.Stop();
        Console.WriteLine("This took: " + stopwatch.ElapsedMilliseconds.ToString() + " ms");

        Console.WriteLine("Please input new file path");
        string savePath = Console.ReadLine();

        bmp.Save(savePath);

        Console.WriteLine("To exit press any key");
        Console.ReadKey(true);
    }
}

