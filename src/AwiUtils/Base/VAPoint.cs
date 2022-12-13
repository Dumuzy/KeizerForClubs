using System;
using System.Collections.Generic;
using System.Text;
using AwiUtils;

namespace AwiUtils
{
    /// <summary> A point class consisting of double X and double Y, useful for some stuff. </summary>
    /// <remarks> There is no simple way to create a C# generic for the VAPoint, which is a pity. </remarks>
    public class VAPointI
    {
        public VAPointI(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public VAPointI(VAPointI b)
        {
            this.X = b.X;
            this.Y = b.Y;
        }

        public int X, Y;
        public override string ToString()
        {
            string s = "X:" + Ext.ToDebug(X) + "  Y:" + Ext.ToDebug(Y);
            return s;
        }

        public VAPointI Add(VAPointI b)
        {
            X += b.X;
            Y += b.Y;
            return this;
        }

        public VAPointI Subtract(VAPointI b)
        {
            X -= b.X;
            Y -= b.Y;
            return this;
        }

        public VAPointI Divide(int d)
        {
            X /= d;
            Y /= d;
            return this;
        }

        public static VAPointI operator +(VAPointI a, VAPointI b)
        {
            VAPointI c = new VAPointI(a).Add(b);
            return c;
        }

        public static VAPointI operator -(VAPointI a, VAPointI b)
        {
            VAPointI c = new VAPointI(a).Subtract(b);
            return c;
        }

        #region Equality
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            VAPointI p = obj as VAPointI;
            if ((System.Object)p == null)
                return false;

            return X == p.X && Y == p.Y;
        }

        public bool Equals(VAPointI p)
        {
            if ((object)p == null)
                return false;

            return X == p.X && Y == p.Y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static bool operator ==(VAPointI a, VAPointI b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
                return false;

            // Return true if the fields match:
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(VAPointI a, VAPointI b)
        {
            return !(a == b);
        }
        #endregion Equality
    }

    /// <summary> A point class consisting of double X and double Y, useful for some stuff. </summary>
    public class VAPointD
    {
        public VAPointD() : this(Double.NaN, Double.NaN) { }
        public VAPointD(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static VAPointD operator +(VAPointD a, VAPointD b) { return new VAPointD(a.X + b.X, a.Y + b.Y); }
        public static VAPointD operator -(VAPointD a, VAPointD b) { return new VAPointD(a.X - b.X, a.Y - b.Y); }
        public static double CrossProduct(VAPointD a, VAPointD b) { return a.X * b.Y - a.Y * b.X; }
        public static double operator *(VAPointD a, VAPointD b) { return a.X * b.X + a.Y * b.Y; }

        public double Length { get { return Math.Sqrt(this * this); } }
        public double RookLength => Math.Abs(X) + Math.Abs(Y);

        public VAPointD MultiplyBy(double d) { X *= d; Y *= d; return this; }
        public double CrossProduct(VAPointD b) { return CrossProduct(this, b); }
        public VAPointD Add(VAPointD a) { X += a.X; Y += a.Y; return this; }

        #region Equality
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            // If parameter cannot be cast to Point return false.
            VAPointD p = obj as VAPointD;
            if (p is null)
                return false;

            return X == p.X && Y == p.Y;
        }

        public bool Equals(VAPointD p)
        {
            if (p is null)
                return false;

            return X == p.X && Y == p.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(VAPointD a, VAPointD b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if ((a is null) || (b is null))
                return false;

            bool isEqual = a.X == b.X && a.Y == b.Y;
            if (!isEqual)
            {
                // Exaktes Vergleichen geht nicht immer bei doubles. 
                var lengthSum = a.RookLength + b.RookLength;
                // Je länger die Vektoren, desto größer die erlaubte relative Differenz. Klingt unsinnig,
                // ist es vielleicht auch. 
                double maxDiff = lengthSum > 1e-3 ? 1e-10 : (lengthSum > 1e-5 ? 1e-7 : 1e-4);
                var diff = a - b;
                var dist = diff.RookLength;
                var relativeDist = dist / lengthSum;
                // Ich gucke hier, ob die Länge der Differenz relativ zur Länge der Vektoren klein ist...
                if (relativeDist < maxDiff)
                {
                    var lengthDiff = Math.Abs(a.RookLength - b.RookLength);
                    var relativeLengthDiff = lengthDiff / lengthSum;
                    // und ob die Differenz der Längen relativ zur Länge der Vektoren klein ist.
                    isEqual = relativeLengthDiff < maxDiff;
                }
            }
            return isEqual;
        }

        public static bool operator !=(VAPointD a, VAPointD b)
        {
            return !(a == b);
        }
        #endregion Equality

        public double X, Y;
        public override string ToString()
        {
            string s = "X:" + Ext.ToDebug1(X) + "  Y:" + Ext.ToDebug1(Y);
            return s;
        }
    }

}
