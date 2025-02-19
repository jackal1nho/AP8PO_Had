using static System.Console;


namespace AP8PO_Had
{
    class Program
    {
        static void Main()
        {
            Game game = new Game(32, 16);
            game.Run();
        }
    }

    class Game
    {
        private int screenWidth;
        private int screenHeight;
        private int score;
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
            snake = new Snake(screenWidth / 2, screenHeight / 2);
            berry = new Berry(random.Next(1, screenWidth - 2), random.Next(1, screenHeight - 2));
            WindowHeight = screenHeight;
            WindowWidth = screenWidth;
        }

        public void Run()
        {
            while (!isGameOver)
            {
                Clear();
                DrawBorders();
                CheckCollisions();
                DrawObjects();
                snake.Move();
                Thread.Sleep(150);
            }

            DisplayGameOver();
        }

        private void DrawBorders()
        {
            ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < screenWidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");
                SetCursorPosition(i, screenHeight - 1);
                Write("■");
            }

            for (int i = 0; i < screenHeight; i++)
            {
                SetCursorPosition(0, i);
                Write("■");
                SetCursorPosition(screenWidth - 1, i);
                Write("■");
            }
        }

        private void CheckCollisions()
        {
            if (snake.HasCollided(screenWidth, screenHeight))
                isGameOver = true;
            if (snake.xHeadPos == berry.XPos && snake.yHeadPos == berry.YPos)
            {
                score++;
                berry = new Berry(random.Next(1, screenWidth - 2), random.Next(1, screenHeight - 2));
                snake.Grow();
            }
        }

        private void DrawObjects()
        {
            snake.Draw();
            berry.Draw();
        }

        private void DisplayGameOver()
        {
            SetCursorPosition(screenWidth / 5, screenHeight / 2);
            WriteLine($"Game over, Score: {score}");
            SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
        }
    }

    class Snake
    {
        private List<int> xBodyPos;
        private List<int> yBodyPos;
        private Direction movement;

        public int xHeadPos => xBodyPos[^1];
        public int yHeadPos => yBodyPos[^1];

        public Snake(int startX, int startY)
        {
            xBodyPos = new List<int> { startX };
            yBodyPos = new List<int> { startY };
            movement = Direction.Right;
        }

        public void Move()
        {
            movement = InputHandler.ReadMovement(movement);
            xBodyPos.Add(xHeadPos);
            yBodyPos.Add(yHeadPos);

            switch (movement)
            {
                case Direction.Up:
                    yBodyPos[^1]--;
                    break;
                case Direction.Down:
                    yBodyPos[^1]++;
                    break;
                case Direction.Left:
                    xBodyPos[^1]--;
                    break;
                case Direction.Right:
                    xBodyPos[^1]++;
                    break;
            }
        }

        public void Grow()
        {
            xBodyPos.Insert(0, xBodyPos[0]);
            yBodyPos.Insert(0, yBodyPos[0]);
        }

        public bool HasCollided(int width, int height)
        {
            if (xHeadPos == 0 || xHeadPos == width - 1 || yHeadPos == 0 || yHeadPos == height - 1)
                return true;
            for (int i = 0; i < xBodyPos.Count - 1; i++)
            {
                if (xBodyPos[i] == xHeadPos && yBodyPos[i] == yHeadPos)
                    return true;
            }

            return false;
        }

        public void Draw()
        {
            ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < xBodyPos.Count; i++)
            {
                SetCursorPosition(xBodyPos[i], yBodyPos[i]);
                Write("■");
            }
        }
    }

    class InputHandler
    {
        public static Direction ReadMovement(Direction movement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && movement != Direction.Down)
                    return Direction.Up;
                else if (key == ConsoleKey.DownArrow && movement != Direction.Up)
                    return Direction.Down;
                else if (key == ConsoleKey.LeftArrow && movement != Direction.Right)
                    return Direction.Left;
                else if (key == ConsoleKey.RightArrow && movement != Direction.Left)
                    return Direction.Right;
            }

            return movement;
        }
    }

    enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    class Berry
    {
        public int XPos { get; }
        public int YPos { get; }

        public Berry(int xPos, int yPos)
        {
            XPos = xPos;
            YPos = yPos;
        }

        public void Draw()
        {
            SetCursorPosition(XPos, YPos);
            ForegroundColor = ConsoleColor.Cyan;
            Write("■");
        }
    }
}