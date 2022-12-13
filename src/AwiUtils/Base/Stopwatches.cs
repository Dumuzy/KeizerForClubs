using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AwiUtils
{
    /// <summary> An array (dictionary) of globally accessible stopwatches. </summary>
    /// <remarks> Only for debugging/profiling purposes. 
    /// USERS MUST ASSURE THREAD SAFETY THEMSELVES, THIS CLASS IS NOT THREAD SAFE.
    /// This means, if different concurrent threads access this class, strange things will
    /// happen and the stopped results will be trash. 
    /// Yet, the class is thread safe insofar as no crashes will happen in this case. </remarks>
    public static class Stopwatches
    {
        /// <summary> Starts the stopwatch with the specified name. 
        /// If such a stopwatch doesn't exist, creates one. </summary>
        /// <param name="name">The name.</param>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        static public void Start(string name)
        {
            Inc(name);
            Get(name).Start();
        }

        /// <summary> Stops the stopwatch with the specified name. 
        /// If such a stopwatch doesn't exist, creates one. </summary>
        /// <param name="name">The name.</param>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        static public void Stop(string name)
        {
            Get(name).Stop();
        }

        /// <summary> Stops the first stopwatch and starts the second. If a stopwatch doesn't exist, creates one. </summary>
        /// <param name="nameToStop">The name of the stopwatch to stop.</param>
        /// <param name="nameToStart">The name of the stopwatch to start.</param>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        static public void Next(string nameToStop, string nameToStart)
        {
            Stop(nameToStop);
            Start(nameToStart);
        }

        /// <summary> starts the stopwatch with the passed name. Stops a second stopwatch, if existing.  
        /// The second one will be created like this: Look for the last '-' character in the nameToStop, take the rest and parse it
        /// as integer, decrease the integer. </summary>
        /// <param name="nameToStart">The name of the stopwatch to stop.</param>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)")]
        static public void Next(string nameToStart)
        {
            if (string.IsNullOrEmpty(nameToStart))
                throw new ArgumentNullException(nameToStart);
            int index = nameToStart.LastIndexOf('-');
            string numberString = nameToStart.Substring(index + 1);
            int number;
            string nameToStop = null;
            if (int.TryParse(numberString, out number))
            {
                number--;
                nameToStop = nameToStart.Substring(0, index + 1) + number.ToString();
            }
            if (nameToStop != null)
                Stop(nameToStop);
            Start(nameToStart);
        }

        /// <summary> Returns the stopwatch with the specified name. 
        /// If such a stopwatch doesn't exist, creates one. </summary>
        /// <param name="name">The name.</param>
        /// <remarks>This function is thread safe insofar as it always returns a valid 
        /// watch and will not crash.
        /// It never returns null. The returned watch may be removed already from the 
        /// list of Stopwatches, though. </remarks>
        static public Stopwatch Get(string name)
        {
            Stopwatch watch;
            if (!watches.TryGetValue(name, out watch))
            {
                watch = new Stopwatch();
                // Es scheint, dass die Zeile 'watches[name] = watch' manchmal crasht, wegen  
                //       ...DataAccess.DataAccessException: 
                //       Bei der Kommunikation mit der Datenbank ist eine Ausnahme aufgetreten. ---> 
                //          System.NullReferenceException: Der Objektverweis wurde nicht auf eine Objektinstanz festgelegt.
                //   bei System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
                //   bei System.Collections.Generic.Dictionary`2.set_Item(TKey key, TValue value)
                //   bei AwiUtils.Stopwatches.Get(String name)
                //   bei AwiUtils.Stopwatches.Start(String name)
                //
                // 
                // Das ist eigentlich völlig unmöglich. Dennoch, ich habe die folgenden zwei Zeilen eingebaut, um das 
                // unmögliche Problem eventuell zu beheben. 
                if (watches == null)
                    watches = new Dictionary<string, Stopwatch>();
                watches[name] = watch;
            }
            return watch;
        }

        /// <summary> Clears all the stopwatches. </summary>
        public static void Clear() { watches.Clear(); counters.Clear(); }

        /// <summary> Prints all stopwatches with System.Diagnostics.Debug.WriteLine that mathc the passed Regex. </summary>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object,System.Object)")]
        public static void Debug(string regex = "")
        {
            System.Diagnostics.Debug.WriteLine("");
            var keys = new List<string>(watches.Keys);
            if (!string.IsNullOrEmpty(regex))
                keys.RemoveAll(k => !Regex.IsMatch(k, regex));

            keys.Sort();

            foreach (string key in keys)
                System.Diagnostics.Debug.WriteLine(ToString(key));
        }

        /// <summary> Returns a list of strings, one  for every existing stopwatch. </summary>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        public static List<string> ToLines(bool isForTable)
        {
            List<string> keys = new List<string>(watches.Keys);
            keys.Sort();
            var lines = keys.Select(k => ToString(k, isForTable)).ToList();
            lines.RemoveAll(x => x == "");
            return lines;
        }

        /// <summary> Returns a string for every existing stopwatch joined together by '\n'. </summary>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        public static new string ToString()
        {
            List<string> lines = ToLines(false);
            var s = string.Join("\n", lines.ToArray());
            return s;
        }

        /// <summary>Returns a string like "key : 24 ms @ 3 calls   8.0 ms/call". 
        /// Will create the stopwatch if it does not already exist. </summary>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        public static string ToString(string key) { return ToString(key, false); }

        /// <summary>Returns a string like "key : 24 ms @ 3 calls   8.0 ms/call, when isForTable == F. 
        /// Will create the stopwatch if it does not already exist. </summary>
        /// <remarks> This function is thread safe insofar as it won't crash. </remarks>
        public static string ToString(string key, bool isForTable)
        {
            Stopwatch sw = Get(key);
            int n;
            counters.TryGetValue(key, out n);
            if (isForTable)
                key = key.Replace(' ', '-');
            string s;
            string perCall = n != 0 ? string.Format(CultureInfo.InvariantCulture, "{0:0.0}", (double)sw.ElapsedMilliseconds / n) : "N/A";
            if (isForTable)
                s = $"Name:{key} ms:{sw.ElapsedMilliseconds} Calls:{n} ms/call:{perCall}";
            else
                s = $"{key} : {sw.ElapsedMilliseconds} ms @ {n} calls   {perCall} ms/call";
            return s;
        }


        static private void Inc(string name)
        {
            int counter;
            counters.TryGetValue(name, out counter);
            counters[name] = ++counter;
        }

        static Dictionary<string, Stopwatch> watches = new Dictionary<string, Stopwatch>();
        static Dictionary<string, int> counters = new Dictionary<string, int>();
    }
}
