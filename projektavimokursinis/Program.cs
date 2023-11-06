using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace flapbird
{
    internal static class Program
    {
        private static int ROWS = 12;
        private static int COLUMNS = 11;
        public static int[][] gameGrid;
        public static bool gameOver;
        private static int score;

        private static int gapBetweenPipes = 5;

        private static List<Pipe> pipes;
        private static Bird bird;

        public static void Main(string[] args)
        {
            InitializeVariables();
            AddBirdToGameGrid();
            QueueThreads();
            
            int counter = gapBetweenPipes;
            while (!gameOver)
            {
                UpdatePipes();
                if (counter % gapBetweenPipes == 0)
                {
                    Pipe pipe = new Pipe(COLUMNS, ROWS);
                    pipes.Add(pipe);
                    AddPipeToGrid(pipe);
                }
                CalculateScore();
                PrintScreen();
                Thread.Sleep(400);
                counter++;
            }

            Console.WriteLine("Game over!!!");
        }

        private static void InitializeVariables()
        {
            pipes = new List<Pipe>();
            gameGrid = new int[ROWS][];
            for (int i = 0; i < ROWS; i++)
            {
                gameGrid[i] = new int[COLUMNS];
            }

            bird = new Bird(ROWS, COLUMNS);
        }

        private static void QueueThreads()
        {
            ThreadPool.QueueUserWorkItem(BirdGravity, new CancellationTokenSource());
            ThreadPool.QueueUserWorkItem(BirdControl, new CancellationTokenSource());
        }

        private static void CalculateScore()
        {
            foreach (var pipe in pipes)
            {
                if (pipe.XPosition == bird.XPosition - 1) 
                    score++;
            }

        }

        private static void AddBirdToGameGrid()
        {
            gameGrid[bird.YPosition][bird.XPosition] = (int)GridEnumeration.Bird;
        }

        private static void UpdatePipes()
        {
            ClearPipesFromGrid();
            for (int i = pipes.Count - 1; i >= 0; i--)
            {
                var pipe = pipes[i];

                if (pipe.XPosition == 0)
                {
                    pipes.RemoveAt(i);
                }
                else
                {
                    pipe.ShiftLeft();
                    AddPipeToGrid(pipe);
                }
            }

            if (Collision())
            {
                RemoveBirdFromGrid();
                gameGrid[bird.YPosition][bird.XPosition] = (int)GridEnumeration.Pipe;
                gameOver = true;
            }
        }

        private static void ClearPipesFromGrid()
        {
            for (int i = 0; i < gameGrid.Length; i++)
            {
                for (int j = 0; j < gameGrid[i].Length; j++)
                {
                    if (gameGrid[i][j] == (int)GridEnumeration.Pipe) gameGrid[i][j] = (int)GridEnumeration.None;
                }
            }
        }


        private static bool Collision()
        {
            foreach (var pipe in pipes)
            {
                if (pipe.XPosition == bird.XPosition && !IsBirdInGap(pipe)) return true;
            }

            return false;
        }

        private static bool IsBirdInGap(Pipe pipe)
        {
            int gapIntervalStart = pipe.GapTopY;
            int gapIntervalEnd = pipe.GapTopY + Pipe.GapHeight - 1;
            return bird.YPosition >= gapIntervalStart && bird.YPosition <= gapIntervalEnd;
        }

        public static void PrintScreen()
        {
            Console.Clear();
            StringBuilder output = new StringBuilder();
            output.AppendLine("-------FLAPPY BIRD------");
            output.Append(' ');
            output.AppendLine(string.Concat(Enumerable.Repeat("_ ", COLUMNS)));
            for (var i = 0; i < gameGrid.Length; i++)
            {
                output.Append('|');
                for (var j = 0; j < gameGrid[i].Length; j++)
                {
                    if (gameGrid[i][j] == 0)
                        if (i == ROWS - 1)
                            output.Append("_ ");
                        else
                            output.Append("  ");
                    else
                        output.Append(gameGrid[i][j] + " ");
                }

                output.AppendLine("|");
            }

            output.AppendLine("Score: " + score);
            Console.Write(output.ToString());
        }

        public static void AddPipeToGrid(Pipe pipe)
        {
            int gapIntervalStart = pipe.GapTopY;
            int gapIntervalEnd = gapIntervalStart + Pipe.GapHeight;
            for (int i = 0; i < ROWS; i++)
            {
                if (i >= gapIntervalStart && i < gapIntervalEnd)
                    gameGrid[i][pipe.XPosition] = (int) GridEnumeration.None;
                else
                    gameGrid[i][pipe.XPosition] = (int) GridEnumeration.Pipe;
            }
        }

        public static void BirdGravity(object? obj)
        {
            while (!gameOver)
            {
                RemoveBirdFromGrid();
                bird.MoveDown();
                if (bird.YPosition > 10 || Collision())
                {
                    gameOver = true;
                } 
                else 
                {
                    AddBirdToGameGrid();
                }

                PrintScreen();
                Thread.Sleep(250);
            }
        }
        
        public static void BirdControl(object? obj)
        {
            ConsoleKeyInfo keyInfo;
            while (!gameOver)
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    if (bird.YPosition > 0)
                    {
                        RemoveBirdFromGrid();
                        bird.MoveUp();
                        if (Collision())
                        {
                            gameOver = true;
                            return;
                        }

                        AddBirdToGameGrid();
                        PrintScreen();
                    }
                    Thread.Sleep(50);
                    if (bird.YPosition > 0)
                    {
                        RemoveBirdFromGrid();
                        bird.MoveUp();
                        if (Collision())
                        {
                            gameOver = true;
                            return;
                        }
                        AddBirdToGameGrid();
                        PrintScreen();
                    }
                }
            }
        }
        private static void RemoveBirdFromGrid()
        {
            gameGrid[bird.YPosition][bird.XPosition] = (int)GridEnumeration.None;
        }
    }
}