using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwiUtils
{
    /// <summary> Ein interface für eine beliebige Koordinatentransformation im 2-dimensionalen Raum. </summary>
    interface ICoordsTransformation2D
    {
        /// <summary> Transformiert die Koordinaten in die eine Richtung.</summary>
        VAPointD Transform(VAPointD p);
    }

    /// <summary> Ein interface für eine umkehrbare Koordinatentransformation im 2-dimensionalen Raum. </summary>
    interface IInvertibleCoordsTransformation2D : ICoordsTransformation2D
    {
        /// <summary> Transformiert die Koordinaten rückwärts. </summary>
        /// <remarks> Undo(Transform(p)) muß == p sein. </remarks>
        VAPointD Undo(VAPointD p);
    }

    /// <summary> Basisklasse, die Funktionen enthält, die für alle 2D-Transformationen gleich sein sollen. </summary>
    abstract public class CoordsTransformation2DBase : IInvertibleCoordsTransformation2D
    {
        public abstract VAPointD Transform(VAPointD p);
        public abstract VAPointD Undo(VAPointD p);

        public VAPointD Transform(double x, double y) { return Transform(new VAPointD(x, y)); }

        public VAPointD Undo(double x, double y) { return Undo(new VAPointD(x, y)); }

        public Li<VAPointD> Transform(IEnumerable<VAPointD> points)
        {
            Li<VAPointD> res = new Li<VAPointD>();
            foreach (var p in points)
                res.Add(Transform(p));
            return res;
        }

        public Li<VAPointD> Undo(IEnumerable<VAPointD> points)
        {
            Li<VAPointD> res = new Li<VAPointD>();
            foreach (var p in points)
                res.Add(Undo(p));
            return res;
        }

        #region Testcode
        protected void TestOneTrafo()
        {
            TestOnePoint(0, 0);
            TestOnePoint(1, 1);
            TestOnePoint(10, 5);
            TestOnePoint(-1000, -300);

            TestSomeRandomPoints(11);
            TestSomeRandomPoints(0.09);
        }

        protected void TestSomeRandomPoints(double multiplicator)
        {
            Li<VAPointD> p = new Li<VAPointD>();
            Random r = new Random();
            var mul = 3.1415;
            for (int i = 0; i < 10; ++i, mul *= multiplicator)
                p.Add(new VAPointD(r.NextDouble() * mul * (i % 3 == 0 ? -1 : 1), r.NextDouble() * mul));
            var r1 = Transform(p);
            var r2 = Undo(r1);
            Debug.Assert(r1.Count == p.Count);
            for (int i = 0; i < p.Count; ++i)
                Debug.Assert(r2[i] == p[i]);
        }

        protected void TestOnePoint(double x, double y) { TestOnePoint(new VAPointD(x, y)); }

        protected void TestOnePoint(VAPointD p)
        {
            var p1 = Transform(p);
            var p2 = Undo(p1);
            Debug.Assert(p2 == p);
        }
        #endregion Testcode
    }

    /// <summary> Transformiert eine VolFlow-Pres-Kurve in ein lineares Diagramm in Pixeln. </summary>
    /// <remarks> Das ist keine allgemeine affine Transformation, sondern eine, in der nur Streckung, 
    /// Spiegelung und Verschiebung vorkommt. </remarks>
    public class VolPres2PixelLinearTransformation : CoordsTransformation2DBase
    {
        public VolPres2PixelLinearTransformation(double volMin, double volMax, double presMin, double presMax,
            double xMinPix, double xMaxPix, double yMinPix, double yMaxPix)
        {
            this.volMin = volMin;
            this.volMax = volMax;
            this.presMin = presMin;
            this.presMax = presMax;
            this.xMinPix = xMinPix;
            this.xMaxPix = xMaxPix;
            this.yMinPix = yMinPix;
            this.yMaxPix = yMaxPix;

            pixelPerVolAndPres = GetPixelPerVolAndPres();
            translation = GetTranslation();
        }

        public override VAPointD Transform(VAPointD p)
        {
            var res = new VAPointD(pixelPerVolAndPres.X * p.X + translation.X, pixelPerVolAndPres.Y * p.Y + translation.Y);
            return res;
        }

        public override VAPointD Undo(VAPointD p)
        {
            var res = new VAPointD((p.X - translation.X) / pixelPerVolAndPres.X, (p.Y - translation.Y) / pixelPerVolAndPres.Y);
            return res;
        }

        public override string ToString()
        {
            string s = "VP2PLT A:" + pixelPerVolAndPres + "  b:" + translation;
            return s;
        }

        #region Testcode
        public static void Test()
        {
            var t1 = new VolPres2PixelLinearTransformation(0, 10, 0, 5, 0, 20, 0, 2);
            t1.TestOneTrafo();

            var t2 = new VolPres2PixelLinearTransformation(4, 10000, 50, 500, 20, 400, 300, 20);
            t2.TestOneTrafo();
        }

        #endregion Testcode
        #region private
        /// <summary> Gibt ein VAPointD aus zwei doubles zurück, das Werte für /Pixel/m3h und Pixel/Pa enthalten. </summary>
        private VAPointD GetPixelPerVolAndPres()
        {
            var pixelPerVol = (xMaxPix - xMinPix) / (volMax - volMin);
            var pixelPerPres = (yMaxPix - yMinPix) / (presMax - presMin);
            return new VAPointD(pixelPerVol, pixelPerPres);
        }

        private VAPointD GetTranslation()
        {
            var xTransInPixel = xMinPix - pixelPerVolAndPres.X * volMin;
            var yTransInPixel = yMinPix - pixelPerVolAndPres.Y * presMin;
            return new VAPointD(xTransInPixel, yTransInPixel);
        }

        private readonly double volMin, volMax, presMin, presMax;
        private readonly double xMinPix, xMaxPix, yMinPix, yMaxPix;
        readonly VAPointD pixelPerVolAndPres;
        readonly VAPointD translation;
        #endregion private
    }

    /// <summary> Für die Transformation einer VolFlow-Pres-Kurve in eine doppeltlogarithmisches Diagram in Pixeln. </summary>
    class VolPres2PixelLogLogTransformation : IInvertibleCoordsTransformation2D
    {
        public VolPres2PixelLogLogTransformation(double volMin, double volMax, double presMin, double presMax,
            double xMinPix, double xMaxPix, double yMinPix, double yMaxPix)
        {
            this.volMin = volMin;
            this.volMax = volMax;
            this.presMin = presMin;
            this.presMax = presMax;
            this.xMinPix = xMinPix;
            this.xMaxPix = xMaxPix;
            this.yMinPix = yMinPix;
            this.yMaxPix = yMaxPix;
        }

        public VAPointD Transform(VAPointD p) => throw new NotImplementedException();
        public VAPointD Undo(VAPointD p) => throw new NotImplementedException();


        public static void Test()
        {

        }

        double volMin, volMax, presMin, presMax,
            xMinPix, xMaxPix, yMinPix, yMaxPix;
    }
}
