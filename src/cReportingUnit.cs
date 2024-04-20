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
            db.Locl_GetText("GUI_LABEL", "Runde") + "-" + runde;

        public bool fReport_Paarungen(int runde)
        {
            try
            {
                var table = fReportPaarungenTable(runde);
                var fileBase = GetPaarungenBasename(runde);
                if (db.GetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_pairing", "board", "nr w b res".Split());
                if (db.GetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (db.GetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -20, -20, 0 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, db.Locl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportPaarungenTable(int runde)
        {
            var t = new TableW2Headers(sTurnier);
            t.Header2 = db.Locl_GetText("GUI_MENU", "Paarungen") + " " + 
                db.Locl_GetText("GUI_LABEL", "Runde") + " " + runde;
            t.AddRow("Pa.Brett Pa.Weiss Pa.Schwarz Pa.Ergebnis".Split().
                    Select(s => db.Locl_GetText("GUI_COLS", s)).ToLi());

            SqliteInterface.stPairing[] pList = new SqliteInterface.stPairing[50];
            int pairingList = db.GetPairingList(ref pList, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            for (int index = 0; index < pairingList; ++index)
            {
                var r = new Li<string>();
                r.Add((index + 1).ToString() + ".");
                r.Add(db.GetPlayerName(pList[index].IdW));
                r.Add(db.GetPlayerName(pList[index].IdB));
                r.Add(db.Locl_GetGameResultText(pList[index].Result));
                t.AddRow(r);
            }
            t.Footer = TableFooter;
            return t;
        }
        #endregion Paarungen 

        #region Tabellenstand
        private string GetFileTabellenstandBasename(int runde) =>
            "export\\" + this.sTurnier + "_" + db.Locl_GetText("GUI_MENU", "Listen.Calc") + "-" + runde;

        public bool fReport_Tabellenstand(int runde)
        {
            try
            {
                TableW2Headers table = runde != db.GetMaxRound() ? 
                    db.ReadTableWHeadersFromDb(TableType.Stand, runde) : fReportTabellenstandTable();
                var fileBase = GetFileTabellenstandBasename(runde);

                if (db.GetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_simpletable", "player", "nr name keizer_sum game_pts".Split());
                if (db.GetConfigBool("OPTION.Csv"))
                    ExportAsCsv(table, fileBase);
                if (db.GetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
                if (db.GetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -25, 6, 6 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, db.Locl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        TableW2Headers fReportTabellenstandTable()
        {
            var t = new TableW2Headers(sTurnier);
            var runde = db.GetMaxRound();
            string str2 = db.Locl_GetText("GUI_LABEL", "Runde") + " " + runde;
            t.Header2 = db.Locl_GetText("GUI_MENU", "Listen.Calc") + " " + str2;
            SqliteInterface.stPlayer[] pList = new SqliteInterface.stPlayer[100];
            var players = db.GetPlayerLi("", " ORDER BY Keizer_SumPts desc ", runde);
            var lih = new Li<string>(new string[] { "", db.Locl_GetText("GUI_TEXT", "Name"),
                db.Locl_GetText("GUI_TEXT", "Keizer-Punkte"), db.Locl_GetText("GUI_TEXT", "Spiel-Punkte") });
            t.AddRow(lih);
            for (int index = 0, num1 = 1; index < players.Count; ++index)
            {
                var player = players[index];
                if (player.State != SqliteInterface.PlayerState.Retired)
                {
                    var li = new Li<string>();
                    li.Add(num1++.ToString() + ".");
                    li.Add(player.Name);
                    li.Add(player.KeizerSumPts.ToString("0.00"));
                    li.Add(db.GetPlayer_PartiePunkte(player.Id).ToString());
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
                TableW2Headers tableVoll = runde != db.GetMaxRound() ?
                        db.ReadTableWHeadersFromDb(TableType.Kreuz, runde) : fReportTabellenstandVollTable();

                var fileBaseVoll = GetFileTabellenstandExBasename(runde);

                if (db.GetConfigBool("OPTION.Csv"))
                    ExportAsCsv(tableVoll, fileBaseVoll);
                if (db.GetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(tableVoll, fileBaseVoll, true,
                        $",,Keizer rank points before round {runde}.,Keizer sum points after round {runde}.".Split(','));
                    frmMainform.OpenWithDefaultApp(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, db.Locl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private string GetFileTabellenstandExBasename(int runde) =>
            "export\\" + this.sTurnier + "_" + db.Locl_GetText("GUI_MENU", "Listen.Calc") + "Ex-" + runde;

        TableW2Headers fReportTabellenstandVollTable()
        {
            var t = new TableW2Headers(sTurnier);
            int maxRound = db.GetMaxRound();
            SqliteInterface.stPairing[] pList4 = new SqliteInterface.stPairing[1];
            var players = db.GetPlayerLi("", " ORDER BY Keizer_SumPts desc, Rating desc ", maxRound);

            string strr = db.Locl_GetText("GUI_LABEL", "Runde") + " " + db.GetMaxRound();
            t.Header2 = db.Locl_GetText("GUI_MENU", "Listen.Calc") + " " + strr;
            // var thead = new Li<string>("Platz Name Rating Rank-P Keizer-Sum GamePts".Split());
            var thead = new Li<string>(db.Locl_GetText("GUI_LABEL", "Kreuztab.Header").Split());
            for (int i = 0; i < maxRound; ++i)
                thead.Add("R " + (i + 1));
            t.AddRow(thead);
            for (int index1 = 0, numPlayer = 1; index1 < players.Count; ++index1)
            {
                var player = players[index1];

                if (player.State == SqliteInterface.PlayerState.Retired && db.CntPlayersPlayedGames(player.Id) == 0)
                    continue;
                var line = new Li<string>();
                if (player.State != SqliteInterface.PlayerState.Retired)
                    line.Add(numPlayer++.ToString("00"));
                else
                    line.Add("(ret)");
                line.Add(player.Name);
                // str1.Add(player.rating.ToString());
                line.Add(player.KeizerPrevPts.ToString());
                line.Add(player.KeizerSumPts.ToString("0.00"));
                line.Add(db.GetPlayer_PartiePunkte(player.Id).ToString());

                for (int runde = 1; runde <= maxRound; ++runde)
                {
                    string sWhere = $" WHERE (PID_W={player.Id} OR PID_B={player.Id}) AND rnd={runde} ";
                    string str3;
                    if (db.GetPairingList(ref pList4, sWhere, "  ") > 0)
                    {
                        var pair = pList4[0];
                        var pWhite = db.GetPlayerById(pair.IdW, maxRound);
                        var pBlack = db.GetPlayerById(pair.IdB, maxRound);
                        string str4 = db.Locl_GetGameResultShort(pair.Result) + " ";
                        if (pair.Result > SqliteInterface.Results.WinBlack)
                        {
                            str3 = str4 + " - - p=" + pair.PtsW.ToString();
                        }
                        else
                        {
                            if (player.Id == pair.IdW)
                            {
                                string str5 = str4 + "w ";
                                str3 = (pBlack.State != SqliteInterface.PlayerState.Retired ?
                                    str5 + " " + pBlack.Name + " " :
                                    str5 + " " + pBlack.Name + " (r) ") +
                                    "p=" + pair.PtsW.ToString() + " ";
                            }
                            else
                            {
                                string str6 = str4 + "b ";
                                str3 = (pWhite.State != SqliteInterface.PlayerState.Retired ?
                                    str6 + " " + pWhite.Name + " " :
                                    str6 + " " + pWhite.Name + " (r) ") +
                                    "p=" + pair.PtsB.ToString() + " ";
                            }
                        }
                    }
                    else
                        str3 = "-";
                    line.Add(str3);
                }
                t.AddRow(line);
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
                if (db.GetConfigBool("OPTION.Xml"))
                    ExportAsXml(table, fileBase, "keizer_player", "player", "nr name rating state".Split());
                if (db.GetConfigBool("OPTION.Txt"))
                    ExportAsTxt(table, fileBase, new int[] { 4, -25, 5, 25 });
                if (db.GetConfigBool("OPTION.Html"))
                {
                    var file = ExportAsHtml(table, fileBase);
                    frmMainform.OpenWithDefaultApp(file);
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message, db.Locl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
                return false;
            }
            return true;
        }


        TableW2Headers fReportTeilnehmerTable(int runde)
        {
            var maxRound = db.GetMaxRound();
            var t = new TableW2Headers(sTurnier);
            t.Header2 = db.Locl_GetText("GUI_MENU", "Listen.Teilnehmer") + " " +
                db.Locl_GetText("GUI_LABEL", "Runde") + " " + runde;

            var players = db.GetPreviousPlayerLi("", " ORDER BY ID ",  runde);
            // players.Sort(p => p.id);  // TODO
            for (int index = 0; index < players.Count; ++index)
            {
                var player = players[index];
                var row = new Li<string>();
                row.Add((index + 1).ToString() + ".");
                row.Add(player.Name);
                row.Add(player.Rating.ToString());
                row.Add(db.Locl_GetPlayerStateText(player.State));
                t.AddRow(row);
            }
            t.Footer = TableFooter;
            return t;
        }

        TableW2Headers fReportTeilnehmerTableOld(SqliteInterface sqlintf)
        {
            var runde = db.GetMaxRound();
            var t = new TableW2Headers(sTurnier);
            t.Header2 = sqlintf.Locl_GetText("GUI_MENU", "Listen.Teilnehmer") + " " +
                db.Locl_GetText("GUI_LABEL", "Runde") + " " + runde; 

            var players = sqlintf.GetPlayerLi("", " ORDER BY ID ", runde);
            for (int index = 0; index < players.Count; ++index)
            {
                var player = players[index];
                var row = new Li<string>();
                row.Add((index + 1).ToString() + ".");
                row.Add(player.Name);
                row.Add(player.Rating.ToString());
                row.Add(sqlintf.Locl_GetPlayerStateText(player.State));
                t.AddRow(row);
            }
            t.Footer = TableFooter;
            return t;
        }

        private string GetTeilnehmerBasename(SqliteInterface sqlintf, int runde) => "export\\" + this.sTurnier + "_" +
                sqlintf.Locl_GetText("GUI_MENU", "Listen.Teilnehmer") + "-" + runde;
        #endregion Teilnehmer

        #region Misc
        public void WriteCurrentTablesToDb()
        {
            int maxRound = db.GetMaxRound();
            if (maxRound > 0)
            {
                db.WriteTableWHeaders2Db(TableType.Stand, maxRound, fReportTabellenstandTable());
                db.WriteTableWHeaders2Db(TableType.Kreuz, maxRound, fReportTabellenstandVollTable());
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
