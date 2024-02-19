using System;
using System.Runtime.InteropServices;

namespace matrix
{
    internal static class DigitalRain
    {
        // Importing necessary Windows API functions
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Constants for ShowWindow function
        private const int SW_MAXIMIZE = 3;
        
        private class Config
        {
            public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.DarkGreen;
            public ConsoleColor HighlightColor { get; set; } = ConsoleColor.DarkGray;
            public ConsoleColor CharacterColor { get; set; } = ConsoleColor.White;
            public int RefreshRate = 50; // Milliseconds
            public float WindowScale = 1f; // Scale relative to the largest window size
            public int MinDropLength = 4;  // Minimum drop length
            public int MaxDropLength = 25; // Maximum drop length
        }

        static void Main(string[] args)
        {
            var config = new Config();

            Console.Title = "Digital Rain";
            Console.ForegroundColor = config.ForegroundColor;
            Console.WindowLeft = Console.WindowTop = 0;
            int windowHeight = (int)(Console.LargestWindowHeight * config.WindowScale);
            int windowWidth = (int)(Console.LargestWindowWidth * config.WindowScale);
            Console.WindowHeight = Console.BufferHeight = windowHeight;
            Console.WindowWidth = Console.BufferWidth = windowWidth;
            
            // Get handle to the console window
            IntPtr handle = GetConsoleWindow();

            // Maximize the console window
            ShowWindow(handle, SW_MAXIMIZE);
            
            Console.Write("Hit any key to start...");
            Console.ReadKey();
            
            Console.CursorVisible = false;
            Initialize(out var width, out var height, out var y, out var l, config);

            while (true)
            {
                DateTime t1 = DateTime.Now;
                MatrixStep(width, height, y, l, config);
                int ms = config.RefreshRate - (int)(DateTime.Now - t1).TotalMilliseconds;
                if (ms > 0)
                {
                    Thread.Sleep(ms);
                }

                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().Key == ConsoleKey.F5)
                    {
                        Initialize(out width, out height, out y, out l, config);
                    }
                }
            }
        }

        private static void MatrixStep(int width, int height, int[] y, int[] l, Config config)
        {
            int x;
            bool thistime = false;
            thistime = !thistime;
            for (x = 0; x < width; ++x)
            {
                if (x % 11 == 10)
                {
                    if (!thistime)
                        continue;
                    Console.ForegroundColor = config.HighlightColor;
                }
                else
                {
                    Console.ForegroundColor = config.ForegroundColor;
                    Console.SetCursorPosition(x, InBoxY(y[x] - 2 - (l[x] / 40 * 2), height));
                    Console.Write(RandomChar);
                    Console.ForegroundColor = config.CharacterColor;
                }
                Console.SetCursorPosition(x, y[x]);
                Console.Write(RandomChar);
                y[x] = InBoxY(y[x] + 1, height);
                Console.SetCursorPosition(x, InBoxY(y[x] - l[x], height));
                Console.Write(' ');
            }
        }

        private static Random _random = new();
        private static void Initialize(out int width, out int height, out int[] y, out int[] l, Config config)
        {
            height = Console.WindowHeight;
            width = Console.WindowWidth - 1;
            y = new int[width];
            l = new int[width];
            int x;
            Console.Clear();
            for (x = 0; x < width; ++x)
            {
                y[x] = _random.Next(height);
                // Adjust the drop lengths based on the configuration
                l[x] = _random.Next(config.MinDropLength, config.MaxDropLength + 1);
            }
        }

        private static char RandomChar
        {
            get
            {
                int t = _random.Next(10);
                if (t <= 2)
                    return (char)('0' + _random.Next(10));
                else if (t <= 4)
                    return (char)('a' + _random.Next(27));
                else if (t <= 6)
                    return (char)('A' + _random.Next(27));
                else
                    return (char)(_random.Next(32, 255));
            }
        }

        private static int InBoxY(int n, int height)
        {
            n = n % (height);
            if (n < 0)
            {
                return n + height;
            }
            return n;
        }
    }
}
