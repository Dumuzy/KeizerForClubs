using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeizerForClubs;

namespace AwiUtils
{
    [Flags]
    public enum ReportingFlags { None, 
        Ex, // Kreuztabelle statt normalem Tabellenstand 
        Podium, // Podiumstablle statt normalem Tabellenstand 
    }

    public class ReportingBase
    {
        public string ExportAsTxt(TableW2Headers t, string fileBase, int[] paddings)
        {
            string fileName = fileBase + ".txt";
            using (var swExport = new StreamWriter(fileName))
            {
                swExport.WriteLine(t.Header1);
                swExport.WriteLine(t.Header2);
                swExport.WriteLine(" ");
                for (int i = 0; i < t.Count; ++i)
                {
                    var line = "";
                    for (int j = 0; j < t[i].Count; ++j)
                    {
                        var c = t[i][j];
                        if (paddings[j] >= 0)
                            line += c.PadLeft(paddings[j]);
                        else
                            line += c.PadRight(-paddings[j]);
                        if (!line.EndsWith(" "))
                            line += " ";
                    }
                    swExport.WriteLine(line);
                }
                swExport.WriteLine("");
                var f = KfcFooter.Replace("Footer0", t.Footer[0]).Replace("Footer1", t.Footer[1]);
                swExport.WriteLine(f);
            }
            return fileName;
        }

        public string ExportAsCsv(TableW2Headers t, string fileBase)
        {
            var sb = ToStringBuilderAsCsv(t);
            string fileName = fileBase + ".csv";
            File.WriteAllText(fileName, sb.ToString());
            return fileName;
        }

        public StringBuilder ToStringBuilderAsCsv(TableW2Headers t)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(t.Header1);
            sb.AppendLine(t.Header2);
            sb.AppendLine("");
            for (int i = 0; i < t.Count; ++i)
            {
                var s = string.Join(';', t[i]);
                sb.AppendLine(s);
            }
            var f = KfcFooter.Replace("Footer0", t.Footer[0]).Replace("Footer1", t.Footer[1]);
            sb.AppendLine(f);
            return sb;
        }

        public string ExportAsHtml(TableW2Headers t, string fileBase,
            ReportingFlags flags = ReportingFlags.None,
            IList<string> headerTitles = null)
        {
            string fileName = fileBase + ".html";
            using (var swExport = new StreamWriter(fileName))
            {
                AddHtmlHeader(swExport, t, flags);
                swExport.WriteLine($"<h1 class='kfc-h1'><span>{t.Header1}</span></h1>");
                swExport.WriteLine($"<h2 class='kfc-h2'><span>{t.Header2}</span></h2>");
                swExport.WriteLine("<table>");
                swExport.WriteLine("<colgroup>");
                for (int i = 0; i < t.ColsCount; ++i)
                    swExport.Write($"<col class='kfc-col{i + 1}' />");
                swExport.WriteLine("</colgroup>");
                for (int i = 0; i < t.Count; ++i)
                {
                    var row = t[i];
                    var n = i > 3 ? "n" : i.ToString();
                    swExport.Write($"<tr class='kfc-place-{n}'>");
                    for (int j = 0; j < row.Count; ++j)
                    {
                        var td = "<td>";
                        if (i == 0 && headerTitles != null && headerTitles.Count > j && headerTitles[j] != "")
                            td = $"<td title=\"{headerTitles[j]}\">";
                        swExport.Write(td + row[j] + "</td>");
                    }
                    swExport.WriteLine("</tr>");
                }
                var f = KfcFooterHtml.Replace("Footer0", t.Footer[0]).Replace("Footer1", t.Footer[1]);
                swExport.WriteLine(f);
                swExport.WriteLine("</table>");
                AddHtmlFooter(swExport);
            }
            return fileName;
        }

        public string ExportAsXml(TableW2Headers t, string fileBase, string toplevelName,
                string rowName, IList<string> tdnames)
        {
            var fileName = fileBase + ".xml";
            using (var swExport = new StreamWriter(fileName))
            {
                swExport.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                swExport.WriteLine($"<?xml-stylesheet type='text/xsl' href='{toplevelName}.xsl' ?>");
                swExport.WriteLine($"<{toplevelName}>");
                swExport.WriteLine("<export>");
                swExport.WriteLine($"<tournament>{t.Header1}</tournament>");
                swExport.WriteLine($"<title>{t.Header2}</title>");
                for (int i = 0; i < t.Count; ++i)
                {
                    swExport.Write($"<{rowName}>");
                    for (int j = 0; j < t[i].Count; ++j)
                        swExport.Write($"<{tdnames[j]}>{t[i][j]}</{tdnames[j]}>");
                    swExport.WriteLine($"</{rowName}>");
                }
                swExport.WriteLine("<footer>");
                var f = KfcFooter.Replace("Footer0", t.Footer[0]).Replace("Footer1", t.Footer[1]);
                swExport.WriteLine(f);
                swExport.WriteLine("</footer>");
                swExport.WriteLine("</export>");
                swExport.WriteLine($"</{toplevelName}>");
            }
            return fileName;
        }

