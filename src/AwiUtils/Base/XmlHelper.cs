using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace AwiUtils
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für XmlHelper
    /// </summary>
    public class XmlHelper
    {
        static public void DebugXmlString(string title, string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            string s2 = title + Beautify(xmlDoc);
            System.Diagnostics.Debug.WriteLine(s2);
        }

        static public string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        static public string FormatXml(string xml, int maxLineLength)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                string x = doc.ToString();

                // Here we have a nicely formatted xml, but with too long lines. 
                var k = BreakLongLines(x.Split("\r\n".ToCharArray()), maxLineLength);
                x = string.Join("\n", k.ToArray());
                return x;
            }
            catch (Exception)
            {
                return xml;
            }
        }

        static List<string> BreakLongLines(IEnumerable<string> lines, int maxLineLength)
        {
            List<string> newLines = new List<string>();
            const string spaces = "                                                                              ";
            foreach (var s in lines)
            {
                if (s.Length == 0)
                    continue;
                if (s.Length <= maxLineLength)
                    newLines.Add(s);
                else
                {
                    int nPrefix = s.Length - s.TrimStart().Length;
                    var parts = new Queue<string>(s.Trim().Split(" ".ToCharArray()));
                    bool isFirstLinePart = true;

                    while (parts.Count > 0)
                    {
                        string z = spaces.Substring(0, nPrefix) + parts.Dequeue();

                        if (parts.Count == 0 || (z.Length + " " + parts.Peek()).Length > maxLineLength)
                        { } // Do nothing
                        else
                            while (parts.Count > 0)
                            {
                                if ((z + " " + parts.Peek()).Length <= maxLineLength)
                                    z += " " + parts.Dequeue();
                                else
                                    break;
                            }
                        newLines.Add(z);

                        if (isFirstLinePart)
                        {
                            isFirstLinePart = false;
                            nPrefix += 2;
                        }
                    }
                }
            }
            return newLines;
        }


    }

}