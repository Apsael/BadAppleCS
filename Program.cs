using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;
using BadApple.Net.Utils;


namespace BadApple
{
    class BadAppleClass
    {

        static void Main(string[] args)

        {
            var frameFiles = Directory.GetFiles("pics");
            Console.SetWindowSize(81, 30);
            var frames = new List<IEnumerable<ColoredString>>();

            for (var j = 0; j < frameFiles.Length; j++)
            {
                Console.Title = $"Procesando frame {j + 1}/{frameFiles.Length}";

                var frame = Parser.Parse(frameFiles[j]);
                var strings = new List<ColoredString>();
                var x = new ColoredString();

                foreach (var height in frame)
                {
                    foreach (var color in height)
                    {
                        if (x.Color == color)
                        {
                            x.Text += " ";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(x.Text))
                                strings.Add(x);

                            x = new ColoredString()
                            {
                                Color = color,
                                Text = " "
                            };
                        }
                    }
                    x.Text += "\n";
                }

                frames.Add(strings);
            }

            Console.Title = ":D";


            const double frameLimit = 1000f / 31.4;
            var sw = Stopwatch.StartNew();

            var sp = new SoundPlayer("audio.wav");
            sp.Play();

            foreach (var frame in frames)
            {
                var startDrawTime = sw.Elapsed.TotalMilliseconds;
                Console.SetCursorPosition(0, 0);

                foreach (var x in frame)
                {
                    Console.BackgroundColor = x.Color;
                    Console.Write(x.Text);
                }

                var durationTime = sw.Elapsed.TotalMilliseconds - startDrawTime;

                if (durationTime < frameLimit)
                    Thread.Sleep((int)(frameLimit - durationTime));
            }

            Console.Title = "¡Fin!";
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Thread.Sleep(1500);
            Console.WriteLine("Presione una tecla para salir... ");
            Console.ReadKey();
        }

    }
}

namespace BadApple.Net.Utils
{
    public class Parser
    {
        public static ConsoleColor[][] Parse(string img)
        {
            var image = new Bitmap(img);
            var colors = new ConsoleColor[image.Height][];

            for (var i = 0; i < colors.Length; i++)
                colors[i] = new ConsoleColor[image.Width];

            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var color = image.GetPixel(x, y);

                    colors[y][x] = color.G > 196 ? ConsoleColor.White :
                        color.G > 128 ? ConsoleColor.Gray :
                        color.G > 64 ? ConsoleColor.DarkGray : ConsoleColor.Black;
                }
            }
            return colors;
        }
    }
}

namespace BadApple.Net.Utils
{
    public class ColoredString
    {
        public string Text = string.Empty;
        public ConsoleColor Color = ConsoleColor.Black;
    }
}
