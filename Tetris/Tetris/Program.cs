using System;
using System.Diagnostics;
using System.Text;

namespace Tetris
{
    internal static class Program
    {
        private const int initialDropRate = 10; //300
        private const int levelDropRateReductionFactor = 22;
        private const int verticalGridSize = 23;
        private const int horizontalGridSize = 10;

        public static string sqr = "■";
        public static int[,] grid = new int[verticalGridSize, horizontalGridSize];
        public static int[,] droppedtetrominoeLocationGrid;
        public static Stopwatch timer;
        public static Stopwatch dropTimer;
        public static Stopwatch inputTimer;
        public static int dropTime;
        public static int dropRate;
        public static bool isDropped;
        private static Tetrominoe tet;
        private static Tetrominoe nexttet;
        public static ConsoleKeyInfo key;
        public static bool isKeyPressed;
        public static int linesCleared;
        public static int score;
        public static int level;

        private static void Main()
        {
            var stayInLoop = true;

            while (stayInLoop)
            {
                Console.OutputEncoding = Encoding.UTF8;

                SetupUI();
                InitGameParameters();

                WaitForStart();

                SetupTimer();

                InitTetrominoe();

                Update();

                stayInLoop = WaitForRestartOrCancel();
            }
        }

        private static void InitTetrominoe()
        {
            nexttet = new Tetrominoe();
            tet = nexttet;
            tet.Spawn();
            nexttet = new Tetrominoe();
        }

        private static bool WaitForRestartOrCancel()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Over \n Replay? (Y/N)");
            var input = Console.ReadLine();

            if (input == "y" || input == "Y")
            {
                InitGameParameters();

                Console.Clear();
                return true;
            }

            return false;
        }

        private static void InitGameParameters()
        {
            droppedtetrominoeLocationGrid = new int[verticalGridSize, horizontalGridSize];
            timer = new Stopwatch();
            dropTimer = new Stopwatch();
            inputTimer = new Stopwatch();
            dropRate = initialDropRate;
            isDropped = false;
            isKeyPressed = false;
            linesCleared = 0;
            score = 0;
            level = 1;
        }

        private static void SetupTimer()
        {
            timer.Start();
            dropTimer.Start();
            var time = timer.ElapsedMilliseconds;
        }

        private static void WaitForStart()
        {
            Console.SetCursorPosition(4, 5);
            Console.WriteLine("Press any key");
            Console.ReadKey(true);
        }

        private static void SetupUI()
        {
            drawBorder();
            Console.SetCursorPosition(25, 0);
            Console.WriteLine("Level " + level);
            Console.SetCursorPosition(25, 1);
            Console.WriteLine("Score " + score);
            Console.SetCursorPosition(25, 2);
            Console.WriteLine("LinesCleared " + linesCleared);
        }

        private static void fillGrid()
        {
            for (var i = 0; i < 23; ++i)
            {
                Console.SetCursorPosition(1, i);
                for (var j = 0; j < 10; j++)
                    Console.Write(sqr);
                Console.WriteLine();
            }
        }

