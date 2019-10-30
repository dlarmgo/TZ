
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;




namespace TZ.Model
{
    public class Tetris
    {
        public static readonly ILogger _logger = Program.LF.CreateLogger("Tetris");

        public int score = 0;
        public int level = 0;
        public int count = 0;

        public bool end = false;

        public int width;
        public int height;
        public Board board;
        public List<Figure> figures;

        public Figure currentFigure;
        public Thread autoThread;

        public Tetris(int w = 8, int h = 15)
        {
            width = w;
            height = h;
            board = new Board(w, h);
            figures = new List<Figure>();
            currentFigure = new T1(3, 11);
            figures.Add(currentFigure);

        }

        public void Scoring()
        {
            for (int y = board.height - 1; y >= 0; y--)
            {
                bool fullRow = true;
                for (int x = 0; x < board.width; x++)
                {
                    if (board.matrix[x, y] == false)
                    {
                        fullRow = false;
                    }
                }

                if (fullRow == true)
                {
                    score += 100 + level * 10;
                    count++;
                    level = count / 3;

                    for (int _y = y; _y < board.height - 1; _y++)
                    {
                        for (int x = 0; x < board.width; x++)
                        {
                            board.matrix[x, _y] = board.matrix[x, _y + 1];
                            board.color[x, _y] = board.color[x, _y + 1];
                        }
                    }

                   
                }
            }
        }


        public bool CheckMotion(MotionSide side)
        {
            bool available = true;
            Motion motion = new Motion(side);
            Figure movedFigure = new Figure();
            movedFigure.anchor.X = currentFigure.anchor.X + motion.dx;
            movedFigure.anchor.Y = currentFigure.anchor.Y + motion.dy;
            if (side == MotionSide.Rotate)
            {
                movedFigure.state = currentFigure.state.nextState;
            }
            else
            {
                movedFigure.state = currentFigure.state;
            }

            for (int x = 0; x < movedFigure.state.currentState.width; x++)
            {
                for (int y = 0; y < movedFigure.state.currentState.height; y++)
                {
                    if (movedFigure.state.currentState.matrix[x, y] == true
                        && (x + movedFigure.anchor.X < 0
                        || x + movedFigure.anchor.X >= board.width
                        || y + movedFigure.anchor.Y < 0
                        || y + movedFigure.anchor.Y >= board.height))
                    {
                        available = false;
                        return available;
                    }
                    if (movedFigure.state.currentState.matrix[x, y] == true
                        && board.matrix[x + movedFigure.anchor.X, y + movedFigure.anchor.Y] == true)
                    {
                        available = false;
                        return available;
                    }
                }

            }
            if (end == true)
            {
                available = false;
            }
            return available;
        }

        public void SpawnFigure()
        {
            if (end == false)
            {
                currentFigure = new T1(3, 11);
            }
            if (CheckMotion(MotionSide.Bottom) == false)
            {
                end = true;
            }
        }

        public void SetFigure()
        {
            currentFigure.isSet = true;
            for (int x = 0; x < currentFigure.state.currentState.width; x++)
            {
                for (int y = 0; y < currentFigure.state.currentState.height; y++)
                {
                    if (currentFigure.state.currentState.matrix[x, y] == true)
                    {
                        board.matrix[x + currentFigure.anchor.X, y + currentFigure.anchor.Y] = true;
                        board.color[x + currentFigure.anchor.X, y + currentFigure.anchor.Y] = currentFigure.color;
                    }
                }
            }
        }

        public void DoMotion(MotionSide side)
        {
            Motion moving = new Motion(side);
            // Setting figure to board
            if (side == MotionSide.Bottom && CheckMotion(side) == false)
            {
                SetFigure();
                Scoring();
                SpawnFigure();
            }
            if (CheckMotion(side) == true)
            {
                currentFigure.anchor.X += moving.dx;
                currentFigure.anchor.Y += moving.dy;

                if (side == MotionSide.Rotate)
                {
                    currentFigure.state = currentFigure.state.nextState;
                }
            }
        }

    }


    public class T1 : Figure
    {
        public static FigurePhase phase1 = new FigurePhase(
            new Board(
                new bool[,] {
                    {false, true, false, false},
                    {false, true, false, false},
                    {false, true, false, false},
                    {false, true, false, false},
                }
            ),
            null
        );

