using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwiUtils
{
    /// <summary> Soll das FusionLogging aktivieren, aber es scheint nicht zu funktionieren. </summary>
    //internal class FusionLog : IDisposable
    //{
    //    public FusionLog(string path) 
    //    { 

    //        // public void RememberEnvironment()
    //        originalDevPath = Environment.GetEnvironmentVariable("DEVPATH");
    //        ExLogger.Instance.LogInfo($"DEVPATH={Ext.ToDebug(originalDevPath)}");
    //        originalFusionLog = Environment.GetEnvironmentVariable("FusionLog");
    //        originalFusionLogPath = Environment.GetEnvironmentVariable("FusionLogPath");
    //        ExLogger.Instance.LogInfo($"FusionLogPath={Ext.ToDebug(originalFusionLogPath)}");
    //        originalFusionLogFailures = Environment.GetEnvironmentVariable("FusionLogFailures");
    //        originalFusionLogBind = Environment.GetEnvironmentVariable("FusionLogBind");
    //        ExLogger.Instance.LogInfo($"FusionLog={Ext.ToDebug(originalFusionLog)} " + 
    //            $"FLF={Ext.ToDebug(originalFusionLogFailures)} FLB={Ext.ToDebug(originalFusionLogBind)}");
    //        EnableFusionLogging(path);
    //    }

    //    void EnableFusionLogging(string path)
    //    {
    //        Environment.SetEnvironmentVariable("DEVPATH", path);
    //        Environment.SetEnvironmentVariable("FusionLog", "1");
    //        Environment.SetEnvironmentVariable("FusionLogPath", path);
    //        Environment.SetEnvironmentVariable("FusionLogFailures", "1");
    //        Environment.SetEnvironmentVariable("FusionLogBind", "1");
    //    }

    //    void RestoreEnvironment()
    //    {
    //        //Environment.SetEnvironmentVariable("DEVPATH", originalDevPath);
    //        //Environment.SetEnvironmentVariable("FusionLog", originalFusionLog);
    //        //Environment.SetEnvironmentVariable("FusionLogPath", originalFusionLogPath);
    //        //Environment.SetEnvironmentVariable("FusionLogFailures", originalFusionLogFailures);
    //        //Environment.SetEnvironmentVariable("FusionLogBind", originalFusionLogBind);
    //    }

    //    readonly string originalDevPath, originalFusionLog, 
    //        originalFusionLogPath, originalFusionLogFailures, originalFusionLogBind;
    //    bool isDisposed;

    //    public void Dispose()
    //    {
    //        Dispose(disposing: true);
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!isDisposed)
    //        {
    //            if (disposing)
    //                RestoreEnvironment();
    //            isDisposed = true;
    //        }
    //    }

    //}
}
