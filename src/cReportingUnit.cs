using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;
using AwiUtils;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace KeizerForClubs
{
    interface ISimpleTable
    {
        public void AddRow(Li<string> row);
        public int Count { get; }
        public Li<string> this[int i] { get; }
    }

    class TableW2Headers : ISimpleTable
    {
        public string Header1, Header2;
        public void AddRow(Li<string> row) => rows.Add(row);
        public int Count => rows.Count;
        public Li<string> this[int i] { get { return rows[i]; } }

        Li<Li<string>> rows = new Li<Li<string>>();
    }

    public class cReportingUnit
    {
        string sTurnier = "";
        StreamWriter swExport;
        public StreamWriter swExportDump;

        public cReportingUnit(string sTurniername) => this.sTurnier = sTurniername;

        #region Paarungen 
        private string GetPaarungenBasename(int runde, cSqliteInterface sqlintf) => "export\\" + this.sTurnier + "_" +
            sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + "-" + runde;

        public bool fReport_Paarungen(int runde, cSqliteInterface sqlintf)
        {
            try
            {
                var table = fReport_Paarungen_Table(runde, sqlintf);
                var fileBase = GetPaarungenBasename(runde, sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    fReport_Paarungen_Xml(runde, sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    var file = fReport_Table_Html(table, sqlintf, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (sqlintf.fGetConfigBool("OPTION.Txt"))
                    fReport_Table_Txt(table, sqlintf, fileBase, new int[] {4, -20, -20, 0});
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        void fReport_Paarungen_Xml(int runde, cSqliteInterface sqlintf)
        {
            string text = sqlintf.fLocl_GetText("GUI_LABEL", "Runde");
            string xmlFile = GetPaarungenBasename(runde, sqlintf) + ".xml";
            swExport = new StreamWriter(xmlFile);
            swExport.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
            swExport.WriteLine("<?xml-stylesheet type='text/xsl' href='keizer_pairing.xsl' ?>");
            swExport.WriteLine("<keizer_pairing>");
            swExport.WriteLine("<export>");
            swExport.WriteLine("<tournament>");
            swExport.WriteLine(this.sTurnier);
            swExport.WriteLine("</tournament>");
            swExport.WriteLine("<title>");
            swExport.WriteLine(text + " " + runde.ToString());
            swExport.WriteLine("</title>");
            cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
            int pairingList = sqlintf.fGetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            for (int index = 0; index < pairingList; ++index)
            {
                swExport.WriteLine("<board>");
                swExport.WriteLine("<nr>" + (index + 1).ToString() + ".</nr>");
                swExport.WriteLine("<w>" + sqlintf.fGetPlayerName(pList[index].id_w) + "</w>");
                swExport.WriteLine("<b>" + sqlintf.fGetPlayerName(pList[index].id_b) + "</b>");
                swExport.WriteLine("<res>" + sqlintf.fLocl_GetGameResultText(pList[index].result) + "</res>");
                swExport.WriteLine("</board>");
            }
            swExport.WriteLine("<footer>");
            swExport.WriteLine(KfcFooter + " " + DateTime.Now.ToShortDateString());
            swExport.WriteLine("</footer>");
            swExport.WriteLine("</export>");
            swExport.WriteLine("</keizer_pairing>");
            swExport.Close();
        }

        TableW2Headers fReport_Paarungen_Table(int runde, cSqliteInterface sqlintf)
        {
            var t = new TableW2Headers();
            t.Header1 = this.sTurnier;
            t.Header2 = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde;

            cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
            int pairingList = sqlintf.fGetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            for (int index = 0; index < pairingList; ++index)
            {
                var r = new Li<string>();
                r.Add( (index + 1).ToString() + ".");
                r.Add(sqlintf.fGetPlayerName(pList[index].id_w));
                r.Add(sqlintf.fGetPlayerName(pList[index].id_b));
                r.Add(sqlintf.fLocl_GetGameResultText(pList[index].result) );
                t.AddRow(r);
            }
            return t;
        }
        #endregion Paarungen 

        private string GetFileTabellenstandBasename(cSqliteInterface sqlintf) =>
            "export\\" + this.sTurnier + "_" + sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + "-" + sqlintf.fGetMaxRound();

        private string GetFileTabellenstandExBasename(cSqliteInterface sqlintf) =>
            "export\\" + this.sTurnier + "_" + sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + "Ex-" + sqlintf.fGetMaxRound();

        public bool fReport_Tabellenstand(cSqliteInterface sqlintf)
        {
            try
            {
                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    fReport_Tabellenstand_Xml(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Csv"))
                    fReport_Tabellenstand_Voll_CSV(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    fReport_Tabellenstand_Html(sqlintf);
                    fReport_Tabellenstand_Voll_Html(sqlintf);
                }
                if (sqlintf.fGetConfigBool("OPTION.Txt"))
                    fReport_Tabellenstand_Txt(sqlintf);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        void fReport_Tabellenstand_Xml(cSqliteInterface sqlintf)
        {
            int num1 = 1;
            string xmlFile = GetFileTabellenstandBasename(sqlintf) + ".xml";
            string str2 = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound();
            Directory.CreateDirectory(Path.GetDirectoryName(xmlFile));
            swExport = new StreamWriter(xmlFile);
            swExport.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
            swExport.WriteLine("<?xml-stylesheet type='text/xsl' href='keizer_simpletable.xsl' ?>");
            swExport.WriteLine("<keizer_table>");
            swExport.WriteLine("<export>");
            swExport.WriteLine("<tournament>");
            swExport.WriteLine(this.sTurnier);
            swExport.WriteLine("</tournament>");
            swExport.WriteLine("<title>");
            swExport.WriteLine(sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + str2);
            swExport.WriteLine("</title>");
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY Keizer_SumPts desc ");
            for (int index = 0; index < playerList; ++index)
            {
                if (pList[index].state != cSqliteInterface.ePlayerState.eRetired)
                {
                    swExport.WriteLine("<player>");
                    swExport.WriteLine("<nr>" + num1.ToString() + ".</nr>");
                    swExport.WriteLine("<name>" + pList[index].name + "</name>");
                    swExport.WriteLine("<keizer_sum>" + pList[index].Keizer_SumPts.ToString() + "</keizer_sum>");
                    swExport.WriteLine("<game_pts>" + sqlintf.fGetPlayer_PartiePunkte(pList[index].id).ToString() + "</game_pts>");
                    swExport.WriteLine("</player>");
                    ++num1;
                }
            }
            swExport.WriteLine("<footer>");
            swExport.WriteLine(KfcFooter + " " + DateTime.Now.ToShortDateString());
            swExport.WriteLine("</footer>");
            swExport.WriteLine("</export>");
            swExport.WriteLine("</keizer_table>");
            swExport.Close();
        }

        void fReport_Tabellenstand_Html(cSqliteInterface sqlintf)
        {
            int num1 = 1;
            string file = GetFileTabellenstandBasename(sqlintf) + ".html";
            string str2 = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound();
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            swExport = new StreamWriter(file);
            AddHtmlHeader(swExport);
            swExport.WriteLine($"<h1>{this.sTurnier}</h1>");
            swExport.WriteLine("<h2>");
            swExport.WriteLine(sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + str2);
            swExport.WriteLine("</h2>");
            swExport.WriteLine("<table>");

            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY Keizer_SumPts desc ");
            for (int index = 0; index < playerList; ++index)
            {
                if (pList[index].state != cSqliteInterface.ePlayerState.eRetired)
                {
                    swExport.WriteLine("<tr>");
                    swExport.WriteLine("<td>" + num1.ToString() + ".</td>");
                    swExport.WriteLine("<td>" + pList[index].name + "</td>");
                    swExport.WriteLine("<td>" + pList[index].Keizer_SumPts.ToString() + "</td>");
                    swExport.WriteLine("<td>" + sqlintf.fGetPlayer_PartiePunkte(pList[index].id).ToString() + "</td>");
                    ++num1;
                    swExport.WriteLine("</tr>");
                }
            }
            swExport.WriteLine(KfcFooterHtml);
            swExport.WriteLine("</table>");
            AddHtmlFooter(swExport);
            swExport.Close();
            frmMainform.OpenWithDefaultApp(file);
        }

        void fReport_Tabellenstand_Txt(cSqliteInterface sqlintf, bool bWriteDump = false)
        {
            int num1 = 1;
            string text = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc");
            string path = GetFileTabellenstandBasename(sqlintf) + ".txt";
            string str = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound().ToString();
            swExport = new StreamWriter(path);
            swExport.WriteLine(this.sTurnier);
            swExport.WriteLine(text + " " + str);
            swExport.WriteLine(" ");
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY Keizer_SumPts desc ");
            for (int index = 0; index < playerList; ++index)
            {
                if (pList[index].state != cSqliteInterface.ePlayerState.eRetired)
                {
                    swExport.WriteLine((num1.ToString("00") + ".").PadLeft(4) + "  " + pList[index].name.PadRight(25) + pList[index].Keizer_SumPts.ToString().PadLeft(5) + sqlintf.fGetPlayer_PartiePunkte(pList[index].id).ToString().PadLeft(6));
                    ++num1;
                }
            }
            swExport.WriteLine(" ");
            swExport.WriteLine(KfcFooter + " " + DateTime.Now.ToShortDateString());
            swExport.Close();
        }

        public void fReport_Tabellenstand_Voll_CSV(cSqliteInterface sqlintf)
        {
            int num1 = 1;
            int maxRound = sqlintf.fGetMaxRound();
            cSqliteInterface.stPlayer[] pList1 = new cSqliteInterface.stPlayer[100];
            cSqliteInterface.stPlayer[] pList2 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPlayer[] pList3 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPairing[] pList4 = new cSqliteInterface.stPairing[1];
            swExport = new StreamWriter(GetFileTabellenstandBasename(sqlintf) + ".csv");
            int playerList = sqlintf.fGetPlayerList(ref pList1, "", " ORDER BY Keizer_SumPts desc, Rating desc ");
            swExport.WriteLine("Platz;Name;Rating;Keizer-RangPkte;Keizer-SummePkte;PartiePkt;Status;Runden...;");
            for (int index1 = 0; index1 < playerList; ++index1)
            {
                string str1;
                if (pList1[index1].state != cSqliteInterface.ePlayerState.eRetired)
                {
                    str1 = num1.ToString("00") + ";";
                    ++num1;
                }
                else
                    str1 = "(ret);";
                string name = pList1[index1].name;
                string str2 = str1 + name + ";" + pList1[index1].rating.ToString() + ";" + pList1[index1].Keizer_StartPts.ToString() + ";" + pList1[index1].Keizer_SumPts.ToString() + ";" + sqlintf.fGetPlayer_PartiePunkte(pList1[index1].id).ToString() + ";" + sqlintf.fLocl_GetPlayerStateText(pList1[index1].state) + ";";
                for (int index2 = 1; index2 <= maxRound; ++index2)
                {
                    string sWhere = " WHERE (PID_W=" + pList1[index1].id.ToString() + " OR    PID_B=" + pList1[index1].id.ToString() + ")  AND rnd=" + index2.ToString();
                    string str3;
                    if (sqlintf.fGetPairingList(ref pList4, sWhere, "  ") > 0)
                    {
                        sqlintf.fGetPlayerList(ref pList2, " WHERE ID=" + (object)pList4[0].id_w, " ");
                        sqlintf.fGetPlayerList(ref pList3, " WHERE ID=" + (object)pList4[0].id_b, " ");
                        string str4 = sqlintf.fLocl_GetGameResultText(pList4[0].result) + " ";
                        if (pList1[index1].id == pList4[0].id_w)
                        {
                            string str5 = str4 + "w ";
                            str3 = (pList3[0].state != cSqliteInterface.ePlayerState.eRetired ? str5 + " " + pList3[0].rank.ToString() + " " : str5 + " (ret) ") + "pkt=" + pList4[0].pts_w.ToString() + " ";
                        }
                        else
                        {
                            string str6 = str4 + "b ";
                            str3 = (pList2[0].state != cSqliteInterface.ePlayerState.eRetired ? str6 + " " + pList2[0].rank.ToString() + " " : str6 + " (ret) ") + "pkt=" + pList4[0].pts_b.ToString() + " ";
                        }
                    }
                    else
                        str3 = "_";
                    str2 = str2 + str3 + ";";
                }
                if (swExportDump != null)
                    swExportDump.WriteLine(str2);
                swExport.WriteLine(str2);
            }
            swExport.WriteLine(" ");
            swExport.Close();
        }

        public void fReport_Tabellenstand_Voll_Html(cSqliteInterface sqlintf)
        {
            int num1 = 1;
            int maxRound = sqlintf.fGetMaxRound();
            cSqliteInterface.stPlayer[] pList1 = new cSqliteInterface.stPlayer[100];
            cSqliteInterface.stPlayer[] pList2 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPlayer[] pList3 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPairing[] pList4 = new cSqliteInterface.stPairing[1];
            int nPlayer = sqlintf.fGetPlayerList(ref pList1, "", " ORDER BY Keizer_SumPts desc, Rating desc ");

            string file = GetFileTabellenstandExBasename(sqlintf) + ".html";
            string strr = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound();
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            swExport = new StreamWriter(file);
            AddHtmlHeader(swExport, true);
            swExport.WriteLine($"<h1>{this.sTurnier}</h1>");
            swExport.WriteLine("<h2>");
            swExport.WriteLine(sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + strr);
            swExport.WriteLine("</h2>");
            swExport.WriteLine("<table>");

            swExport.Write("<tr><td>Platz</td><td>Name</td><td>Rating</td><td>Keizer-P</td><td>Keizer-Sum</td><td>GamePts</td>");
            for (int i = 0; i < sqlintf.fGetMaxRound(); ++i)
                swExport.Write($"<td>R {i}</td>");
            swExport.WriteLine("</tr>");
            string td2 = "</td><td>";
            for (int index1 = 0; index1 < nPlayer; ++index1)
            {
                string str1 = "<tr><td>";
                if (pList1[index1].state != cSqliteInterface.ePlayerState.eRetired)
                {
                    str1 += num1.ToString("00") + td2;
                    ++num1;
                }
                else
                    str1 += "(ret)" + td2;
                string name = pList1[index1].name;
                string str2 = str1 + name + td2 + pList1[index1].rating.ToString() + td2 +
                    pList1[index1].Keizer_StartPts.ToString() + td2 +
                    pList1[index1].Keizer_SumPts.ToString() + td2 +
                    sqlintf.fGetPlayer_PartiePunkte(pList1[index1].id).ToString() + td2;
                for (int index2 = 1; index2 <= maxRound; ++index2)
                {
                    string sWhere = " WHERE (PID_W=" + pList1[index1].id.ToString() +
                        " OR    PID_B=" + pList1[index1].id.ToString() + ")  AND rnd=" + index2.ToString();
                    string str3;
                    if (sqlintf.fGetPairingList(ref pList4, sWhere, "  ") > 0)
                    {
                        sqlintf.fGetPlayerList(ref pList2, " WHERE ID=" + (object)pList4[0].id_w, " ");
                        sqlintf.fGetPlayerList(ref pList3, " WHERE ID=" + (object)pList4[0].id_b, " ");
                        string str4 = sqlintf.fLocl_GetGameResultShort(pList4[0].result) + " ";
                        if (pList4[0].result > cSqliteInterface.eResults.eWin_Black)
                        {
                            str3 = str4 + " - - p=" + pList4[0].pts_w.ToString();
                        }
                        else
                        {
                            if (pList1[index1].id == pList4[0].id_w)
                            {
                                string str5 = str4 + "w ";
                                str3 = (pList3[0].state != cSqliteInterface.ePlayerState.eRetired ?
                                    str5 + " " + pList3[0].name + " " :
                                    str5 + " (ret) ") + "p=" + pList4[0].pts_w.ToString() + " ";
                            }
                            else
                            {
                                string str6 = str4 + "b ";
                                str3 = (pList2[0].state != cSqliteInterface.ePlayerState.eRetired ?
                                    str6 + " " + pList2[0].name + " " : str6 + " (ret) ") +
                                    "p=" + pList4[0].pts_b.ToString() + " ";
                            }
                        }
                    }
                    else
                        str3 = "_";
                    str2 += str3 + td2;
                }
                str2 = str2.Substring(0, str2.Length - 4);
                str2 += "</tr>";
                if (swExportDump != null)
                    swExportDump.WriteLine(str2);
                swExport.WriteLine(str2);
            }

            swExport.WriteLine("</table> ");
            swExport.Close();
        }

        #region Teilnehmer
        public bool fReport_Teilnehmer(cSqliteInterface sqlintf)
        {
            try
            {
                var table = fReport_Teilnehmer_Table(sqlintf);
                var fileBase = GetTeilnehmerBasename(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    fReport_Teilnehmer_Xml(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Txt"))
                    fReport_Table_Txt(table, sqlintf, fileBase, new int[] { 4, -25, 5, 25 });
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    var file = fReport_Table_Html(table, sqlintf, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        void fReport_Teilnehmer_Xml(cSqliteInterface sqlintf)
        {
            string text = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer");
            string xmlFile = GetTeilnehmerBasename(sqlintf) + ".xml";
            swExport = new StreamWriter(xmlFile);
            swExport.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
            swExport.WriteLine("<?xml-stylesheet type='text/xsl' href='keizer_player.xsl' ?>");
            swExport.WriteLine("<keizer_player>");
            swExport.WriteLine("<export>");
            swExport.WriteLine("<tournament>");
            swExport.WriteLine(this.sTurnier);
            swExport.WriteLine("</tournament>");
            swExport.WriteLine("<title>");
            swExport.WriteLine(text);
            swExport.WriteLine("</title>");
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY ID ");
            for (int index = 0; index < playerList; ++index)
            {
                swExport.WriteLine("<player>");
                swExport.WriteLine("<nr>" + (index + 1).ToString() + ".</nr>");
                swExport.WriteLine("<name>" + pList[index].name + "</name>");
                swExport.WriteLine("<rating>" + pList[index].rating.ToString() + "</rating>");
                swExport.WriteLine("<state>" + sqlintf.fLocl_GetPlayerStateText(pList[index].state) + "</state>");
                swExport.WriteLine("</player>");
            }
            swExport.WriteLine("<footer>");
            swExport.WriteLine(KfcFooter + " " + DateTime.Now.ToShortDateString());
            swExport.WriteLine("</footer>");
            swExport.WriteLine("</export>");
            swExport.WriteLine("</keizer_player>");
            swExport.Close();
        }

        TableW2Headers fReport_Teilnehmer_Table(cSqliteInterface sqlintf)
        {
            var t = new TableW2Headers();
            t.Header1 = this.sTurnier;
            t.Header2 = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer");

            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY ID ");
            for (int index = 0; index < playerList; ++index)
            {
                var row = new Li<string>();
                row.Add((index + 1).ToString() + ".");
                row.Add(pList[index].name);
                row.Add(pList[index].rating.ToString());
                row.Add(sqlintf.fLocl_GetPlayerStateText(pList[index].state));
                t.AddRow(row);
            }
            return t;
        }

        private string GetTeilnehmerBasename(cSqliteInterface sqlintf) => "export\\" + this.sTurnier + "_" +
                sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer") + "-" + sqlintf.fGetMaxRound();
        #endregion Teilnehmer

        string fReport_Table_Txt(TableW2Headers t, cSqliteInterface sqlintf, string fileBase, int[] paddings)
        {
            string fileName = fileBase + ".txt";
            swExport = new StreamWriter(fileName);
            swExport.WriteLine(t.Header1);
            swExport.WriteLine(t.Header2);
            swExport.WriteLine(" ");
            for (int i = 0; i < t.Count; ++i)
            {
                var line = "";
                for(int j = 0; j < t[i].Count; ++j)
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
            swExport. WriteLine(KfcFooter + "   " +  DateHeader);
            swExport.Close();
            return fileName;
        }

        string fReport_Table_Html(TableW2Headers t, cSqliteInterface sqlintf, string fileBase)
        {
            string fileName = fileBase + ".html";
            swExport = new StreamWriter(fileName);
            AddHtmlHeader(swExport);
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
            swExport.Close();
            return fileName;
        }


        Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string KfcLongVersion => Version.ToString();
        string KfcShortVersion => $"{Version.Major}.{Version.Minor}";
        string KfcFooter => "KeizerForClubs v" + KfcShortVersion;
        string DateHeader => DateTime.Now.ToShortDateString();
        private string KfcFooterHtml => $"<tr><td colspan=\"2\">{KfcFooter}</td> <td colspan=\"2\">{DateHeader}</td></tr>";

        private void AddHtmlHeader(StreamWriter sw, bool isEx = false)
        {
            var ex = isEx ? "ex" : "";
            sw.WriteLine($@"<!DOCTYPE html><html><head>
                <link rel=""stylesheet"" href=""keizer.css""></head><body><div id=""{ex}wrapper"">");
        }

        private void AddHtmlFooter(StreamWriter sw)
        {
            sw.WriteLine(@"</div></body></html>");
        }

    }
}
