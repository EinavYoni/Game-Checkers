using System;

namespace GameLogic
{
    public struct Cell
    {
        public Location m_Location;
        public eObjectInCell m_CellObject;
    }

    public class BoardManager
    {
        private readonly Cell[,] r_Board;

        public BoardManager(int i_Row, int i_Col, PlayerManager i_Player1, PlayerManager i_Player2)
        {
            r_Board = new Cell[i_Row, i_Col];
            InitializeProperties(i_Player1, i_Player2);
        }

        public Cell[,] Board
        {
            get
            {
                return r_Board;
            }
        }

        public static bool ValidateSize(string i_SizeInput)
        {
            int size = int.Parse(i_SizeInput);
            bool isValidSize = (int)eLegalSize.Small == size
                            || (int)eLegalSize.Medium == size
                            || (int)eLegalSize.Large == size;

            return isValidSize;
        }

        public void InitializeProperties(PlayerManager i_Player1, PlayerManager i_Player2)
        {
            int width = GetWidth();
            int length = GetLength();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    r_Board[i, j].m_Location = new Location(i, j);
                    if ((i + 1 < width / 2) && ((j % 2 != 0 && i % 2 == 0) || (j % 2 == 0 && i % 2 != 0)))
                    {
                        r_Board[i, j].m_CellObject = eObjectInCell.Man1;
                        i_Player1.AddSoldiers(r_Board[i, j]);
                    }
                    else if ((i > (width / 2)) && ((j % 2 == 0 && i % 2 != 0) || (j % 2 != 0 && i % 2 == 0)))
                    {
                        r_Board[i, j].m_CellObject = eObjectInCell.Man2;
                        i_Player2.AddSoldiers(r_Board[i, j]);
                    }
                    else
                    {
                        r_Board[i, j].m_CellObject = eObjectInCell.None;
                    }
                }
            }
        }

        public Cell GetCell(int i_Row, int i_Col)
        {
            return r_Board[i_Row, i_Col];
        }

        public Cell GetCell(Location i_Location)
        {
            return r_Board[i_Location.Row, i_Location.Col];
        }
        public void SetCell(Location i_Location, eObjectInCell i_cellObject)
        {
            r_Board[i_Location.Row, i_Location.Col].m_CellObject = i_cellObject;
        }

        public int GetLength()
        {
            return r_Board.GetLength(0);
        }

        public int GetWidth()
        {
            return r_Board.GetLength(1);
        }

        public void SetStartAndDestinationLocations(string i_MoveTo, out Location o_StartLocation, out Location o_DestinationLocation)
        {
            const int k_StartLocationIndex = 0;
            const int k_DestinationLocationIndex = 3;

            o_StartLocation = ConvertStringToLocation(i_MoveTo, k_StartLocationIndex);
            o_DestinationLocation = ConvertStringToLocation(i_MoveTo, k_DestinationLocationIndex);
        }

        public Location ConvertStringToLocation(string i_Location, int i_StartIndex)
        {
            Location location = new Location(i_Location[i_StartIndex + 1] - 'a', i_Location[i_StartIndex] - 'A');

            return location;
        }

        public bool ValidateLocationInBoundaries(Location i_Location)
        {
            int row = i_Location.Row;
            int col = i_Location.Col;
            int amountOfRowsInBoard = GetLength();
            int amountOfColsInBoard = GetWidth();
            bool validLocation = row < amountOfRowsInBoard &&
                                 row >= 0 &&
                                 col < amountOfColsInBoard &&
                                 col >= 0;

            return validLocation;
        }

        public bool ValidateDistance(Location i_StartCell, Location i_DestinationCell, out bool o_IsMoveOverOpponent)
        {
            const int k_OneCellDistance = 1;
            const int k_TwoCellDistance = 2;
            bool isValidMove;
            int distanceBetweenCols, distanceBetweenRows;
            int startCellRow = i_StartCell.Row;
            int startCellCol = i_StartCell.Col;
            int destinationCellRow = i_DestinationCell.Row;
            int destinationCellCol = i_DestinationCell.Col;
            o_IsMoveOverOpponent = !true; 
            distanceBetweenCols = Math.Abs(startCellCol - destinationCellCol);
            distanceBetweenRows = Math.Abs(startCellRow - destinationCellRow);
            isValidMove = distanceBetweenCols == k_OneCellDistance && distanceBetweenRows == k_OneCellDistance;
            if (distanceBetweenCols == k_TwoCellDistance && distanceBetweenRows == k_TwoCellDistance)
            {
                o_IsMoveOverOpponent = !o_IsMoveOverOpponent;
                isValidMove = o_IsMoveOverOpponent;
            }

            return isValidMove;
        }

        public Cell GetTargetCell(Location i_StartCell, Location i_DestinationCell)
        {
            int distanceCols, distanceRows;
            eDirection targetRow, targetCol;
            Cell targetCell;

            distanceRows = i_DestinationCell.Row - i_StartCell.Row;
            distanceCols = i_DestinationCell.Col - i_StartCell.Col;
            targetRow = distanceRows > 0 ? eDirection.Down : eDirection.Up;
            targetCol = distanceCols > 0 ? eDirection.Right : eDirection.Left;
            targetCell = r_Board[i_StartCell.Row + (int)targetRow, i_StartCell.Col + (int)targetCol];

            return targetCell;
        }

        public string ConvertTwoLocationsToMoveString(Location i_StartLocation, Location i_DestinationLocation)
        {
            string move;

            move =
                string.Format(
@"{0}{1}>{2}{3}",
                (char)(i_StartLocation.Col + 'A'), (char)(i_StartLocation.Row + 'a'),
                (char)(i_DestinationLocation.Col + 'A'), (char)(i_DestinationLocation.Row + 'a'));

            return move;
        }

        public void UpdateTargetCellInfo(Cell i_TargerCell)
        {
            SetCell(i_TargerCell.m_Location, eObjectInCell.None);
        }
    }
}
