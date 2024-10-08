﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeizerForClubs;

namespace AwiUtils
{
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

        public string ExportAsHtml(TableW2Headers t, string fileBase, bool isExWrapper = false, 
            IList<string> headerTitles = null)
        {
            string fileName = fileBase + ".html";
            using (var swExport = new StreamWriter(fileName))
            {
                AddHtmlHeader(swExport, t, isExWrapper);
                swExport.WriteLine($"<h1>{t.Header1}</h1>");
                swExport.WriteLine($"<h2>{t.Header2}</h2>");
                swExport.WriteLine("<table>");
                for (int i = 0; i < t.Count; ++i)
                {
                    var row = t[i];
                    swExport.Write("<tr>");
                    for(int j = 0; j < row.Count; ++j)
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
            if (css == null)
                css = File.ReadAllText("export/keizer.css");
            return css;
        }
        static string css = null;

        protected string RemoveProblematicChars(string s) => Regex.Replace(s, "[^-_A-Za-z0-9]", "");

        static readonly Dictionary<string, string> TableType2Id = Helper.ToDictionary(@"None none 
            Stand stand Kreuz cross Spieler player Paarungen pairs");

        protected string GetHtmlId(TableW2Headers t)
        {
            var tt = TableType2Id[t.TableType.ToString()];
            var s = RemoveProblematicChars($"kfc-{t.Header1}-{tt}-{t.Runde}");
            return s;
        }

        protected void AddHtmlHeader(StreamWriter sw, TableW2Headers t, bool isEx = false)
        {
            var ex = isEx ? "ex" : "";

            sw.WriteLine($@"
<!DOCTYPE html><html><head><meta charset='UTF-8'>
<style>
{GetCss()}
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
        public TableW2Headers(string header1, TableType  tt, int runde) {
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
        public int Count => rows.Count;
        public Li<string> this[int i] { get { return rows[i]; } }
        public int ColsCount => rows.FirstOrDefault()?.Count ?? 0;

        Li<Li<string>> rows = new Li<Li<string>>();
    }

}
