using static System.Console;

namespace AP8PO_Had
{
    class Program
    {
        static void Main()
        {
            Game game = new Game(64, 24);
            game.Run();
        }
    }
    
    class Game
    {
        private int screenWidth, screenHeight, score;
        private bool isGameOver;
        private Snake snake;
        private Berry berry;
        private Random random;

        public Game(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
            score = 5;
            isGameOver = false;
            random = new Random();
            snake = new Snake(new Point(screenWidth / 2, screenHeight / 2));
            berry = new Berry(new Point(random.Next(1, screenWidth - 2), random.Next(1, screenHeight - 2)));

            try
            {
                SetWindowSize(screenWidth, screenHeight);
                BufferWidth = screenWidth;
                BufferHeight = screenHeight;
            }
            catch
            {
                WriteLine("Warning: Cannot resize console window.");
            }
        }

        public void Run()
        {
            while (!isGameOver)
            {
                Clear();
                ConsoleRenderer.DrawBorder(screenWidth, screenHeight);
                CheckCollisions();
                snake.Move();
                snake.Draw();
                berry.Draw();

                int speed = Math.Max(50, 150 - (score * 5));
                Thread.Sleep(speed);
            }
            DisplayGameOverAd();
        }

        private void CheckCollisions()
        {
            if (snake.HasCollided(screenWidth, screenHeight))
                isGameOver = true;
            if (snake.Head.Equals(berry.Position))
            {
                score++;
                berry = new Berry(new Point(random.Next(1, screenWidth - 2), random.Next(1, screenHeight - 2)));
                snake.Grow();
            }
        }

        private void DisplayGameOverAd()
        {
            SetCursorPosition(screenWidth / 3, screenHeight / 2);
            WriteLine($"Game Over! Score: {score}");
            
            SetCursorPosition(screenWidth / 4, screenHeight / 2 + 2);
            WriteLine("*************************************");
            SetCursorPosition(screenWidth / 4, screenHeight / 2 + 3);
            WriteLine("The best games at throwtable.com (╯°□°)╯︵ ┻━┻");
            SetCursorPosition(screenWidth / 4, screenHeight / 2 + 4);
            WriteLine("*************************************");
    
            SetCursorPosition(screenWidth / 4, screenHeight / 2 + 6);
            WriteLine("Press any key to exit...");
            ReadKey();
        }

    }

    class Snake
    {
        private List<Point> body;
        private Direction movement;
        private bool grew;

        public Point Head => body[^1];

        public Snake(Point start)
        {
            body = new List<Point> { start };
            movement = Direction.Right;
            grew = false;
        }

        public void Move()
        {
            movement = InputHandler.ReadMovement(movement);
            Point newHead = movement switch
            {
                Direction.Up => new Point(Head.X, Head.Y - 1),
                Direction.Down => new Point(Head.X, Head.Y + 1),
                Direction.Left => new Point(Head.X - 1, Head.Y),
                Direction.Right => new Point(Head.X + 1, Head.Y),
                _ => Head
            };
            body.Add(newHead);
            if (!grew) body.RemoveAt(0);
            else grew = false;
        }

        public void Grow() => grew = true;

        public bool HasCollided(int width, int height)
        {
            return Head.X == 0 || Head.X == width - 1 || Head.Y == 0 || Head.Y == height - 1 || body.Take(body.Count - 1).Contains(Head);

        }

        public void Draw()
        {
            foreach (var part in body)
            {
                ConsoleRenderer.Draw(part, ConsoleColor.Green, "■");
            }
        }
    }

    abstract class ConsoleRenderer
    {
        public static void DrawBorder(int width, int height)
        {
            ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < width; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");
                SetCursorPosition(i, height - 1);
                Write("■");
            }
            for (int i = 0; i < height; i++)
            {
                SetCursorPosition(0, i);
                Write("■");
                SetCursorPosition(width - 1, i);
                Write("■");
            }
        }

        public static void Draw(Point point, ConsoleColor color, string symbol)
        {
            ForegroundColor = color;
            SetCursorPosition(point.X, point.Y);
            Write(symbol);
        }
    }

    abstract class InputHandler
    {
        public static Direction ReadMovement(Direction movement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && movement != Direction.Down) return Direction.Up;
                if (key == ConsoleKey.DownArrow && movement != Direction.Up) return Direction.Down;
                if (key == ConsoleKey.LeftArrow && movement != Direction.Right) return Direction.Left;
                if (key == ConsoleKey.RightArrow && movement != Direction.Left) return Direction.Right;
            }
            return movement;
        }
    }

    enum Direction
    {
        Up, Down, Right, Left
    }
    
    struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Point point && X == point.X && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
    
    class Berry
    {
        public Point Position { get; }

        public Berry(Point position)
        {
            Position = position;
        }

        public void Draw()
        {
            ConsoleRenderer.Draw(Position, ConsoleColor.Cyan, "■");
        }
    }
}
