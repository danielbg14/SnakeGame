using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    internal class Program
    {
        const int Width = 35;
        const int Height = 11;

        static readonly Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            try
            {
                bool restart;

                do
                {
                    restart = PlayGame();
                }
                while (restart);
            }
            finally
            {
                Console.ResetColor();
                Console.CursorVisible = true;
                Console.Clear();
            }
        }

        static bool PlayGame()
        {
            Console.Clear();

            List<(int X, int Y)> snake = new List<(int X, int Y)>
            {
                (Width / 2, Height / 2),
                (Width / 2 - 1, Height / 2),
                (Width / 2 - 2, Height / 2)
            };

            int dx = 1;
            int dy = 0;
            int score = 0;
            int delay = 150;

            (int X, int Y) apple = CreateApple(snake);

            DrawWalls();
            DrawSnake(snake);
            DrawApple(apple);
            DrawInfo(score);

            while (true)
            {
                bool directionChanged = false;

                // Read available keys without blocking movement.
                while (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape)
                        return false;

                    if (key == ConsoleKey.R)
                        return true;

                    if (!directionChanged)
                    {
                        directionChanged =
                            ChangeDirection(key, ref dx, ref dy);
                    }
                }

                Thread.Sleep(delay);

                var head = snake[0];
                var nextHead = (X: head.X + dx, Y: head.Y + dy);

                bool eatingApple =
                    nextHead == apple;

                // Check for wall collisions.
                if (nextHead.X <= 0 ||
                    nextHead.X >= Width - 1 ||
                    nextHead.Y <= 0 ||
                    nextHead.Y >= Height - 1)
                {
                    break;
                }

                // When not eating, the tail moves away.
                // Therefore, moving into the old tail position is allowed.
                int bodyLengthToCheck = eatingApple
                    ? snake.Count
                    : snake.Count - 1;

                bool hitSelf = false;

                for (int i = 0; i < bodyLengthToCheck; i++)
                {
                    if (snake[i] == nextHead)
                    {
                        hitSelf = true;
                        break;
                    }
                }

                if (hitSelf)
                    break;

                // Move the snake's head.
                snake.Insert(0, nextHead);

                // Draw the new head.
                Console.SetCursorPosition(nextHead.X, nextHead.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("@");

                if (eatingApple)
                {
                    score++;

                    // Increase speed gradually, with a limit.
                    delay = Math.Max(55, 150 - score * 5);

                    apple = CreateApple(snake);
                    DrawApple(apple);
                    DrawInfo(score);
                }
                else
                {
                    // Remove the tail to maintain the current length.
                    var tail = snake[snake.Count - 1];
                    snake.RemoveAt(snake.Count - 1);

                    Console.SetCursorPosition(tail.X, tail.Y);
                    Console.Write(" ");
                }
            }

            return ShowGameOver(score);
        }

        static bool ChangeDirection(
            ConsoleKey key, ref int dx, ref int dy)
        {
            int newDx = dx;
            int newDy = dy;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    newDx = 0;
                    newDy = -1;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    newDx = 0;
                    newDy = 1;
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    newDx = -1;
                    newDy = 0;
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    newDx = 1;
                    newDy = 0;
                    break;

                default:
                    return false;
            }

            // Prevent reversing directly into the snake's body.
            if (newDx == -dx && newDy == -dy)
                return false;

            dx = newDx;
            dy = newDy;

            return true;
        }

        static (int X, int Y) CreateApple(
            List<(int X, int Y)> snake)
        {
            while (true)
            {
                int x = rnd.Next(1, Width - 1);
                int y = rnd.Next(1, Height - 1);

                bool occupied = false;

                foreach (var segment in snake)
                {
                    if (segment.X == x && segment.Y == y)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied)
                    return (x, y);
            }
        }

        static void DrawWalls()
        {
            Console.ForegroundColor = ConsoleColor.White;

            for (int x = 0; x < Width; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("*");

                Console.SetCursorPosition(x, Height - 1);
                Console.Write("*");
            }

            for (int y = 1; y < Height - 1; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("*");

                Console.SetCursorPosition(Width - 1, y);
                Console.Write("*");
            }
        }

        static void DrawSnake(List<(int X, int Y)> snake)
        {
            for (int i = 0; i < snake.Count; i++)
            {
                Console.SetCursorPosition(snake[i].X, snake[i].Y);

                Console.ForegroundColor = i == 0
                    ? ConsoleColor.Green
                    : ConsoleColor.DarkGreen;

                Console.Write(i == 0 ? "@" : "o");
            }
        }

        static void DrawApple((int X, int Y) apple)
        {
            Console.SetCursorPosition(apple.X, apple.Y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("O");
        }

        static void DrawInfo(int score)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(0, Height + 1);
            Console.Write($"Score: {score}                 ");

            Console.SetCursorPosition(0, Height + 2);
            Console.Write("Arrows/WASD: Move | R: Restart");

            Console.SetCursorPosition(0, Height + 3);
            Console.Write("Escape: Quit                  ");
        }

        static bool ShowGameOver(int score)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(Width / 2 - 5, Height + 5);
            Console.Write("GAME OVER!");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Width / 2 - 7, Height + 6);
            Console.Write($"Final score: {score}");

            Console.SetCursorPosition(Width / 2 - 11, Height + 8);
            Console.Write("Press R to restart");

            Console.SetCursorPosition(Width / 2 - 10, Height + 9);
            Console.Write("Press Esc to quit");

            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.R)
                    return true;

                if (key == ConsoleKey.Escape)
                    return false;
            }
        }
    }
}