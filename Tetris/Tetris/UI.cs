using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class UI
    {
        private readonly Config _Config;
        private readonly Statistics _Statistics;
        public UI(Config config, Statistics statistics)
        {
            this._Config = config;
            _Statistics = statistics;
        }

        internal void Setup()
        {
            DrawBorder();
            Console.SetCursorPosition(25, 0);
            Console.WriteLine("Level " + _Statistics.Level);
            Console.SetCursorPosition(25, 1);
            Console.WriteLine("Score " + _Statistics.Score);
            Console.SetCursorPosition(25, 2);
            Console.WriteLine("LinesCleared " + _Statistics.LinesCleared);
        }

        public void DrawBorder()
        {
            for (var lengthCount = 0; lengthCount <= _Config.levelDropRateReductionFactor; ++lengthCount)
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
