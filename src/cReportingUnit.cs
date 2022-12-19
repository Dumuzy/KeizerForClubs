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

                var tableVoll = fReportTabellenstandVollTable(sqlintf);
                var fileBaseVoll = GetFileTabellenstandExBasename(sqlintf);

                if (sqlintf.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_simpletable", "player", "nr name keizer_sum game_pts".Split());
                if (sqlintf.fGetConfigBool("OPTION.Csv"))
                {
                    ExportAsCsv(tableVoll, fileBaseVoll);
                    ExportAsCsv(table, fileBase);
                }
                if (sqlintf.fGetConfigBool("OPTION.Html"))
                {
                    ExportAsHtml(tableVoll, fileBaseVoll, true);
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

        public TableW2Headers fReportTabellenstandVollTable(cSqliteInterface sqlintf)
        {
            var t = new TableW2Headers();
            int maxRound = sqlintf.fGetMaxRound();
            cSqliteInterface.stPlayer[] pList1 = new cSqliteInterface.stPlayer[100];
            cSqliteInterface.stPlayer[] pList2 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPlayer[] pList3 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPairing[] pList4 = new cSqliteInterface.stPairing[1];
            int nPlayer = sqlintf.fGetPlayerList(ref pList1, "", " ORDER BY Keizer_SumPts desc, Rating desc ");

            string strr = sqlintf.fLocl_GetText("GUI_LABEL", "Runde") + " " + sqlintf.fGetMaxRound();
            t.Header1 = sTurnier;
            t.Header2 = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + strr;
            var thead = new Li<string>("Platz Name Rating Keizer-P Keizer-Sum GamePts".Split());
            for (int i = 0; i < maxRound; ++i)
                thead.Add("R " + i);
            t.AddRow(thead);
            for (int index1 = 0, numPlayer = 1; index1 < nPlayer; ++index1)
            {
                var str1 = new Li<string>();
                if (pList1[index1].state != cSqliteInterface.ePlayerState.eRetired)
                    str1.Add(numPlayer++.ToString("00"));
                else
                    str1.Add("(ret)");
                string name = pList1[index1].name;
                str1.Add(name);
                str1.Add(pList1[index1].rating.ToString());
                str1.Add(pList1[index1].Keizer_StartPts.ToString());
                str1.Add(pList1[index1].Keizer_SumPts.ToString());
                str1.Add(sqlintf.fGetPlayer_PartiePunkte(pList1[index1].id).ToString());

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
                        str3 = "-";
                    str1.Add(str3);
                }
                // str2 = str2.Substring(0, str2.Length - 4);  // Hä??
                // DumpLine(str2);
                t.AddRow(str1);
            }
            return t;
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

        #region Debugging
        public void DebugPairingsAndStandings(cSqliteInterface db, int runde)
        {
            using (StreamWriter swExportDump = new StreamWriter(dumpcalcFile, true))
            {
                swExportDump.WriteLine("");
                if (runde > 0)
                {
                    swExportDump.WriteLine("Runde " + runde.ToString());
                    var tpair = fReportPaarungenTable(runde, db);
                    var sbp = ToStringBuilderAsCsv(tpair);
                    swExportDump.WriteLine(sbp.ToString());
                }
                else
                    swExportDump.WriteLine("Stand vor 1. Runde");

                swExportDump.WriteLine("");
                swExportDump.WriteLine("Tabelle ");
                var table = fReportTabellenstandVollTable(db);
                var sb = ToStringBuilderAsCsv(table);
                swExportDump.WriteLine(sb.ToString());
                swExportDump.WriteLine("");
                swExportDump.WriteLine("");
            }
        }

        public void DeleteDump()
        {
            try { System.IO.File.Delete(dumpcalcFile); } catch (Exception) { }
        }

        const string dumpcalcFile = "export\\dumpcalc.csv";
        #endregion Debugging

        Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string KfcLongVersion => Version.ToString();
        string KfcShortVersion => $"{Version.Major}.{Version.Minor}";
        protected override string KfcFooter => "KeizerForClubs v" + KfcShortVersion;
        protected override string KfcFooterHtml => $"<tr><td colspan=\"2\">{KfcFooter}</td> <td colspan=\"2\">{DateHeader}</td></tr>";


    }
}
