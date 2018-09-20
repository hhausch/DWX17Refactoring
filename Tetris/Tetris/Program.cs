using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Tetris
{
    static class Program
    {

        public static string sqr = "■";
        public static int[,] grid = new int[23, 10];
        public static int[,] droppedtetrominoeLocationGrid = new int[23, 10];
        public static Stopwatch timer = new Stopwatch();
        public static Stopwatch dropTimer = new Stopwatch();
        public static Stopwatch inputTimer = new Stopwatch();
        public static int dropTime, dropRate = 300;
        public static bool isDropped = false;
        static Tetrominoe tet;
        static Tetrominoe nexttet;
        public static ConsoleKeyInfo key;
        public static bool isKeyPressed = false;
        public static int linesCleared = 0, score = 0, level = 1;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            drawBorder();
            Console.SetCursorPosition(4, 5);
            Console.WriteLine("Press any key");
            Console.ReadKey(true);
            timer.Start();
            dropTimer.Start();
            long time = timer.ElapsedMilliseconds;
            Console.SetCursorPosition(25, 0);
            Console.WriteLine("Level " + level);
            Console.SetCursorPosition(25, 1);
            Console.WriteLine("Score " + score);
            Console.SetCursorPosition(25, 2);
            Console.WriteLine("LinesCleared " + linesCleared);
            nexttet = new Tetrominoe();
            tet = nexttet;
            tet.Spawn();
            nexttet = new Tetrominoe();

            Update();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Over \n Replay? (Y/N)");
            string input = Console.ReadLine();

            if (input == "y" || input == "Y")
            {
                int[,] grid = new int[23, 10];
                droppedtetrominoeLocationGrid = new int[23, 10];
                timer = new Stopwatch();
                dropTimer = new Stopwatch();
                inputTimer = new Stopwatch();
                dropRate = 300;
                isDropped = false;
                isKeyPressed = false;
                linesCleared = 0;
                score = 0;
                level = 1;
                GC.Collect();
                Console.Clear();
                Main();
            }
            else return;

        }

        private static void fillGrid()
        {
            for (int i = 0; i < 23; ++i)
            {
                Console.SetCursorPosition(1, i);
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(sqr);
                }
                Console.WriteLine();
            }
        }

        private static void Update()
        {
            while (true)//Update Loop
            {
                dropTime = (int)dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0;
                    dropTimer.Restart();
                    tet.Drop();
                }
                if (isDropped == true)
                {
                    tet = nexttet;
                    nexttet = new Tetrominoe();
                    tet.Spawn();

                    isDropped = false;
                }
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[0, j] == 1)
                        return;
                }

                Input();
                ClearBlock();
            } //end Update
        }
        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[i, j] == 0)
                        break;
                }
                if (j == 10)
                {
                    linesCleared++;
                    combo++;
                    for (j = 0; j < 10; j++)
                    {
                        droppedtetrominoeLocationGrid[i, j] = 0;
                    }
                    int[,] newdroppedtetrominoeLocationGrid = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            newdroppedtetrominoeLocationGrid[k + 1, l] = droppedtetrominoeLocationGrid[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            droppedtetrominoeLocationGrid[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                    for (int l = 0; l < 10; l++)
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

            dropRate = 300 - 22 * level;

        }
        private static void Input()
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                isKeyPressed = true;
            }
            else
                isKeyPressed = false;

            if (Program.key.Key == ConsoleKey.LeftArrow & !tet.isSomethingLeft() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] -= 1;
                }
                tet.Update();
                //    Console.Beep();
            }
            else if (Program.key.Key == ConsoleKey.RightArrow & !tet.isSomethingRight() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] += 1;
                }
                tet.Update();
            }
            if (Program.key.Key == ConsoleKey.DownArrow & isKeyPressed)
            {
                tet.Drop();
            }
            if (Program.key.Key == ConsoleKey.UpArrow & isKeyPressed)
            {
                for (; tet.isSomethingBelow() != true;)
                {
                    tet.Drop();
                }
            }
            if (Program.key.Key == ConsoleKey.Spacebar & isKeyPressed)
            {
                //rotate
                tet.Rotate();
                tet.Update();
            }
        }
        public static void Draw()
        {
            for (int i = 0; i < 23; ++i)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.SetCursorPosition(1 + 2 * j, i);
                    if (grid[i, j] == 1 | droppedtetrominoeLocationGrid[i, j] == 1)
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
        }

        public static void drawBorder()
        {
            for (int lengthCount = 0; lengthCount <= 22; ++lengthCount)
            {
                Console.SetCursorPosition(0, lengthCount);
                Console.Write("*");
                Console.SetCursorPosition(21, lengthCount);
                Console.Write("*");
            }
            Console.SetCursorPosition(0, 23);
            for (int widthCount = 0; widthCount <= 10; widthCount++)
            {
                Console.Write("*-");
            }

        }

    }
}