        protected virtual string KfcFooter { get; }
        protected virtual string KfcFooterHtml { get; }
        protected string DateHeader => DateTime.Now.ToShortDateString();

        private string GetCss()
        {
            var fn = "export/keizer.css";
            if (File.Exists(fn))
                if (css == null || cssDateTime < File.GetLastWriteTime(fn))
                {
                    css = File.ReadAllText(fn);
                    cssDateTime = File.GetLastWriteTime(fn);
                }
            return css ?? "";
        }

        private string GetPodiumCss()
        {
            var fn = "export/podium.css";
            if (File.Exists(fn))
                if (podiumCss == null || podiumCssDateTime < File.GetLastWriteTime(fn))
                {
                    podiumCss = File.ReadAllText(fn);
                    podiumCssDateTime = File.GetLastWriteTime(fn);
                }
            return podiumCss ?? "";
        }

        private string GetUserCss()
        {
            var fn = "export/user.css";
            if (File.Exists(fn))
                if (userCss == null || userCssDateTime < File.GetLastWriteTime(fn))
                {
                    userCss = File.ReadAllText(fn);
                    userCssDateTime = File.GetLastWriteTime(fn);
                }
            return userCss ?? "";
        }

        static string css = null, podiumCss = null, userCss = null;
        static DateTime cssDateTime = new DateTime(), podiumCssDateTime = new DateTime(), userCssDateTime = new DateTime();

        protected string RemoveProblematicChars(string s) => Regex.Replace(s, "[^-_A-Za-z0-9]", "");

        static readonly Dictionary<string, string> TableType2Id = Helper.ToDictionary(@"None none 
            Stand stand Kreuz cross Spieler player Paarungen pairs");

        protected string GetHtmlId(TableW2Headers t)
        {
            var tt = TableType2Id[t.TableType.ToString()];
            var s = RemoveProblematicChars($"kfc-{t.Header1}-{tt}-{t.Runde}");
            return s;
        }

        protected void AddHtmlHeader(StreamWriter sw, TableW2Headers t,
            ReportingFlags flags = ReportingFlags.None)
        {
            var ex = Ext.HasFlag(flags, ReportingFlags.Ex) ? "ex" : "";
            var podium = Ext.HasFlag(flags, ReportingFlags.Podium) ? GetPodiumCss() : "";

            sw.WriteLine($@"
<!DOCTYPE html><html><head><meta charset='UTF-8'>
<style>
{GetCss()}
{podium}
{GetUserCss()}
</style></head>
<body>

<div id='{GetHtmlId(t)}' class='my-{ex}wrapper kfc-{ex}wrapper kfc-{ex}wrapper-{t.ColsCount:00}'>");
        }

        protected void AddHtmlFooter(StreamWriter sw) => sw.WriteLine(@"</div>

</body></html>");

    }

    public interface ISimpleTable
    {
        public void AddRow(Li<string> row);
        public int Count { get; }
        public Li<string> this[int i] { get; }
    }

    public class TableW2Headers : ISimpleTable
    {
        public TableW2Headers(string header1, TableType tt, int runde)
        {
            Header1 = header1;
            TableType = tt;
            Runde = runde;
        }
        public string Header1, Header2;
        public readonly TableType TableType;
        public readonly int Runde;
        public Li<string> Footer;
        public void AddRow(Li<string> row) => rows.Add(row);
        public void RemoveColAt(int idx)
        {
            if (idx < ColsCount && idx >= 0)
                foreach (var li in rows)
                    li.RemoveAt(idx);
        }
        public void RemoveRowAt(int row) => rows.RemoveAt(row);
        public void ClearRows() => rows.Clear();
        public int Count => rows.Count;
        public Li<string> this[int i] { get { return rows[i]; } }
        public int ColsCount => rows.FirstOrDefault()?.Count ?? 0;

        Li<Li<string>> rows = new Li<Li<string>>();
    }

}
