using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                swExport.WriteLine(KfcFooter + "   " + DateHeader);
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
            return sb;
        }

        public string ExportAsHtml(TableW2Headers t, string fileBase, bool isExWrapper = false)
        {
            string fileName = fileBase + ".html";
            using (var swExport = new StreamWriter(fileName))
            {
                AddHtmlHeader(swExport, isExWrapper);
                swExport.WriteLine($"<h1>{t.Header1}</h1>");
                swExport.WriteLine($"<h2>{t.Header2}</h2>");
                swExport.WriteLine("<table>");
                for (int i = 0; i < t.Count; ++i)
                {
                    var row = t[i];
                    swExport.Write("<tr>");
                    foreach (var c in row)
                        swExport.Write("<td>" + c + "</td>");
                    swExport.WriteLine("</tr>");
                }
                swExport.WriteLine(KfcFooterHtml);
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
                swExport.WriteLine(KfcFooter + " " + DateTime.Now.ToShortDateString());
                swExport.WriteLine("</footer>");
                swExport.WriteLine("</export>");
                swExport.WriteLine($"</{toplevelName}>");
            }
            return fileName;
        }

        protected virtual string KfcFooter { get; }
        protected virtual string KfcFooterHtml { get; }
        protected string DateHeader => DateTime.Now.ToShortDateString();

        protected void AddHtmlHeader(StreamWriter sw, bool isEx = false)
        {
            var ex = isEx ? "ex" : "";
            sw.WriteLine($@"<!DOCTYPE html><html><head>
                <link rel=""stylesheet"" href=""keizer.css""></head><body><div id=""{ex}wrapper"">");
        }

        protected void AddHtmlFooter(StreamWriter sw) => sw.WriteLine(@"</div></body></html>");

    }

    public interface ISimpleTable
    {
        public void AddRow(Li<string> row);
        public int Count { get; }
        public Li<string> this[int i] { get; }
    }

    public class TableW2Headers : ISimpleTable
    {
        public string Header1, Header2;
        public void AddRow(Li<string> row) => rows.Add(row);
        public int Count => rows.Count;
        public Li<string> this[int i] { get { return rows[i]; } }

        Li<Li<string>> rows = new Li<Li<string>>();
    }
}
