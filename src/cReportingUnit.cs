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
        cSqliteInterface db;

        public cReportingUnit(string sTurniername, cSqliteInterface db)
        {
            this.sTurnier = sTurniername;
            this.db = db;
        }
        #region Paarungen 
        private string GetPaarungenBasename(int runde) => "export\\" + this.sTurnier + "_" +
            db.fLocl_GetText("GUI_LABEL", "Runde") + "-" + runde;

        public bool fReport_Paarungen(int runde)
        {
            try
            {
                var table = fReportPaarungenTable(runde);
                var fileBase = GetPaarungenBasename(runde);
                if (db.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_pairing", "board", "nr w b res".Split());
                if (db.fGetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (db.fGetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -20, -20, 0 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, db.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportPaarungenTable(int runde)
        {
            var t = new TableW2Headers(sTurnier);
            t.Header2 = db.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde;
            t.AddRow("Pa.Brett Pa.Weiss Pa.Schwarz Pa.Ergebnis".Split().
                    Select(s => db.fLocl_GetText("GUI_COLS", s)).ToLi());

            cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
            int pairingList = db.fGetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            for (int index = 0; index < pairingList; ++index)
            {
                var r = new Li<string>();
                r.Add((index + 1).ToString() + ".");
                r.Add(db.fGetPlayerName(pList[index].id_w));
                r.Add(db.fGetPlayerName(pList[index].id_b));
                r.Add(db.fLocl_GetGameResultText(pList[index].result));
                t.AddRow(r);
            }
            return t;
        }
        #endregion Paarungen 

        #region Tabellenstand
        private string GetFileTabellenstandBasename() =>
            "export\\" + this.sTurnier + "_" + db.fLocl_GetText("GUI_MENU", "Listen.Calc") + "-" + db.fGetMaxRound();

        public bool fReport_Tabellenstand()
        {
            try
            {
                var table = fReportTabellenstandTable();
                var fileBase = GetFileTabellenstandBasename();

                if (db.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_simpletable", "player", "nr name keizer_sum game_pts".Split());
                if (db.fGetConfigBool("OPTION.Csv"))
                    ExportAsCsv(table, fileBase);
                if (db.fGetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (db.fGetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -25, 6, 6 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, db.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportTabellenstandTable()
        {
            var t = new TableW2Headers(sTurnier);
            string str2 = db.fLocl_GetText("GUI_LABEL", "Runde") + " " + db.fGetMaxRound();
            t.Header2 = db.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + str2;
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            var players = db.fGetPlayerLi("", " ORDER BY Keizer_SumPts desc ");
            var lih = new Li<string>(new string[] { "", db.fLocl_GetText("GUI_TEXT", "Name"), 
                db.fLocl_GetText("GUI_TEXT", "Keizer-Punkte"), db.fLocl_GetText("GUI_TEXT", "Spiel-Punkte") });
            t.AddRow(lih);
            for (int index = 0, num1 = 1; index < players.Count; ++index)
            {
                var player = players[index];
                if (player.state != cSqliteInterface.ePlayerState.eRetired)
                {
                    var li = new Li<string>();
                    li.Add(num1++.ToString() + ".");
                    li.Add(player.name);
                    li.Add(player.Keizer_SumPts.ToString("0.00"));
                    li.Add(db.fGetPlayer_PartiePunkte(player.id).ToString());
                    t.AddRow(li);
                }
            }
            return t;
        }
        #endregion Tabellenstand

        #region TabellenstandVoll

        public bool fReport_TabellenstandVoll()
        {
            try
            {
                var tableVoll = fReportTabellenstandVollTable();
                var fileBaseVoll = GetFileTabellenstandExBasename();

                if (db.fGetConfigBool("OPTION.Csv"))
                    ExportAsCsv(tableVoll, fileBaseVoll);
                if (db.fGetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(tableVoll, fileBaseVoll, true);
                    frmMainform.OpenWithDefaultApp(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, db.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private string GetFileTabellenstandExBasename() =>
            "export\\" + this.sTurnier + "_" + db.fLocl_GetText("GUI_MENU", "Listen.Calc") + "Ex-" + db.fGetMaxRound();

        TableW2Headers fReportTabellenstandVollTable()
        {
            var t = new TableW2Headers(sTurnier);
            int maxRound = db.fGetMaxRound();
            cSqliteInterface.stPairing[] pList4 = new cSqliteInterface.stPairing[1];
            var players = db.fGetPlayerLi("", " ORDER BY Keizer_SumPts desc, Rating desc ");

            string strr = db.fLocl_GetText("GUI_LABEL", "Runde") + " " + db.fGetMaxRound();
            t.Header2 = db.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + strr;
            // var thead = new Li<string>("Platz Name Rating Keizer-P Keizer-Sum GamePts".Split());
            var thead = new Li<string>("Platz Name Keizer-&#8721; GamePts".Split());
            for (int i = 0; i < maxRound; ++i)
                thead.Add("R " + (i + 1));
            t.AddRow(thead);
            for (int index1 = 0, numPlayer = 1; index1 < players.Count; ++index1)
            {
                var player = players[index1];
                var str1 = new Li<string>();
                if (player.state != cSqliteInterface.ePlayerState.eRetired)
                    str1.Add(numPlayer++.ToString("00"));
                else
                    str1.Add("(ret)");
                string name = player.name;
                str1.Add(name);
                // str1.Add(player.rating.ToString());
                // str1.Add(player.Keizer_StartPts.ToString());
                str1.Add(player.Keizer_SumPts.ToString("0.00"));
                str1.Add(db.fGetPlayer_PartiePunkte(player.id).ToString());

                for (int index2 = 1; index2 <= maxRound; ++index2)
                {
                    string sWhere = " WHERE (PID_W=" + player.id.ToString() +
                        " OR    PID_B=" + player.id.ToString() + ")  AND rnd=" + index2.ToString();
                    string str3;
                    if (db.fGetPairingList(ref pList4, sWhere, "  ") > 0)
                    {
                        var pair = pList4[0];
                        var pWhite = db.fGetPlayer(" WHERE ID=" + (object)pair.id_w, " ");
                        var pBlack = db.fGetPlayer(" WHERE ID=" + (object)pair.id_b, " ");
                        string str4 = db.fLocl_GetGameResultShort(pair.result) + " ";
                        if (pair.result > cSqliteInterface.eResults.eWin_Black)
                        {
                            str3 = str4 + " - - p=" + pair.pts_w.ToString();
                        }
                        else
                        {
                            if (player.id == pair.id_w)
                            {
                                string str5 = str4 + "w ";
                                str3 = (pBlack.state != cSqliteInterface.ePlayerState.eRetired ?
                                    str5 + " " + pBlack.name + " " :
                                    str5 + " " + pBlack.name + " (r) ") + 
                                    "p=" + pair.pts_w.ToString() + " ";
                            }
                            else
                            {
                                string str6 = str4 + "b ";
                                str3 = (pWhite.state != cSqliteInterface.ePlayerState.eRetired ?
                                    str6 + " " + pWhite.name + " " : 
                                    str6 + " " + pWhite.name + " (r) ") +
                                    "p=" + pair.pts_b.ToString() + " ";
                            }
                        }
                    }
                    else
                        str3 = "-";
                    str1.Add(str3);
                }
                t.AddRow(str1);
            }
            return t;
        }

        #endregion TabellenstandVoll

        #region Teilnehmer
        public bool fReport_Teilnehmer()
        {
            try
            {
                var table = fReportTeilnehmerTable(db);
                var fileBase = GetTeilnehmerBasename(db);
                if (db.fGetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_player", "player", "nr name rating state".Split());
                if (db.fGetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -25, 5, 25 });
                if (db.fGetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, db.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportTeilnehmerTable(cSqliteInterface sqlintf)
        {
            var t = new TableW2Headers(sTurnier);
            t.Header2 = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer");

            var players = sqlintf.fGetPlayerLi("", " ORDER BY ID ");
            for (int index = 0; index < players.Count; ++index)
            {
                var player = players[index];
                var row = new Li<string>();
                row.Add((index + 1).ToString() + ".");
                row.Add(player.name);
                row.Add(player.rating.ToString());
                row.Add(sqlintf.fLocl_GetPlayerStateText(player.state));
                t.AddRow(row);
            }
            return t;
        }

        private string GetTeilnehmerBasename(cSqliteInterface sqlintf) => "export\\" + this.sTurnier + "_" +
                sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer") + "-" + sqlintf.fGetMaxRound();
        #endregion Teilnehmer

        #region Debugging
        public void DebugPairingsAndStandings(int runde)
        {
            using (StreamWriter swExportDump = new StreamWriter(dumpcalcFile, true))
            {
                swExportDump.WriteLine("");
                if (runde > 0)
                {
                    swExportDump.WriteLine("Runde " + runde.ToString());
                    var tpair = fReportPaarungenTable(runde);
                    var sbp = ToStringBuilderAsCsv(tpair);
                    swExportDump.WriteLine(sbp.ToString());
                }
                else
                    swExportDump.WriteLine("Stand vor 1. Runde");

                swExportDump.WriteLine("");
                swExportDump.WriteLine("Tabelle ");
                var table = fReportTabellenstandVollTable();
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
        protected override string KfcFooter => "KeizerForClubs v" + KfcLongVersion;
        protected override string KfcFooterHtml => $"<tr><td colspan=\"3\"><a href=\"https://github.com/Dumuzy/KeizerForClubs/releases\">{KfcFooter}</a></td> <td colspan=\"2\">{DateHeader}</td></tr>";
    }
}
