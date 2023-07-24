using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;
using AwiUtils;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static KeizerForClubs.SqliteInterface;

namespace KeizerForClubs
{
    public class ReportingUnit : ReportingBase
    {
        string sTurnier = "";
        SqliteInterface db;

        public ReportingUnit(string sTurniername, SqliteInterface db)
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
            t.Header2 = db.fLocl_GetText("GUI_MENU", "Paarungen") + " " + 
                db.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde;
            t.AddRow("Pa.Brett Pa.Weiss Pa.Schwarz Pa.Ergebnis".Split().
                    Select(s => db.fLocl_GetText("GUI_COLS", s)).ToLi());

            SqliteInterface.stPairing[] pList = new SqliteInterface.stPairing[50];
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
            t.Footer = TableFooter;
            return t;
        }
        #endregion Paarungen 

        #region Tabellenstand
        private string GetFileTabellenstandBasename(int runde) =>
            "export\\" + this.sTurnier + "_" + db.fLocl_GetText("GUI_MENU", "Listen.Calc") + "-" + runde;

        public bool fReport_Tabellenstand(int runde)
        {
            try
            {
                TableW2Headers table = runde != db.fGetMaxRound() ? 
                    db.ReadTableWHeadersFromDb(eTableType.Stand, runde) : fReportTabellenstandTable();
                var fileBase = GetFileTabellenstandBasename(runde);

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
            var runde = db.fGetMaxRound();
            string str2 = db.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde;
            t.Header2 = db.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + str2;
            SqliteInterface.stPlayer[] pList = new SqliteInterface.stPlayer[100];
            var players = db.fGetPlayerLi("", " ORDER BY Keizer_SumPts desc ", runde);
            var lih = new Li<string>(new string[] { "", db.fLocl_GetText("GUI_TEXT", "Name"),
                db.fLocl_GetText("GUI_TEXT", "Keizer-Punkte"), db.fLocl_GetText("GUI_TEXT", "Spiel-Punkte") });
            t.AddRow(lih);
            for (int index = 0, num1 = 1; index < players.Count; ++index)
            {
                var player = players[index];
                if (player.state != SqliteInterface.ePlayerState.eRetired)
                {
                    var li = new Li<string>();
                    li.Add(num1++.ToString() + ".");
                    li.Add(player.name);
                    li.Add(player.Keizer_SumPts.ToString("0.00"));
                    li.Add(db.fGetPlayer_PartiePunkte(player.id).ToString());
                    t.AddRow(li);
                }
            }
            t.Footer = TableFooter;
            return t;
        }
        #endregion Tabellenstand

        #region TabellenstandVoll

        public bool fReport_TabellenstandVoll(int runde)
        {
            try
            {
                TableW2Headers tableVoll = runde != db.fGetMaxRound() ?
                        db.ReadTableWHeadersFromDb(eTableType.Kreuz, runde) : fReportTabellenstandVollTable();

                var fileBaseVoll = GetFileTabellenstandExBasename(runde);

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

        private string GetFileTabellenstandExBasename(int runde) =>
            "export\\" + this.sTurnier + "_" + db.fLocl_GetText("GUI_MENU", "Listen.Calc") + "Ex-" + runde;

        TableW2Headers fReportTabellenstandVollTable()
        {
            var t = new TableW2Headers(sTurnier);
            int runde = db.fGetMaxRound();
            SqliteInterface.stPairing[] pList4 = new SqliteInterface.stPairing[1];
            var players = db.fGetPlayerLi("", " ORDER BY Keizer_SumPts desc, Rating desc ", runde);

            string strr = db.fLocl_GetText("GUI_LABEL", "Runde") + " " + db.fGetMaxRound();
            t.Header2 = db.fLocl_GetText("GUI_MENU", "Listen.Calc") + " " + strr;
            // var thead = new Li<string>("Platz Name Rating Keizer-P Keizer-Sum GamePts".Split());
            var thead = new Li<string>("Platz Name Keizer-&#8721; GamePts".Split());
            for (int i = 0; i < runde; ++i)
                thead.Add("R " + (i + 1));
            t.AddRow(thead);
            for (int index1 = 0, numPlayer = 1; index1 < players.Count; ++index1)
            {
                var player = players[index1];
                var str1 = new Li<string>();
                if (player.state != SqliteInterface.ePlayerState.eRetired)
                    str1.Add(numPlayer++.ToString("00"));
                else
                    str1.Add("(ret)");
                string name = player.name;
                str1.Add(name);
                // str1.Add(player.rating.ToString());
                // str1.Add(player.Keizer_StartPts.ToString());
                str1.Add(player.Keizer_SumPts.ToString("0.00"));
                str1.Add(db.fGetPlayer_PartiePunkte(player.id).ToString());

                for (int index2 = 1; index2 <= runde; ++index2)
                {
                    string sWhere = " WHERE (PID_W=" + player.id.ToString() +
                        " OR    PID_B=" + player.id.ToString() + ")  AND rnd=" + index2.ToString();
                    string str3;
                    if (db.fGetPairingList(ref pList4, sWhere, "  ") > 0)
                    {
                        var pair = pList4[0];
                        var pWhite = db.fGetPlayer(" WHERE ID=" + (object)pair.id_w, " ", runde);
                        var pBlack = db.fGetPlayer(" WHERE ID=" + (object)pair.id_b, " ", runde);
                        string str4 = db.fLocl_GetGameResultShort(pair.result) + " ";
                        if (pair.result > SqliteInterface.eResults.eWin_Black)
                        {
                            str3 = str4 + " - - p=" + pair.pts_w.ToString();
                        }
                        else
                        {
                            if (player.id == pair.id_w)
                            {
                                string str5 = str4 + "w ";
                                str3 = (pBlack.state != SqliteInterface.ePlayerState.eRetired ?
                                    str5 + " " + pBlack.name + " " :
                                    str5 + " " + pBlack.name + " (r) ") +
                                    "p=" + pair.pts_w.ToString() + " ";
                            }
                            else
                            {
                                string str6 = str4 + "b ";
                                str3 = (pWhite.state != SqliteInterface.ePlayerState.eRetired ?
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
            t.Footer = TableFooter;
            return t;
        }

        #endregion TabellenstandVoll

        #region Teilnehmer
        public bool fReport_Teilnehmer(int runde)
        {
            try
            {
                TableW2Headers table = fReportTeilnehmerTable(runde);

                var fileBase = GetTeilnehmerBasename(db, runde);
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


        TableW2Headers fReportTeilnehmerTable(int runde)
        {
            var maxRound = db.fGetMaxRound();
            var t = new TableW2Headers(sTurnier);
            t.Header2 = db.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer") + " " +
                db.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde;

            var players = db.GetPreviousPlayerLi("", " ORDER BY ID ",  runde);
            // players.Sort(p => p.id);  // TODO
            for (int index = 0; index < players.Count; ++index)
            {
                var player = players[index];
                var row = new Li<string>();
                row.Add((index + 1).ToString() + ".");
                row.Add(player.name);
                row.Add(player.rating.ToString());
                row.Add(db.fLocl_GetPlayerStateText(player.state));
                t.AddRow(row);
            }
            t.Footer = TableFooter;
            return t;
        }

        TableW2Headers fReportTeilnehmerTableOld(SqliteInterface sqlintf)
        {
            var runde = db.fGetMaxRound();
            var t = new TableW2Headers(sTurnier);
            t.Header2 = sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer") + " " +
                db.fLocl_GetText("GUI_LABEL", "Runde") + " " + runde; 

            var players = sqlintf.fGetPlayerLi("", " ORDER BY ID ", runde);
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
            t.Footer = TableFooter;
            return t;
        }

        private string GetTeilnehmerBasename(SqliteInterface sqlintf, int runde) => "export\\" + this.sTurnier + "_" +
                sqlintf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer") + "-" + runde;
        #endregion Teilnehmer

        #region Misc
        public void WriteCurrentTablesToDb()
        {
            int maxRound = db.fGetMaxRound();
            if (maxRound > 0)
            {
                db.WriteTableWHeaders2Db(eTableType.Stand, maxRound, fReportTabellenstandTable());
                db.WriteTableWHeaders2Db(eTableType.Kreuz, maxRound, fReportTabellenstandVollTable());
            }
        }
        #endregion Misc

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
        protected override string KfcFooter => "Footer0 Footer1";
        protected override string KfcFooterHtml => "<tr><td colspan=\"3\"><a href=\"https://github.com/Dumuzy/KeizerForClubs/releases\">Footer0</a></td> <td colspan=\"2\">Footer1</td></tr>";

        Li<string> TableFooter => new string[] { "KeizerForClubs v" + KfcLongVersion, DateHeader }.ToLi();
    }
}
