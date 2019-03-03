using System;

namespace MineSweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        static void MainMenu()
        {
            Console.Clear();
            bool startGame = false;
            int boardSize = 0;
            int numbOfBombs = 0;

            //Makes sure the boardsize is not too large/small and that the number of bombs is equally not too large/small.
            while (startGame == false)
            {
                Console.WriteLine("What size board would you like to play on? (Max size is 25x25. Min size is 5x5)");
                try
                {
                    boardSize = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("The board size given was not a number, please try again.");
                    Console.WriteLine();
                    continue;
                }

                if (boardSize < 5 || boardSize > 25)
                {
                    Console.Clear();
                    Console.WriteLine("The board size was outside the given parameters, please try again.");
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine("How many bombs would you like to have? (Min number is " + boardSize + ". Max number is " + boardSize * 5 + ")");
                try
                {
                    numbOfBombs = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("The number of bombs given was not a number, please try again.");
                    Console.WriteLine();
                    continue;
                }

                if (numbOfBombs < boardSize || numbOfBombs > boardSize * 5)
                {
                    Console.Clear();
                    Console.WriteLine("The number of bombs was outside the given parameters, please try again.");
                    Console.WriteLine();
                    continue;
                }

                startGame = true;
            }

            SetupBoard(boardSize, numbOfBombs);
        }

        static void SetupBoard(int pBoardSize, int pNumbOfBombs)
        {
            int numbOfBombsNearby = 0;
            int tempX = 0;
            int tempY = 0;
            Tile[,] gameBoard = new Tile[pBoardSize, pBoardSize];
            Random rand = new Random();

            //Randomly places bombs.
            for (int tempNumbOfBombs = 0; tempNumbOfBombs < pNumbOfBombs; tempNumbOfBombs++)
            {
                tempX = rand.Next(0, pBoardSize);
                tempY = rand.Next(0, pBoardSize);
                if (gameBoard[tempX, tempY].bomb == false)
                {
                    gameBoard[tempX, tempY].bomb = true;
                }
                else
                {
                    tempNumbOfBombs--;
                }
            }

            //Assign each tile a number based on the number of bombs around it.
            for (int i = 0; i < pBoardSize; i++)
            {
                for (int j = 0; j < pBoardSize; j++)
                {
                    if (gameBoard[i, j].bomb == false)
                    {
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int l = j - 1; l < j + 2; l++)
                            {
                                try
                                {
                                    if (k != i || l != j)
                                    {
                                        if (gameBoard[k, l].bomb == true)
                                        {
                                            numbOfBombsNearby++;
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }

                        gameBoard[i, j].tileNumber = numbOfBombsNearby;
                        numbOfBombsNearby = 0;
                    }
                }
            }

            StartGame(gameBoard, pNumbOfBombs, pBoardSize);
        }

        static void StartGame(Tile[,] pGameBoard, int pNumbOfBombs, int pGameBoardSize)
        {
            int pointerX = 0;
            int pointerY = 0;
            bool gameOver = false;
            bool gameWon = false;

            do
            {
                gameWon = true;
                PrintBoard(pGameBoard, pGameBoardSize, pointerX, pointerY, ref pNumbOfBombs);

                //Control inputs
                /* The arrows move the cursor up, down, left and right.
                 * Pressing enter reveals the current tile.
                 * Pressing F places or removes a flag from the current tile.
                 */
                switch (Console.ReadKey().Key)
                {
                    //The 4 arrow switch-case statements for movement with boundary protection.
                    case ConsoleKey.UpArrow:
                        if (pointerY > 0)
                        {
                            pointerY--;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (pointerY < pGameBoardSize - 1)
                        {
                            pointerY++;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (pointerX > 0)
                        {
                            pointerX--;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (pointerX < pGameBoardSize - 1)
                        {
                            pointerX++;
                        }
                        break;

                    case ConsoleKey.Enter:
                        if (pGameBoard[pointerX, pointerY].tileState != TileState.Flagged)
                        {
                            //If the current tile is a bomb, you lose the game.
                            if (pGameBoard[pointerX, pointerY].bomb == true)
                            {
                                pGameBoard[pointerX, pointerY].tileState = TileState.Bomb;
                                gameOver = true;
                                for (int i = 0; i < pGameBoardSize; i++)
                                {
                                    for (int j = 0; j < pGameBoardSize; j++)
                                    {
                                        if (pGameBoard[i, j].bomb == true)
                                        {
                                            pGameBoard[i, j].tileState = TileState.Bomb;
                                        }
                                    }
                                }

                                PrintBoard(pGameBoard, pGameBoardSize, pointerX, pointerY, ref pNumbOfBombs);

                                Console.WriteLine("Game over!");
                            }
                            //If the current tile is a 0, it is a clear tile with clear neigbours.
                            //ClearNearbySpaces clears all surrounding spaces recursively until no clear spaces are left.
                            else if (pGameBoard[pointerX, pointerY].tileNumber == 0)
                            {
                                pGameBoard[pointerX, pointerY].tileState = TileState.Clear;
                                ClearNearbySpaces(ref pGameBoard, pointerX, pointerY, ref pNumbOfBombs);
                            }
                            //If the tile is not any of the things before, it must be a non-0 number.
                            else
                            {
                                pGameBoard[pointerX, pointerY].tileState = TileState.Number;
                            }
                        }
                        break;

                    case ConsoleKey.F:
                        //Sets a flag on the current tile if no flag is present, and vice versa.
                        if (pGameBoard[pointerX, pointerY].tileState == TileState.Unknown || pGameBoard[pointerX, pointerY].tileState == TileState.Flagged)
                        {
                            if (pGameBoard[pointerX, pointerY].tileState == TileState.Flagged)
                            {
                                pGameBoard[pointerX, pointerY].tileState = TileState.Unknown;
                                pNumbOfBombs++;
                            }
                            else
                            {
                                pGameBoard[pointerX, pointerY].tileState = TileState.Flagged;
                                pNumbOfBombs--;
                            }
                        }
                        break;
                }
                //Checks to see if any tiles are still set to Unknown.
                //If no tiles are set to Unknown, then the board has been cleared and the game has been won.
                for (int i = 0; i < pGameBoardSize; i++)
                {
                    for (int j = 0; j < pGameBoardSize; j++)
                    {
                        if (pGameBoard[i, j].bomb == false && pGameBoard[i,j].tileState == TileState.Unknown)
                        {
                            gameWon = false;
                        }
                    }
                }

                if (gameWon == true)
                {
                    gameOver = true;
                }
            }
            while (gameOver == false);

            //An infinite while is usually bad practice, but I like it here..
            while (true)
            {
                PrintBoard(pGameBoard, pGameBoardSize, pointerX, pointerY, ref pNumbOfBombs);
                if (gameWon == true)
                {
                    Console.WriteLine("Well done! You won!");
                }

                Console.WriteLine("Would you like to play again? (Press Y for yes or N for no)");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        MainMenu();
                        break;
                    case ConsoleKey.N:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        //A recursive function that checks each tile one by one and its surrounding neigbours, after which it marks the tiles as being checked so it doesn't loop infinitely.
        static void ClearNearbySpaces(ref Tile[,] pGameBoard, int pX, int pY, ref int pNumbOfBombs)
        {
            //These two for loops check the surrounding 8 tiles of the current tile.
            for (int i = pX - 1; i < pX + 2; i++)
            {
                for (int j = pY - 1; j < pY + 2; j++)
                {
                    //In case the surrounding tiles don't exist (such as on a corner) we can use a try statement to ignore the exceptions.
                    try
                    {
                        if (pGameBoard[i, j].spaceChecked == false)
                        {
                            pGameBoard[i, j].spaceChecked = true;

                            //If any flagged tiles are cleared, we add the bombs back here.
                            if (pGameBoard[i, j].tileState == TileState.Flagged)
                            {
                                pNumbOfBombs++;
                            }

                            //If the tile is clear, the function recurs.
                            if (pGameBoard[i, j].bomb == false && pGameBoard[i, j].tileNumber == 0)
                            {
                                pGameBoard[i, j].tileState = TileState.Clear;
                                ClearNearbySpaces(ref pGameBoard, i, j, ref pNumbOfBombs);
                            }
                            //If it's a non-0 number tile, it just reveals it and moves on.
                            else if (pGameBoard[i, j].bomb == false && pGameBoard[i, j].tileNumber != 0)
                            {
                                pGameBoard[i, j].tileState = TileState.Number;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        static void PrintBoard(Tile[,] pGameBoard, int pGameBoardSize, int pPointerX, int pPointerY, ref int pNumbOfBombsLeft)
        {
            Console.Clear();

            Console.WriteLine("Number of bombs left: " + pNumbOfBombsLeft);

            for (int i = 0; i < pGameBoardSize; i++)
            {
                for (int j = 0; j < pGameBoardSize; j++)
                {
                    switch (pGameBoard[j, i].tileState)
                    {
                        case TileState.Unknown:
                            if (j == pPointerX && i == pPointerY)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                            }
                            Console.Write("#");
                            break;
                        case TileState.Flagged:
                            if (j == pPointerX && i == pPointerY)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                            }
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("!");
                            break;
                        case TileState.Number:
                            if (j == pPointerX && i == pPointerY)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                            }

                            switch (pGameBoard[j, i].tileNumber)
                            {
                                case 1:
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                                case 2:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case 3:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 4:
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    break;
                                case 5:
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    break;
                                case 6:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 7:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 8:
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    break;
                            }
                            Console.Write(pGameBoard[j, i].tileNumber);
                            break;
                        case TileState.Clear:
                            if (j == pPointerX && i == pPointerY)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                            }
                            Console.Write(" ");
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        case TileState.Bomb:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("@");
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }

    struct Tile
    {
        public bool bomb;
        public bool spaceChecked;
        public TileState tileState;
        public int tileNumber;

        public Tile(bool bomb, TileState tileState, int tileNumber)
        {
            spaceChecked = false;
            this.bomb = bomb;
            this.tileState = tileState;
            this.tileNumber = tileNumber;
        }
    }

    enum TileState
    {
        Unknown,
        Flagged,
        Bomb,
        Clear,
        Number
    };
}
