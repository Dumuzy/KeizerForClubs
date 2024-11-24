using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AwiUtils
{
    /// <summary>
    /// Leider, diese "offizielle" OS-Version kann falsch sein, je nachdem, wo und wie das 
    /// Programm gestartet wird. OperatingSystem os = Environment.OSVersion; 
    /// Darum hab ich diese RealOSInfo gebaut, die nach allem was ich bisher getestet hab, immer korrekt ist. 
    /// </summary>
    internal class RealOSInfo
    {
        public RealOSInfo()
        {
            try
            {
                wmicValues = GetWmicOsInfos();
                WmicCaption = wmicValues["Caption"];
                WmicOSArchitecture = wmicValues["OSArchitecture"];
                WmicVersion = wmicValues["Version"];
                WmicServicePack = wmicValues["ServicePackMajorVersion"] + "." + wmicValues["ServicePackMinorVersion"];
            }
            catch { }

            try
            {
                CmdVersion = GetCmdVer();
            }
            catch { }
        }

        public readonly string CmdVersion;
        public readonly string WmicCaption;
        public readonly string WmicOSArchitecture;
        public readonly string WmicVersion;
        public readonly string WmicServicePack;

        public override string ToString() => $"CmdVer=${CmdVersion} WmicCap={WmicCaption} WmicArch={WmicOSArchitecture}";

        string GetCmdVer()
        {
            var verLines = GetLinesFromCmd("ver");
            return verLines[0];
        }

        Dictionary<string, string> GetWmicOsInfos()
        {
            var wmicLines = GetLinesFromCmd("wmic os get Version,OSArchitecture,Caption,ServicePackMajorVersion,ServicePackMinorVersion");
            wmicValues = GetWmicOsInfos(wmicLines);
            return wmicValues;
        }

        static Li<string> GetLinesFromCmd(string cmd)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + cmd,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            Li<string> lines = new();

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
                lines.Add(proc.StandardOutput.ReadLine());
            lines.RemoveAll(li => string.IsNullOrWhiteSpace(li));
            return lines;
        }


        static Dictionary<string, string> GetWmicOsInfos(Li<string> lines)
        {
            Dictionary<string, string> d = new();
            var headers = lines[0];
            var values = lines[1];

            // Die Rückgabe von wmic sind idR 2 Zeilen mit den Namen in der 1. Zeile und den 
            // Werten in der 2. Zeile. Überschrift oder Wert wird mit genug Leerzeichen aufgefüllt, so daß
            // Überschrift+Leerzeichen genauso lang wie Wert+Leerzeichen ist. 
            var ms = Regex.Matches(headers, @"\w+\s+");
            foreach (Match m in ms)
            {
                var h = m.Groups[0].Value;
                var v = values.Substring(0, h.Length);
                values = values.Substring(h.Length);
                d.Add(h.Trim(), v.Trim());
            }
            return d;
        }

        Dictionary<string, string> wmicValues;
    }
}
