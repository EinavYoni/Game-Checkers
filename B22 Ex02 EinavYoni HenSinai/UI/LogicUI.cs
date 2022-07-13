using System;
using System.Linq;

namespace UserInterface
{
    public class LogicUI
    {
        private const char k_Space = ' ';
        private const int k_MaxUserNameLen = 20;

        public static bool ValidateUserName(string i_NameInput)
        {
            bool isValidInput = !i_NameInput.Contains(k_Space) && i_NameInput.Length <= k_MaxUserNameLen;

            if (!isValidInput)
            {
                Console.WriteLine("Invalid input!");
            }

            return isValidInput;
        }

        public static bool ValidateNumber(string i_SizeInput)
        {
            bool isNumber = int.TryParse(i_SizeInput, out int number);

            return isNumber;
        }

        public static bool ValidateFormatMove(string i_PlayerMove)
        {
            const int k_Len = 5;
            const char k_To = '>';
            bool isValidMove = i_PlayerMove.Length == k_Len;

            if (isValidMove)
            {
                isValidMove = char.IsUpper(i_PlayerMove[0]) &&
                              char.IsUpper(i_PlayerMove[3]) &&
                              char.IsLower(i_PlayerMove[1]) &&
                              char.IsLower(i_PlayerMove[4]) &&
                              i_PlayerMove[2] == k_To;
            }

            return isValidMove;
        }

        public static bool ValidateWhetherPlayerWantMoreGame(string i_UserInput, out bool o_IsContinue)
        {
            const int k_ContinuePlaying = 1;
            const int StopPlaying = 2;
            bool isValidInput = true;
            int userChoice = int.Parse(i_UserInput);

            if (userChoice != k_ContinuePlaying && userChoice != StopPlaying)
            {
                isValidInput = !isValidInput;
            }

            o_IsContinue = userChoice == k_ContinuePlaying;

            return isValidInput;
        }
    }
}
