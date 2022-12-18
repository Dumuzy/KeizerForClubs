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


    public class cReportingUnit : ReportingBase
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
                var table = fReportPaarungenTable(runde, sqlintf);
                var fileBase = GetPaarungenBasename(runde, sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_pairing", "board", "nr w b res".Split());
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (sqlintf.fGetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -20, -20, 0 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportPaarungenTable(int runde, cSqliteInterface sqlintf)
        {
            var t = new TableW2Headers();
            t.Header1 = this.sTurnier;
            t.Header2 = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde;

            cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
            int pairingList = sqlintf.fGetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            for (int index = 0; index < pairingList; ++index)
            {
                var r = new Li<string>();
                r.Add((index + 1).ToString() + ".");
                r.Add(sqlintf.fGetPlayerName(pList[index].id_w));
                r.Add(sqlintf.fGetPlayerName(pList[index].id_b));
                r.Add(sqlintf.fLocl_GetGameResultText(pList[index].result));
                t.AddRow(r);
            }
            return t;
        }
        #endregion Paarungen 

        #region Tabellenstand
        private string GetFileTabellenstandBasename(cSqliteInterface sqlintf) =>
            "export\\" + this.sTurnier + "_" + sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + "-" + sqlintf.fGetMaxRound();

        public bool fReport_Tabellenstand(cSqliteInterface sqlintf)
        {
            try
            {
                var table = fReportTabellenstandTable(sqlintf);
                var fileBase = GetFileTabellenstandBasename(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_simpletable", "player", "nr name keizer_sum game_pts".Split());
                if (sqlintf.fGetConfigBool("OPTION.Csv"))
                    fReport_Tabellenstand_Voll_CSV(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    fReport_Tabellenstand_Voll_Html(sqlintf);
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (sqlintf.fGetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -25, 6, 6 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, sqlintf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportTabellenstandTable(cSqliteInterface sqlintf)
        {
            var t = new TableW2Headers();
            t.Header1 = this.sTurnier;
            string str2 = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound();
            t.Header2 = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + str2;
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = sqlintf.fGetPlayerList(ref pList, "", " ORDER BY Keizer_SumPts desc ");
            for (int index = 0, num1 = 1; index < playerList; ++index)
            {
                if (pList[index].state != cSqliteInterface.ePlayerState.eRetired)
                {
                    var li = new Li<string>();
                    li.Add(num1++.ToString() + ".");
                    li.Add(pList[index].name);
                    li.Add(pList[index].Keizer_SumPts.ToString());
                    li.Add(sqlintf.fGetPlayer_PartiePunkte(pList[index].id).ToString());
                    t.AddRow(li);
                }
            }
            return t;
        }
        #endregion Tabellenstand

        #region TabellenstandVoll

        private string GetFileTabellenstandExBasename(cSqliteInterface sqlintf) =>
            "export\\" + this.sTurnier + "_" + sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + "Ex-" + sqlintf.fGetMaxRound();

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
        #endregion TabellenstandVoll

        #region Teilnehmer
        public bool fReport_Teilnehmer(cSqliteInterface sqlintf)
        {
            try
            {
                var table = fReportTeilnehmerTable(sqlintf);
                var fileBase = GetTeilnehmerBasename(sqlintf);
                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_player", "player", "nr name rating state".Split());
                if (sqlintf.fGetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -25, 5, 25 });
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
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

        TableW2Headers fReportTeilnehmerTable(cSqliteInterface sqlintf)
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


        Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string KfcLongVersion => Version.ToString();
        string KfcShortVersion => $"{Version.Major}.{Version.Minor}";
        protected override string KfcFooter => "KeizerForClubs v" + KfcShortVersion;
        protected override string KfcFooterHtml => $"<tr><td colspan=\"2\">{KfcFooter}</td> <td colspan=\"2\">{DateHeader}</td></tr>";


    }
}
