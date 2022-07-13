using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class GameManager
    {
        private BoardManager m_BoardManager;
        private PlayerManager m_Player1, m_Player2;
        private bool m_IsPlayer1Turn;
        private string m_LastMove = null;
        private const bool k_Ai = true;

        public GameManager(int i_Row, int i_Col, bool i_IsAi, string i_PlayerName1, string i_PlayerName2)
        {
            m_Player1 = new PlayerManager(!k_Ai, i_PlayerName1);
            m_Player2 = new PlayerManager(i_IsAi, i_PlayerName2);
            m_BoardManager = new BoardManager(i_Row, i_Col, m_Player1, m_Player2);
            m_IsPlayer1Turn = true;
        }

        public BoardManager BoardManager
        {
            get 
            { 
                return m_BoardManager;
            }
        }

        public PlayerManager Player1
        {
            get
            {
                return m_Player1;
            }
        }

        public PlayerManager Player2
        {
            get
            {
                return m_Player2;
            }
        }

        public bool IsPlayer1Turn
        {
            get
            {
                return m_IsPlayer1Turn;
            }

            set
            {
                m_IsPlayer1Turn = value;
            }
        }

        public string LastMove
        {
            set
            {
                m_LastMove = value;
            }

            get
            {
                return m_LastMove;
            }
        }

        public void MovePlayer(string i_MoveTo, ref bool io_IsPlayerCanEat)
        {
            const bool v_PlayerCanEat = true;
            Location startLocation, destinationLocation;
            PlayerManager currentPlayer;
            int index;
            Cell targetCell;

            currentPlayer = m_IsPlayer1Turn ? m_Player1 : m_Player2;
            m_BoardManager.SetStartAndDestinationLocations(i_MoveTo, out startLocation, out destinationLocation);
            index = currentPlayer.Soldiers.IndexOf(m_BoardManager.GetCell(startLocation));
            updateStartAndDestinationCellsInfo(startLocation, destinationLocation);
            currentPlayer.Soldiers[index] = m_BoardManager.GetCell(destinationLocation);
            m_LastMove = i_MoveTo;
            if (io_IsPlayerCanEat)
            {
                targetCell = m_BoardManager.GetTargetCell(startLocation, destinationLocation);
                m_BoardManager.UpdateTargetCellInfo(targetCell);
                updateOpponentSoldiers(targetCell);
                currentPlayer.ClearMoves();
                getValidOptionsMoves(currentPlayer, destinationLocation, out io_IsPlayerCanEat);
                io_IsPlayerCanEat = io_IsPlayerCanEat ? v_PlayerCanEat : !v_PlayerCanEat;
            }
            else
            {
                io_IsPlayerCanEat = !v_PlayerCanEat;
                currentPlayer.ClearMoves();
            }

            if (IsAiTurn())
            {
                System.Threading.Thread.Sleep(3000);
            }
        }

        private void updateOpponentSoldiers(Cell i_TargerCell)
        {
            PlayerManager opponentPlayer;

            opponentPlayer = m_IsPlayer1Turn ? m_Player2 : m_Player1;
            opponentPlayer.RemoveSoldier(i_TargerCell);
        }

        private void getValidOptionsMoves(PlayerManager i_Player, Location i_StartLocation, out bool i_IsPlayerCanEat)
        {
            Cell cellDestination;
            List<string> eatMoves = new List<string>();
            List<string> notEatMoves = new List<string>();

            cellDestination = m_BoardManager.GetCell(i_StartLocation);
            addToMovesList(eatMoves, notEatMoves, cellDestination, i_Player);
            chooseEatListIfNotEmpty(i_Player, eatMoves, notEatMoves, out i_IsPlayerCanEat);
        }

        private void chooseEatListIfNotEmpty(PlayerManager i_Player, List<string> i_EatMoves, List<string> i_NotEatMoves, out bool isPlayerCanEat)
        {
            const bool v_PlayerAte = true;

            if (i_EatMoves.Count != 0)
            {
                i_Player.Moves = i_EatMoves;
                isPlayerCanEat = v_PlayerAte;
            }
            else
            {
                i_Player.Moves = i_NotEatMoves;
                isPlayerCanEat = !v_PlayerAte;
            }
        }

        private void updateStartAndDestinationCellsInfo(Location i_StartLocation, Location i_DestinationLocation)
        {
            eObjectInCell currPlayerObj;
            PlayerManager currentPlayer = m_IsPlayer1Turn ? m_Player1 : m_Player2;

            if (checkIfReachBoardLimit(m_BoardManager.GetCell(i_DestinationLocation), currentPlayer))
            {
                currPlayerObj = m_IsPlayer1Turn ? eObjectInCell.King1 : eObjectInCell.King2;
            }
            else
            {
                currPlayerObj = m_BoardManager.GetCell(i_StartLocation).m_CellObject;
            }

            m_BoardManager.SetCell(i_StartLocation, eObjectInCell.None);
            m_BoardManager.SetCell(i_DestinationLocation, currPlayerObj);
        }

        public void UpdateTurns()
        {
            m_IsPlayer1Turn = !m_IsPlayer1Turn;
        }

        public List<string> GetPlayerMovesList()
        {
            PlayerManager player = m_IsPlayer1Turn ? m_Player1 : m_Player2;

            return player.Moves;
        }

        public void AddAllMovesToList(out bool isPlayerCanEat)
        {
            PlayerManager player;
            List<string> eatMoves = new List<string>();
            List<string> notEatMoves = new List<string>();

            player = m_IsPlayer1Turn ? m_Player1 : m_Player2;
            foreach (Cell soldier in player.Soldiers)
            {
                addToMovesList(eatMoves, notEatMoves, soldier, player);
            }

            chooseEatListIfNotEmpty(player, eatMoves, notEatMoves, out isPlayerCanEat);
        }

        private void addAllMovesToList(PlayerManager i_Player, out bool isPlayerCanEat)
        {
            List<string> eatMoves = new List<string>();
            List<string> notEatMoves = new List<string>();

            foreach (Cell soldier in i_Player.Soldiers)
            {
                addToMovesList(eatMoves, notEatMoves, soldier, i_Player);
            }

            chooseEatListIfNotEmpty(i_Player, eatMoves, notEatMoves, out isPlayerCanEat);
        }

        private void addToMovesList(List<string> i_EatMoves, List<string> i_NotEatMoves, Cell i_Cell, PlayerManager i_Player)
        {
            if (i_Cell.m_CellObject == eObjectInCell.Man1 || i_Cell.m_CellObject == eObjectInCell.Man2)
            {
                addManMoves(i_EatMoves, i_NotEatMoves, i_Cell.m_Location, i_Player);
            }
            else
            {
                addKingMoves(i_EatMoves, i_NotEatMoves, i_Cell.m_Location, i_Player);
            }
        }

        private void isPlayersHasAnyMove(out bool i_IsCurrPlayerHasMoves, out bool i_IsOpponentPlayerHasMoves)
        {
            PlayerManager currPlayer, opponentPlayer;

            if (m_IsPlayer1Turn)
            {
                currPlayer = m_Player1;
                opponentPlayer = m_Player2;
            }
            else
            {
                currPlayer = m_Player2;
                opponentPlayer = m_Player1;
            }

            i_IsCurrPlayerHasMoves = isPlayerExistMoves(currPlayer);
            i_IsOpponentPlayerHasMoves = isPlayerExistMoves(opponentPlayer);
        }

        private bool isPlayerExistMoves(PlayerManager i_Player)
        {
            bool isExistMoves;

            i_Player.ClearMoves();
            addAllMovesToList(i_Player, out bool isPlayerCanEat);
            isExistMoves = i_Player.IsExistMove();
            i_Player.ClearMoves();

            return isExistMoves;
        }

        public bool IsPlayerExistMoves()
        {
            PlayerManager player;
            bool isExistMoves;

            player = m_IsPlayer1Turn ? m_Player1 : m_Player2;
            player.ClearMoves();
            AddAllMovesToList(out bool isPlayerCanEat);
            isExistMoves = player.IsExistMove();

            return isExistMoves;
        }

        public string GetPlayerNameByLocation(Location i_locationCell)
        {
            string playerName;
            eObjectInCell objectInCell = m_BoardManager.GetCell(i_locationCell).m_CellObject;
            if (objectInCell == eObjectInCell.King1 || objectInCell == eObjectInCell.Man1)
            {
                playerName = m_Player1.Name;
            }
            else
            {
                playerName = m_Player2.Name;
            }

            return playerName;
        }

        public bool IsMoveOneOfPlayerOption(string i_MoveTo, PlayerManager i_Player, out bool o_IsMoveOverOpponent)
        {
            Location startLocation, destinationLocation;
            bool validMove;
            o_IsMoveOverOpponent = !true;
            m_BoardManager.SetStartAndDestinationLocations(i_MoveTo, out startLocation, out destinationLocation);
            validMove = m_BoardManager.ValidateLocationInBoundaries(destinationLocation) &&
                        m_BoardManager.ValidateLocationInBoundaries(startLocation);

            if (validMove)
            {
                validMove = m_BoardManager.GetCell(destinationLocation).m_CellObject == eObjectInCell.None &&
                            isCellMatchPlayer(startLocation, i_Player) &&
                            isMoveOneOfPlayerOptions(startLocation, destinationLocation, i_Player, out o_IsMoveOverOpponent);
            }

            return validMove;
        }

        private bool isMoveOneOfPlayerOptions(
            Location i_StartCell, 
            Location i_DestinationCell, 
            PlayerManager i_Player, 
            out bool o_IsMoveOverOpponent)
        {
            bool isValidMove = m_BoardManager.ValidateDistance(i_StartCell, i_DestinationCell, out o_IsMoveOverOpponent);
            eObjectInCell currPlayerLevel = m_BoardManager.Board[i_StartCell.Row, i_StartCell.Col].m_CellObject;

            if (isValidMove)
            {
                if (currPlayerLevel == eObjectInCell.Man1 || currPlayerLevel == eObjectInCell.Man2)
                {
                    isValidMove = validateVerticalDirection(i_StartCell, i_DestinationCell, i_Player);
                }

                if (o_IsMoveOverOpponent && isValidMove)
                {
                    isValidMove = validateMoveOverOpponent(i_StartCell, i_DestinationCell);
                    o_IsMoveOverOpponent = isValidMove;
                }
            }

            return isValidMove;
        }

        private bool isCellMatchPlayer(Location i_StartCell, PlayerManager i_Player)
        {
            bool isMatch;
            eObjectInCell objectInCell = m_BoardManager.GetCell(i_StartCell).m_CellObject;

            if (i_Player == m_Player1)
            {
                isMatch = objectInCell == eObjectInCell.King1 ||
                          objectInCell == eObjectInCell.Man1;
            }
            else
            {
                isMatch = objectInCell == eObjectInCell.King2 ||
                          objectInCell == eObjectInCell.Man2;
            }

            return isMatch;
        }

        private bool validateMoveOverOpponent(Location i_StartCell, Location i_DestinationCell)
        {
            bool isValidMove;
            eObjectInCell startObjectInCell, targetObjectInCell;

            startObjectInCell = m_BoardManager.GetCell(i_StartCell).m_CellObject;
            targetObjectInCell = m_BoardManager.GetTargetCell(i_StartCell, i_DestinationCell).m_CellObject;
            if (startObjectInCell == eObjectInCell.Man1 || startObjectInCell == eObjectInCell.King1)
            {
                isValidMove = targetObjectInCell == eObjectInCell.Man2 || targetObjectInCell == eObjectInCell.King2;
            }
            else
            { 
                isValidMove = targetObjectInCell == eObjectInCell.Man1 || targetObjectInCell == eObjectInCell.King1;
            }

            return isValidMove;
        }

        private bool validateVerticalDirection(
            Location i_StartCell, 
            Location i_DestinationCell, 
            PlayerManager i_Player)
        {
            bool isValidDirection;
            int distanceRows = i_DestinationCell.Row - i_StartCell.Row;

            isValidDirection = (i_Player == m_Player1 && distanceRows > 0) || (i_Player == m_Player2 && distanceRows < 0);

            return isValidDirection;
        }

        private void addHorizonMoves(
            List<string> i_Moves, 
            Location i_StartLocation, 
            bool i_IsDirectionDown,
            PlayerManager i_Player, 
            int i_CellsDistance)
        {
            const bool v_DirectionDown = true;
            string moveLeft, moveRight;
            bool isMoveOverOpponent;
            eMargin rowMargin;

            rowMargin = i_IsDirectionDown == v_DirectionDown ? eMargin.DownRow : eMargin.UpRow;
            moveLeft = m_BoardManager.ConvertTwoLocationsToMoveString(
                i_StartLocation,
                i_StartLocation.GetLocation(i_StartLocation, rowMargin, eMargin.LeftCol, i_CellsDistance));
            moveRight = m_BoardManager.ConvertTwoLocationsToMoveString(
                i_StartLocation,
                i_StartLocation.GetLocation(i_StartLocation, rowMargin, eMargin.RightCol, i_CellsDistance));

            if (IsMoveOneOfPlayerOption(moveLeft, i_Player, out isMoveOverOpponent))
            {
                i_Moves.Add(moveLeft);
            }

            if (IsMoveOneOfPlayerOption(moveRight, i_Player, out isMoveOverOpponent))
            {
                i_Moves.Add(moveRight);
            }
        }

        private void addManMoves(
            List<string> i_EatMoves, 
            List<string> i_NotEatMoves, 
            Location i_StartLocation, 
            PlayerManager i_Player)
        {
            const int k_OneCellDistance = 1;
            const int k_TwoCellsDistance = 2;
            const bool v_DirectionDown = true;
            bool isDirectionDown;

            isDirectionDown = i_Player == m_Player1 ? v_DirectionDown : !v_DirectionDown;
            addHorizonMoves(i_EatMoves, i_StartLocation, isDirectionDown, i_Player, k_TwoCellsDistance);
            addHorizonMoves(i_NotEatMoves, i_StartLocation, isDirectionDown, i_Player, k_OneCellDistance);
        }

        private void addKingMoves(
            List<string> i_EatMoves, 
            List<string> i_NotEatMoves, 
            Location i_StartLocation, 
            PlayerManager i_Player)
        {
            const int k_OneCellDistance = 1;
            const int k_TwoCellsDistance = 2;
            const bool v_DirectionDown = true;

            addHorizonMoves(i_EatMoves, i_StartLocation, v_DirectionDown, i_Player, k_TwoCellsDistance);
            addHorizonMoves(i_EatMoves, i_StartLocation, !v_DirectionDown, i_Player, k_TwoCellsDistance);
            addHorizonMoves(i_NotEatMoves, i_StartLocation, v_DirectionDown, i_Player, k_OneCellDistance);
            addHorizonMoves(i_NotEatMoves, i_StartLocation, !v_DirectionDown, i_Player, k_OneCellDistance);
        }

        private bool checkIfReachBoardLimit(Cell i_DestinationCell, PlayerManager i_Player)
        {
            bool inBoardLimits;
            inBoardLimits = (i_Player == m_Player1 && i_DestinationCell.m_Location.Row == (m_BoardManager.GetWidth() - 1)) ||
                           (i_Player == m_Player2 && i_DestinationCell.m_Location.Row == 0);

            return inBoardLimits;
        }

        public bool IsThereWinner()
        {
            bool i_IsCurrPlayerHasMoves, i_IsOpponentPlayerHasMoves, isThereWinner;

            isPlayersHasAnyMove(out i_IsCurrPlayerHasMoves, out i_IsOpponentPlayerHasMoves);
            isThereWinner = m_Player1.Soldiers.Count == 0 || m_Player2.Soldiers.Count == 0 ||
                            (i_IsCurrPlayerHasMoves && !i_IsOpponentPlayerHasMoves);

            return isThereWinner;
        }

        private bool isTie()
        {
            bool i_IsCurrPlayerHasMoves, i_IsOpponentPlayerHasMoves, isTie;

            isPlayersHasAnyMove(out i_IsCurrPlayerHasMoves, out i_IsOpponentPlayerHasMoves);
            isTie = !i_IsCurrPlayerHasMoves && !i_IsOpponentPlayerHasMoves;

            return isTie;
        }

        public bool IsGameOver()
        {
            bool isGameOver = IsThereWinner() || isTie();

            return isGameOver;
        }

        private int claculatePlayerScore(PlayerManager i_Player)
        {
            const int k_ScoreForMan = 1;
            const int k_ScoreForKing = 4;
            int countScore = 0;
            bool isSoldierKing;

            foreach (Cell soldier in i_Player.Soldiers)
            {
                isSoldierKing = soldier.m_CellObject == eObjectInCell.King1 || soldier.m_CellObject == eObjectInCell.King2;
                countScore += isSoldierKing ? k_ScoreForKing : k_ScoreForMan;
            }

            return countScore;
        }

        public void UpdateScore()
        {
            PlayerManager winner = m_IsPlayer1Turn ? m_Player1 : m_Player2;
            int scorePlayer1 = claculatePlayerScore(m_Player1);
            int scorePlayer2 = claculatePlayerScore(m_Player2);

            if (winner == m_Player1)
            {
                m_Player1.Score += scorePlayer1 - scorePlayer2;
            }
            else
            {
                m_Player2.Score += scorePlayer2 - scorePlayer1;
            }
        }

        public void PrintPlayersScore()
        {
            string playersScore;

            playersScore =
                string.Format(
@"Score:
{0}: {1}
{2}: {3}",
                m_Player1.Name, m_Player1.Score, m_Player2.Name, m_Player2.Score);
            Console.WriteLine(playersScore);
        }

        public void ResetGame()
        {
            resetPlayersInfo();
            m_BoardManager.InitializeProperties(m_Player1, m_Player2);
        }

        private void resetPlayersInfo()
        {
            m_LastMove = null;
            m_IsPlayer1Turn = true;
            m_Player1.Soldiers.Clear();
            m_Player2.Soldiers.Clear();
            m_Player1.ClearMoves();
            m_Player2.ClearMoves();
        }

        public string ComputerMove()
        {
            var rand = new Random();
            int randIndex;
            string nextMove;

            randIndex = rand.Next(m_Player2.Moves.Count);
            nextMove = m_Player2.Moves[randIndex];
            m_Player2.ClearMoves();

            return nextMove;
        }

        public bool IsAiTurn()
        {
            bool isAi = Player2.IsAi && !IsPlayer1Turn;

            return isAi;
        }
    }
}
