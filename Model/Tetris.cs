
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;




namespace TZ.Model
{
    public class Tetris
    {
        public int width;
        public int height;
        public Board board;
        public List<Figure> figures;

        public Tetris(int w = 8, int h = 15)
        {
            width = w;
            height = h;
            board = new Board(w, h);
            figures = new List<Figure>();
            figures.Add(new T1(5, 5));
            figures.Add(new T1(0, 3));
            figures.Add(new T1(1, 1));
        }

    }


    public class T1: Figure
    {
        public static FigurePhase phase1 = new FigurePhase(
            new Board(
                new bool[, ] {
                    {false, true, false, false},
                    {false, true, false, false},
                    {false, true, false, false},
                    {false, true, false, false},
                }
            ),
            phase2
        );

        public static FigurePhase phase2 = new FigurePhase(
            new Board(
                new bool[, ] {
                    {false, false, false, false},
                    { true,  true,  true,  true},
                    {false, false, false, false},
                    {false, false, false, false},
                }
            ),
            phase1
        );
        public T1 (int x, int y)
        {
            this.anchor = new Position(x, y);
            this.state = phase2;
            var random = new Random();
            this.color = string.Format("#{0:X6}", random.Next(65535) * 256);
        }
    }






    public class FigurePhase
    {
        public Board currentState;
        public FigurePhase nextState;
        public string color;
        public FigurePhase (Board b, FigurePhase next)
        {
            currentState = b;
            nextState = next;
        }
    }










    public  class Figure
    {
        public Position anchor;
        public FigurePhase state;
        public bool CheckRotation (Board board)
        {
            bool available = true;
            for (int x = 0; x < state.nextState.currentState.width; x++)
            {
                for (int y = 0; y < state.nextState.currentState.height; y++)
                {
                    if (board.matrix[x + anchor.X, y + anchor.Y] == true)
                    {
                        available = false;
                    }
                }

            }
            return available;
        }
        public void SetFigureToBoard (Board board)
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
            for (int x = 0; x < state.currentState.width; x++)
            {
                for (int y = 0; y < state.currentState.height; y++)
                {
                    if (state.currentState.matrix[x,y] == true 
                        && (x + anchor.X + motion.dx < 0
                        || x + anchor.X + motion.dx >= board.width
                        || y + anchor.Y + motion.dy < 0
                        || y + anchor.Y + motion.dy >= board.height) )
                    {
                        available = false;
                        return available;
                    }
                    if (state.currentState.matrix[x,y] == true 
                        && board.matrix[x + anchor.X + motion.dx, y + anchor.Y + motion.dy] == true)
                    {
                        available = false;
                        return available;
                    }
                }

            }
            return available;
        }
        public void DoMotion(Board board, MotionSide  side)
        {
            Motion moving = new Motion(side);
            if (CheckMotion(board, side) == true)
            {
                this.anchor.X += moving.dx;
                this.anchor.Y += moving.dy;
            }
        }
        public string color = "black";
    }

    public enum MotionSide
    {
        Left,
        Bottom,
        Right,
    }

    public class Motion
    {
        public int dx;
        public int dy;

        public Motion (MotionSide side)
        {
            if (side == MotionSide.Left) {dx = -1; dy = 0;}
            if (side == MotionSide.Bottom) {dx = 0; dy = -1;}
            if (side == MotionSide.Right) {dx = 1; dy = 0;}
        }
    }


    public class Position
    {
        public int X;
        public int Y;
        public Position(int x, int y) {X = x; Y = y;}
    }

    public class Board
    {
        public int width;
        public int height;
        public bool[, ] matrix;
        public Board(int w, int h)
        {
            width = w;
            height = h;
            matrix = new bool[w, h];
        }

        public Board(bool[,] m)
        {
            width = m.GetLength(0);
            height = m.GetLength(1);
            matrix = m;
        }


    }








}
