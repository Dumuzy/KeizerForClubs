using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AwiUtils
{
    /// <summary> Diese Klasse ist ein Ersatz für die List-Klasse. Sie hat vor allem eine viel schönere Debug-Ausgabe.</summary>
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    [DebuggerStepThrough]
    [Serializable()]
    public class Li<T> : List<T>
    {
        public Li() : base() { }

        public Li(IEnumerable<T> ienum) : base(ienum) { }

        public bool IsEmpty { get { return Count == 0; } }

        public override string ToString() => LiExtensions.ToDebugString(this);
    }

    /// <summary> Diese Klasse ist ein Ersatz für die ReadOnlyCollection-Klasse. Sie hat vor allem eine viel schönere Debug-Ausgabe.</summary>
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    [DebuggerStepThrough]
    [Serializable()]
    public class Liro<T> : ReadOnlyCollection<T>
    {
        public Liro() : base(new List<T>()) { }

        public Liro(IList<T> ilist) : base(ilist) { }

        public bool IsEmpty { get { return Count == 0; } }

        public override string ToString() => LiExtensions.ToDebugString(this);
    }

    [DebuggerStepThrough]
    public static class LiExtensions
    {
        public static Li<T> ToLi<T>(this IEnumerable<T> ienum) { return new Li<T>(ienum); }
        public static Liro<T> ToLiro<T>(this IList<T> ilist) { return new Liro<T>(ilist); }
        public static Liro<T> ToLiro<T>(this IEnumerable<T> ilist) { return ilist.ToLi().ToLiro(); }
        internal static string ToDebugString<T>(IList<T> ilist)
        {
            bool isTString = typeof(T) == typeof(string);
            int nMax = 120;
            string s = "N:" + ilist.Count + "; ";
            int i = 0;
            for (; i < ilist.Count && s.Length < nMax; ++i)
            {
                if (ilist[i] == null)
                    s += "(null); ";
                else
                    s += (isTString ? Helper.NormalizeNumberPoint(ilist[i].ToString()) : ilist[i].ToString()) + "; ";
            }
            if (i < ilist.Count)
                s = s.Substring(0, nMax - 3) + "...";
            return s;
        }
    }
}
