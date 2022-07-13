using System.Collections.Generic;

namespace GameLogic
{
    public class PlayerManager
    {
        private readonly bool r_IsAi;
        private int m_Score = 0;
        private string m_Name;
        private List<Cell> m_Soldiers;
        private List<string> m_Moves;

        public PlayerManager(bool i_IsAi, string i_Name)
        {
            r_IsAi = i_IsAi;
            m_Name = i_Name;
            m_Soldiers = new List<Cell>();
            m_Moves = new List<string>();
        }

        public int Score
        {
            get 
            { 
                return m_Score;
            }

            set 
            { 
                m_Score = value;
            }
        }
        
        public bool IsAi
        {
            get 
            { 
                return r_IsAi;
            }
        }

        public string Name
        {
            get 
            { 
                return m_Name;
            }
        }

        public List<Cell> Soldiers
        {
            get 
            { 
                return m_Soldiers;
            }
        }

        public List<string> Moves
        {
            get
            {
                return m_Moves;
            }

            set
            {
                m_Moves = new List<string>(value);
            }
        }

        public void AddSoldiers(Cell i_Cell)
        {
            m_Soldiers.Add(i_Cell);   
        }

        public void RemoveSoldier(Cell i_Cell)
        {
            m_Soldiers.Remove(i_Cell);
        }

        public void ClearMoves()
        {
            m_Moves.Clear();
        }

        public bool IsExistMove()
        {
            return m_Moves.Count != 0;
        }

        public static bool CheckOpponent(string i_OpponentInput)
        {
            const int k_Human = 1;
            const int k_Computer = 2;
            int opponentInput = int.Parse(i_OpponentInput);
            bool isValid = opponentInput == k_Human || opponentInput == k_Computer;

            return isValid;
        }
    }
}