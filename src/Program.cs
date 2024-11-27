using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using AwiUtils;

namespace KeizerForClubs
{
    internal sealed class Program
    {
        public static string AssemblyVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Contains("-d"))
            {
                ExLogger.Create(".", "KFC2", null, null, null);
                var largs = new Li<string>(args);
                largs.RemoveAll(s => s == "-d");
                args = largs.ToArray();
            }
            else
                ExLogger.Create(null, null, null, null, null);

            ExLogger.Instance.LogInfo("Program.Main 1.01");

            if (!File.Exists(ExLogger.Instance.LogfilePath))
            {
                ExLogger.Destroy();
                ExLogger.Create(Helper.GetTempDir(), "KFC2", null, null, null);
                ExLogger.Instance.LogWarning("Could not create log file in current directory.");
            }
            ExLogger.Instance.LogInfo($"Program.Main 1.11 KFC2 Version={AssemblyVersion}");
            LogOsInfo();
            TryLoadSqliteDll("");

            try
            {
                if (args.Contains("-fx"))
                {
                    var largs = new Li<string>(args);
                    largs.RemoveAll(s => s == "-fx");
                    args = largs.ToArray();
                    ForceBadImageException();
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)new frmMainform(args));
            }
            catch (Exception ex)
            {
                ExLogger.Instance.LogException(ex);
                ExLogger.Instance.LogInfo("Program.Main 2.1 Exception caught. Exiting.");
                throw;
            }
            ExLogger.Instance.LogInfo("Program.Main 2.0");
        }

        static void LogOsInfo()
        {
            var os = new RealOSInfo();
            ExLogger.Instance.LogInfo("OSInfo       Cmd Version=" + os.CmdVersion);
            ExLogger.Instance.LogInfo("OSInfo      WMIC Caption=" + os.WmicCaption);
            ExLogger.Instance.LogInfo("OSInfo      WMIC Version=" + os.WmicVersion);
            ExLogger.Instance.LogInfo("OSInfo  WMIC Architecture=" + os.WmicOSArchitecture);
            ExLogger.Instance.LogInfo("OSInfo  WMIC ServicePacks=" + os.WmicServicePack);

        }

        static void ForceBadImageException()
        {
            TryLoadSqliteDll("");
            TryLoadSqliteDll(".w32");
            TryLoadSqliteDll(".x64");
        }

        static void TryLoadSqliteDll(string endOfName)
        {
            try
            {
                string file = "System.Data.SQLite" + endOfName + ".dll";
                var assembly = Assembly.GetExecutingAssembly();
                string p = Path.GetDirectoryName(assembly.Location);
                string fullPath = Path.Combine(p, file);
                ExLogger.Instance.LogInfo($"Trying to load {fullPath} by Assembly.LoadFile.");
                var a0 = Assembly.LoadFile(fullPath);
                ExLogger.Instance.LogInfo($"Succeeded loading {file} by Assembly.LoadFile.");
            }
            catch (Exception ex)
            {
                ExLogger.Instance.LogException(ex);
            }
        }

    }
}