        public static FigurePhase phase2 = new FigurePhase(
            new Board(
                new bool[,] {
                    {false, false, false, false},
                    { true,  true,  true,  true},
                    {false, false, false, false},
                    {false, false, false, false},
                }
            ),
            null
        );
        public T1(int x, int y)
        {
            this.anchor = new Position(x, y);
            this.state = phase1;
            var random = new Random();
            this.color = string.Format("#{0:X6}", random.Next(65535) * 256);
        }
        static T1()
        {
            phase1.nextState = phase2;
            phase2.nextState = phase1;
        }
    }


    public class FigurePhase
    {
        public Board currentState;
        public FigurePhase nextState;
        public FigurePhase(Board b, FigurePhase next)
        {
            currentState = b;
            nextState = next;
        }
    }


    public class Figure
    {
        public Position anchor;
        public FigurePhase state;
        public bool isSet = false;
        public void SetFigureToBoard(Board board)
        {
            for (int x = 0; x < state.currentState.width; x++)
            {
                for (int y = 0; y < state.currentState.height; y++)
                {
                    if (state.currentState.matrix[x, y] == true)
                    {
                        board.matrix[x + anchor.X, y + anchor.Y] = true;
                    }
                }
            }
        }
        public bool CheckMotion(Board board, MotionSide side)
        {
            bool available = true;
            Motion motion = new Motion(side);
            Figure movedFigure = new Figure();
            movedFigure.anchor.X = this.anchor.X + motion.dx;
            movedFigure.anchor.Y = this.anchor.Y + motion.dy;
            if (side == MotionSide.Rotate)
            {
                movedFigure.state = this.state.nextState;
            }
            else
            {
                movedFigure.state = this.state;
            }

            for (int x = 0; x < movedFigure.state.currentState.width; x++)
            {
                for (int y = 0; y < movedFigure.state.currentState.height; y++)
                {
                    if (movedFigure.state.currentState.matrix[x, y] == true
                        && (x + movedFigure.anchor.X < 0
                        || x + movedFigure.anchor.X >= board.width
                        || y + movedFigure.anchor.Y < 0
                        || y + movedFigure.anchor.Y >= board.height))
                    {
                        available = false;
                        return available;
                    }
                    if (movedFigure.state.currentState.matrix[x, y] == true
                        && board.matrix[x + movedFigure.anchor.X, y + movedFigure.anchor.Y] == true)
                    {
                        available = false;
                        return available;
                    }
                }

            }
            return available;
        }
        public void DoMotion(Board board, MotionSide side)
        {
            Motion moving = new Motion(side);
            // Setting figure to board
            if (side == MotionSide.Bottom && CheckMotion(board, side) == false)
            {
                this.isSet = true;
                for (int x = 0; x < state.currentState.width; x++)
                {
                    for (int y = 0; y < state.currentState.height; y++)
                    {
                        board.matrix[x + this.anchor.X, y + this.anchor.Y] = true;
                    }
                }

            }
            if (CheckMotion(board, side) == true)
            {
                this.anchor.X += moving.dx;
                this.anchor.Y += moving.dy;

                if (side == MotionSide.Rotate)
                {
                    this.state = this.state.nextState;
                }
            }
        }
        public string color = "black";
        public Figure()
        {
            anchor = new Position(0, 0);
            state = new FigurePhase(new Board(0, 0), this.state);

        }
    }

    public enum MotionSide
    {
        Left,
        Bottom,
        Right,
        Rotate
    }

    public class Motion
    {
        public int dx;
        public int dy;

        public Motion(MotionSide side)
        {
            if (side == MotionSide.Left) { dx = -1; dy = 0; }
            if (side == MotionSide.Bottom) { dx = 0; dy = -1; }
            if (side == MotionSide.Right) { dx = 1; dy = 0; }
            if (side == MotionSide.Rotate) { dx = 0; dy = 0; }
        }
    }


    public class Position
    {
        public int X;
        public int Y;
        public Position(int x, int y) { X = x; Y = y; }
    }

    public class Board
    {
        public int width;
        public int height;
        public bool[,] matrix;
        public string[,] color;
        public Board(int w, int h)
        {
            width = w;
            height = h;
            matrix = new bool[w, h];
            color = new string[w, h];
        }

        public Board(bool[,] m)
        {
            width = m.GetLength(0);
            height = m.GetLength(1);
            matrix = m;
        }


    }








}
