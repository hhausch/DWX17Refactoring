using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    internal class Tetrominoe
    {
        private UI _Ui;
        public int[,] I;
        public int[,] O;
        public int[,] T;
        public int[,] S;
        public int[,] Z;
        public int[,] J;
        public int[,] L;
        public List<int[,]> Tetrominoes;

        private bool isErect = false;
        private int[,] shape;
        private int[] pix = new int[2];
        public List<int[]> location = new List<int[]>();

        public Tetrominoe(UI ui)
        {
            _Ui = ui;
            I = new int[1, 4] { { 1, 1, 1, 1 } };
            O = new int[2, 2] { { 1, 1 }, { 1, 1 } };
            T = new int[2, 3] {{0, 1, 0}, {1, 1, 1}};
            S = new int[2, 3] { { 0, 1, 1 }, { 1, 1, 0 } };
            Z = new int[2, 3] { { 1, 1, 0 }, { 0, 1, 1 } };
            J = new int[2, 3] { { 1, 0, 0 }, { 1, 1, 1 } };
            L = new int[2, 3] { { 0, 0, 1 }, { 1, 1, 1 } };
            Tetrominoes = new List<int[,]>() { I, O, T, S, Z, J, L };

            Random rnd = new Random();
            shape = Tetrominoes[rnd.Next(0, 7)];
            for (int i = 23; i < 33; ++i)
            {
                for (int j = 3; j < 10; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write("  ");
                }

            }

            _Ui.DrawBorder();

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        Console.SetCursorPosition(((10 - shape.GetLength(1)) / 2 + j) * 2 + 20, i + 5);
                        Console.Write(Program.Instance.sqr);
                    }
                }
            }
        }

        internal void Spawn()
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        location.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            Update();
        }

        internal void Drop()
        {

            if (isSomethingBelow())
            {
                for (int i = 0; i < 4; i++)
                {
                    Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] = 1;
                }
                Program.Instance.isDropped = true;

            }
            else
            {
                for (int numCount = 0; numCount < 4; numCount++)
                {
                    location[numCount][0] += 1;
                }
                Update();
            }
        }

        internal void Rotate()
        {
            List<int[]> templocation = new List<int[]>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        templocation.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }

            if (shape == Tetrominoes[0])
            {
                if (isErect == false)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                    }
                }
                else
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Counterclockwise");
                    }
                }
            }

            else if (shape == Tetrominoes[3])
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[3], "Clockwise");
                }
            }

            else if (shape == Tetrominoes[1]) return;
            else
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                }
            }


            for (int count = 0; isOverlayLeft(templocation) != false | isOverlayRight(templocation) != false | isOverlayBelow(templocation) != false; count++)
            {
                if (isOverlayLeft(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] += 1;
                    }
                }

                if (isOverlayRight(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] -= 1;
                    }
                }
                if (isOverlayBelow(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][0] -= 1;
                    }
                }
                if (count == 3)
                {
                    return;
                }
            }

            location = templocation;

        }

        private int[] TransformMatrix(int[] coord, int[] axis, string dir)
        {
            int[] pcoord = { coord[0] - axis[0], coord[1] - axis[1] };
            if (dir == "Counterclockwise")
            {
                pcoord = new int[] { -pcoord[1], pcoord[0] };
            }
            else if (dir == "Clockwise")
            {
                pcoord = new int[] { pcoord[1], -pcoord[0] };
            }

            return new int[] { pcoord[0] + axis[0], pcoord[1] + axis[1] };
        }

        internal bool isSomethingBelow()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][0] + 1 >= 23)
                    return true;
                if (location[i][0] + 1 < 23)
                {
                    if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0] + 1, location[i][1]] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool? isOverlayBelow(List<int[]> location)
        {
            List<int> ycoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                ycoords.Add(location[i][0]);
                if (location[i][0] >= 23)
                    return true;
                if (location[i][0] < 0)
                    return null;
                if (location[i][1] < 0)
                {
                    return null;
                }
                if (location[i][1] > 9)
                {
                    return null;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (ycoords.Max() - ycoords.Min() == 3)
                {
                    if (ycoords.Max() == location[i][0] | ycoords.Max() - 1 == location[i][0])
                    {
                        if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (ycoords.Max() == location[i][0])
                    {
                        if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        internal bool isSomethingLeft()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 0)
                {
                    return true;
                }
                else if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1] - 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private bool? isOverlayLeft(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] < 0)
                {
                    return true;
                }
                if (location[i][1] > 9)
                {
                    return false;
                }
                if (location[i][0] >= 23)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Min() == location[i][1] | xcoords.Min() + 1 == location[i][1])
                    {
                        if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (xcoords.Min() == location[i][1])
                    {
                        if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool isSomethingRight()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 9)
                {
                    return true;
                }
                else if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1] + 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private bool? isOverlayRight(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] > 9)
                {
                    return true;
                }
                if (location[i][1] < 0)
                {
                    return false;
                }
                if (location[i][0] >= 23)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Max() == location[i][1] | xcoords.Max() - 1 == location[i][1])
                    {
                        if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (xcoords.Max() == location[i][1])
                    {
                        if (Program.Instance.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal void Update()
        {
            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Program.Instance.grid[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                Program.Instance.grid[location[i][0], location[i][1]] = 1;
            }
            Program.Instance.Draw();
        }
    }
}
