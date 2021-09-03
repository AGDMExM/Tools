using System;
using System.Collections.Generic;
using System.Text;

namespace Hex_Grid
{
    public struct Hex
    {
        private int col;
        private int row;

        public int Col { get { return col; } }
        public int Row { get { return row; } }

        public Hex(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
    }
}
