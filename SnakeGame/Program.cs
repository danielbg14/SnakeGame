namespace SnakeGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            Console.CursorVisible = false;

            int x = 35;
            int y = 10;

            int playerX = 1;
            int playerY = 1;

            int appleX = rnd.Next(1, 34);
            int appleY = rnd.Next(1, 9);
            int score = 0;

            WallsBuild(x, y);

            while (true)
            {
                Console.SetCursorPosition(0, 12);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Score: {score}");
                Console.WriteLine("To stop the game, press Escape");
                Console.WriteLine("To restart the game, press R");

                Console.SetCursorPosition(appleX, appleY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("O");
                
                Console.SetCursorPosition(playerX, playerY);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("@");

                ConsoleKeyInfo key = Console.ReadKey(true);

                Console.SetCursorPosition(playerX, playerY);
                Console.Write(" ");

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        playerY--;
                        break;

                    case ConsoleKey.DownArrow:
                        playerY++;
                        break;

                    case ConsoleKey.LeftArrow:
                        playerX--;
                        break;

                    case ConsoleKey.RightArrow:
                        playerX++;
                        break;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        Console.SetCursorPosition(x / 2 - 5, 1);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Game over!");
                        Thread.Sleep(750);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(x / 2 - 7, 2);
                        Console.WriteLine($"Final score: {score}");
                        Console.ReadKey();
                        return;
                    case ConsoleKey.R:
                        Console.Clear();
                        Console.SetCursorPosition(x / 2 - 5, 1);
                        Console.WriteLine("Game Restarted!");
                        Thread.Sleep(1000);
                        Console.Clear();
                        WallsBuild(x, y);
                        playerX = 1;
                        playerY = 1;
                        appleX = rnd.Next(1, x - 1);
                        appleY = rnd.Next(1, y);
                        score = 0;
                        break;
                }

                bool hitWall =
                    playerX <= 0 ||
                    playerX >= x - 1 ||
                    playerY <= 0 ||
                    playerY >= y;

                if (hitWall)
                {
                    Console.Clear();
                    WallsBuild(x, y);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(x / 2 - 5, 5);
                    Console.WriteLine("GAME OVER!");
                    Console.SetCursorPosition(x / 2 - 7, 6);
                    Console.WriteLine($"Final score: {score}");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }

                if (playerX == appleX && playerY == appleY)
                {
                    appleX = rnd.Next(1, 34);
                    appleY = rnd.Next(1, 9);
                    score++;
                }
            }
        }
        static void WallsBuild(int x, int y)
        {
            Console.ForegroundColor= ConsoleColor.White;
            for (int row = 1; row <= x; row++)
            {
                Console.Write("*");
            }
            Console.WriteLine();

            for (int columns = 1; columns <= y; columns++)
            {
                Console.Write("*");
                for (int space = 1; space <= x - 2; space++)
                {
                    Console.Write(" ");
                }

                Console.Write("*");
                Console.WriteLine();

                if (columns == y)
                {
                    for (int row = 1; row <= x; row++)
                    {
                        Console.Write("*");
                    }
                }
            }
        }
    }
}