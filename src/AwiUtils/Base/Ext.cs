
using DeepCopyExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace AwiUtils
{
    // Es gibt keine echten Extension Methods in C# 2.0
    // Deshalb hier diese Klasse E, die Methoden enthält, die ich sonst als Extension Methods gebaut hätte. 
    public static class Ext
    {
        public static string ToDebug(params object[] oo)
        {
            string s;
            if (oo == null)
                s = "(null)";
            else
                s = string.Join(", ", oo.Select(o => o == null ? "(null)" : o.ToString()));
            return s;
        }

        public static string ToDebug(bool b)
        {
            string s = b ? "T" : "F";
            return s;
        }

        public static string ToDebug(bool? b)
        {
            string s = b.HasValue ? ToDebug(b.Value) : "(null)";
            return s;
        }

        public static string ToShortString(bool b)
        {
            string s = b ? "T" : "F";
            return s;
        }

        public static string ToDebug(char c)
        {
            string s;
            var isPrintable = !Char.IsControl(c) || Char.IsWhiteSpace(c);
            if (isPrintable)
                s = "" + c;
            else
                s = "ᛝ";
            return s;
        }

        public static string ToDebug(float f)
        {
            string s;
            if (Double.IsNaN(f))
                s = "NaN";
            else if (Single.MaxValue == f)
                s = "Max";
            else
                s = string.Format("{0:0.#}", f);
            return s;
        }

        public static string ToDebug(double f)
        {
            string s;
            if (Double.IsNaN(f))
                s = "NaN";
            else if (Double.MaxValue == f)
                s = "Max";
            else
                s = string.Format("{0:0.#}", f);
            return s;
        }

        public static string ToDebug(List<string> ll)
        {
            var s = ll == null ? "(null)" : "(" + string.Join(", ", ll.Select(item => Ext.ToDebug(item))) + ")";
            return s;
        }

        public static string ToDebug(List<List<string>> ll)
        {
            var s = ll == null ? "(null)" : "[" + string.Join(", ", ll.Select(item => ToDebug(item))) + "]";
            return s;
        }

        public static string ToDebug(List<List<List<string>>> ll)
        {
            var s = ll == null ? "(null)" : "{" + string.Join(", ", ll.Select(item => ToDebug(item))) + "}";
            return s;
        }



        public static string ToDebug1(float? f)
        {
            string s = f == null ? "(null)" : ToDebug1(f.Value);
            return s;
        }

        public static string ToDebug1(decimal? f)
        {
            string s = f == null ? "(null)" : ToDebug1(f.Value);
            return s;
        }

        public static string ToDebug1(float f)
        {
            string s;
            try
            {
                s = Double.IsNaN(f) ? "NaN" : ToDebug1((decimal)f);
            }
            catch (System.Exception ex)
            {
                if (typeof(OverflowException) == ex.GetType())
                    s = "Overflow";
                else
                    s = ex.GetType().Name;
            }
            return s;
        }

        public static string ToDebug1(double f)
        {
            string s = Double.IsNaN(f) ? "NaN" : ToDebug1((decimal)f);
            return s;
        }

        public static string ToDebug1(decimal f)
        {
            string s;
            if (f >= 10)
                s = string.Format("{0:0.0}", f);
            else if (f >= 1)
                s = string.Format("{0:0.00}", f);
            else
                s = string.Format("{0:0.000}", f);
            return s;
        }

        public static bool HasFlag(int flags, int flag)
        {
            return (flags & flag) == flag;
        }

        // erst ab C# 7.3 erlaubt. 
        //public static bool HasFlag<T>(T flags, T flag) where T : System.Enum
        //{
        //    return ((int)flags & (int)flag) == (int)flag;
        //}

        //public static bool HasFlag(this int flags, int flag)
        //{
        //    return (flags & flag) == flag;
        //}


        public static string GetAllMessages(Exception exp)
        {
            string msg = "";
            do
            {
                msg = msg + " > " + Ext.ToDebug(exp.Message);
                exp = exp.InnerException;
            }
            while (exp != null);
            return msg;
        }

        /// <summary> Schneidet den string am ersten Vorkommnis von from ab.</summary>
        public static string TrimEndFrom(this string value, string from)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            var idx = value.IndexOf(from);
            if (idx != -1)
                value = value.Substring(0, idx);
            return value;
        }

        /// <summary> Verkürzt einen String auf maxLength, falls nötig. Falls ellipsis != null, hänge ellipsis an.</summary>
        public static string Truncate(this string value, int maxLength, string ellipsis)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            if (ellipsis == null)
                ellipsis = "";
            if (maxLength < ellipsis.Length)
                throw new ArgumentOutOfRangeException();
            return value.Length <= maxLength ? value :
                value.Substring(0, maxLength - ellipsis.Length) + ellipsis;
        }

        /// <summary> Splittet den übergebenen String an Leerzeichen, Tabs und Linebreaks und gibt ein Array der Wörter zurück.</summary>
        /// <remarks> Praktisch um Wortlisten einfach zu definieren. </remarks>
        [DebuggerStepThrough]
        public static string[] SplitToWords(this string str)
        {
            return str.Split(" \t\r\n".ToCharArray()).Where(s => !string.IsNullOrEmpty(s.Trim())).ToArray();
        }

        /// <summary> Splittet den übergebenen String an Linebreaks und gibt ein Array der getrimmten nichtleeren Zeilen zurück. </summary>
        [DebuggerStepThrough]
        public static string[] SplitToLines(this string str, params char[] splitters)
        {
            if (splitters.Length == 0)
                splitters = "\r\n".ToCharArray();
            return str.Split(splitters).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        /// <summary> Splittet den übergebenen String an jedem Trennzeichen und gibt ein Array der Wörter zurück.</summary>
        /// <remarks>  Leere Strings werden aus der Wortliste gelöscht. Praktisch um Wortlisten einfach zu definieren.</remarks>
        public static string[] SplitToWords(this string str, char[] splitter)
        {
            return str.Split(splitter).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        /// <summary> Wie List.Contains, ignoriert aber Groß-Kleinschreibung beim Vergleich. </summary>
        public static bool ContainsIgnoreCase(this List<string> hay, string needle)
        {
            var index = hay.FindIndex(x => x.Equals(needle, StringComparison.OrdinalIgnoreCase));
            return index != -1;
        }

        /// <summary> Wie List.Contains, ignoriert aber Groß-Kleinschreibung beim Vergleich. </summary>
        public static bool ContainsIgnoreCase(this IList<string> hay, string needle)
        {
            bool contains = false;
            for (int i = 0; i < hay.Count && !contains; ++i)
                if (hay[i].Equals(needle, StringComparison.OrdinalIgnoreCase))
                    contains = true;
            return contains;
        }

        public static bool ContainsIgnoreCase(this string hay, string needle)
        {
            var index = hay.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return index != -1;
        }

        /// <summary> True, if string needle is contained in list hay. </summary>
        public static bool IsContainedIn<T>(this T needle, IList<T> hay) => hay.Contains(needle);


        /// <summary> This function treats the input string list as a string-string dictionary where keys are at index numbers 
        /// 0, 2, 4, 6, ... and values are at index numbers 1, 3, 5, 7, ...</summary>
        /// <returns> True if the key was found, false otherwise. </returns>
        public static bool TryGetValue(this IList<string> dictionary, string key, out string value)
        {
            value = null;
            bool isFound = false;
            for (int i = 0; i < dictionary.Count - 1; i += 2)
                if (dictionary[i] == key)
                {
                    value = dictionary[i + 1];
                    isFound = true;
                    break;
                }
            return isFound;
        }

        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <summary> Clones the object by serialization and deserialization. Langsam, aber funktioniert im 
        /// Gegensatz zu DeepTClone und DeepRClone auch zB  mit einem XmlDocument. </summary>
        public static T DeepSClone<T>(this T original)
        {
            var xml = original.Serialize();
            var clone = (T)Helper.WebHelper_DeserializeFromXmlString(xml, typeof(T));
            return clone;
        }

        /// <summary> Clones the object by using ExpressionTree and Reflection. </summary>
        [DebuggerStepThrough]
        public static T DeepTClone<T>(this T original) => original.DeepCopyByExpressionTree(null);

        #region DeepClone
        // This region is copied and adapted from https://github.com/Burtsev-Alexey/net-object-deep-copy/blob/master/ObjectExtensions.cs on 13 June 2018. 

        /// <summary> Clones the object by using MemberwiseClone and Reflection. </summary>
        /// <param name="originalObject"></param>
        /// <remarks> This is 2 to 3 times faster than cloning by serializing and deserializing.
        /// DeepCopyByExpressionTrees is around 10-50% faster than this one, if used many times. Then only use that one. 
        /// But still, handcrafted Clone-functions are 100 times faster than serializing and deserializing. </remarks>
        public static Object DeepRClone(Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }

        /// <remarks> This is 2 to 3 times faster than cloning by serializing and deserializing.
        /// DeepCopyByExpressionTrees is around 10-50% faster than this one, if used many times. Then only use that one. 
        /// But still, handcrafted Clone-functions are 100 times faster than serializing and deserializing. </remarks>
        public static T DeepRClone<T>(this T original)
        {
            return (T)DeepRClone((Object)original);
        }

        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        private static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String))
                return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null)
                return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
                return originalObject;
            if (visited.ContainsKey(originalObject))
                return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
                return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    ArrayExtensions.ForEach(clonedArray, (array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false)
                    continue;
                if (IsPrimitive(fieldInfo.FieldType))
                    continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        class ReferenceEqualityComparer : EqualityComparer<Object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }
            public override int GetHashCode(object obj)
            {
                if (obj == null)
                    return 0;
                return obj.GetHashCode();
            }
        }

        static class ArrayExtensions
        {
            public static void ForEach(Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0)
                    return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do
                    action(array, walker.Position);
                while (walker.Step());
            }
        }

        class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion DeepClone


        #region Sort Functions

        #region testcode
        //static public void TestInsertionShellSort()
        //{
        //    for (int k = 0; k < 3; k++)
        //    {
        //        Li<int> ll = new Li<int>();
        //        ll.Add(1); ll.Add(2); ll.Add(3); ll.Add(5); ll.Add(14); ll.Add(15); ll.Add(17); ll.Add(18); ll.Add(22);
        //        //ll.Add(1); ll.Add(1); ll.Add(2); ll.Add(2); ll.Add(5);
        //        //ll.Add(1); ll.Add(2); ll.Add(2); ll.Add(1); ll.Add(2); ll.Add(3); ll.Add(1); ll.Add(1); ll.Add(2); ll.Add(2); ll.Add(1);
        //        //ll.Add(1); ll.Add(2); ll.Add(2); ll.Add(1); ll.Add(2); ll.Add(3); ll.Add(1); ll.Add(1); ll.Add(2); ll.Add(2); ll.Add(3);
        //        //ll.Add(2); ll.Add(3); ll.Add(4); ll.Add(5); ll.Add(7); ll.Add(2); ll.Add(1); ll.Add(2); ll.Add(3); ll.Add(4);
        //        //ll.Add(5); ll.Add(7); ll.Add(8); ll.Add(2); ll.Add(1); ll.Add(7); ll.Add(8); ll.Add(8); ll.Add(8); ll.Add(14);
        //        //ll.Add(15); ll.Add(12); ll.Add(13); ll.Add(14); ll.Add(15); ll.Add(17); ll.Add(18); ll.Add(22);
        //        //ll.Add(20); ll.Add(21); ll.Add(22); ll.Add(25); ll.Add(25); ll.Add(25); ll.Add(25);
        //        //ll.Add(32); ll.Add(33); ll.Add(34); ll.Add(35); ll.Add(37); ll.Add(41);

        //        string t = "sorted";
        //        if (k == 1)
        //            t = "reverted";
        //        else if (k == 2)
        //            t = "shuffled";
        //        System.Diagnostics.Debug.WriteLine("-\n" + t);
        //        for (int j = 0; j < 5; ++j)
        //        {
        //            if (k == 0 || k == 1)
        //                ll.Sort();
        //            if (k == 1)
        //                ll.Reverse();
        //            if (k == 2) // Das hier mischt das Feld. 
        //                ll = ll.OrderBy(item => rnd.Next()).ToLi();

        //            for (int i = 0; i < 2000000 / Math.Pow(ll.Count, 1.5); ++i)
        //            {
        //                Li<int> ll2 = new Li<int>(ll);
        //                Stopwatches.Start("QuickSort");
        //                ll2.Sort(CompareInt);
        //                Stopwatches.Stop("QuickSort");
        //                Li<int> ll3 = new Li<int>(ll);
        //                Stopwatches.Start("ShellSort");
        //                Ext.ShellSort(ll3, CompareInt);
        //                Stopwatches.Stop("ShellSort");

        //                TestShellSort("ShellSortHibard", ll, hibard);
        //                TestShellSort("ShellSortSedgewick1", ll, sedgewick1);
        //                TestShellSort("ShellSortSedgewick2", ll, sedgewick2);
        //                TestShellSort("ShellSortCiura", ll, ciura);
        //                TestShellSort("ShellSortFuchs", ll, fuchs);
        //            }
        //            ll.AddRange(ll.ToArray());
        //        }
        //        System.Diagnostics.Debug.WriteLine(string.Join("\n", Stopwatches.ToLines(false).ToArray()));
        //        Stopwatches.Clear();
        //    }
        //}

        //static readonly Random rnd = new Random();

        //private static void TestShellSort(string name, Li<int> toSort, int[] distanzFolge)
        //{
        //    Li<int> ll = new Li<int>(toSort);
        //    Stopwatches.Start(name);
        //    ShellSort(ll, CompareInt, distanzFolge);
        //    Stopwatches.Stop(name);
        //}

        //static int CompareInt(int a, int b) { return a - b; }

        //private static readonly int[] hibard = { 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 
        //                                           65535, 131071, 262143, 524287, 1048575, 2097151};
        //private static readonly int[] knuth = { 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 48004, 144013, 432040, 1296121 };
        //private static readonly int[] sedgewick1 = { 1, 8, 23, 77, 281, 1073, 4193, 16577, 48004, 144013, 432040, 1296121, 3888364 };
        //private static readonly int[] sedgewick2 = { 1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905, 8929, 16001, 48004, 144013, 432040, 1296121, 3888364 };
        //private static readonly int[] ciura = { 1, 4, 10, 23, 57, 132, 301, 701, 1753, 4383, 10958, 27396, 82189, 246568, 739705, 2219116 };
        #endregion testcode

        /// <summary> Dieses Interface können Objekte implementieren, die nicht das komplette IList 
        /// implementieren wollen aber dennoch sortiert werden wollen. Man kann die dann per ShellSort sortieren. </summary>
        public interface IShellSortable<T>
        {
            int Count { get; }
            T this[int index] { get; }
            /// <summary> Vertausche die Elemente mit den Indexen i und j in der Liste. </summary>
            void SwapAt(int i, int j);
        }

        public static void ShellSort<T>(IShellSortable<T> list, Func<T, T, int> compareFunc)
        {
            ShellSort(list, compareFunc, fuchs);
        }



        /// <summary> Generische Shellsort-Funktion. </summary>
        /// <param name="list">Die zu sortierende Liste. </param>
        /// <param name="compareFunc">Muss Zahl > 0 zurückgeben, falls 1. Argument > 2. Argument, 0 falls gleich.  </param>
        /// <remarks>Shellsort ist bei Listen die vorsortiert oder umgedreht sind, wesentlich besser als Quicksort, 
        /// das standardmäßig von List.Sort verwendet wird.         
        /// Und ShellSort ist bei nicht langen Listen kaum langsamer oder sogar schneller als Quicksort. 
        /// S.a. https://www.toptal.com/developers/sorting-algorithms </remarks>
        public static void ShellSort<T>(IList<T> list, Func<T, T, int> compareFunc)
        {
            // Meine Tests ergaben, dass für kleine, im Wesentlichen vorsortierte Felder, die Sedgewick1-Folge am besten ist.  
            // Fast genausogut. Und Fuchs ist schneller bei rumgedrehten oder unsortierten Feldern. 
            // Sie ist grob 1.5 mal bis doppelt so schnell wie Quicksort. Je vorsortierter, desto besser wird Shellsort. 
            // Bei komplett schon sortierten Felder ist Shellsort-Fuchs rund  doppelt so schnell wie Quicksort. 
            // Bei sortiertern rumgedrehten Feldern ist Shellsort-Fuchs rund 1.5 mal so schnell wie Quicksort. 
            // Sogar bei gemischten Feldern, mindestens bis Länge 140, ist ShellSort-Fuchs schneller als Quicksort. 
            ShellSort(list, compareFunc, fuchs);
        }

        private static readonly int[] fuchs = { 1, 4, 13, 40, 124, 385, 1195, 3709, 11512, 35713, 110848, 246568, 739705, 2219116, 6657349 };

        /// <summary> Generische Shellsort-Funktion. </summary>
        /// <param name="list">Die zu sortierende Liste. </param>
        /// <param name="compareFunc">Muss Zahl > 0 zurückgeben, falls 1. Argument > 2. Argument, 0 falls gleich.  </param>
        /// <param name="distanzFolge">Muss von klein nach groß sortiert sein und mindestens 2 Elemente haben. </param>
        private static void ShellSort<T>(IList<T> list, Func<T, T, int> compareFunc, int[] distanzFolge)
        {
            int n = list.Count;
            int distanzFolgeIndex;

            for (distanzFolgeIndex = 0; distanzFolgeIndex < distanzFolge.Length; ++distanzFolgeIndex)
                if (distanzFolge[distanzFolgeIndex] > list.Count)
                    break;
            --distanzFolgeIndex;
            // Der distanzFolgeIndex zeigt nun auf das größte Element, das kleiner ist als die Länge der Liste. 

            while (distanzFolgeIndex >= 0)
            {
                int h = distanzFolge[distanzFolgeIndex];
                for (int i = h; i < n; i++)
                {
                    int k = i - h;
                    for (int j = i; j >= h && compareFunc(list[j], list[k]) < 0; k -= h)
                    {
                        T temp = list[j];
                        list[j] = list[k];
                        list[k] = temp;
                        j = k;
                    }
                }
                --distanzFolgeIndex;
            }
        }

        /// <summary> Generische Shellsort-Funktion. </summary>
        /// <param name="list">Die zu sortierende Liste. </param>
        /// <param name="compareFunc">Muss Zahl > 0 zurückgeben, falls 1. Argument > 2. Argument, 0 falls gleich.  </param>
        /// <param name="distanzFolge">Muss von klein nach groß sortiert sein und mindestens 2 Elemente haben. </param>
        private static void ShellSort<T>(IShellSortable<T> list, Func<T, T, int> compareFunc, int[] distanzFolge)
        {
            int n = list.Count;
            int distanzFolgeIndex;

            for (distanzFolgeIndex = 0; distanzFolgeIndex < distanzFolge.Length; ++distanzFolgeIndex)
                if (distanzFolge[distanzFolgeIndex] > list.Count)
                    break;
            --distanzFolgeIndex;
            // Der distanzFolgeIndex zeigt nun auf das größte Element, das kleiner ist als die Länge der Liste. 

            while (distanzFolgeIndex >= 0)
            {
                int h = distanzFolge[distanzFolgeIndex];
                for (int i = h; i < n; i++)
                {
                    int k = i - h;
                    for (int j = i; j >= h && compareFunc(list[j], list[k]) < 0; k -= h)
                    {
                        list.SwapAt(j, k);
                        j = k;
                    }
                }
                --distanzFolgeIndex;
            }
        }



        // InsertionSort sollte nie benutzt werden, da es im schlechtesten Fall (umgedrehte Listen)
        // sehr viel schlechter als ShellSort ist und im günstigsten Fall nicht viel besser. 
        static void InsertionSort<T>(IList<T> list, Func<T, T, int> compareFunc)
        {
            for (int i = 1; i < list.Count; ++i)
                for (int k = i; k >= 1 && compareFunc(list[k], list[k - 1]) < 0; --k)
                {
                    T temp = list[k];
                    list[k] = list[k - 1];
                    list[k - 1] = temp;
                }
        }
        #endregion Sort Functions
    }

    /// <summary> Enthält echte Extension-Methods für XmlNodes u.ä.</summary>
    public static class XmlNodeExt
    {
        /// <summary> Sucht rekursiv alle Nodes unterhalb von oder gleich node, auf die das isFitting-Prädikat passt und gibt die in einer Li zurück. </summary>
        /// <returns> Li mit passenden Nodes. Kann leer sein. Niemals null. </returns>
        public static Li<XmlNode> FindFittingRecursive(this XmlNode node, Predicate<XmlNode> isFitting)
        {
            var result = new Li<XmlNode>();
            AddFittingRecursive(node, isFitting, result);
            return result;
        }

        /// <summary> Addiert rekursiv alle Nodes unterhalb von oder gleich node, auf die das isFitting-Prädikat passt, zur übergebenen Li. </summary>
        public static void AddFittingRecursive(this XmlNode node, Predicate<XmlNode> isFitting, Li<XmlNode> result)
        {
            if (isFitting(node))
                result.Add(node);
            var children = node.ChildNodes;
            for (int i = 0; i < children.Count; ++i)
                AddFittingRecursive(children[i], isFitting, result);
        }

    }

}
