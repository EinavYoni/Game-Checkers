using System;
using System.Collections.Generic;
using GameLogic;

namespace UserInterface
{
    public class UI
    {
        private GameManager m_GameManager;
        private const char k_Quit = 'Q';

        public void Start()
        {
            const bool k_WantMoreGame = true;
            const bool k_GameRunning = true;
            char? charUserInput = null;
            string stringUserInput, player1Name, player2Name;
            int chosenOpponent, boardSize;
            bool isWantMoreGame, isGameRunning, isPlayerCanEat, isVsHuman = true;
            
            player1Name = insertUserName();
            boardSize = insertBoardSize();
            chosenOpponent = insertChooseOpponent();
            player2Name = getPlayer2Info(ref isVsHuman, chosenOpponent);
            m_GameManager = new GameManager(boardSize, boardSize, !isVsHuman, player1Name, player2Name);
            do
            {
                isGameRunning = k_GameRunning;
                printBoard(m_GameManager.BoardManager);
                printPlayerNameTurnAndLastMove();
                stringUserInput = getMoveInput(out charUserInput);
                m_GameManager.AddAllMovesToList(out isPlayerCanEat);
                while (!isChoseToQuit(charUserInput) && isGameRunning)
                {
                    if (!m_GameManager.IsAiTurn())
                    {
                        reInsertValidMoveIfNeeded(m_GameManager.GetPlayerMovesList(), ref stringUserInput, ref charUserInput);
                    }
                        
                    m_GameManager.MovePlayer(stringUserInput, ref isPlayerCanEat);
                    printBoard(m_GameManager.BoardManager);
                    if (!isPlayerCanEat)
                    {
                        isGameRunning = !m_GameManager.IsGameOver();
                        if (isGameRunning)
                        {
                            m_GameManager.UpdateTurns();
                            printPlayerNameTurnAndLastMove();
                            m_GameManager.AddAllMovesToList(out isPlayerCanEat);
                            stringUserInput = getInput(ref charUserInput);
                        }
                    }
                    else
                    {
                        printPlayerNameTurnAndLastMove();
                        stringUserInput = getInput(ref charUserInput);
                    }    
                }

                if (m_GameManager.IsThereWinner())
                {
                    m_GameManager.UpdateScore();
                }

                printBoard(m_GameManager.BoardManager);
                m_GameManager.PrintPlayersScore();
                if (insertWhetherPlayerWantMoreGame())
                {
                    m_GameManager.ResetGame();
                    stringUserInput = null;
                    charUserInput = null;
                    isWantMoreGame = k_WantMoreGame;
                }
                else
                {
                    isWantMoreGame = !k_WantMoreGame;
                }
            }
            while (isWantMoreGame);
        }

        private string getInput(ref char? io_CharUserInput)
        {
            string stringUserInput;

            if (m_GameManager.IsAiTurn())
            {
                stringUserInput = m_GameManager.ComputerMove();
            }
            else
            {
                stringUserInput = getMoveInput(out io_CharUserInput);
            }

            return stringUserInput;
        }

        private void reInsertValidMoveIfNeeded(
            List<string> i_ValidPlayerMovesList, 
            ref string io_StringUserInput,
            ref char? io_CharUserInput)
        {
            while (io_CharUserInput != k_Quit && !i_ValidPlayerMovesList.Contains(io_StringUserInput))
            {
                Console.WriteLine("invalid move! Please enter valid move.");
                io_StringUserInput = getMoveInput(out io_CharUserInput);
            }
        }

        private string getPlayer2Info(ref bool io_IsVsHuman, int i_ChosenOpponent)
        {
            string player2Name;

            if (i_ChosenOpponent == (int)eOpponent.Computer)
            {
                io_IsVsHuman = !io_IsVsHuman;
                player2Name = "Computer";
            }
            else
            {
                player2Name = insertUserName();
            }

            return player2Name;
        }

        private string insertUserName()
        {
            string userInput;

            do
            {
                Console.WriteLine("Hey! please enter your name: (max 20 charcters)");
                userInput = Console.ReadLine();
            } 
            while (!LogicUI.ValidateUserName(userInput));

            return userInput;
        }