        private static void Update()
        {
            while (true) //Update Loop
            {
                dropTime = (int) dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0;
                    dropTimer.Restart();
                    tet.Drop();
                }
                if (isDropped)
                {
                    tet = nexttet;
                    nexttet = new Tetrominoe();
                    tet.Spawn();

                    isDropped = false;
                }
                int j;
                for (j = 0; j < 10; j++)
                    if (droppedtetrominoeLocationGrid[0, j] == 1)
                        return;

                Input();
                ClearBlock();
            } //end Update
        }

        private static void ClearBlock()
        {
            var combo = 0;
            for (var i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 10; j++)
                    if (droppedtetrominoeLocationGrid[i, j] == 0)
                        break;
                if (j == 10)
                {
                    linesCleared++;
                    combo++;
                    for (j = 0; j < 10; j++)
                        droppedtetrominoeLocationGrid[i, j] = 0;
                    var newdroppedtetrominoeLocationGrid = new int[23, 10];
                    for (var k = 1; k < i; k++)
                    for (var l = 0; l < 10; l++)
                        newdroppedtetrominoeLocationGrid[k + 1, l] = droppedtetrominoeLocationGrid[k, l];
                    for (var k = 1; k < i; k++)
                    for (var l = 0; l < 10; l++)
                        droppedtetrominoeLocationGrid[k, l] = 0;
                    for (var k = 0; k < 23; k++)
                    for (var l = 0; l < 10; l++)
                        if (newdroppedtetrominoeLocationGrid[k, l] == 1)
                            droppedtetrominoeLocationGrid[k, l] = 1;
                    Draw();
                }
            }
            if (combo == 1)
                score += 40 * level;
            else if (combo == 2)
                score += 100 * level;
            else if (combo == 3)
                score += 300 * level;
            else if (combo > 3)
                score += 300 * combo * level;

            if (linesCleared < 5) level = 1;
            else if (linesCleared < 10) level = 2;
            else if (linesCleared < 15) level = 3;
            else if (linesCleared < 25) level = 4;
            else if (linesCleared < 35) level = 5;
            else if (linesCleared < 50) level = 6;
            else if (linesCleared < 70) level = 7;
            else if (linesCleared < 90) level = 8;
            else if (linesCleared < 110) level = 9;
            else if (linesCleared < 150) level = 10;


            if (combo > 0)
            {
                Console.SetCursorPosition(25, 0);
                Console.WriteLine("Level " + level);
                Console.SetCursorPosition(25, 1);
                Console.WriteLine("Score " + score);
                Console.SetCursorPosition(25, 2);
                Console.WriteLine("LinesCleared " + linesCleared);
            }

            dropRate = initialDropRate - levelDropRateReductionFactor * level;
        }

        private static void Input()
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                isKeyPressed = true;
            }
            else
            {
                isKeyPressed = false;
            }

            if ((key.Key == ConsoleKey.LeftArrow) & !tet.isSomethingLeft() & isKeyPressed)
            {
                for (var i = 0; i < 4; i++)
                    tet.location[i][1] -= 1;
                tet.Update();
                //    Console.Beep();
            }
            else if ((key.Key == ConsoleKey.RightArrow) & !tet.isSomethingRight() & isKeyPressed)
            {
                for (var i = 0; i < 4; i++)
                    tet.location[i][1] += 1;
                tet.Update();
            }
            if ((key.Key == ConsoleKey.DownArrow) & isKeyPressed)
                tet.Drop();
            if ((key.Key == ConsoleKey.UpArrow) & isKeyPressed)
                for (; tet.isSomethingBelow() != true;)
                    tet.Drop();
            if ((key.Key == ConsoleKey.Spacebar) & isKeyPressed)
            {
                //rotate
                tet.Rotate();
                tet.Update();
            }
        }

        public static void Draw()
        {
            for (var i = 0; i < 23; ++i)
            for (var j = 0; j < 10; j++)
            {
                Console.SetCursorPosition(1 + 2 * j, i);
                if ((grid[i, j] == 1) | (droppedtetrominoeLocationGrid[i, j] == 1))
                {
                    Console.SetCursorPosition(1 + 2 * j, i);
                    Console.Write(sqr);
                }
                else
                {
                    Console.Write("  ");
                }
            }
        }

        public static void drawBorder()
        {
            for (var lengthCount = 0; lengthCount <= levelDropRateReductionFactor; ++lengthCount)
            {
                Console.SetCursorPosition(0, lengthCount);
                Console.Write("*");
                Console.SetCursorPosition(21, lengthCount);
                Console.Write("*");
            }
            Console.SetCursorPosition(0, 23);
            for (var widthCount = 0; widthCount <= 10; widthCount++)
                Console.Write("*-");
        }
    }
}