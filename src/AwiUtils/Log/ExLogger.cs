using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AwiUtils
{
    public interface IExLogger
    {
        void LogException(Exception ex);
        void LogException(string message, Exception ex);
        void LogError(string message);
        void LogWarning(string message);
        void LogInfo(string message);
        string LogfilePath { get; }
    }

    public class ExLogger : IExLogger
    {
        /// <summary> Creates  the ExLogger-instance. </summary>
        /// <param name="logfileDir">Directory, where the logfiles shall be written. Default is $TEMP.</param>
        /// <param name="logfileBase">Basename of logfile. May be extended with .log and other characters. Default is ExLogger.</param>
        /// <param name="getUserAddressFunc">A static function which can be called to get the address of the current user. </param>
        /// <param name="getRawUrlFunc">A static function which can be called to get the rawurl of the last web request. </param>
        /// <param name="getLogFlagsFunc">A static function which can be called to get the currently set logflags. </param>
        public static void Create(string logfileDir, string logfileBase, Func<string> getUserAddressFunc, Func<string> getRawUrlFunc, Func<int> getLogFlagsFunc)
        {
            if (instance == null || instance.getUserAddressFunc == null)
            {
                instance = new ExLogger(logfileDir, logfileBase, getUserAddressFunc, getRawUrlFunc, getLogFlagsFunc);
                Trace.Listeners.Clear();
                // EventLogTraceListener does not work currently, man hat irgendwelche Rechte nicht, weshalb diese Exception fliegt. 
                // System.Security.SecurityException: Die Quelle wurde nicht gefunden, aber einige oder alle Ereignisprotokolle konnten 
                // nicht durchsucht werden. Protokolle, auf die kein Zugriff möglich war: Security
                // Trace.Listeners.Add(new EventLogTraceListener("VAServer"));   
                // This one writes to the given logfile: 
                Trace.Listeners.Add(new TextWriterTraceListener(instance.LogfilePath));
                // This one writes to the output-window in VS: 
                Trace.Listeners.Add(new System.Diagnostics.DefaultTraceListener());
                // Trace.Listeners.Add(new ConsoleTraceListener());
                Trace.AutoFlush = true;
                instance.LogInfo("ExLogger created.");
            }
        }

        /// <summary> Must be called before a new one can be created with the Create function. </summary>
        public static void Destroy() { instance.getUserAddressFunc = null; }

        public static ExLogger Instance { get { return instance; } }

        public bool IsNullLogger { get { return logfileDir == null && logfileBase == null && getUserAddressFunc == null; } }

        private ExLogger(string logfileDir, string logfileBase, Func<string> getUserAddressFunc, Func<string> getRawUrlFunc, Func<int> getLogFlagsFunc)
        {
            this.logfileDir = logfileDir;
            this.logfileBase = logfileBase;
            this.getUserAddressFunc = getUserAddressFunc;
            this.getRawUrlFunc = getRawUrlFunc;
            this.getLogFlagsFunc = getLogFlagsFunc;
        }

        /// <summary> Gibt Information über das Logfile in einer String-Liste zurück, die so aufgebaut ist:
        /// Zuerst kommen einige Zeilen mit Key-Value-Paaren, die mit KeyValueSeparator getrennt sind. 
        /// Dann kommt eine Zeile mit dem LinesFromInfoseparator, danach kommen die letzten n = maxLinesOfFile vom Logfile. </summary>
        /// <param name="maxLinesOfFile">Maximale Anzahl der Zeilen, die vom Logfile selber zurückgegeben werden. </param>
        public List<string> GetLogfileInfo(int maxLinesOfFile)
        {
            ExLogger e = ExLogger.Instance;
            List<string> info = new List<string>();

            if (!IsNullLogger)
            {
                info.Add("Logfile: " + e.LogfilePath);
                FileInfo fi = new FileInfo(e.LogfilePath);
                if (fi.Exists)
                {
                    info.Add("LastWrite: " + fi.LastWriteTime.ToString("yyyyMMdd  HH:mm:ss"));
                    info.Add("kBytes: " + fi.Length / 1000);

                    int nLines;
                    IEnumerable<string> lines = ReadLogfile(maxLinesOfFile, out nLines);
                    info.Add("Lines: " + nLines);
                    info.Add(LinesFromInfoSeparator);
                    info.AddRange(lines);
                }
            }
            else
                info.Add("Logfile: (null)");
            return info;
        }

        public const string LinesFromInfoSeparator = "----";
        public const string KeyValueSeparator = ":";

        #region IExLogger Member

        public void LogException(Exception ex)
        {
            LogError(ex.ToString());
            var bife = ex as System.BadImageFormatException;
            if (bife != null)
                LogBadImageFormatException(bife);
        }

        public void LogException(string message, Exception ex) { LogError((message ?? "(null)") + " " + ex.ToString()); }

        public void LogError(string message)
        {
            if (!IsNullLogger)
                Trace.TraceError(Decorate(message));
        }

        public void LogWarning(string message)
        {
            if (!IsNullLogger)
                Trace.TraceWarning(Decorate(message));
        }

        public void LogInfo(string message)
        {
            if (!IsNullLogger)
                Trace.TraceInformation(Decorate(message));
        }

        /// <summary> Wenn von diesen Flags in den GlobalSettings gesetzt sind, werden entsprechende LogOnFlag-Messages 
        /// geloggt. Sonst nicht.  Sollen nur zum temporären Debuggen verwendet und nur temporär angeschaltet werden. 
        /// Vor allem zum Debuggen von Ausdrucken aus dem Jasper nützlich.  </summary>
        [Flags]
        public enum Flags
        {
            None = 0,
            Table = 1,      // Für das Loggen von Tabellen für die Datenblätter etc.  
            Graphic = 2,    // Für das Loggen von Grafiken für die Datenblätter etc.  
        }

        /// <summary> Schreibt die Message in das Logfile, falls eines der übergebenen Logflags gesetzt ist. </summary>
        public void LogOnFlag(Flags flags, string message)
        {
            if (IsAnyOfFlagsSet(flags))
                LogInfo(message);
        }

        /// <summary> Schreibt den Buffer in ein File in Subdir unterhalb des Log-Directories, falls eines der Logflags gesetzt ist. </summary>
        public void LogToFileOnFlag(Flags flags, string message, Byte[] buffer, string subdir, string baseName, string extension)
        {
            if (IsAnyOfFlagsSet(flags))
                LogToFile(message, buffer, subdir, baseName, extension);
        }

        /// <summary> Schreibt den Buffer in ein File in Subdir unterhalb des Log-Directories. 
        /// Der Filename wird per GetTempFilename anhand von baseName und extension erzeugt. </summary>
        /// <param name="message"> Diese Message wird ins Logfile geschrieben, ergänzt um 'file written to Filename'. </param>
        /// <remarks> Falls buffer == null, wird kein File geschrieben, nur eine Message ins Logfile. </remarks>
        public void LogToFile(string message, Byte[] buffer, string subdir, string baseName, string extension)
        {
            if (buffer != null)
            {
                var dir = Path.Combine(Path.GetDirectoryName(LogfilePath), subdir);
                Directory.CreateDirectory(dir);
                var fn = Helper.GetTempFileName(dir, baseName, extension);
                System.IO.File.WriteAllBytes(fn, buffer);
                ExLogger.Instance.LogInfo((message ?? "") + "  file written to " + Ext.ToDebug(fn));
            }
            else
                ExLogger.Instance.LogInfo((message ?? "") + "  buffer is null");
        }


        private bool IsAnyOfFlagsSet(Flags flags) => ((getLogFlagsFunc?.Invoke() ?? 0) & (int)flags) != 0;

        public string LogfilePath
        {
            get
            {
                if (logfilePath == null)
                {
                    string directory = string.IsNullOrEmpty(logfileDir) ? Path.GetTempPath() : logfileDir;
                    Directory.CreateDirectory(directory);
                    string basename = string.IsNullOrEmpty(logfileBase) ? "ExLogger" : logfileBase;
                    string extension = ".log";
                    FileInfo fi = FindAlreadyExistingLogfileSmallerThan(directory, basename, extension, 10 * 1000 * 1000);
                    if (fi != null)
                        logfilePath = fi.FullName.Replace("\\", "/");
                    else
                        logfilePath = Helper.GetNextTempFileName(directory, basename, extension).Replace("\\", "/");
                }
                return logfilePath;
            }
        }

        #endregion

        #region private stuff

        private void LogBadImageFormatException(System.BadImageFormatException ex)
        {
            string s = "";
            s += $"\nException Type={ex.GetType().FullName}";
            s += $"\nFileName={ex.FileName}";
            s += $"\nFusionLog={ex.FusionLog}";
            s += $"\nSource={ex.Source}";
            s += "\nInner Exception=" + (ex.InnerException?.ToString() ?? "None");
            s += "\nTarget Site=" + (ex.TargetSite?.ToString() ?? "None");
            s += "\nHResult=0x" + ex.HResult.ToString("X");
            s += "\nData=" + (ex.Data.Count > 0 ? ex.Data : "None");
            var lines = s.Split('\n');
            foreach (var li in lines)
                LogError("   BIFE " + li);
        }

        private FileInfo FindAlreadyExistingLogfileSmallerThan(string directory, string basename, string extension, int maxSize)
        {
            FileInfo f = null;
            try
            {
                var files = Helper.FindAlreadyExistingTempFiles(directory, basename, extension);
                Array.Sort(files, (f1, f2) => f1.Name.CompareTo(f2.Name));
                foreach (var fi in files)
                    if (fi.Length < maxSize)
                    {
                        f = fi;
                        break;
                    }
            }
            catch (System.Exception) { }
            return f;
        }

        /// <summary> Liest das Logfile und gibt die n = maxLines von hinten zurück.</summary>
        /// <param name="maxLines"></param>
        /// <param name="nLines">Da wird reingeschrieben, wieviele Zeilen das Logfile wirklich hat. </param>
        /// <returns></returns>
        private IEnumerable<string> ReadLogfile(int maxLines, out int nLines)
        {
            FixedSizedQueue<string> lines = new FixedSizedQueue<string>(maxLines);
            nLines = -1;
            try
            {
                using (var fs = new FileStream(LogfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    nLines = 0;
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        nLines++;
                        lines.Enqueue(line);
                    }
                }
            }
            catch (System.Exception) { }
            return lines;
        }

        private string GetRawUrlShortInfo()
        {
            string ru = getRawUrlFunc != null ? getRawUrlFunc() : "//?si=-----&way=-&editway=-";
            var parts = ru.Split("?&".ToCharArray());
            // Erster Teil ist die Adresse, die ignorier ich hier. Restliche Teile sind Parameter. 
            var kvSplitter = "=".ToCharArray();
            string[] values = new string[] { "-----", "-", "-" };
            foreach (var part in parts)
            {
                var kv = part.Split(kvSplitter, 2);
                if (kv.Length == 2)
                    switch (kv[0])
                    {
                        case "si": values[0] = kv[1]; break;
                        case "way": values[1] = kv[1]; break;
                        case "editway": values[2] = kv[1]; break;
                    }
            }

            string s = values[0].Substring(0, Math.Min(5, values[0].Length)) + ".." + values[1] + values[2];
            return s;
        }

        private string Decorate(string message)
        {
            if (message == null)
                message = "(null)";
            string s;
            try
            {
                string address = getUserAddressFunc != null ? getUserAddressFunc() : "00000.000";
                if (address == null)
                    address = "00000.000";
                string timestamp = DateTime.Now.ToString("yyyyMMdd HHmmss");
                string r = getRawUrlFunc == null ? "" : " " + GetRawUrlShortInfo();
                s = timestamp + " : " + address + r + " : " + message;
            }
            catch (System.Exception) { s = " : : " + message; }
            return s;
        }

        private Func<string> getUserAddressFunc, getRawUrlFunc;
        private string logfileDir, logfileBase, logfilePath;
        private Func<int> getLogFlagsFunc;


        // Makes sure, that an instance always exists, so that a call of ExLogger.Instance.xyz won't throw.
        static ExLogger()
        {
            instance = new ExLogger(null, null, null, null, null);
        }

        static private ExLogger instance;
        #endregion private stuff
    }
}
