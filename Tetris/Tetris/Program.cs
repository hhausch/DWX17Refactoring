using System;
using System.Diagnostics;
using System.Text;

namespace Tetris
{
    internal class Program
    {

        public static Program Instance { get; } = new Program();
        private readonly Config _Config;
        private readonly Data _Data;
        private readonly Statistics _Statistics;
        private readonly UI _Ui;

        public string sqr = "■";
        public int[,] grid;
        public int[,] droppedtetrominoeLocationGrid;
        public Stopwatch timer;
        public Stopwatch dropTimer;
        public Stopwatch inputTimer;
        public int dropTime;
        public int dropRate;
        public bool isDropped;
        private Tetrominoe tet;
        private Tetrominoe nexttet;
        public ConsoleKeyInfo key;
        public bool isKeyPressed;

        private Program()
        {
            _Config = new Config();
            _Data = new Data();
            _Statistics = new Statistics();
            _Ui = new UI(_Config, _Statistics);
        }

        private static void Main()
        {
            Instance.Run();
        }

        private void Run()
        {
            var stayInLoop = true;

            while (stayInLoop)
            {
                Console.OutputEncoding = Encoding.UTF8;

                _Ui.Setup();

                InitGameParameters();

                WaitForStart();

                SetupTimer();

                InitTetrominoe();

                Update();

                stayInLoop = Instance.WaitForRestartOrCancel();
            }
        }

        private void InitTetrominoe()
        {
            nexttet = new Tetrominoe(_Ui);
            tet = nexttet;
            tet.Spawn();
            nexttet = new Tetrominoe(_Ui);
        }

        private bool WaitForRestartOrCancel()
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

        private void InitGameParameters()
        {
            grid = new int[_Config.VerticalGridSize, _Config.HorizontalGridSize];
            droppedtetrominoeLocationGrid = new int[_Config.VerticalGridSize, _Config.HorizontalGridSize];
            timer = new Stopwatch();
            dropTimer = new Stopwatch();
            inputTimer = new Stopwatch();
            dropRate = _Config.initialDropRate;
            isDropped = false;
            isKeyPressed = false;
            _Statistics.LinesCleared = 0;
            _Statistics.Score = 0;
            _Statistics.Level = 1;
        }

        private void SetupTimer()
        {
            timer.Start();
            dropTimer.Start();
            var time = timer.ElapsedMilliseconds;
        }

        private void WaitForStart()
        {
            Console.SetCursorPosition(4, 5);
            Console.WriteLine("Press any key");
            Console.ReadKey(true);
        }


        private void Update()
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
                    nexttet = new Tetrominoe(_Ui);
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

        private void ClearBlock()
        {
            var combo = 0;
            for (var i = 0; i < _Config.VerticalGridSize; i++)
            {
                int j;
                for (j = 0; j < _Config.HorizontalGridSize; j++)
                    if (droppedtetrominoeLocationGrid[i, j] == 0)
                        break;
                if (j == 10)
                {
                    _Statistics.LinesCleared++;
                    combo++;
                    for (j = 0; j < _Config.HorizontalGridSize; j++)
                        droppedtetrominoeLocationGrid[i, j] = 0;
                    var newdroppedtetrominoeLocationGrid = new int[_Config.VerticalGridSize, _Config.HorizontalGridSize];
                    for (var k = 1; k < i; k++)
                        for (var l = 0; l < _Config.HorizontalGridSize; l++)
                            newdroppedtetrominoeLocationGrid[k + 1, l] = droppedtetrominoeLocationGrid[k, l];
                    for (var k = 1; k < i; k++)
                        for (var l = 0; l < _Config.HorizontalGridSize; l++)
                            droppedtetrominoeLocationGrid[k, l] = 0;
                    for (var k = 0; k < _Config.VerticalGridSize; k++)
                        for (var l = 0; l < _Config.HorizontalGridSize; l++)
                            if (newdroppedtetrominoeLocationGrid[k, l] == 1)
                                droppedtetrominoeLocationGrid[k, l] = 1;
                    Draw();
                }
            }
            if (combo == 1)
                _Statistics.Score += 40 * _Statistics.Level;
            else if (combo == 2)
                _Statistics.Score += 100 * _Statistics.Level;
            else if (combo == 3)
            _Statistics.Score += 300 * _Statistics.Level;
            else if (combo > 3)
                _Statistics.Score += 300 * combo * _Statistics.Level;

            if (_Statistics.LinesCleared < 5) _Statistics.Level = 1;
            else if (_Statistics.LinesCleared < 10) _Statistics.Level = 2;
            else if (_Statistics.LinesCleared < 15) _Statistics.Level = 3;
            else if (_Statistics.LinesCleared < 25) _Statistics.Level = 4;
            else if (_Statistics.LinesCleared < 35) _Statistics.Level = 5;
            else if (_Statistics.LinesCleared < 50) _Statistics.Level = 6;
            else if (_Statistics.LinesCleared < 70) _Statistics.Level = 7;
            else if (_Statistics.LinesCleared < 90) _Statistics.Level = 8;
            else if (_Statistics.LinesCleared < 110) _Statistics.Level = 9;
            else if (_Statistics.LinesCleared < 150) _Statistics.Level = 10;

            UpdateStatistics(combo);

            dropRate = _Config.initialDropRate - _Config.levelDropRateReductionFactor * _Statistics.Level;
        }

        private void UpdateStatistics(int combo)
        {
            if (combo > 0)
            {
                Console.SetCursorPosition(25, 0);
                Console.WriteLine("Level " + _Statistics.Level);
                Console.SetCursorPosition(25, 1);
                Console.WriteLine("Score " + _Statistics.Score);
                Console.SetCursorPosition(25, 2);
                Console.WriteLine("LinesCleared " + _Statistics.LinesCleared);
            }
        }

        private void Input()
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

        public void Draw()
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


    }
}