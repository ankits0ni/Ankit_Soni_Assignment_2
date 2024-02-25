using System;

namespace GemHuntersGame
{
    class GemHuntersGame
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Gem Hunters!");
                
            Game game = new Game();
            game.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
     
    class Location
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
     
    class Player
    {
        public string Name { get; }
        public Location Position { get; set; }
        public int GemCount { get; set; }

        public Player(string name, Location position)
        {
            Name = name;
            Position = position;
            GemCount = 0;
        }

        public void Move(char direction)
        {
            if (direction == 'U' && Position.Y > 0)
                Position = new Location(Position.X, Position.Y - 1);
            else if (direction == 'D' && Position.Y < 5)
                Position = new Location(Position.X, Position.Y + 1);
            else if (direction == 'L' && Position.X > 0)
                Position = new Location(Position.X - 1, Position.Y);
            else if (direction == 'R' && Position.X < 5)
                Position = new Location(Position.X + 1, Position.Y);
        }
    }
     
    class Cell
    {
        public string Contents { get; set; }

        public Cell(string contents = "-")
        {
            Contents = contents;
        }
    }

    class Board
    {
        public Cell[,] Grid { get; }
        private Random random;

        public Board()
        {
            Grid = new Cell[6, 6];
            random = new Random();

            // Initialize the grid with empty cells
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Grid[i, j] = new Cell();
                }
            }

            // Place gems (G)
            for (int i = 0; i < 6; i++)
            {
                int gemX = random.Next(6);
                int gemY = random.Next(6);
                Grid[gemY, gemX].Contents = "G";
            }

            // Place obstacles (O)
            for (int i = 0; i < 6; i++)
            {
                int obstacleX = random.Next(6);
                int obstacleY = random.Next(6);
                if (Grid[obstacleY, obstacleX].Contents != "G")
                {
                    Grid[obstacleY, obstacleX].Contents = "O";
                }
                else
                {
                    i--;
                }
            }
        }

        public void Display(Player player1, Player player2)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (player1.Position.X == j && player1.Position.Y == i)
                        Console.Write("P1 ");
                    else if (player2.Position.X == j && player2.Position.Y == i)
                        Console.Write("P2 ");
                    else
                        Console.Write(Grid[i, j].Contents + " ");
                }
                Console.WriteLine();
            }
        }

        public bool IsMoveValid(Player player, char direction)
        {
            int x = player.Position.X;
            int y = player.Position.Y;
            if (direction == 'U')
                y--;
            else if (direction == 'D')
                y++;
            else if (direction == 'L')
                x--;
            else if (direction == 'R')
                x++;

            if (x < 0 || x >= 6 || y < 0 || y >= 6)
                return false;
            if (Grid[y, x].Contents == "O")
            {
                Console.WriteLine("Obstacle detected");
                return false; // Cannot move onto a square with an obstacle
            }
            return true;
        }

        public void CollectGem(Player player)
        {
            int x = player.Position.X;
            int y = player.Position.Y;
            if (Grid[y, x].Contents == "G")
            {
                player.GemCount++;
                Grid[y, x].Contents = "-";
                Console.WriteLine($"{player.Name} collected a gem!");
            }
        }
    }

    class Game
    {
        public Board GameBoard { get; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public Player CurrentPlayer { get; set; }
        public int TotalMoves { get; set; }

        public Game()
        {
            GameBoard = new Board();
            Player1 = new Player("P1", new Location(0, 0));
            Player2 = new Player("P2", new Location(5, 5));
            CurrentPlayer = Player1;
            TotalMoves = 0;
        }

        public void Start()
        {
            while (!IsGameOver())
            {
                GameBoard.Display(Player1, Player2);
                Console.WriteLine($"{CurrentPlayer.Name}'s turn.");
                Console.WriteLine("Enter direction (U/D/L/R): ");
                char direction = ' '; // Initialize direction with a default value
                bool isValidInput = false;

                // Keep asking for input until a valid direction is entered
                while (!isValidInput)
                {
                    string input = Console.ReadLine();
                    if (input.Length == 1)
                    {
                        direction = char.ToUpper(input[0]); // Convert to uppercase
                        if (direction == 'U' || direction == 'D' || direction == 'L' || direction == 'R')
                            isValidInput = true;
                        else
                            Console.WriteLine("Invalid input. Please enter U, D, L, or R.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a single character (U, D, L, or R).");
                    }
                }

                if (GameBoard.IsMoveValid(CurrentPlayer, direction))
                {
                    Location previousLocation = new Location(CurrentPlayer.Position.X, CurrentPlayer.Position.Y);
                    CurrentPlayer.Move(direction); // Update player's position
                    GameBoard.CollectGem(CurrentPlayer);

                    // Update grid with new player position
                    GameBoard.Grid[previousLocation.Y, previousLocation.X].Contents = "-";
                    if (CurrentPlayer == Player1)
                        GameBoard.Grid[CurrentPlayer.Position.Y, CurrentPlayer.Position.X].Contents = "P1";
                    else
                        GameBoard.Grid[CurrentPlayer.Position.Y, CurrentPlayer.Position.X].Contents = "P2";

                    TotalMoves++;
                    SwitchPlayer();
                }
                else
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            }

            AnnounceWinner();
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == Player1) ? Player2 : Player1;
        }

        public bool IsGameOver()
        {
            return TotalMoves >= 30;
        }

        public void AnnounceWinner()
        {
            GameBoard.Display(Player1, Player2);
            if (Player1.GemCount > Player2.GemCount)
                Console.WriteLine("Player 1 wins!");
            else if (Player1.GemCount < Player2.GemCount)
                Console.WriteLine("Player 2 wins!");
            else
                Console.WriteLine("It's a tie!");
        }
    }
}