        private bool insertWhetherPlayerWantMoreGame()
        {
            string userInput;
            string msg;
            bool isContinue;
            int number;

            do
            {
                msg =
                    string.Format(
@"Would you like to play another game?
Yes (Press 1)
No (Press 2)");
                Console.WriteLine(msg);
                userInput = Console.ReadLine();
            } 
            while (!int.TryParse(userInput, out number) ||
                     !LogicUI.ValidateWhetherPlayerWantMoreGame(userInput, out isContinue));

            return isContinue;
        }

        private int insertBoardSize()
        {
            string userInput;

            do
            {
                Console.WriteLine("Please enter board size (Options: 6, 8, 10):");
                userInput = Console.ReadLine();
            } 
            while (!LogicUI.ValidateNumber(userInput) || !GameLogic.BoardManager.ValidateSize(userInput));

            return int.Parse(userInput);
        }

        private int insertChooseOpponent()
        {
            string userInput;
            string msg;

            do
            {
                msg =
                    string.Format(
    @"Please enter opponenet:
VS the Computer (Press 1)
VS Human (Press 2)");
                Console.WriteLine(msg);
                userInput = Console.ReadLine();
            } 
            while (!LogicUI.ValidateNumber(userInput) || !GameLogic.PlayerManager.CheckOpponent(userInput));

            return int.Parse(userInput);
        }

        private void printFirstLine(int i_Width)
        {
            const string k_Space = "   ";
            string msg;

            for (int i = 0; i < i_Width; i++)
            {
                msg =
                    string.Format(
    @"{0}{1}", k_Space, (char)(i + 'A'));
                Console.Write(msg);
            }

            Console.WriteLine(k_Space);
        }

        private char getPlayerCharacter(Cell i_Cell)
        {
            ePlayerChar currPlayerChar = ePlayerChar.Empty;
            eObjectInCell currPlayer = i_Cell.m_CellObject;

            if (currPlayer == eObjectInCell.Man1)
            {
                currPlayerChar = ePlayerChar.Man1;
            }
            else if (currPlayer == eObjectInCell.Man2)
            {
                currPlayerChar = ePlayerChar.Man2;
            }
            else if (currPlayer == eObjectInCell.King1)
            {
                currPlayerChar = ePlayerChar.King1;
            }
            else if (currPlayer == eObjectInCell.King2)
            {
                currPlayerChar = ePlayerChar.King2;
            }

            return (char)currPlayerChar;
        }

        private void printBoard(BoardManager i_Board)
        {
            const char k_Equals = '=', k_SideLine = '|', k_Space = ' ';
            int width, length, amountOfStars;
            string msgLine;

            width = i_Board.GetWidth();
            length = i_Board.GetLength();
            amountOfStars = width * 4 + 1;
            msgLine = new string(k_Equals, amountOfStars);
            clearScreen();
            printFirstLine(width);
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine("{0}{1}", k_Space, msgLine);

                for (int j = 0; j <= width; j++)
                {
                    if (j == 0)
                    {
                        Console.Write("{0}{1} ", (char)(i + 'a'), k_SideLine);
                    }
                    else
                    {
                        Console.Write("{0} {1} ", getPlayerCharacter(i_Board.Board[i, j - 1]), k_SideLine);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine("{0}{1}", k_Space, msgLine);
        }

        private void clearScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
        }

        private void printPlayerNameTurnAndLastMove()
        {
            PlayerManager currPlayer;
            char currPlayerChar, lastMoveChar;
            eObjectInCell objectInCell;
            Location lastMoveLocation;

            const char k_charPlayer1 = (char)ePlayerChar.Man1;
            const char k_charPlayer2 = (char)ePlayerChar.Man2;

            if (m_GameManager.IsPlayer1Turn)
            {
                currPlayer = m_GameManager.Player1;
                currPlayerChar = k_charPlayer1;
            }
            else
            {
                currPlayer = m_GameManager.Player2;
                currPlayerChar = k_charPlayer2;
            }

            if (m_GameManager.LastMove != null)
            {
                lastMoveLocation = m_GameManager.BoardManager.ConvertStringToLocation(m_GameManager.LastMove, 3);
                objectInCell = m_GameManager.BoardManager.GetCell(lastMoveLocation).m_CellObject;
                lastMoveChar = objectInCell == eObjectInCell.Man1 || objectInCell == eObjectInCell.King1 ? (char)ePlayerChar.Man1 : (char)ePlayerChar.Man2;
                Console.WriteLine("{0}'s move was ({1}): {2}", m_GameManager.GetPlayerNameByLocation(lastMoveLocation), lastMoveChar, m_GameManager.LastMove);
            }
            
            Console.WriteLine("{0}'s turn ({1}): ", currPlayer.Name, currPlayerChar);
        }

        private void insertFirstChar(string i_StringUserInput, out char? o_FirstChar)
        {
            bool isStringOnlyOneChar = i_StringUserInput.Length == 1;

            if (isStringOnlyOneChar)
            {
                o_FirstChar = i_StringUserInput[0];
            }
            else
            {
                o_FirstChar = null;
            }
        }

        private bool moveValidate(string i_UserInput)
        {
            bool isMoveValid;

            isMoveValid = LogicUI.ValidateFormatMove(i_UserInput);
            if (!isMoveValid)
            {
                Console.WriteLine("invalid move! Please enter valid move.");
            }

            return isMoveValid;
        }

        private string getMoveInput(out char? o_CharUserInput)
        {
            string userInput;

            do
            {
                userInput = Console.ReadLine();
                insertFirstChar(userInput, out o_CharUserInput);
            }
            while (o_CharUserInput != k_Quit && !moveValidate(userInput));

            return userInput;
        }
        private bool isChoseToQuit(char? charUserInput)
        {
            bool isChoseToQuit;

            isChoseToQuit = charUserInput == k_Quit;

            return isChoseToQuit;
        }
    }
}