using System;

namespace Hex_Grid
{
    public class HexGrid
    {
        private int width_cell;
        private int height_cell;

        private enum TypeOffset { UpRight, Right, DownRight, DownLeft, Left, UpLeft, None };

        public void Setup(int width, int height = 0)
        {
            width_cell = width;
            height_cell = height;
        }

        public Hex GetHexAtPoint(float x, float y)
        {
            int col = 0;
            int row = 0;

            // General Horizontal Line
            if (Math.Abs(y) <= height_cell / 4)
            {
                if (x > 0)
                {
                    col = (int)((x + (width_cell / 2)) / width_cell);
                    float remainsX = (x + (width_cell / 2)) % width_cell;
                    if (remainsX > 0)
                        col++;
                }
                if (x < 0)
                {
                    col = (int)((x - (width_cell / 2)) / width_cell);
                    float remainsX = (x - (width_cell / 2)) % width_cell;
                    if (remainsX < 0)
                        col--;
                }
                // y = 0
            }

            row = 2 * (int)(y / (1.5f * height_cell));
            float remainsY = y % (1.5f * height_cell);
            if(Math.Abs(remainsY) >= height_cell)
            {
                if(remainsY > 0)
                {
                    row += 2;
                }
                if (remainsY < 0)
                {
                    row -= 2;
                }

                if (x > 0)
                {
                    col = (int)((x + (width_cell / 2)) / width_cell);
                    float remainsX = (x + (width_cell / 2)) % width_cell;
                    if (remainsX > 0)
                        col++;
                }
                if (x < 0)
                {
                    col = (int)((x - (width_cell / 2)) / width_cell);
                    float remainsX = (x - (width_cell / 2)) % width_cell;
                    if (remainsX < 0)
                        col--;
                }
            }
            if(remainsY < height_cell)
            {
                if (remainsY > 0)
                {
                    row++;
                }
                if (remainsY < 0)
                {
                    row--;
                }

                col = (int)(x / width_cell);
                float remainsX = x % width_cell;
                if (Math.Abs(remainsX) > 0)
                {
                    if (remainsX < 0)
                        col--;
                }
            }

            Coordinate coordHex = GetHexPosition(col, row);

            TypeOffset type = ComputeRelateToHex(coordHex.X - width_cell / 2, coordHex.Y - height_cell / 2,
                                                coordHex.X + width_cell / 2, coordHex.Y + height_cell / 2,
                                                x, y);
            if(type == TypeOffset.None)
            {
                return new Hex(col, row);
            }
            else
            {
                switch(type)
                {
                    case TypeOffset.UpLeft:
                        return new Hex(col, row + 1);

                    case TypeOffset.UpRight:
                        return new Hex(col + 1, row + 1);

                    case TypeOffset.Right:
                        return new Hex(col + 1, row);

                    case TypeOffset.DownRight:
                        return new Hex(col + 1, row - 1);

                    case TypeOffset.DownLeft:
                        return new Hex(col, row - 1);

                    case TypeOffset.Left:
                        return new Hex(col - 1, row);
                }
            }

            return new Hex(col, row);
        }

        public Coordinate GetHexPosition(int col, int row)
        {
            float x;
            float y;
            if (row % 2 == 0)
            {
                x = col * width_cell;
            }
            else
            {
                x = width_cell * (col + 1) - width_cell / 2;
            }

            y = row * 0.75f * height_cell;

            return new Coordinate(x, y);
        }

        private TypeOffset ComputeRelateToHex(float startCoordinateX, float startCoordinateY,
                            float endCoordinateX, float endCoordinateY,
                            float x, float y)
        {
            float distToCenterX = width_cell / 2;
            float SIN = 0.5f;

            // dot in quadrat
            if (y <= endCoordinateY - (height_cell / 4) && y >= startCoordinateY + (height_cell / 4))
            {
                return TypeOffset.None;
            }

            // Left
            if (x < startCoordinateX)
            {
                return TypeOffset.Left;
            }

            // Right
            if (x > endCoordinateX)
            {
                return TypeOffset.Right;
            }

            // Up Left triangle
            if (x < startCoordinateX + distToCenterX && y > startCoordinateY + 0.75f * height_cell)
            {
                float yUnderAngle = SIN * (x - startCoordinateX);
                if (y > yUnderAngle + 0.75f * height_cell + startCoordinateY)
                {
                    return TypeOffset.UpLeft;
                }

                return TypeOffset.None;
            }

            // Up Right triangle
            if (x > startCoordinateX + distToCenterX && y > startCoordinateY + 0.75f * height_cell)
            {
                float yUnderAngle = SIN * (endCoordinateX - x);
                if (y > yUnderAngle + 0.75f * height_cell + startCoordinateY)
                {
                    return TypeOffset.UpRight;
                }

                return TypeOffset.None;
            }

            // Down Left triangle
            if (x < startCoordinateX + distToCenterX && y < endCoordinateY - 0.75f * height_cell)
            {
                float yUnderAngle = SIN * (x - startCoordinateX);
                if (y < endCoordinateY - yUnderAngle - 0.75f * height_cell)
                {
                    return TypeOffset.DownLeft;
                }

                return TypeOffset.None;
            }

            // Down Right triangle
            if (x > startCoordinateX + distToCenterX && y < endCoordinateY - 0.75f * height_cell)
            {
                float yUnderAngle = SIN * (endCoordinateX - x);
                if (y < endCoordinateY - yUnderAngle - 0.75f * height_cell)
                {
                    return TypeOffset.DownRight;
                }

                return TypeOffset.None;
            }

            return TypeOffset.None;
        }
    }
}
