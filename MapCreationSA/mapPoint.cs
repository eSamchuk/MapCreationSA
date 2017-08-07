using System;


namespace MapCreationSA
{
    public class mapPoint
    {
        private int _x;
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private int _row;
        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        private int _col;
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }

        public mapPoint() {}

        public mapPoint(int x, int y, int row, int col)
        {
            this.X = x;
            this.Y = y;
            this.Row = row;
            this.Col = col;
        }
    }
}
