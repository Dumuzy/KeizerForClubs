using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using AwiUtils;

namespace KeizerForClubs
{
    internal sealed class Program
    {
        static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        static string AssemblyDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            var largs = args.ToLi();
            bool shallUseLogging = largs.Contains("-d");
            if (shallUseLogging)
            {
                largs.RemoveAll(s => s == "-d");
                ExLogger.Create(".", "KFC2", null, null, null);
            }
            else
                ExLogger.CreateDummy();

            InnerMain(largs);
        }

        private static void InnerMain(Li<string> largs)
        {
            ExLogger.Instance.LogInfo("Program.Main 1.01");

            if (!File.Exists(ExLogger.Instance.LogfilePath))
            {
                ExLogger.Destroy();
                ExLogger.Create(Helper.GetTempDir(), "KFC2", null, null, null);
                ExLogger.Instance.LogWarning("Could not create log file in current directory.");
            }
            ExLogger.Instance.LogInfo($"Program.Main 1.11 KFC2 Version={AssemblyVersion}");
            LogOsInfo();
            sqliteAssembly = TryLoadSqliteDll(null);

            try
            {
                if (largs.Contains("-fx"))
                {
                    largs.RemoveAll(s => s == "-fx");
                    ForceBadImageException();
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)new frmMainform(largs));
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
            var devPath = Environment.GetEnvironmentVariable("DEVPATH");
            ExLogger.Instance.LogInfo($"DEVPATH={Ext.ToDebug(devPath)}");  // Might be a problem.
        }

        static void ForceBadImageException()
        {
            sqlitew32 = TryLoadSqliteDll(".w32");
            sqlitex64 = TryLoadSqliteDll(".x64");
        }

        static Assembly TryLoadSqliteDll(string endOfName, string name = null)
        {
            Assembly assem = null;
            try
            {
                string file = name ?? "System.Data.SQLite" + (endOfName ?? "") + ".dll";
                string fullPath = Path.Combine(AssemblyDir, file);
                ExLogger.Instance.LogInfo($"Trying to load {fullPath} by Assembly.LoadFile.");
                assem = Assembly.LoadFrom(fullPath);
                ExLogger.Instance.LogInfo($"Succeeded loading {file} by Assembly.LoadFile.");
            }
            catch (Exception ex)
            {
                ExLogger.Instance.LogException(ex);
            }
            return assem;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("System.Data.SQLite"))
            {
                ExLogger.Instance.LogInfo($"CurrentDomain_AssemblyResolve({args.Name})");
                return TryLoadSqliteDll("");
            }
            return null;
        }

        // Some stuff bc T82. 
        static Assembly sqliteAssembly, sqlitex64, sqlitew32;
    }

}
