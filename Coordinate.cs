using System;
using System.Collections.Generic;
using System.Text;

namespace Hex_Grid
{
    public struct Coordinate
    {
        private float x;
        private float y;

        public float X { get { return x; } }
        public float Y { get { return y; } }

        public Coordinate(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
