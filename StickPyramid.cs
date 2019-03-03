using System;
using System.IO;

namespace StickPyramid
{
    enum Turn
    {
        ONE,
        TWO
    };

    class Program
    {
        static void Main(string[] args)
        {
            bool menuLoop = true;
            Console.WriteLine("Welcome to Pick up Sticks!");

            while (menuLoop == true)
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Play the game (Requires 2 players)");
                Console.WriteLine("2. Read the rules");
                Console.WriteLine("3. Exit");
                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        StartGame();
                        break;
                    case '2':
                        Console.Clear();
                        Console.WriteLine("Pick up Sticks is a 2 player game in which both players take it in turns to remove sticks from each row.");
                        Console.WriteLine("You can only takes sticks from one row at a time, and you have to take between 1-3 sticks each turn.");
                        Console.WriteLine("The player who picks up the final stick wins!");
                        break;
                    case '3':
                        menuLoop = false;
                        break;
                    default:
                        Console.Clear();
                        Console.Write("Welcome to Pick up Sticks!");
                        break;
                }
                Console.WriteLine();
            }
        }

        static void StartGame()
        {
            Turn player = new Turn();
            int currentLine = 0;
            bool win = false;

            //Set up the game board. If all of these are ' ', the game ends and the set player on Player wins.
            char[][] gameBoard = new char[5][]
            {
                new char[] { '|'},
                new char[] { '|', '|'},
                new char[] { '|', '|', '|' },
                new char[] { '|', '|', '|', '|' },
                new char[] { '|', '|', '|', '|', '|' },
            };

            //While someone hasn't won basically
            while (win == false)
            {
                {
                    PrintScreen(gameBoard, currentLine, player);
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.DownArrow:
                            //Arrow loop system
                            if (currentLine >= 4)
                            {
                                currentLine -= 4;
                            }
                            else
                            {
                                currentLine += 1;
                            }
                            PrintScreen(gameBoard, currentLine, player);
                            break;

                        case ConsoleKey.UpArrow:
                            //Arrow loop system
                            if (currentLine <= 0)
                            {
                                currentLine += 4;
                            }
                            else
                            {
                                currentLine -= 1;
                            }
                            PrintScreen(gameBoard, currentLine, player);
                            break;

                        case ConsoleKey.Enter:
                            int numberOfSticks = 0;
                            int numberOfPossibleSticks = 0;
                            bool numberLegitimate = false;

                            //When the player selects a line, they must enter a number. This checks if the number is legitimate first
                            while (numberLegitimate == false)
                            {
                                //Is it even a number?
                                Console.WriteLine("How many sticks would you like to pick up?");
                                try
                                {
                                    numberOfSticks = int.Parse(Console.ReadLine());
                                }
                                catch
                                {
                                    //Nah fam
                                    PrintScreen(gameBoard, currentLine, player);
                                    Console.WriteLine("That is not a valid number.");
                                    continue;
                                }

                                //Acts as an escape if we accidentally selected an empty row
                                if (numberOfSticks == 0)
                                {
                                    break;
                                }

                                //Checks how many sticks are currently in the selected row
                                foreach (char chara in gameBoard[currentLine])
                                {
                                    if (chara == '|')
                                    {
                                        numberOfPossibleSticks++;
                                    }
                                }

                                //Check if we can take that many sticks from this row
                                if (numberOfSticks > numberOfPossibleSticks || numberOfSticks < 1 || numberOfSticks > 3)
                                {
                                    PrintScreen(gameBoard, currentLine, player);
                                    Console.WriteLine("Please pick a number between 1 and 3 that is valid.");
                                }
                                else
                                {
                                    numberLegitimate = true;
                                }
                            }
                            
                            RemoveSticks(ref gameBoard, currentLine, numberOfSticks);
                            PrintScreen(gameBoard, currentLine, player);
                            win = CheckForWin(gameBoard);

                            //If there's a win, we quit here
                            if (win == true)
                            {
                                Console.WriteLine("PLAYER " + player + " WINS!");
                            }

                            //This is in case the player picks 0 and thus doesn't use their turn
                            if (numberOfSticks != 0)
                            {
                                if (player == Turn.ONE)
                                {
                                    player = Turn.TWO;
                                }
                                else
                                {
                                    player = Turn.ONE;
                                }
                            }

                            break;
                    }
                }
            }
        }

        static bool CheckForWin (char[][] pGameBoard)
        {
            //Checks every character in the board until a | is found. If no | is found, it returns true for a win
            foreach (char[] row in pGameBoard)
            {
                foreach (char item in row)
                {
                    if (item == '|')
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static void RemoveSticks(ref char[][] pGameBoard, int pCurrentLine, int pNoOfSticks)
        {
            for (int i = 0; i < pNoOfSticks; i++)
            {
                //If sticks have already been removed, we need to remove the next ones along instead
                if (pGameBoard[pCurrentLine][i] != ' ')
                {
                    pGameBoard[pCurrentLine][i] = ' ';
                }
                else
                {
                    pNoOfSticks++;
                }
            }
        }

        static void PrintScreen(char[][] pGameBoard, int pCurrentLine, Turn pCurrentPlayer)
        {
            Console.Clear();
            Console.WriteLine("PLAYER " + pCurrentPlayer.ToString() + "'S TURN");
            for (int i = 0; i < pGameBoard.Length; i++)
            {
                Console.Write((i + 1) + "|");
                for (int j = 0; j < pGameBoard[i].Length; j++)
                {
                    if (j == 0)
                    {
                        for (int k = 5; k > i; k--)
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.Write(" ");
                    if (pGameBoard[i][j] == ' ')
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write("|");
                    }
                }
                if (pCurrentLine == i)
                {
                    Console.Write(" <");
                }
                Console.WriteLine();
            }
        }
    }
}
