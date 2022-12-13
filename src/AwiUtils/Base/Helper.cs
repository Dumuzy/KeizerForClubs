using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

#pragma warning disable SYSLIB0023

namespace AwiUtils
{
    public class Helper
    {
        /// <summary> Returns a string like "FileVersion: 1.0.0.2622; BuildDate: 2014-Oct-14"</summary>
        public static string GetFileVersionInfo()
        {
            System.Diagnostics.FileVersionInfo v = System.Diagnostics.FileVersionInfo.
                        GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string s = "FileVersion: " + v.FileVersion + "; " + v.Comments;
            return s;
        }

        /// <summary> Returns the FileVersion of this dll as string. E.g. "1.0.0.2650". </summary>
        public static string GetDllVersion()
        {
            System.Diagnostics.FileVersionInfo v = System.Diagnostics.FileVersionInfo.
                        GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return v.FileVersion;
        }

        public static DateTime GetLinkerTime(Assembly assembly)
        {
            // Best method to get build timestamp of an assembly, 
            // copied from http://stackoverflow.com/questions/1600962/displaying-the-build-date
            // and adapted to .NET 2.0
            DateTime localTime;
            try
            {
                string filePath = assembly.Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;

                byte[] buffer = new byte[2048];

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    stream.Read(buffer, 0, 2048);

                int offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
                int secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                DateTime linkTimeUtc = epoch.AddSeconds(secondsSince1970);

                //TimeZoneInfo tz = target ?? TimeZoneInfo.Local;
                //localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);
                localTime = linkTimeUtc.ToLocalTime();
            }
            catch (SystemException)
            {
                localTime = new DateTime();
            }
            return localTime;
        }

