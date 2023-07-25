using System;
using System.Runtime;
using System.Drawing;
using System.Numerics;
using System.Collections;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Authentication;
using System.IO.Ports;
using System.Xml;

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

        // original colors
        int o_r = 0;
        int o_g = 0;
        int o_b = 0;

        for (int x = 1; x < bmp.Width - 1; x++)
        {
            for (int y = 0; y < bmp.Height - 1; y++)
            {
                Color pixelColor = bmp.GetPixel(x, y);

                int red = pixelColor.R;
                int green = pixelColor.G;
                int blue = pixelColor.B;
                int gray = (byte)(.299 * red + .587 * green + .114 * blue);

                // Original colors
                o_r = red;
                o_g = green;
                o_b = blue;
            }
        }

        Bitmap qBmp = Quantization(bmp);
        Bitmap dBmp = FloydSteinberg(bmp, qBmp);

        for (int x = 1; x < bmp.Width - 1; x++)
        {
            for (int y = 0; y < bmp.Height - 1; y++)
            {
                Color pixelColor = bmp.GetPixel(x, y);

                int red = pixelColor.R;
                int green = pixelColor.G;
                int blue = pixelColor.B;
                int gray = (byte)(.299 * red + .587 * green + .114 * blue);
                
                // GRAYSCALE
                //red = gray;
                //green = gray;
                //blue = gray;
                //bmp.SetPixel(x, y, Color.FromArgb(red, green, blue));

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
            }
        }

        stopwatch.Stop();
        Console.WriteLine("This took: " + stopwatch.ElapsedMilliseconds.ToString() + " ms");

        Console.WriteLine("Please input new file path");
        string savePath = Console.ReadLine();

        dBmp.Save(savePath);

        Console.WriteLine("To exit press any key");
        Console.ReadKey(true);
    }

    static Bitmap FloydSteinberg(Bitmap original, Bitmap quant)
    {
        //Bitmap dBmp = null;

        for (int x = 1; x < original.Width - 1; x++)
        {
            for (int y = 0; y < original.Height - 1; y++)
            {
                // Original colors
                Color originalColor = original.GetPixel(x, y);
                int o_red = originalColor.R;
                int o_green = originalColor.G;
                int o_blue = originalColor.B;

                // Quant colors
                Color quantColor = quant.GetPixel(x, y);
                int q_red = quantColor.R;
                int q_green = quantColor.G;
                int q_blue = quantColor.B;

                // Dither error
                int errorR = o_red - q_red;
                int errorG = o_green - q_green;
                int errorB = o_blue - q_blue;

                quant = OffsetDither(x + 1, y, quant, errorR, errorG, errorB, 7);
                quant = OffsetDither(x - 1, y + 1, quant, errorR, errorG, errorB, 3);
                quant = OffsetDither(x, y + 1, quant, errorR, errorG, errorB, 5);
                quant = OffsetDither(x + 1, y + 1, quant, errorR, errorG, errorB, 1);
            }
        }

        return quant;
    }

    static Bitmap Quantization(Bitmap bmp)
    {
        Bitmap quantizedBMP = null;

        for (int x = 1; x < bmp.Width - 1; x++)
        {
            for (int y = 0; y < bmp.Height - 1; y++)
            {
                // Original colors
                Color pixelColor = bmp.GetPixel(x, y);
                int o_red = pixelColor.R;
                int o_green = pixelColor.G;
                int o_blue = pixelColor.B;

                // Quantizated colors
                int quantAmount = 1;
                int q_r = (quantAmount * o_red / 255) * (255 / quantAmount);
                int q_g = (quantAmount * o_green / 255) * (255 / quantAmount);
                int q_b = (quantAmount * o_blue / 255) * (255 / quantAmount);

                // QUANTIZATION
                Color ditherPixel = Color.FromArgb(q_r, q_g, q_b);
                bmp.SetPixel(x, y, ditherPixel);
                quantizedBMP = bmp;
            }
        }

        return quantizedBMP;
    }

    static Bitmap OffsetDither(int x, int y, Bitmap bmp, int errorR, int errorG, int errorB, int amount)
    {
        Color ditherPixel = bmp.GetPixel(x, y);
        int newR = ditherPixel.R + errorR * amount / 16;
        int newG = ditherPixel.G + errorG * amount / 16;
        int newB = ditherPixel.B + errorB * amount / 16;

        int clampedR =  Clamp(newR, 0, 255);
        int clampedG = Clamp(newG, 0, 255);
        int clampedB = Clamp(newB, 0, 255);

        Color newDitherColor = Color.FromArgb(clampedR, clampedG, clampedB);
        bmp.SetPixel(x, y, newDitherColor);
        return bmp;
    }

    public static int Clamp(int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
}

