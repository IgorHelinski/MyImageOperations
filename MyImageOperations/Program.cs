using System;
using System.Drawing;
using System.Numerics;
using System.Collections;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Authentication;
using System.IO.Ports;

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

        for (int x = 1; x < bmp.Width - 1; x++)
        {
            for (int y = 0; y < bmp.Height - 1; y++)
            {
                Color pixelColor = bmp.GetPixel(x, y);

                int index = GetImgIndex(x, y, bmp.Width);

                int red = pixelColor.R;
                int green = pixelColor.G;
                int blue = pixelColor.B;
                int gray = (byte)(.299 * red + .587 * green + .114 * blue);

                float _r = (float)red;
                float _g = (float)green;
                float _b = (float)blue; 

                decimal r = Math.Round((decimal)(1 * _r / 255)) * (255 / 1);
                decimal g = Math.Round((decimal)(1 * _g / 255)) * (255 / 1);
                decimal b = Math.Round((decimal)(1 * _b / 255)) * (255 / 1);

                // JUST LOWERING DOWN THE DETAILS
                //Color ditherPixel = Color.FromArgb((int)r, (int)g, (int)b);
                //bmp.SetPixel(x, y, ditherPixel);

                float errorR = red - (float)r;
                float errorG = green - (float)g;
                float errorB = blue - (float)b;

                OffsetDither(x + 1, y    , bmp, errorR, errorG, errorB, 7);
                OffsetDither(x - 1, y + 1, bmp, errorR, errorG, errorB, 3);
                OffsetDither(x    , y + 1, bmp, errorR, errorG, errorB, 5);
                OffsetDither(x + 1, y + 1, bmp, errorR, errorG, errorB, 1);

                // INVERSE
                //Color inversePixel = Color.FromArgb(255 - red, 255 - green, 255 - blue);
                //bmp.SetPixel(x, y, inversePixel);

                // BLACK OR WHITE

                //Color afterDither = bmp.GetPixel(x, y);

                //int _red = afterDither.R;
                //int _green = afterDither.G;
                //int _blue = afterDither.B;
                //int _gray = (byte)(.299 * _red + .587 * _green + .114 * _blue);

                //if (_gray < 128)
                //{
                //    bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                //}
                //else if (_gray >= 128)
                //{
                //    bmp.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                //}

                // GRAYSCALE
                //red = gray;
                //green = gray;
                //blue = gray;
                //bmp.SetPixel(x, y, Color.FromArgb(red, green, blue));
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

    static void OffsetDither(int x, int y, Bitmap bmp, float errorR, float errorG, float errorB, int amount)
    {
        Color ditherPixel = bmp.GetPixel(x, y);
        float newR = (float)ditherPixel.R + errorR * amount / 16;
        float newG = (float)ditherPixel.G + errorG * amount / 16;
        float newB = (float)ditherPixel.B + errorB * amount / 16;

        if (newR >= 256)
            newR = 255;
        else if (newR < 0)
            newR = 0;

        if (newG >= 256)
            newG = 255;
        else if (newG < 0)
            newG = 0;

        if (newB >= 256)
            newB = 255;
        else if (newB < 0)
            newB = 0;

        Color newDitherColor = Color.FromArgb((int)newR, (int)newG, (int)newB);
        bmp.SetPixel(x, y, newDitherColor);
    }

    static int GetImgIndex(int x, int y, int width)
    {
        return x + y * width;
    }
}

