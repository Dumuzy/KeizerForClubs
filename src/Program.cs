using System;
using System.Windows.Forms;
using AwiUtils;

namespace KeizerForClubs
{
    internal sealed class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "-d")
            {
                ExLogger.Create(".", "KFC2", null, null, null);
                var largs = new Li<string>(args);
                largs.RemoveAt(0);
                args = largs.ToArray();
            }
            else
                ExLogger.Create(null, null, null, null, null);

            if (!File.Exists(ExLogger.Instance.LogfilePath))
            {
                ExLogger.Destroy();
                ExLogger.Create(Helper.GetTempDir(), "KFC2", null, null, null);
                ExLogger.Instance.LogWarning("Could not create log file in current directory.");
            }
            ExLogger.Instance.LogInfo("Program.Main 1.1");
            try
            {
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
    }
}
