using System.Collections.Generic;
using System.Windows.Media;
using System;

namespace TetrisGameManager
{
    public class Cell
    {
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public Shapes Shape { get; set; }
        
        internal Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal Cell(int x, int y, Shapes shape)
        {
            X = x;
            Y = y;
            Shape = shape;
        }
        
        public static bool operator ==(Cell c1, Cell c2)
        {
            return ((c1.Y == c2.Y) && (c1.X == c2.X));
        }
        public static bool operator !=(Cell c1, Cell c2)
        {
            return !(c1==c2);
        }

        public override int GetHashCode()
        {
            var hashCode = -6654;
            hashCode = hashCode * -1569 + X.GetHashCode();
            hashCode = hashCode * -1569 + X.GetHashCode();
            return hashCode;

        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cell))
            {
                return false;
            }
            var cell = (Cell)obj;
            return X == cell.X &&
                   Y == cell.Y;
        }
    }
    
}