        public static object DeserializeFromXmlString(string xml,
                                                      Type objectType)
        {
            object response;

            using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(xml)))
            {
                XmlSerializer xs = new XmlSerializer(objectType);
                response = xs.Deserialize(memoryStream);
            }
            return response;
        }

        /// <summary> Serialisiert ein Objekt in einen String. </summary>
        /// <returns>XML-String</returns>
        /// <remarks> Dieses hier muss auf dem VAServer verwendet werden, das WebHelper... auf dem Client, wenn ein ConfXml serialisiert werden soll.  </remarks>
        public static string SerializeToXmlString(object obj)
        {
            string response;
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            using (MemoryStream memStrm = new MemoryStream())
            {
                UTF8Encoding utf8E = new UTF8Encoding();

                using (XmlTextWriter xmlSink = new XmlTextWriter(memStrm, utf8E))
                {
                    xmlSerializer.Serialize(xmlSink, obj);
                    byte[] utf8EncodedData = memStrm.ToArray();
                    response = utf8E.GetString(utf8EncodedData);
                }
            }
            return response;
        }
        /// <summary> Deserialisiert einen XML-String in ein Objekt. </summary>
        /// <param name="xml"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static object WebHelper_DeserializeFromXmlString(string xml, Type objectType)
        {
            Stopwatches.Start("WebHelper_DeserializeFromXmlString");
            //XmlSerializer für den Typ des Objekts erzeugen. Das ist das erste Mal lahm, später schneller. 
            XmlSerializer serializer = new XmlSerializer(objectType);
            object o;
            using (StringReader stringReader = new StringReader(xml))
            {
                o = serializer.Deserialize(stringReader);
            }
            Stopwatches.Stop("WebHelper_DeserializeFromXmlString");
            return o;
        }

        /// <summary> Erstellt ein Dictionary aus einem key/value-string.</summary>
        /// <param name="s">Zeichenkette mit key/value Paaren, zwischen Paaren ist ';' 
        /// der Trenner, der Trenner zwischen key und Value ist ':'.</param>
        /// <returns>Dictionary Objekt</returns>
        public static Dictionary<string, string> DeserializeStartParams(string s)
        {
            const string keyValueSeparator = ":";
            const string pairSeparator = ";";
            var res = DeserializeToDictionary(s, pairSeparator, keyValueSeparator, StringComparer.OrdinalIgnoreCase);
            return res;
        }

        [DebuggerStepThrough]
        public static bool ParseBool(string source)
        {
            bool b = false;
            if (source != null)
            {
                string s = source.ToLower();
                if (s == "t" || s == "1" || s == "true")
                    b = true;
                else if (s == "f" || s == "0" || s == "false")
                    b = false;
            }
            return b;
        }

        [DebuggerStepThrough]
        public static bool ParseBool(object source)
        {
            bool b = false;
            string s = source as string;
            if (s != null)
                b = ParseBool(s);
            else
                b = Convert.ToBoolean(source);  // should work for anything but strings. 
            return b;
        }

        [DebuggerStepThrough]
        public static bool ToBool(object source) { return ParseBool(source); }

        /// <summary> Replacement for Convert.ToSingle, Convert.ToDouble and Double.Parse. </summary>
        /// <remarks> Because of problems with culture-dependent behaviour of those two functions, always this 
        /// replacement must be used. </remarks>
        [DebuggerStepThrough]
        public static decimal ToDecimal(object source)
        {
            decimal value = 0;
            string s = source as string;
            if (s != null)
                value = ToDecimal(s);
            else
                value = Convert.ToDecimal(source); // should work for anything but strings. 
            return value;
        }

        /// <summary> Replacement for Convert.ToDecimal and Decimal.Parse. </summary>
        /// <remarks> Because of problems with culture-dependent behaviour of those two functions, always this 
        /// replacement must be used. </remarks>
        [DebuggerStepThrough]
        public static decimal ToDecimal(string source)
        {
            decimal value;
            Match m = ReWithThousandPoint.Match(source);
            if (m.Success)
            {
                if (m.Groups["FirstPoint"].Value != m.Groups["SecondPoint"].Value)
                    source = m.Groups[1].Value + m.Groups[2].Value + '.' + m.Groups[3].Value;
                // else die Zahl hat zwei . oder zwei , drin, dann kann es es keine Kommazahl mit 
                // Tausender-Trenner sein, sondern sonst was. 
            }
            else
                source = source.Replace(',', '.');
            decimal.TryParse(source, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            return value;
        }
        static readonly Regex ReWithThousandPoint = new Regex(@"^(\d+)(?<FirstPoint>[\.,])(\d\d\d)(?<SecondPoint>[\.,])(\d+)$");

        /// <summary> Replacement for Convert.ToSingle and Double.Parse. </summary>
        /// <remarks> Because of problems with culture-dependent behaviour of those two functions, always this 
        /// replacement must be used. </remarks>
        [DebuggerStepThrough]
        public static float ToSingle(object source)
        {
            float value = 0f;
            string s = source as string;
            if (s != null)
                value = ToSingle(s);
            else
                value = Convert.ToSingle(source); // should work for anything but strings. 
            return value;
        }

        /// <summary> Replacement for Convert.ToSingle and Double.Parse. </summary>
        /// <remarks> Because of problems with culture-dependent behaviour of those two functions, always this 
        /// replacement must be used. </remarks>
        [DebuggerStepThrough]
        public static float ToSingle(string source)
        {
            double value;
            if (source != null)
                source = source.Replace(',', '.');
            double.TryParse(source, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            return (float)value;
        }

        /// <summary> Replacement for Convert.ToDouble and Double.Parse. </summary>
        /// <remarks> Because of problems with culture-dependent behaviour of those two functions, always this 
        /// replacement must be used. </remarks>
        [DebuggerStepThrough]
        public static double ToDouble(object source)
        {
            double value;
            string s = source as string;
            if (s != null)
            {
                s = s.Replace(',', '.');
                double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            }
            else
                value = Convert.ToDouble(source); // should work for anything but strings. 
            return value;
        }

        /// <summary> Replacement for Convert.ToInt und int.Parse. Doubles etc werden gerundet. </summary>
        [DebuggerStepThrough]
        public static int ToInt(object source)
        {
            double d = Helper.ToDouble(source);
            int value = (int)Math.Round(d, 0);
            return value;
        }

        public static string ReplaceFullWordIgnoreCase(string source, string toReplace, string replaceWith)
        {
            Regex regex = new Regex(toReplace + @"\b", RegexOptions.IgnoreCase);
            string s = regex.Replace(source, replaceWith);
            return s;
        }

        /// <summary> Wie List{string}.IndexOf, nur dass dies hier nicht auf Gleichheit prüft, sondern darauf, 
        /// ob ein Regex auf den String matcht. </summary>
        /// <returns>Index des matchenden Regex oder -1 falls keiner passt. </returns>
        public static int RegexIndexOf(IList<Regex> regexe, string s)
        {
            int i = 0;
            for (; i < regexe.Count; ++i)
                if (regexe[i].IsMatch(s))
                    break;

            if (i == regexe.Count)
                i = -1;
            return i;
        }

        public delegate string NumberNormalizerFunc(string value);

        /// <summary> Returns the input string with the first character converted to uppercase, or defaultValue if input string is null or empty.</summary>
        /// <remarks>According to Stackoverflow, this is the fastest way to do this. </remarks>
        public static string FirstLetterToUpperCaseOrDefault(string s, string defaultValue)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary> Normalisiert eine als string übergebene Zahl auf 5 Nachkommastellen und InvariantCulture als Format,
        /// also immer mit . als Dezimaltrenner. </summary>
        public static string NormalizeNumberPoint(string number) { return NormalizeNumberPoint(number, 5); }

        /// <summary> Normalisiert eine als string übergebene Zahl auf decimals Nachkommastellen und InvariantCulture als Format,
        /// also immer mit . als Dezimaltrenner. </summary>
        public static string NormalizeNumberPoint(string number, int decimals) { return NormalizeNumber(number, decimals, '.'); }

        /// <summary> Normalisiert eine double übergebene Zahl auf decimals Nachkommastellen und InvariantCulture als Format,
        /// also immer mit . als Dezimaltrenner. </summary>
        public static string NormalizeNumberPoint(double number, int decimals) { return NormalizeNumber(number, decimals, '.'); }

        /// <summary> Normalisiert eine als string übergebene Zahl auf 5 Nachkommastellen mit Komma als Dezimaltrenner. </summary>
        public static string NormalizeNumberComma(string number) { return NormalizeNumber(number, 5, ','); }

        /// <summary> Normalisiert eine als string übergebene Zahl auf decimals Nachkommastellen mit , als Dezimaltrenner. </summary>
        public static string NormalizeNumberComma(string number, int decimals) { return NormalizeNumber(number, decimals, ','); }

        private static string NormalizeNumber(string number, int decimals, char decimalPoint)
        {
            string res;
            if (number != null && number.IndexOfAny(new char[] { ',', '.' }) != -1)
            {
                string s = number.Replace(',', '.');
                double d;
                if (!Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                    res = number;
                else
                    res = NormalizeNumber(d, decimals, decimalPoint);
            }
            else
                res = number;
            return res;
        }

        private static string NormalizeNumber(double f, int decimals, char decimalPoint)
        {
            string res = Double.IsNaN(f) ? "" : Math.Round(f, decimals).ToString(CultureInfo.InvariantCulture);
            if (decimalPoint != '.')
                res = res.Replace('.', decimalPoint);
            return res;
        }

        public static string NormalizeNumberPointSignificantDigits(string number, int significantDigits)
        {
            string res;
            string s = number.Replace(',', '.');
            double d;
            if (!Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                res = number;
            else
                res = ToStringSignificantDigits(d, significantDigits, CultureInfo.InvariantCulture);
            return res;
        }


        public static double RoundSignificantDigits(double value, int significantDigits)
        {
            int unneededRoundingPosition;
            return RoundSignificantDigits(value, significantDigits, out unneededRoundingPosition);
        }

        /// <summary> This method will round and then append zeros if needed.
        /// I.e. if you round .002 to two significant figures, the resulting number should be .0020. </summary>
        public static string ToStringSignificantDigits(double value, int significantDigits, CultureInfo culture)
        {
            // Im wesentlichen von da: http://stackoverflow.com/questions/158172/formatting-numbers-with-significant-figures-in-c-sharp

            if (double.IsNaN(value) || double.IsInfinity(value))
                return value.ToString(culture);

            int roundingPosition;
            double roundedValue = RoundSignificantDigits(value, significantDigits, out roundingPosition);

            // when rounding causes a cascading round affecting digits of greater significance, 
            // need to re-round to get a correct rounding position afterwards
            // this fixes a bug where rounding 9.96 to 2 figures yeilds 10.0 instead of 10
            RoundSignificantDigits(roundedValue, significantDigits, out roundingPosition);

            string res;
            if (Math.Abs(roundingPosition) > 9)  // use exponential notation format
                res = string.Format(culture, "{0:E" + (significantDigits - 1) + "}", roundedValue);
            // string.format is only needed with decimal numbers (whole numbers won't need to be padded with zeros to the right.)
            else if (roundingPosition > 0)
                res = string.Format(culture, "{0:F" + roundingPosition + "}", roundedValue);
            else
                res = roundedValue.ToString(culture);

            return res;
        }

        /// <summary> This method will return a rounded double value at a number of signifigant figures.
        /// the sigFigures parameter must be between 0 and 15, exclusive. </summary>
        private static double RoundSignificantDigits(double value, int significantDigits, out int roundingPosition)
        {
            // Im wesentlichen von da: http://stackoverflow.com/questions/158172/formatting-numbers-with-significant-figures-in-c-sharp

            roundingPosition = 0;

            if (double.IsNaN(value) || double.IsInfinity(value))
                return value;

            if (Math.Abs(value) < 0.00000000001)
            {
                roundingPosition = significantDigits - 1;
                return 0d;
            }

            if (significantDigits < 1 || significantDigits > 15)
                throw new ArgumentOutOfRangeException("significantDigits", value, "The significantDigits argument must be between 1 and 15.");

            // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
            roundingPosition = significantDigits - 1 - (int)(Math.Floor(Math.Log10(Math.Abs(value))));

            // try to use a rounding position directly, if no scale is needed.
            // this is because the scale mutliplication after the rounding can introduce error, although 
            // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
            if (roundingPosition > 0 && roundingPosition < 16)
                return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);

            // Shouldn't get here unless we need to scale it.
            // Set the scaling value, for rounding whole numbers or decimals past 15 places
            double scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));

            return Math.Round(value / scale, significantDigits, MidpointRounding.AwayFromZero) * scale;
        }

        /// <summary> There is no LINQ and no Dictionary.Except function in .NET 2.  This is my replacement. </summary>
        public static List<string> Except(Dictionary<string, bool> first, Dictionary<string, bool> second)
        {
            List<string> result = Except(first.Keys, second);
            return result;
        }

        /// <summary> There is no LINQ and no Dictionary.Except function in .NET 2.  This is a replacement. </summary>
        public static List<string> Except(IEnumerable<string> first, Dictionary<string, bool> second)
        {
            List<string> result = new List<string>();
            foreach (string s in first)
                if (!second.ContainsKey(s))
                    result.Add(s);
            return result;
        }

        /// <summary> There is no LINQ and no List.Except function in .NET 2.  This is my replacement. 
        /// It works with touppered strings and therefore is slow. </summary>
        public static List<string> Except(IEnumerable<string> first, IEnumerable<string> second)
        {
            List<string> result = new List<string>();
            List<string> secondUpper = new List<string>();
            foreach (string s in second)
                secondUpper.Add(s.ToUpper());

            foreach (string s in first)
            {
                string sUpper = s.ToUpper();
                if (!secondUpper.Contains(sUpper))
                    result.Add(s);
            }
            return result;
        }

        /// <summary> There is no LINQ and no List.Intersect function in .NET 2.  This is my replacement. 
        /// It works with touppered strings and therefore is slow. </summary>
        public static List<string> IntersectIgnorecase(IEnumerable<string> first, IEnumerable<string> second)
        {
            List<string> result = new List<string>();
            List<string> secondUpper = new List<string>();
            foreach (string s in second)
                secondUpper.Add(s.ToUpper());

            foreach (string s in first)
            {
                string sUpper = s.ToUpper();
                if (secondUpper.Contains(sUpper))
                    result.Add(s);
            }
            return result;
        }

        /// <summary> Gibt true zurück, wenn die beiden Listen gleich lang sind und den gleichen Inhalt haben. </summary>
        public static bool AreStringListsEqual(IList<string> first, IList<string> second)
        {
            bool ok = false;
            if (first.Count == second.Count)
            {
                ok = true;
                for (int i = 0; i < second.Count && ok; ++i)
                    if (first[i] != second[i])
                        ok = false;
            }
            return ok;
        }

        /// <summary> Gibt das "Außenprodukt" eines string mit einem IEnumerable zurück. </summary>
        /// <returns>Ein Li mit den Ergebnissen.</returns>
        public static Li<string> OuterProduct(string a, IEnumerable<string> b)
        {
            var arra = new string[] { a };
            return arra.Join(b, aa => true, bb => true, (s, z) => s + z).ToLi();
        }

        /// <summary> Gibt das "Außenprodukt" eines string mit einem IEnumerable zurück. </summary>
        /// <param name="resultSelector">Die Funktion, die ein Element der ersten mit einem Element der zweiten IEnumerable verknüpft. </param>
        /// <returns>Ein Li mit den Ergebnissen.</returns>
        public static Li<R> OuterProduct<T, R>(string a, IEnumerable<T> b, Func<string, T, R> resultSelector)
        {
            var arra = new string[] { a };
            return arra.Join(b, aa => true, bb => true, resultSelector).ToLi();
        }

        /// <summary> Gibt das "Außenprodukt" eines strings mit einem IEnumerable zurück. </summary>
        /// <param name="a">Erstes IEnumerable.</param>
        /// <param name="b">Zweites IEnumerable.</param>
        /// <param name="resultSelector">Die Funktion, die ein Element der ersten mit einem Element der zweiten IEnumerable verknüpft. </param>
        /// <returns>Ein Li mit den Ergebnissen.</returns>
        public static Li<R> OuterProduct<S, R>(IEnumerable<S> a, string b, Func<S, string, R> resultSelector)
        {
            var arrb = new string[] { b };
            return a.Join(arrb, aa => true, bb => true, resultSelector).ToLi();
        }

        /// <summary> Gibt das "Außenprodukt" zweier IEnumerables zurück. </summary>
        /// <param name="a">Erstes IEnumerable.</param>
        /// <param name="b">Zweites IEnumerable.</param>
        /// <param name="resultSelector">Die Funktion, die ein Element der ersten mit einem Element der zweiten IEnumerable verknüpft. </param>
        /// <returns>Ein Li mit den Ergebnissen.</returns>
        /// <remarks> Beispiel mit S=T=R=string. OuterProduct({"a", "b"}, {"1", "2"}, (m,n) => m + n) ergibt "a1", "a2", "b1", "b2". </remarks>
        public static Li<R> OuterProduct<S, T, R>(IEnumerable<S> a, IEnumerable<T> b, Func<S, T, R> resultSelector)
        {
            return a.Join(b, aa => true, bb => true, resultSelector).ToLi();
        }

        /// <summary> Angenommen man habe eine Zeile wie   "1; 2 asbcdfg;5"  und die Überschriften "A; b; MAL; F2", 
        /// dann versucht diese Funktion die Überschriften der Spalten so auf die Zeile zu verteilen, dass Überschriften zB im 
        /// BeyondCompare über den Spalten stehen. Und addiert dann die Überschriftszeile zur Liste. </summary>
        public static void InsertTitleForComparison(List<string> values, char separator, string title)
        {
            var t = values.Any() ? GetTitleForComparison(values[0], separator, title) : title;
            values.Insert(0, t);
        }

        /// <summary> Angenommen man habe eine Zeile wie   "1; 2 asbcdfg;5"  und die Überschriften "A; b; MAL; F2", 
        /// dann versucht diese Funktion die Überschriften der Spalten so auf die Zeile zu verteilen, dass Überschriften zB im 
        /// BeyondCompare über den Spalten stehen. </summary>
        public static string GetTitleForComparison(string line, char separator, string title)
        {
            var lparts = line.Split(new char[] { separator }).ToList();
            var tparts = title.Split(new char[] { separator }).Select(p => p.Trim()).ToList();
            if (lparts.Count != tparts.Count)
                throw new VAException("Cannot GetTitleForComparison, parts do not have equal length.");

            string t = "";
            for (int neg = 0, i = 0; i < lparts.Count; ++i)
            {
                var llength = lparts[i].Length;
                var tpart = tparts[i];
                if (tpart.Length > llength)
                    neg += tpart.Length - llength;
                else
                {
                    int j = 0;
                    while (tpart.Length + neg < llength)
                    {
                        if (j++ % 2 == 0)
                            tpart = " " + tpart;
                        else
                            tpart += " ";
                    }
                    neg = 0;
                }
                t += tpart + (i < lparts.Count - 1 ? "" + separator : "");
            }
            return t;
        }

        /// <summary> Erstellt ein Dictionary aus einer Zeichenkette. </summary>
        /// <param name="s">Zeichenkette</param>
        /// <param name="pairseperator">Paarseperator</param>
        /// <param name="keyvalueseperator">Wertpaarseperator</param>
        /// <returns>Dictionary Objekt</returns>
        public static Dictionary<string, string> DeserializeToDictionary(string s, string pairseperator, string keyvalueseperator, StringComparer sc)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(sc);
            if (!string.IsNullOrEmpty(s))
            {
                string[] keyValuePairs = s.Split(pairseperator.ToCharArray());
                foreach (string keyValue in keyValuePairs)
                {
                    if (string.IsNullOrEmpty(keyValue))
                        continue;
                    string[] kv = keyValue.Split(keyvalueseperator.ToCharArray(), 2);
                    if (kv.Length == 2)
                        result[kv[0]] = kv[1];
                }
            }
            return result;
        }

        /// <summary> Erstellt ein Dictionary aus einem String, so: Zuerst splitte den String mit SplitToWords, 
        /// und mach dann die Einträge mit index 0, 2, 4,... zu keys und die anderen zu Values. </summary>
        /// <remarks> Bsp: "a 1   c 3 "   ->   a:1,  c:3 </remarks>
        public static Dictionary<string, string> ToDictionary(string s, IEqualityComparer<string> eqComparer = null)
        {
            var d = eqComparer == null ? new Dictionary<string, string>() : new Dictionary<string, string>(eqComparer);
            var e = s.SplitToWords();
            for (int i = 0; i < e.Length - 1; i += 2)
                d.Add(e[i], e[i + 1]);
            return d;
        }

        /// <summary> Erstellt ein Dictionary aus einem String, so: Zuerst splitte den String mit SplitToLines, 
        /// und mache dann bei Zeilen, die mit "-- xyz" oder mit "xyz:"  anfangen, xyz zu einem Key und alle 
        /// folgenden Zeilen (bis zum nächsten Key) zu einem String zusammengesetzt zum Value.   </summary>
        /// <remarks> Was nach einem Key, der mit : endet, steht, wird zum Value addiert. Was nach einem Key, 
        /// der mit -- beginnt, kommt, wird ignoriert. Beispiele s. TestToDictionary2. </remarks>
        public static Dictionary<string, string> ToDictionary2(string s, bool shallDeleteEmptyClasses = true, IEqualityComparer<string> eqComparer = null)
        {
            var d = eqComparer == null ? new Dictionary<string, string>() : new Dictionary<string, string>(eqComparer);
            var lines = s.SplitToLines();
            var reStartTyp = new Regex(@"^(?:-- +(\w+)\b|(\w+):)(.*)");

            string currTyp = "";
            foreach (var line in lines)
            {
                var m = reStartTyp.Match(line);
                if (m.Success)
                {
                    string currArtikels = "";
                    if (!string.IsNullOrEmpty(m.Groups[1].Value))
                        currTyp = m.Groups[1].Value;
                    else
                    {
                        currTyp = m.Groups[2].Value;
                        currArtikels = m.Groups[3].Value;
                    }
                    d.Add(currTyp, currArtikels);
                }
                else
                    d[currTyp] += " \r\n" + line;

            }
            if (shallDeleteEmptyClasses)
                foreach (var k in d.Keys.ToLi())
                    if (string.IsNullOrWhiteSpace(d[k]))
                        d.Remove(k);
            return d;
        }

        private static void TestToDictionary2()
        {
            foreach (var shallDeleteEmpty in new bool[] { true, false })
            {
                TestToDictionary2(@"
                    IE6: 
                        IE3: a b ccc 
                                    d     e     f  g
                         IE2:a     b      ccc  d e f
                                    g
                        IE4:a      b    ccc d  e f g
                        -- IE1.Das hier ist ignorierter Kommentar. 
                            a      b    ccc d  e f g", shallDeleteEmpty);
                TestToDictionary2(@" -- IE1 

                         a b ccc d e f g

            -- IE2 
                            a b  
            
                    ccc d
                            e
                            f g
                       -- IE7
                       IE4: a b ccc d e f
                            g
                       -- IE3 
                        a b ccc d 
                        e f g 
                       -- IE6 keine", shallDeleteEmpty);
            }
        }

        private static void TestToDictionary2(string s, bool shallDeleteEmpty)
        {
            var d = ToDictionary2(s, shallDeleteEmpty);
            foreach (var k in d.Keys)
            {
                var v = string.Join(" ", d[k].SplitToWords());
                if (k.IsContainedIn("IE1 IE2 IE3 IE4".Split()))
                    Debug.Assert(v == "a b ccc d e f g");
                else
                {
                    if (shallDeleteEmpty)
                        Debug.Assert(false);
                    else
                        Debug.Assert(v == "");
                }
            }

        }

        /// <summary> Erstellt ein Dictionary aus einem String, so: Zuerst splitte den String an den lineSeparators, 
        /// das ergibt die Zeilen. Splitte dann jede Zeile am ersten keyValSeparator. Das vor dem keyValSeparator
        /// ergibt den key, das nach dem keyValSeparator ergibt den Value; jeweils getrimmt. </summary>
        public static Dictionary<string, string> ToDictionary3(string s, string lineSeparator = ";", string keyValSeparator = ":",
                bool shallDeleteEmptyClasses = true, IEqualityComparer<string> eqComparer = null)
        {
            var d = eqComparer == null ? new Dictionary<string, string>() : new Dictionary<string, string>(eqComparer);
            var lines = s.Split(new string[] { lineSeparator }, StringSplitOptions.RemoveEmptyEntries);
            var kvs = new string[] { keyValSeparator };
            foreach (var line in lines)
            {
                var parts = line.Split(kvs, 2, StringSplitOptions.None).Select(p => p.Trim()).ToLi();
                if (parts.Count == 2 && (shallDeleteEmptyClasses == false || parts[1] != ""))
                    d.Add(parts[0], parts[1]);
            }
            return d;
        }



        /// <summary> Creates a html-table from the input.</summary>
        /// <param name="styleStuff"> this string will be added to the {table} tag. May be empty or contain things like
        /// border="1" class"xyz"</param>
        /// <param name="info"></param>
        public static StringBuilder ListListToHtmlTable(string styleStuff, List<List<string>> info, int numCols)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<table " + styleStuff + " >");
            foreach (var line in info)
            {
                sb.Append("<tr>");
                for (int i = 0; i < line.Count; ++i)
                {
                    if (numCols > 1 && line.Count < numCols && i == line.Count - 1)
                        sb.Append("<td colspan=\"" + (numCols - i) + "\">");
                    else
                        sb.Append("<td>");
                    sb.Append(line[i]);
                    sb.Append("</td>");
                }
                sb.AppendLine("</tr>");
            }
            sb.Append("</table>");

            return sb;
        }

        /// <summary> Replaces every character in the string that is not allowed in filenames with a '-'. </summary>
        public static string MakeFilenameSafe(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            // Add the URI reserverd characters (from RFC3986, section 2.2), some have already caused the 
            // problems, some might cause problems in the future.
            var union = invalid.Union(":/?#[]@!$&'()*+,;=".ToCharArray()).ToLi();

            foreach (char c in union)
                fileName = fileName.Replace(c.ToString(), "-");
            return fileName;
        }

        public static FileInfo[] FindAlreadyExistingTempFiles(string directory, string baseName, string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            DirectoryInfo di = new DirectoryInfo(directory);
            var files = di.GetFiles(baseName + "-*" + extension);
            return files;
        }

        public static string GetNextTempFileName(string directory, string baseName, string extension)
        {
            bool ok = false;
            if (!extension.StartsWith("."))
                extension = "." + extension;
            FileInfo[] existing = FindAlreadyExistingTempFiles(directory, baseName, extension);
            List<string> names = existing.Select(f => f.Name).ToList();
            string fileName;
            names.Sort();
            do
            {
                fileName = null;
                var last = names.LastOrDefault();

                if (last == null)
                    fileName = Path.Combine(directory, baseName + '-' + "00" + extension);
                else
                {
                    string[] parts = last.Split(".-".ToCharArray());
                    string number = parts[parts.Length - 2];
                    uint n;
                    if (uint.TryParse(number, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out n))
                    {
                        n++;
                        fileName = Path.Combine(directory, baseName + '-' + n.ToString("X2") + extension);
                    }
                }
                if (fileName != null && !File.Exists(fileName))
                    try
                    {   // (a) 
                        using (new FileStream(fileName, FileMode.CreateNew))
                        { };
                        ok = true;
                    }
                    catch (IOException)
                    {
                        // May happen if the file has been created in between by some other process
                        // or if parsing of uint did not work.
                    }
                if (names.Any())
                    names.RemoveAt(names.Count - 1);
            } while (!ok && names.Any());
            if (!ok)
                fileName = GetTempFileName(directory, baseName, extension, "X2");
            //     ^^^ Might happen under certain circumstances. ^^^ 

            return fileName;
        }

        public static string GetTempDir()
        {
            // AW's special temp dir is E:\temp. Try this first. 
            const string myTempDir = @"E:\temp";
            string tempDir;
            if (Directory.Exists(myTempDir))
                tempDir = myTempDir;
            else
                tempDir = Path.GetTempPath();
            return tempDir;
        }

        /// <summary> Erzeugt ein neues Temp-Verzeichnis im directory.</summary>
        /// <param name="baseName"> Der Basisteil des Verzeichnisnamens. </param>
        /// <param name="directory"> Falls null oder leer wird das normale TempDir genommen. </param>
        public static string GetFreshTempDir(string baseName = "", string directory = "")
        {
            if (string.IsNullOrEmpty(baseName))
                baseName = "trax";
            if (string.IsNullOrEmpty(directory))
                directory = GetTempDir();
            string dirName;
            int nDivisor = 16, nTries = 0;
            bool ok = false;
            Exception lastException = null;
            do
            {
                int n = random.Next() % (nDivisor);
                dirName = Path.Combine(directory, baseName + '-' + n.ToString("X"));
                nDivisor = (int)(nDivisor * 1.5);
                if (!Directory.Exists(dirName))
                    try
                    {
                        Directory.CreateDirectory(dirName);
                        var fse = Directory.GetFileSystemEntries(dirName);
                        ok = fse.Length == 0;  // Falls nicht 0, war das Directory schon vorher da, ich brauch aber ein neues. 
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // May happen if the directory is existing and locked, e.g. by another process. 
                        lastException = ex;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }

            } while (!ok && nTries++ < 1000);
            if (!ok)
                throw new VAException("Cannot create temp directory. Last exception is attached.", lastException);
            return dirName;
        }

        public static string GetTempFileName(string directory, string baseName, string extension)
        {
            return GetTempFileName(directory, baseName, extension, "X");
        }

        private static string GetTempFileName(string directory, string baseName, string extension, string pattern)
        {
            if (string.IsNullOrEmpty(directory))
                directory = GetTempDir();
            if (!extension.StartsWith(".") && extension != "")
                extension = "." + extension;
            string fileName;
            int nDivisor = 16, nTries = 0;
            bool ok = false;
            Exception lastException = null;
            do
            {
                int n = random.Next() % (nDivisor);
                fileName = Path.Combine(directory,
                    baseName + '-' + n.ToString(pattern) + extension);
                nDivisor = (int)(nDivisor * 1.5);
                if (!File.Exists(fileName))
                    try
                    {   // (a) 
                        using (new FileStream(fileName, FileMode.CreateNew))
                        { };
                        ok = true;
                    }
                    catch (IOException ex)
                    {
                        // May happen if the file has been created at point (a) by some other process. 
                        lastException = ex;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // May happen if the file is existing and locked, e.g. by another process. 
                        lastException = ex;
                    }

            } while (!ok && nTries++ < 1000);
            if (!ok)
                throw new VAException("Cannot create temp file name. Last exception is attached.", lastException);
            return fileName;
        }

        public static string GetTempFileName(string baseName, string extension)
        {
            return GetTempFileName(Path.GetTempPath(), baseName, extension);
        }

        /// <summary> Löscht alle Verzeichnisse mitsamt deren Inhalt, falls möglich. Wirft nicht, falls irgendwas nicht gelöscht werden konnte. </summary>
        /// <param name="baseName"> Der Basisteil des Verzeichnisnamens. </param>
        /// <param name="directory"> Falls null oder leer wird das normale TempDir genommen. </param>
        public static void DeleteTempDirs(string baseName = "", string directory = "")
        {
            if (string.IsNullOrEmpty(baseName))
                baseName = "trax";
            if (string.IsNullOrEmpty(directory))
                directory = GetTempDir();
            var dirs = Directory.GetDirectories(directory, baseName + "-*");
            foreach (var dir in dirs)
            {
                TryDeleteContent(dir);
                try
                { Directory.Delete(dir); }
                catch (System.Exception) { }
            }
        }

        public static void DeleteTempFiles(string baseName, string extension)
        {
            DeleteTempFiles(Path.GetTempPath(), baseName, extension);
        }

        public static void DeleteTempFiles(string directory, string baseName, string extension)
        {
            if (string.IsNullOrEmpty(directory))
                directory = GetTempDir();
            if (!extension.StartsWith("."))
                extension = "." + extension;
            string pattern = baseName + "*" + extension;
            string[] files = Directory.GetFiles(directory, pattern);
            foreach (string file in files)
                if (file.Length <= 4)
                    throw new VAException("Something wrong in DeleteTempFiles. " +
                        "The exception is thrown for security reasons" + ", " + baseName + ", " + pattern);
                else
                    try
                    { File.Delete(file); }
                    catch (System.Exception) { }
        }

        /// <summary> Versucht den kompletten Inhalt eines Verzeichnisses zu löschen. </summary>
        public static void TryDeleteContent(string directory)
        {
            if (directory.Length <= 4)
                throw new VAException("Something wrong in DeleteContent. " +
                    "The exception is thrown for security reasons, " + directory);
            foreach (var file in Directory.EnumerateFiles(directory))
                try
                { File.Delete(file); }
                catch (System.Exception) { }
            foreach (var dir in Directory.EnumerateDirectories(directory))
            {
                TryDeleteContent(dir);
                try
                { Directory.Delete(dir); }
                catch (System.Exception) { }
            }
        }

        /// <summary> Erzeugt ein Verzeichnis mit einem zufälligen Namen im angegebenen Verzeichnis. 
        /// Der Name beginnt mit mit baseName und wird zurückgegeben. </summary>
        public static string CreateTempDir(string directory, string baseName)
        {
            string name = null;
            Directory.CreateDirectory(directory);
            bool ok = false;
            for (int i = 0; i < 50 && !ok; ++i)
                try
                {
                    name = GetTempFileName(directory, baseName, "");
                    // Nun existiert ein File diesen Namens, kein Directory. 
                    File.Delete(name);
                    Directory.CreateDirectory(name);
                    ok = true;
                }
                catch (Exception) { }
            if (name == null)
                throw new VAException("Could not create temp directory.");
            return name;
        }

        public static bool IsFileLocked(string fullPath)
        {
            try
            {
                FileInfo file = new FileInfo(fullPath);
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        /// <summary> Erzeugt einen MD5-Hash aus dem eingegebenen String und codiert den MD5-Hash in einem string 
        /// wobei die Basis frei gewählt werden kann. </summary>
        /// <param name="input"></param>
        /// <param name="basis">Falls null, wird eine Default-Basis verwendet, die diverse Sonderzeichen enthält, 
        ///  aber keine Tilde und kein Strichpunkt. </param>
        public static string CreateMD5(string input, string basis)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // hashBytes ist md5,  also immer 128 bit = 16 byte, sollte im folgenden also immer funktionieren. 

                // Convert the byte array to encoded string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i += 4)
                {
                    int num = BitConverter.ToInt32(hashBytes, i);
                    sb.Append(Encode(num, basis));
                }
                return sb.ToString();
            }
        }


        private const string myBasis = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+*#/-_²³!§$%&()=?ÄÖÜäöüß{}<>|^°@€.:";

        /// <summary> Kodiert einen int in einem string wobei die Basis frei gewählt werden kann. </summary>
        /// <param name="basis">Falls null, wird eine Default-Basis verwendet, die diverse Sonderzeichen enthält, 
        ///  aber keine Tilde und kein Strichpunkt. </param>
        public static string Encode(int num, string basis)
        {
            if (basis == null)
                basis = myBasis;
            string response = "";
            while (num > 0)
            {
                response += basis[num % basis.Length];
                num /= basis.Length;
            }
            return response;
        }

        static Random random = new Random();
            
    }
}