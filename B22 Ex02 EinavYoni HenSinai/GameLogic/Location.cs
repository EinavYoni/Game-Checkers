namespace GameLogic
{
    public class Location
    {
        private int m_Row;
        private int m_Col;

        public Location(int i_Row, int i_Col)
        {
            SetLocation(i_Row, i_Col);
        }

        public int Row
        {
            get 
            { 
                return m_Row;
            }

            set 
            { 
                m_Row = value;
            }
        }

        public int Col
        {
            get 
            { 
                return m_Col;
            }

            set 
            { 
                m_Col = value;
            }
        }

        public void SetLocation(int i_Row, int i_Col)
        {
            m_Row = i_Row;
            m_Col = i_Col;
        }

        public Location GetLocation(Location location, eMargin i_RowMargin, eMargin i_ColMargin, int i_CellsDistance)
        {
            Location possibleLocation = new Location(location.Row + ((int)i_RowMargin * i_CellsDistance), 
                                                     location.Col + ((int)i_ColMargin * i_CellsDistance));

            return possibleLocation;
        }
    }
}