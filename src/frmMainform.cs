using System.ComponentModel;
using System.Diagnostics;

namespace KeizerForClubs
{
    public class frmMainform : Form
    {
        private cSqliteInterface SQLiteIntf = new cSqliteInterface();
        private string sTurniername = "";
        private int iPairingRekursionCnt;
        private int iPairingPlayerCntAll;
        private int iPairingPlayerCntAvailable;
        private int iPairingMinFreeCnt;
        private cSqliteInterface.stPlayer[] pPairingPlayerList = new cSqliteInterface.stPlayer[100];
        private cSqliteInterface.stPairing[] pPairingList = new cSqliteInterface.stPairing[50];
        private IContainer components;
        private ToolStripMenuItem mnuHelpDocumentation;
        private ToolStripMenuItem mnuHelpFaq,  mnuHelp, mnuHelpAbout;
        private Label lblBonus1Value;
        private Label lblBonus2Value;
        private Label lblBonus3Value;
        private Label lblBonus4Value;
        private ToolStripMenuItem mnuListenParticipants;
        private ToolStripMenuItem mnuListenStanding;
        private ToolStripMenuItem mnuListenPairing;
        private NumericUpDown numRoundsGameRepeat;
        private Label lblRoundsGameRepeat, lblOutputTo;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuStartStart;
        private CheckBox chkFreilosVerteilen;
        private CheckBox chkHtml, chkXml, chkTxt, chkCsv;
        private TrackBar tbBonusRetired;
        private Label lblBonusRetired;
        private ToolStripMenuItem mnuStartLanguage;
        private CheckBox chkPairingOnlyPlayed;
        private ToolStripMenuItem mnuPaarungDropLast;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuPaarungManuell;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mnuPaarungNext;
        private DataGridViewTextBoxColumn colPairgBoard;
        private Label lblRunde;
        private NumericUpDown numRoundSelect;
        private Panel pnlPairingPanel;
        private DataGridViewTextBoxColumn colRating;
        private DataGridViewTextBoxColumn colPlayerName;
        private DataGridViewTextBoxColumn colPlayerID;
        private OpenFileDialog dlgOpenTournament;
        private DataGridViewComboBoxColumn colPairingResult;
        private DataGridViewComboBoxColumn colPlayerState;
        private TrackBar tbBonusExcused;
        private TrackBar tbBonusUnexcused;
        private TrackBar tbBonusHindered;
        private Label lblBonusExcused;
        private Label lblBonusUnexcused;
        private Label lblBonusClubgame;
        private TabPage tabSettings;
        private DataGridViewTextBoxColumn colPairingAddInfoB;
        private DataGridViewTextBoxColumn colPairingNameBlack;
        private DataGridViewTextBoxColumn colPairingIDB;
        private DataGridViewTextBoxColumn colPairingWhiteAddinfo;
        private DataGridViewTextBoxColumn colPairingNameWhite;
        private DataGridViewTextBoxColumn IDW;
        private DataGridView grdPairings;
        private ToolStripMenuItem mnuListen;
        private ToolStripMenuItem mnuPaarungen;
        private ToolStripMenuItem mnuTurnierstart;
        private MenuStrip mnuMainmenu;
        private TabPage tabPairings;
        private DataGridView grdPlayers;
        private TabPage tabPlayer;
        private TabControl tabMainWindow;
        private readonly string[] Args;

        public frmMainform(string[] args)
        {
            Args = args;
            CopyCfgDocsExport();
            InitializeComponent();
        }

        private void OpenStartTournamentToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (dlgOpenTournament.ShowDialog() != DialogResult.OK)
                return;
            string fileName = dlgOpenTournament.FileName;
            OpenTournament(fileName);
            SQLiteIntf.fSetConfigText("TournamentFile", fileName);
        }

        private void OpenTournament(string fileName)
        {
            if (File.Exists(fileName))
                SQLiteIntf.fOpenTournament(fileName);
            else
                SQLiteIntf.fOpenTournament(fileName);
            sTurniername = Path.GetFileName(fileName);
            sTurniername = sTurniername.Replace(".s3db", "");
            Text = "KeizerForClubs " + sTurniername;
            if (SQLiteIntf.cLangCode == "")
                fSelectLanguage();
            fApplyLanguageText();
            tabMainWindow.Enabled = true;
            mnuListen.Enabled = true;
            mnuHelp.Enabled = true;
            mnuStartLanguage.Enabled = true;
            fLoadPlayerlist();
            fLoadPairingList();
            tbBonusHindered.Value = SQLiteIntf.fGetConfigInt("BONUS.Clubgame");
            tbBonusExcused.Value = SQLiteIntf.fGetConfigInt("BONUS.Excused");
            tbBonusUnexcused.Value = SQLiteIntf.fGetConfigInt("BONUS.Unexcused");
            tbBonusRetired.Value = SQLiteIntf.fGetConfigInt("BONUS.Retired");
            chkFreilosVerteilen.Checked = SQLiteIntf.fGetConfigBool("OPTION.DistBye");
            chkPairingOnlyPlayed.Checked = SQLiteIntf.fGetConfigBool("OPTION.ShowOnlyPlayed");
            numRoundsGameRepeat.Value = (Decimal)SQLiteIntf.fGetConfigInt("OPTION.GameRepeat");
            chkHtml.Checked = SQLiteIntf.fGetConfigBool("OPTION.Html");
            chkXml.Checked = SQLiteIntf.fGetConfigBool("OPTION.Xml");
            chkTxt.Checked = SQLiteIntf.fGetConfigBool("OPTION.Txt");
            chkCsv.Checked = SQLiteIntf.fGetConfigBool("OPTION.Csv");
            numRoundSelect.Value = (Decimal)SQLiteIntf.fGetMaxRound();
            // Der Name des db-Files ist einem ini-File gemerkt, alle anderen Settings in 
            // der Config-Datenbank. Weil die Config-Db nur schwer geöffnet werden kann ohne die 
            // Haupt-Db. Weil die Config-Db eine an die Haupt-Db attached DB ist. 
            if (fileName != ReadDBFileName())
                SaveDBFileName(fileName);
        }

        const string inifile = "cfg/KFC2.ini";
        private static string ReadDBFileName()
        {
            string s = "";
            if (File.Exists(inifile))
                s = File.ReadAllText(inifile).Trim();
            return s;
        }

        private static void SaveDBFileName(string filename) => File.WriteAllText(inifile, filename);

        private void fSelectLanguage()
        {
            frmLangSelect frmLangSelect = new frmLangSelect(SQLiteIntf.cLangCode);
            int num1 = (int)frmLangSelect.ShowDialog();
            SQLiteIntf.cLangCode = !frmLangSelect.radEnglisch.Checked ? (!frmLangSelect.radDeutsch.Checked ? "NL" : "DE") : "EN";
            double num2 = (double)SQLiteIntf.fSetConfigText("LANGCODE", SQLiteIntf.cLangCode);
        }

        void fShowAboutBox()
        {
            var f = new frmAboutBox();
            f.ShowDialog();
        }

        private void MnuProgQuitClick(object sender, EventArgs e) => this.Close();

        private void TabMainWindowSelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage selectedTab = this.tabMainWindow.SelectedTab;
            TabPage tabPlayer = this.tabPlayer;
            this.mnuPaarungen.Enabled = this.tabMainWindow.SelectedTab == this.tabPairings;
        }

        public static void OpenWithDefaultApp(string path)
        {
            using Process fileopener = new Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        void OpenWithDefaultAppByLanguage(string pathWithPlaceholder)
        {
            var lngs = new string[] { SQLiteIntf.cLangCode, "en", "de" };
            foreach (var lng in lngs)
            {
                var p = pathWithPlaceholder.Replace("%LNG%", lng);
                if (File.Exists(p))
                {
                    OpenWithDefaultApp(p);
                    break;
                }
            }
        }

        void MnuHelpDocumentationClick(object sender, EventArgs e) => OpenWithDefaultAppByLanguage("docs\\KeizerForClubs.%LNG%.pdf");

        void MnuHelpKeizerClick(object sender, EventArgs e) => OpenWithDefaultAppByLanguage("docs\\Keizer.%LNG%.pdf");

        void MnuHelpFAQClick(object sender, EventArgs e) => OpenWithDefaultAppByLanguage("docs\\KeizerForClubs.FAQ.%LNG%.pdf");

        void NumRoundSelectValueChanged(object sender, EventArgs e) => fLoadPairingList();

        void CalcRankingToolStripMenuItemClick(object sender, EventArgs e) => fRankingCalculate();

        private void MnuPairingNextRoundClick(object sender, EventArgs e)
        {
            if (SQLiteIntf.fGetPairings_NoResult() == 0)
                fExecutePairing();
            else
                MessageBox.Show(SQLiteIntf.fLocl_GetText("GUI_TEXT", "Hinweis.RundeUnv"), "No....", MessageBoxButtons.OK);
        }

        private void MnuPairingManualClick(object sender, EventArgs e)
        {
            if (SQLiteIntf.fGetPairings_NoResult() == 0)
                fExecutePairingManual();
            else
                MessageBox.Show(SQLiteIntf.fLocl_GetText("GUI_TEXT", "Hinweis.RundeUnv"), "No....", MessageBoxButtons.OK);
        }

        private void MnuPairingDropLastClick(object sender, EventArgs e)
        {
            int maxRound = SQLiteIntf.fGetMaxRound();
            if (this.numRoundSelect.Value != (Decimal)maxRound)
            {
                int num = (int)MessageBox.Show(SQLiteIntf.fLocl_GetText("GUI_TEXT", "Hinweis.DelLetzteRd"), "No....", MessageBoxButtons.OK);
            }
            else
            {
                SQLiteIntf.fDelPairings(maxRound);
                this.numRoundSelect.Value = (Decimal)(maxRound - 1);
                this.fLoadPairingList();
            }
        }

        void MnuAboutBoxClick(object sender, EventArgs e) => fShowAboutBox();

        void MnuStartLanguageClick(object sender, EventArgs e)
        {
            this.fSelectLanguage();
            this.fApplyLanguageText();
            this.fLoadPlayerlist();
            this.fLoadPairingList();
        }

        private void GrdPlayersDataError(object sender, DataGridViewDataErrorEventArgs e) => e.Cancel = false;

        private void ChkPairingOnlyPlayedCheckedChanged(object sender, EventArgs e)
        {
            this.fLoadPairingList();
            SQLiteIntf.fSetConfigBool("OPTION.ShowOnlyPlayed", chkPairingOnlyPlayed.Checked);
        }
        private void FrmMainformFormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.tabMainWindow.Enabled)
            {
                SQLiteIntf.fSetConfigInt("BONUS.Clubgame", this.tbBonusHindered.Value);
                SQLiteIntf.fSetConfigInt("BONUS.Excused", this.tbBonusExcused.Value);
                SQLiteIntf.fSetConfigInt("BONUS.Unexcused", this.tbBonusUnexcused.Value);
                SQLiteIntf.fSetConfigInt("BONUS.Retired", this.tbBonusRetired.Value);
                SQLiteIntf.fSetConfigBool("OPTION.DistBye", this.chkFreilosVerteilen.Checked);
                SQLiteIntf.fSetConfigInt("OPTION.GameRepeat", (int)Convert.ToInt16(this.numRoundsGameRepeat.Value));
                SQLiteIntf.fSetConfigBool("OPTION.Html", this.chkHtml.Checked);
                SQLiteIntf.fSetConfigBool("OPTION.Xml", this.chkXml.Checked);
                SQLiteIntf.fSetConfigBool("OPTION.Txt", this.chkTxt.Checked);
                SQLiteIntf.fSetConfigBool("OPTION.Csv", this.chkCsv.Checked);
            }
            e.Cancel = false;
        }

        private void MnuListenPairingClick(object sender, EventArgs e) => new cReportingUnit(this.sTurniername).fReport_Paarungen((int)Convert.ToInt16(this.numRoundSelect.Value), this.SQLiteIntf);

        private void MnuListenStandingClick(object sender, EventArgs e)
        {
            SQLiteIntf.BeginnTransaktion();
            this.fRankingCalculate();
            SQLiteIntf.EndeTransaktion();
            new cReportingUnit(this.sTurniername).fReport_Tabellenstand(this.SQLiteIntf);
        }

        private void MnuListenParticipantsClick(object sender, EventArgs e) => new cReportingUnit(this.sTurniername).fReport_Teilnehmer(this.SQLiteIntf);

        private void TbBonusValueChanged(object sender, EventArgs e)
        {
            this.lblBonus1Value.Text = this.tbBonusHindered.Value.ToString();
            this.lblBonus2Value.Text = this.tbBonusExcused.Value.ToString();
            this.lblBonus3Value.Text = this.tbBonusUnexcused.Value.ToString();
            this.lblBonus4Value.Text = this.tbBonusRetired.Value.ToString();
        }

        private void GrdPlayersCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.grdPlayers.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string str = this.grdPlayers.Rows[e.RowIndex].Cells[1].Value.ToString();
            if (str == null)
                return;
            if (this.grdPlayers.Rows[e.RowIndex].Cells[0].Value == null)
            {
                if (this.grdPlayers.Rows[e.RowIndex].Cells[2].Value == null)
                    this.grdPlayers.Rows[e.RowIndex].Cells[2].Value = (object)0;
                if (this.grdPlayers.Rows[e.RowIndex].Cells[3].Value == null)
                    this.grdPlayers.Rows[e.RowIndex].Cells[3].Value = (object)SQLiteIntf.fLocl_GetPlayerStateText(cSqliteInterface.ePlayerState.eAvailable);
                if (SQLiteIntf.fCntPlayerNames(str) > 0)
                {
                    int num = (int)MessageBox.Show(SQLiteIntf.fLocl_GetText("GUI_TEXT", "Hinweis.NameDoppelt"), "No....", MessageBoxButtons.OK);
                }
                else
                {
                    SQLiteIntf.fInsPlayerNew(str, (int)Convert.ToInt16(this.grdPlayers.Rows[e.RowIndex].Cells[2].Value));
                    this.grdPlayers.Rows[e.RowIndex].Cells[0].Value = (object)SQLiteIntf.fGetPlayerID(str);
                }
            }
            else
                SQLiteIntf.fUpdPlayer((int)Convert.ToInt16(this.grdPlayers.Rows[e.RowIndex].Cells[0].Value), this.grdPlayers.Rows[e.RowIndex].Cells[1].Value.ToString(), (int)Convert.ToInt16(this.grdPlayers.Rows[e.RowIndex].Cells[2].Value), SQLiteIntf.fLocl_GetPlayerState(this.grdPlayers.Rows[e.RowIndex].Cells[3].Value.ToString()));
        }

        private void GrdPairingsCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            cSqliteInterface.eResults gameResult = SQLiteIntf.fLocl_GetGameResult(this.grdPairings.Rows[e.RowIndex].Cells[7].Value.ToString());
            int int16_1 = (int)Convert.ToInt16(this.grdPairings.Rows[e.RowIndex].Cells[1].Value);
            int int16_2 = (int)Convert.ToInt16(this.grdPairings.Rows[e.RowIndex].Cells[4].Value);
            SQLiteIntf.fUpdPairingResult((int)Convert.ToInt16(this.numRoundSelect.Value), int16_1, int16_2, gameResult);
        }

        private void fApplyLanguageText()
        {
            string[] texte = new string[20];
            colPlayerState.Items.Clear();
            int topicTexte1 = SQLiteIntf.fLocl_GetTopicTexte("PLAYERSTATE", " AND key<>2 ", ref texte);
            for (int index = 0; index < topicTexte1; ++index)
                colPlayerState.Items.Add((object)texte[index]);
            colPairingResult.Items.Clear();
            int topicTexte2 = SQLiteIntf.fLocl_GetTopicTexte("GAMERESULT", " ", ref texte);
            for (int index = 0; index < topicTexte2; ++index)
                colPairingResult.Items.Add((object)texte[index]);
            colPlayerState.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Sp.Status");
            colRating.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Sp.Rating");
            colPlayerName.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Sp.Name");
            colPairingNameWhite.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.Weiss");
            colPairingWhiteAddinfo.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.WeissAdd");
            colPairingNameBlack.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.Schwarz");
            colPairingAddInfoB.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.SchwarzAdd");
            colPairingResult.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.Ergebnis");
            mnuPaarungen.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Paarungen");
            mnuPaarungNext.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Paarung.Next");
            mnuPaarungManuell.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Paarung.NextMan");
            mnuPaarungDropLast.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Paarung.Drop");
            mnuListen.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Listen");
            mnuListenStanding.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Listen.Calc");
            mnuListenPairing.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Listen.Paarungen");
            mnuListenParticipants.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Listen.Teilnehmer");
            mnuHelp.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Hilfe");
            mnuHelpDocumentation.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Hilfe.Doku");
            mnuHelpAbout.Text = SQLiteIntf.fLocl_GetText("GUI_MENU", "Hilfe.About");
            tabPlayer.Text = SQLiteIntf.fLocl_GetText("GUI_TABS", "Spieler");
            tabPairings.Text = SQLiteIntf.fLocl_GetText("GUI_TABS", "Paarungen");
            tabSettings.Text = SQLiteIntf.fLocl_GetText("GUI_TABS", "Einstellungen");
            lblBonusExcused.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Bonus entschuldigt");
            lblBonusUnexcused.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Bonus unentschuldigt");
            lblBonusClubgame.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Bonus verhindert");
            lblBonusRetired.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Bonus Rueckzug");
            chkPairingOnlyPlayed.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Nur gespielte");
            chkFreilosVerteilen.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "FreilosVerteilen");
            lblRunde.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Runde");
            numRoundSelect.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "Runde");
            lblRoundsGameRepeat.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "NumRundeWdh");
            lblOutputTo.Text = SQLiteIntf.fLocl_GetText("GUI_LABEL", "OutputTo");
        }

        private void fLoadPlayerlist()
        {
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = SQLiteIntf.fGetPlayerList(ref pList, "", " ORDER BY ID ");
            this.grdPlayers.Rows.Clear();
            for (int index = 0; index < playerList; ++index)
            {
                this.grdPlayers.Rows.Add();
                this.grdPlayers.Rows[index].Cells[0].Value = (object)pList[index].id;
                this.grdPlayers.Rows[index].Cells[1].Value = (object)pList[index].name;
                this.grdPlayers.Rows[index].Cells[2].Value = (object)pList[index].rating.ToString();
                this.grdPlayers.Rows[index].Cells[3].Value = (object)SQLiteIntf.fLocl_GetPlayerStateText(pList[index].state);
            }
        }

        private void fLoadPairingList()
        {
            cSqliteInterface.stPairing[] pList = new cSqliteInterface.stPairing[50];
            int pairingList = SQLiteIntf.fGetPairingList(ref pList, " WHERE rnd=" + this.numRoundSelect.Value.ToString(), " ORDER BY board ");
            this.grdPairings.Rows.Clear();
            for (int index = 0; index < pairingList; ++index)
            {
                if (!this.chkPairingOnlyPlayed.Checked || pList[index].board < 100)
                {
                    this.grdPairings.Rows.Add();
                    this.grdPairings.Rows[index].Cells[0].Value = (object)pList[index].board.ToString();
                    this.grdPairings.Rows[index].Cells[1].Value = (object)pList[index].id_w.ToString();
                    this.grdPairings.Rows[index].Cells[2].Value = (object)SQLiteIntf.fGetPlayerName(pList[index].id_w);
                    this.grdPairings.Rows[index].Cells[4].Value = (object)pList[index].id_b.ToString();
                    this.grdPairings.Rows[index].Cells[5].Value = (object)SQLiteIntf.fGetPlayerName(pList[index].id_b);
                    this.grdPairings.Rows[index].Cells[7].Value = (object)SQLiteIntf.fLocl_GetGameResultText(pList[index].result);
                }
            }
        }

        private void fRankingCalculate()
        {
            cReportingUnit cReportingUnit = (cReportingUnit)null;
            int maxRound = SQLiteIntf.fGetMaxRound();
            SQLiteIntf.fUpdPairing_ResetValues();
            this.fRankingInit();
            this.fRanking_PlayerPktSumme();
            if (cReportingUnit != null)
            {
                cReportingUnit.swExportDump = new StreamWriter("export\\dumpcalc.csv");
                cReportingUnit.swExportDump.WriteLine("Stand vor 1. Runde");
                cReportingUnit.fReport_Tabellenstand_Voll_CSV(this.SQLiteIntf);
            }
            for (int runde1 = 1; runde1 <= maxRound; ++runde1)
            {
                SQLiteIntf.fUpdPairing_ResetValues();
                for (int runde2 = 1; runde2 <= runde1; ++runde2)
                    this.fRanking_GameEvals(runde2);
                this.fRanking_PlayerPktSumme();
                this.fRankingCalcRanks();
                if (cReportingUnit != null)
                {
                    cReportingUnit.swExportDump.WriteLine("");
                    cReportingUnit.swExportDump.WriteLine("Runde " + runde1.ToString());
                    cReportingUnit.fReport_Paarungen_Txt(runde1, this.SQLiteIntf);
                    cReportingUnit.swExportDump.WriteLine("");
                    cReportingUnit.swExportDump.WriteLine("Tabelle ");
                    cReportingUnit.fReport_Tabellenstand_Voll_CSV(this.SQLiteIntf);
                    cReportingUnit.swExportDump.WriteLine("");
                    cReportingUnit.swExportDump.WriteLine("");
                }
            }
            cReportingUnit?.swExportDump.Close();
        }

        private void fRankingInit()
        {
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerCount = SQLiteIntf.fGetPlayerList(ref pList, " WHERE state NOT IN (9) ", " ORDER BY rating desc ");
            // Das ist die Stelle, die die anfänglichen Keizer-Punkte verteilt. 
            float single = Convert.ToSingle(playerCount - 1 + (playerCount - 1) / 2);
            for (int index = 0; index < playerCount; ++index)
            {
                SQLiteIntf.fUpdPlayer_Init_RankPts(pList[index].id, index + 1, single);
                --single;
            }
        }

        private void fRanking_PlayerPktSumme()
        {
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = SQLiteIntf.fGetPlayerList(ref pList, "", " ");
            for (int index = 0; index < playerList; ++index)
                pList[index].Keizer_SumPts = SQLiteIntf.fGetPlayer_PunktSumme(pList[index].id);
            for (int index = 0; index < playerList; ++index)
                SQLiteIntf.fUpdPlayer_PunktSumme(pList[index].id, pList[index].Keizer_SumPts);
        }

        private void fRankingCalcRanks()
        {
            cSqliteInterface.stPlayer[] pList = new cSqliteInterface.stPlayer[100];
            int playerList = SQLiteIntf.fGetPlayerList(ref pList, " WHERE state NOT IN (9) ", " ORDER BY Keizer_SumPts desc, rating desc ");
            float single = Convert.ToSingle(playerList - 1 + (playerList - 1) / 2);
            for (int index = 0; index < playerList; ++index)
            {
                SQLiteIntf.fUpdPlayer_Init_RankPts(pList[index].id, index + 1, single);
                --single;
            }
        }

        private bool fRanking_GameEvals(int runde)
        {
            cSqliteInterface.stPairing[] pList1 = new cSqliteInterface.stPairing[50];
            cSqliteInterface.stPlayer[] pList2 = new cSqliteInterface.stPlayer[1];
            cSqliteInterface.stPlayer[] pList3 = new cSqliteInterface.stPlayer[1];
            int pairingList = SQLiteIntf.fGetPairingList(ref pList1, " WHERE rnd=" + runde.ToString(), " ORDER BY board ");
            if (pairingList == 0)
                return false;
            for (int index = 0; index < pairingList; ++index)
            {
                float erg_w = 0.0f;
                float erg_s = 0.0f;
                SQLiteIntf.fGetPlayerList(ref pList2, " WHERE ID=" + (object)pList1[index].id_w, " ");
                SQLiteIntf.fGetPlayerList(ref pList3, " WHERE ID=" + (object)pList1[index].id_b, " ");
                if (pList1[index].result == cSqliteInterface.eResults.eWin_White)
                    erg_w = pList3[0].Keizer_StartPts;
                else if (pList1[index].result == cSqliteInterface.eResults.eDraw)
                {
                    erg_w = pList3[0].Keizer_StartPts / 2f;
                    erg_s = pList2[0].Keizer_StartPts / 2f;
                }
                else if (pList1[index].result == cSqliteInterface.eResults.eWin_Black)
                    erg_s = pList2[0].Keizer_StartPts;
                else if (pList1[index].result == cSqliteInterface.eResults.eExcused)
                    erg_w = (float)((double)pList2[0].Keizer_StartPts * (double)this.tbBonusExcused.Value / 100.0);
                else if (pList1[index].result == cSqliteInterface.eResults.eUnexcused)
                    erg_w = (float)((double)pList2[0].Keizer_StartPts * (double)this.tbBonusUnexcused.Value / 100.0);
                else if (pList1[index].result == cSqliteInterface.eResults.eHindered)
                    erg_w = (float)((double)pList2[0].Keizer_StartPts * (double)this.tbBonusHindered.Value / 100.0);
                else if (pList1[index].result == cSqliteInterface.eResults.eFreeWin)
                    erg_w = pList2[0].Keizer_StartPts / 2f;
                if (pList2[0].state == cSqliteInterface.ePlayerState.eRetired)
                {
                    erg_s = (float)((double)pList3[0].Keizer_StartPts * (double)this.tbBonusRetired.Value / 100.0);
                    erg_w = 0.0f;
                }
                if (pList3[0].state == cSqliteInterface.ePlayerState.eRetired)
                {
                    erg_w = (float)((double)pList2[0].Keizer_StartPts * (double)this.tbBonusRetired.Value / 100.0);
                    erg_s = 0.0f;
                }
                SQLiteIntf.fUpdPairingValues(runde, pList1[index].board, erg_w, erg_s);
            }
            return true;
        }

        private bool fExecutePairing()
        {
            this.iPairingPlayerCntAvailable = SQLiteIntf.fGetPlayerList_Available(ref this.pPairingPlayerList);
            this.iPairingMinFreeCnt = 999;
            for (int index = 0; index < this.iPairingPlayerCntAvailable; ++index)
            {
                if (this.pPairingPlayerList[index].FreeCnt < this.iPairingMinFreeCnt)
                    this.iPairingMinFreeCnt = this.pPairingPlayerList[index].FreeCnt;
            }
            SQLiteIntf.BeginnTransaktion();
            this.fRankingCalculate();
            this.iPairingPlayerCntAll = SQLiteIntf.fGetPlayerList_NotDropped(ref this.pPairingPlayerList, " ORDER BY rank ");
            var currRunde = SQLiteIntf.fGetMaxRound() + 1;  // currRunde ist die aktuell ausgeloste Runde. 
            this.iPairingRekursionCnt = 0;
            if (this.fPairingRekursion(0, currRunde))
            {
                this.numRoundSelect.Value = (Decimal)currRunde;
                for (int index = 0; index < this.iPairingPlayerCntAvailable / 2; ++index)
                    SQLiteIntf.fInsPairingNew(currRunde, index + 1, this.pPairingList[index].id_w, this.pPairingList[index].id_b);
                this.fPairingInsertNoPlaying();
                SQLiteIntf.EndeTransaktion();
                this.fLoadPairingList();
                return true;
            }
            int num = (int)MessageBox.Show("No success; adjust options or try manually", "Error");
            return false;
        }

        private void fDealFreilos()
        {
            if (iPairingPlayerCntAvailable % 2 == 1)
                for (int index = this.iPairingPlayerCntAll - 1; index >= 0; --index)
                {
                    if ((!this.chkFreilosVerteilen.Checked || pPairingPlayerList[index].FreeCnt <= this.iPairingMinFreeCnt)
                        &&
                        pPairingPlayerList[index].state == cSqliteInterface.ePlayerState.eAvailable)
                    {
                        pPairingPlayerList[index].state = cSqliteInterface.ePlayerState.eFreilos;
                        break;
                    }
                }
        }

        private bool fPairingRekursion(int brett, int currRunde)
        {
            Debug.WriteLine($"fPairingRekursion brett:{brett}");
            if (brett == 0)
                fDealFreilos();
            int minrunde = currRunde - Convert.ToInt16(this.numRoundsGameRepeat.Value);
            if (this.iPairingRekursionCnt++ > 100000)
                return false;
            if (brett * 2 + 1 >= this.iPairingPlayerCntAvailable)
                return true;
            for (int index1 = 0; index1 < this.iPairingPlayerCntAll; ++index1)
            {
                if (this.pPairingPlayerList[index1].state == cSqliteInterface.ePlayerState.eAvailable)
                {
                    this.pPairingPlayerList[index1].state = cSqliteInterface.ePlayerState.ePaired;
                    for (int index2 = 0; index2 < this.iPairingPlayerCntAll; ++index2)
                    {
                        if (index1 != index2 && this.pPairingPlayerList[index2].state ==
                            cSqliteInterface.ePlayerState.eAvailable
                            && !SQLiteIntf.fGetPairing_CheckVorhanden(minrunde, this.pPairingPlayerList[index1].id,
                                    this.pPairingPlayerList[index2].id))
                        {
                            this.pPairingPlayerList[index2].state = cSqliteInterface.ePlayerState.ePaired;
                            if (SQLiteIntf.fGetPlayerFarbzaehlung(this.pPairingPlayerList[index1].id) >=
                                SQLiteIntf.fGetPlayerFarbzaehlung(this.pPairingPlayerList[index2].id))
                            {
                                this.pPairingList[brett].id_w = this.pPairingPlayerList[index2].id;
                                this.pPairingList[brett].id_b = this.pPairingPlayerList[index1].id;
                            }
                            else
                            {
                                this.pPairingList[brett].id_w = this.pPairingPlayerList[index1].id;
                                this.pPairingList[brett].id_b = this.pPairingPlayerList[index2].id;
                            }
                            Debug.WriteLine($"fPairingRekursion brett:{brett} paired:{pPairingPlayerList[index1].name} vs {pPairingPlayerList[index2].name}");
                            if (this.fPairingRekursion(brett + 1, currRunde))
                                return true;
                            this.pPairingPlayerList[index2].state = cSqliteInterface.ePlayerState.eAvailable;
                            if (this.iPairingRekursionCnt > 100000)
                                return false;
                        }
                    }
                    this.pPairingPlayerList[index1].state = cSqliteInterface.ePlayerState.eAvailable;
                }
            }
            return false;
        }

        private bool fPairingInsertNoPlaying()
        {
            int maxRound = SQLiteIntf.fGetMaxRound();
            int num = 100;
            for (int index = 0; index < this.iPairingPlayerCntAll; ++index)
            {
                if (this.pPairingPlayerList[index].state == cSqliteInterface.ePlayerState.eFreilos)
                {
                    SQLiteIntf.fInsPairingNew(maxRound, num++, this.pPairingPlayerList[index].id, -1);
                    SQLiteIntf.fUpdPairingResult(maxRound, this.pPairingPlayerList[index].id, -1, cSqliteInterface.eResults.eFreeWin);
                }
                if (this.pPairingPlayerList[index].state == cSqliteInterface.ePlayerState.eHindered)
                {
                    SQLiteIntf.fInsPairingNew(maxRound, num++, this.pPairingPlayerList[index].id, -1);
                    SQLiteIntf.fUpdPairingResult(maxRound, this.pPairingPlayerList[index].id, -1, cSqliteInterface.eResults.eHindered);
                }
                if (this.pPairingPlayerList[index].state == cSqliteInterface.ePlayerState.eExcused)
                {
                    SQLiteIntf.fInsPairingNew(maxRound, num++, this.pPairingPlayerList[index].id, -1);
                    SQLiteIntf.fUpdPairingResult(maxRound, this.pPairingPlayerList[index].id, -1, cSqliteInterface.eResults.eExcused);
                }
                if (this.pPairingPlayerList[index].state == cSqliteInterface.ePlayerState.eUnexcused)
                {
                    SQLiteIntf.fInsPairingNew(maxRound, num++, this.pPairingPlayerList[index].id, -1);
                    SQLiteIntf.fUpdPairingResult(maxRound, this.pPairingPlayerList[index].id, -1, cSqliteInterface.eResults.eUnexcused);
                }
            }
            return true;
        }

        private bool fExecutePairingManual()
        {
            frmPairingManual frmPairingManual = new frmPairingManual();
            frmPairingManual.btnCancel.Text = SQLiteIntf.fLocl_GetText("GUI_TEXT", "Abbruch");
            frmPairingManual.colWhite.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.Weiss");
            frmPairingManual.colBlack.HeaderText = SQLiteIntf.fLocl_GetText("GUI_COLS", "Pa.Schwarz");
            this.iPairingPlayerCntAvailable = SQLiteIntf.fGetPlayerList_Available(ref this.pPairingPlayerList);
            for (int index = 0; index < this.iPairingPlayerCntAvailable; ++index)
            {
                if (this.pPairingPlayerList[index].state == cSqliteInterface.ePlayerState.eAvailable)
                    frmPairingManual.lstNames.Items.Add((object)this.pPairingPlayerList[index].name);
            }
            for (int index = 0; index < frmPairingManual.lstNames.Items.Count; index += 2)
                frmPairingManual.grdPaarungen.Rows.Add();
            this.iPairingPlayerCntAll = SQLiteIntf.fGetPlayerList_NotDropped(ref this.pPairingPlayerList, " ORDER BY rank ");
            if (frmPairingManual.ShowDialog() == DialogResult.OK)
            {
                int runde = SQLiteIntf.fGetMaxRound() + 1;
                this.numRoundSelect.Value = (Decimal)runde;
                int num = 1;
                for (int index1 = 0; index1 < frmPairingManual.grdPaarungen.Rows.Count; ++index1)
                {
                    if (frmPairingManual.grdPaarungen.Rows[index1].Cells[0].Value != null && frmPairingManual.grdPaarungen.Rows[index1].Cells[1].Value != null)
                    {
                        string sName1 = frmPairingManual.grdPaarungen.Rows[index1].Cells[0].Value.ToString();
                        string sName2 = frmPairingManual.grdPaarungen.Rows[index1].Cells[1].Value.ToString();
                        int playerId1 = SQLiteIntf.fGetPlayerID(sName1);
                        int playerId2 = SQLiteIntf.fGetPlayerID(sName2);
                        SQLiteIntf.fInsPairingNew(runde, num++, playerId1, playerId2);
                        for (int index2 = 0; index2 < this.iPairingPlayerCntAll; ++index2)
                        {
                            if (this.pPairingPlayerList[index2].id == playerId1 || this.pPairingPlayerList[index2].id == playerId2)
                                this.pPairingPlayerList[index2].state = cSqliteInterface.ePlayerState.ePaired;
                        }
                    }
                }
                this.fPairingInsertNoPlaying();
            }
            this.fLoadPairingList();
            return true;
        }
        /// <summary> Returns the directory where the cfg, docs and export directories are located after checkout. </summary>
        private string GetCheckoutBaseDir()
        {
            if (baseDir == null)
            {
                baseDir = Path.Join(Directory.GetCurrentDirectory(), "..");
                bool ok = false;
                for (int i = 0; i < 4 && !ok; ++i)
                {
                    ok = Directory.EnumerateFiles(baseDir, "KFC*.sln").Count() == 1;
                    if (ok)
                    {
                        var dirsInDir = Directory.EnumerateDirectories(baseDir).Select(d => new DirectoryInfo(d).Name).ToList();
                        ok = cfgDocsExport.Intersect(dirsInDir).Count() == cfgDocsExport.Count();
                    } 
                    if (!ok)
                        baseDir = Path.Join(baseDir, "..");
                }
                if (!ok)
                    baseDir = "";
            }
            return baseDir;
        }
        static string baseDir;

        static string[] cfgDocsExport = "cfg docs export".Split();

        private void CopyCfgDocsExport()
        {
            // This is only needed the first time after downloading KFC from Github, compiling and running.
            // A post-build step in C#. 
            foreach (var dir in cfgDocsExport)
                CopyCdeDirectory(dir);
        }

        private void CopyCdeDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var baseDir = GetCheckoutBaseDir();
            if (!string.IsNullOrEmpty(baseDir))
            {
                var files = Directory.EnumerateFiles(Path.Join(baseDir, dir));
                foreach (var f in files)
                {
                    var target = Path.Join(dir, Path.GetFileName(f));
                    if (!File.Exists(target))
                        File.Copy(f, target);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabMainWindow = new TabControl();
            this.tabPlayer = new TabPage();
            this.grdPlayers = new DataGridView();
            this.colPlayerID = new DataGridViewTextBoxColumn();
            this.colPlayerName = new DataGridViewTextBoxColumn();
            this.colRating = new DataGridViewTextBoxColumn();
            this.colPlayerState = new DataGridViewComboBoxColumn();
            this.tabPairings = new TabPage();
            this.pnlPairingPanel = new Panel();
            this.chkPairingOnlyPlayed = new CheckBox();
            this.numRoundSelect = new NumericUpDown();
            this.lblRunde = new Label();
            this.grdPairings = new DataGridView();
            this.colPairgBoard = new DataGridViewTextBoxColumn();
            this.IDW = new DataGridViewTextBoxColumn();
            this.colPairingNameWhite = new DataGridViewTextBoxColumn();
            this.colPairingWhiteAddinfo = new DataGridViewTextBoxColumn();
            this.colPairingIDB = new DataGridViewTextBoxColumn();
            this.colPairingNameBlack = new DataGridViewTextBoxColumn();
            this.colPairingAddInfoB = new DataGridViewTextBoxColumn();
            this.colPairingResult = new DataGridViewComboBoxColumn();
            this.tabSettings = new TabPage();
            this.lblBonus4Value = new Label();
            this.lblBonus3Value = new Label();
            this.lblBonus2Value = new Label();
            this.lblBonus1Value = new Label();
            this.lblRoundsGameRepeat = new Label();
            this.lblOutputTo = new Label();
            this.numRoundsGameRepeat = new NumericUpDown();
            this.chkFreilosVerteilen = new CheckBox();
            this.chkHtml = new CheckBox();
            this.chkXml = new CheckBox();
            this.chkTxt = new CheckBox();
            this.chkCsv = new CheckBox();
            this.lblBonusRetired = new Label();
            this.tbBonusRetired = new TrackBar();
            this.lblBonusClubgame = new Label();
            this.lblBonusUnexcused = new Label();
            this.lblBonusExcused = new Label();
            this.tbBonusHindered = new TrackBar();
            this.tbBonusUnexcused = new TrackBar();
            this.tbBonusExcused = new TrackBar();
            this.mnuMainmenu = new MenuStrip();
            this.mnuTurnierstart = new ToolStripMenuItem();
            this.mnuStartStart = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripSeparator();
            this.mnuStartLanguage = new ToolStripMenuItem();
            this.mnuPaarungen = new ToolStripMenuItem();
            this.mnuPaarungNext = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripSeparator();
            this.mnuPaarungManuell = new ToolStripMenuItem();
            this.toolStripMenuItem3 = new ToolStripSeparator();
            this.mnuPaarungDropLast = new ToolStripMenuItem();
            this.mnuListen = new ToolStripMenuItem();
            this.mnuListenPairing = new ToolStripMenuItem();
            this.mnuListenStanding = new ToolStripMenuItem();
            this.mnuListenParticipants = new ToolStripMenuItem();
            this.mnuHelp = new ToolStripMenuItem();
            dlgOpenTournament = new OpenFileDialog();
            this.mnuHelpDocumentation = new ToolStripMenuItem();
            this.mnuHelpFaq = new ToolStripMenuItem();
            this.mnuHelpAbout = new ToolStripMenuItem();
            this.tabMainWindow.SuspendLayout();
            this.tabPlayer.SuspendLayout();
            ((ISupportInitialize)this.grdPlayers).BeginInit();
            this.tabPairings.SuspendLayout();
            this.pnlPairingPanel.SuspendLayout();
            this.numRoundSelect.BeginInit();
            ((ISupportInitialize)this.grdPairings).BeginInit();
            this.tabSettings.SuspendLayout();
            this.numRoundsGameRepeat.BeginInit();
            this.tbBonusRetired.BeginInit();
            this.tbBonusHindered.BeginInit();
            this.tbBonusUnexcused.BeginInit();
            this.tbBonusExcused.BeginInit();
            this.mnuMainmenu.SuspendLayout();
            this.SuspendLayout();
            this.tabMainWindow.Alignment = TabAlignment.Bottom;
            this.tabMainWindow.Controls.Add((Control)this.tabPlayer);
            this.tabMainWindow.Controls.Add((Control)this.tabPairings);
            this.tabMainWindow.Controls.Add((Control)this.tabSettings);
            this.tabMainWindow.Dock = DockStyle.Fill;
            this.tabMainWindow.Enabled = false;
            this.tabMainWindow.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.tabMainWindow.Location = new Point(0, 24);
            this.tabMainWindow.Name = "tabMainWindow";
            this.tabMainWindow.SelectedIndex = 0;
            this.tabMainWindow.Size = new Size(704, 401);
            this.tabMainWindow.TabIndex = 0;
            this.tabMainWindow.SelectedIndexChanged += new EventHandler(this.TabMainWindowSelectedIndexChanged);
            this.tabPlayer.Controls.Add((Control)this.grdPlayers);
            this.tabPlayer.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.tabPlayer.Location = new Point(4, 4);
            this.tabPlayer.Name = "tabPlayer";
            this.tabPlayer.Padding = new Padding(3);
            this.tabPlayer.Size = new Size(696, 373);
            this.tabPlayer.TabIndex = 0;
            this.tabPlayer.Text = "Player";
            this.tabPlayer.UseVisualStyleBackColor = true;
            this.grdPlayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPlayers.Columns.AddRange((DataGridViewColumn)this.colPlayerID, (DataGridViewColumn)this.colPlayerName, (DataGridViewColumn)this.colRating, (DataGridViewColumn)this.colPlayerState);
            this.grdPlayers.Dock = DockStyle.Fill;
            this.grdPlayers.Location = new Point(3, 3);
            this.grdPlayers.Name = "grdPlayers";
            this.grdPlayers.Size = new Size(690, 367);
            this.grdPlayers.TabIndex = 0;
            this.grdPlayers.CellEndEdit += new DataGridViewCellEventHandler(this.GrdPlayersCellEndEdit);
            this.grdPlayers.DataError += new DataGridViewDataErrorEventHandler(this.GrdPlayersDataError);
            this.colPlayerID.HeaderText = "ID";
            this.colPlayerID.Name = "colPlayerID";
            this.colPlayerID.ReadOnly = true;
            this.colPlayerID.Width = 40;
            this.colPlayerName.HeaderText = "Name";
            this.colPlayerName.Name = "colPlayerName";
            this.colPlayerName.Width = 200;
            this.colRating.HeaderText = "Rating";
            this.colRating.Name = "colRating";
            this.colPlayerState.HeaderText = "State";
            this.colPlayerState.Name = "colPlayerState";
            this.colPlayerState.Width = 200;
            this.tabPairings.Controls.Add((Control)this.pnlPairingPanel);
            this.tabPairings.Controls.Add((Control)this.grdPairings);
            this.tabPairings.Location = new Point(4, 4);
            this.tabPairings.Name = "tabPairings";
            this.tabPairings.Padding = new Padding(3);
            this.tabPairings.Size = new Size(696, 373);
            this.tabPairings.TabIndex = 1;
            this.tabPairings.Text = "Pairings";
            this.tabPairings.UseVisualStyleBackColor = true;
            this.pnlPairingPanel.Controls.Add((Control)this.chkPairingOnlyPlayed);
            this.pnlPairingPanel.Controls.Add((Control)this.numRoundSelect);
            this.pnlPairingPanel.Controls.Add((Control)this.lblRunde);
            this.pnlPairingPanel.Dock = DockStyle.Top;
            this.pnlPairingPanel.Location = new Point(3, 3);
            this.pnlPairingPanel.Name = "pnlPairingPanel";
            this.pnlPairingPanel.Size = new Size(690, 51);
            this.pnlPairingPanel.TabIndex = 1;
            this.chkPairingOnlyPlayed.Checked = true;
            this.chkPairingOnlyPlayed.CheckState = CheckState.Checked;
            this.chkPairingOnlyPlayed.Location = new Point(480, 18);
            this.chkPairingOnlyPlayed.Name = "chkPairingOnlyPlayed";
            this.chkPairingOnlyPlayed.Size = new Size(164, 24);
            this.chkPairingOnlyPlayed.TabIndex = 2;
            this.chkPairingOnlyPlayed.Text = "only played games";
            this.chkPairingOnlyPlayed.UseVisualStyleBackColor = true;
            this.chkPairingOnlyPlayed.CheckedChanged += new EventHandler(this.ChkPairingOnlyPlayedCheckedChanged);
            this.numRoundSelect.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.numRoundSelect.Location = new Point(237, 16);
            this.numRoundSelect.Maximum = new Decimal(new int[4]
            {
        20,
        0,
        0,
        0
            });
            this.numRoundSelect.Name = "numRoundSelect";
            this.numRoundSelect.Size = new Size(59, 26);
            this.numRoundSelect.TabIndex = 1;
            this.numRoundSelect.ValueChanged += new EventHandler(this.NumRoundSelectValueChanged);
            this.lblRunde.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblRunde.Location = new Point(109, 17);
            this.lblRunde.Name = "lblRunde";
            this.lblRunde.Size = new Size(113, 23);
            this.lblRunde.TabIndex = 0;
            this.lblRunde.Text = "Round";
            this.lblRunde.TextAlign = ContentAlignment.MiddleRight;
            this.grdPairings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.grdPairings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPairings.Columns.AddRange((DataGridViewColumn)this.colPairgBoard, (DataGridViewColumn)this.IDW, (DataGridViewColumn)this.colPairingNameWhite, (DataGridViewColumn)this.colPairingWhiteAddinfo, (DataGridViewColumn)this.colPairingIDB, (DataGridViewColumn)this.colPairingNameBlack, (DataGridViewColumn)this.colPairingAddInfoB, (DataGridViewColumn)this.colPairingResult);
            this.grdPairings.Location = new Point(3, 59);
            this.grdPairings.Name = "grdPairings";
            this.grdPairings.Size = new Size(690, 314);
            this.grdPairings.TabIndex = 0;
            this.grdPairings.CellEndEdit += new DataGridViewCellEventHandler(this.GrdPairingsCellEndEdit);
            this.colPairgBoard.HeaderText = "Bd.";
            this.colPairgBoard.Name = "colPairgBoard";
            this.colPairgBoard.ReadOnly = true;
            this.colPairgBoard.Width = 30;
            this.IDW.HeaderText = "ID_W";
            this.IDW.Name = "IDW";
            this.IDW.ReadOnly = true;
            this.IDW.Visible = false;
            this.IDW.Width = 40;
            this.colPairingNameWhite.HeaderText = "White";
            this.colPairingNameWhite.Name = "colPairingNameWhite";
            this.colPairingNameWhite.ReadOnly = true;
            this.colPairingNameWhite.Width = 180;
            this.colPairingWhiteAddinfo.HeaderText = "AddInfo";
            this.colPairingWhiteAddinfo.Name = "colPairingWhiteAddinfo";
            this.colPairingWhiteAddinfo.ReadOnly = true;
            this.colPairingWhiteAddinfo.Width = 50;
            this.colPairingIDB.HeaderText = "ID_B";
            this.colPairingIDB.Name = "colPairingIDB";
            this.colPairingIDB.ReadOnly = true;
            this.colPairingIDB.Visible = false;
            this.colPairingIDB.Width = 40;
            this.colPairingNameBlack.HeaderText = "Black";
            this.colPairingNameBlack.Name = "colPairingNameBlack";
            this.colPairingNameBlack.ReadOnly = true;
            this.colPairingNameBlack.Width = 180;
            this.colPairingAddInfoB.HeaderText = "AddInfo";
            this.colPairingAddInfoB.Name = "colPairingAddInfoB";
            this.colPairingAddInfoB.ReadOnly = true;
            this.colPairingAddInfoB.Width = 50;
            this.colPairingResult.HeaderText = "Result";
            this.colPairingResult.Name = "colPairingResult";
            this.colPairingResult.Width = 140;
            this.tabSettings.Controls.Add((Control)this.lblBonus4Value);
            this.tabSettings.Controls.Add((Control)this.lblBonus3Value);
            this.tabSettings.Controls.Add((Control)this.lblBonus2Value);
            this.tabSettings.Controls.Add((Control)this.lblBonus1Value);
            this.tabSettings.Controls.Add((Control)this.lblRoundsGameRepeat);
            this.tabSettings.Controls.Add((Control)this.numRoundsGameRepeat);
            this.tabSettings.Controls.Add((Control)this.chkFreilosVerteilen);
            this.tabSettings.Controls.Add((Control)this.lblBonusRetired);
            this.tabSettings.Controls.Add((Control)this.tbBonusRetired);
            this.tabSettings.Controls.Add((Control)this.lblBonusClubgame);
            this.tabSettings.Controls.Add((Control)this.lblBonusUnexcused);
            this.tabSettings.Controls.Add((Control)this.lblBonusExcused);
            this.tabSettings.Controls.Add((Control)this.tbBonusHindered);
            this.tabSettings.Controls.Add((Control)this.tbBonusUnexcused);
            this.tabSettings.Controls.Add((Control)this.tbBonusExcused);
            this.tabSettings.Controls.Add(this.chkHtml);
            this.tabSettings.Controls.Add(this.chkXml);
            this.tabSettings.Controls.Add(this.chkTxt);
            this.tabSettings.Controls.Add(this.chkCsv);
            this.tabSettings.Controls.Add(this.lblOutputTo);
            this.tabSettings.Location = new Point(4, 4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new Padding(3);
            this.tabSettings.Size = new Size(696, 373);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            this.lblBonus4Value.Location = new Point(485, 181);
            this.lblBonus4Value.Name = "lblBonus4Value";
            this.lblBonus4Value.Size = new Size(100, 23);
            this.lblBonus4Value.TabIndex = 14;
            this.lblBonus4Value.Text = "...";
            this.lblBonus3Value.Location = new Point(485, 131);
            this.lblBonus3Value.Name = "lblBonus3Value";
            this.lblBonus3Value.Size = new Size(100, 23);
            this.lblBonus3Value.TabIndex = 13;
            this.lblBonus3Value.Text = "...";
            this.lblBonus2Value.Location = new Point(485, 86);
            this.lblBonus2Value.Name = "lblBonus2Value";
            this.lblBonus2Value.Size = new Size(100, 23);
            this.lblBonus2Value.TabIndex = 12;
            this.lblBonus2Value.Text = "...";
            this.lblBonus1Value.Location = new Point(485, 30);
            this.lblBonus1Value.Name = "lblBonus1Value";
            this.lblBonus1Value.Size = new Size(100, 23);
            this.lblBonus1Value.TabIndex = 11;
            this.lblBonus1Value.Text = "...";
            this.lblRoundsGameRepeat.Location = new Point(44, 264);
            this.lblRoundsGameRepeat.Name = "lblRoundsGameRepeat";
            this.lblRoundsGameRepeat.Size = new Size(222, 23);
            this.lblRoundsGameRepeat.TabIndex = 10;
            this.lblRoundsGameRepeat.Text = "# Rounds before paired again";
            this.numRoundsGameRepeat.Location = new Point(301, 262);
            this.numRoundsGameRepeat.Maximum = new Decimal(new int[4]
            {
        50,
        0,
        0,
        0
            });
            this.numRoundsGameRepeat.Name = "numRoundsGameRepeat";
            this.numRoundsGameRepeat.Size = new Size(40, 21);
            this.numRoundsGameRepeat.TabIndex = 9;
            this.chkFreilosVerteilen.CheckAlign = ContentAlignment.MiddleRight;
            this.chkFreilosVerteilen.Checked = true;
            this.chkFreilosVerteilen.CheckState = CheckState.Checked;
            this.chkFreilosVerteilen.Location = new Point(44, 232);
            this.chkFreilosVerteilen.Name = "chkFreilosVerteilen";
            this.chkFreilosVerteilen.Size = new Size(270, 24);
            this.chkFreilosVerteilen.TabIndex = 8;
            this.chkFreilosVerteilen.Text = "Assign bye's even";
            this.chkFreilosVerteilen.UseVisualStyleBackColor = true;

            var yOutput = 287;

            this.chkHtml.CheckAlign = ContentAlignment.MiddleLeft;
            this.chkHtml.Location = new Point(246, yOutput);
            this.chkHtml.Name = "chkHtml";
            this.chkHtml.Size = new Size(45, 24);
            this.chkHtml.TabIndex = 10;
            this.chkHtml.Text = "Html";
            this.chkHtml.UseVisualStyleBackColor = true;

            this.chkXml.CheckAlign = ContentAlignment.MiddleLeft;
            this.chkXml.Location = new Point(315, yOutput);
            this.chkXml.Name = "chkXml";
            this.chkXml.Size = new Size(44, 24);
            this.chkXml.TabIndex = 11;
            this.chkXml.Text = "Xml";
            this.chkXml.UseVisualStyleBackColor = true;

            this.chkTxt.CheckAlign = ContentAlignment.MiddleLeft;
            this.chkTxt.Location = new Point(384, yOutput);
            this.chkTxt.Name = "chkTxt";
            this.chkTxt.Size = new Size(44, 24);
            this.chkTxt.TabIndex = 12;
            this.chkTxt.Text = "Txt";
            this.chkTxt.UseVisualStyleBackColor = true;

            this.chkCsv.CheckAlign = ContentAlignment.MiddleLeft;
            this.chkCsv.Location = new Point(453, yOutput);
            this.chkCsv.Name = "chkCsv";
            this.chkCsv.Size = new Size(44, 24);
            this.chkCsv.TabIndex = 13;
            this.chkCsv.Text = "Csv";
            this.chkCsv.UseVisualStyleBackColor = true;

            this.lblOutputTo.Location = new Point(44, yOutput);
            this.lblOutputTo.Name = "lblOutputTo";
            this.lblOutputTo.Size = new Size(100, 23);
            this.lblOutputTo.Text = "# Rounds before paired again";
            this.lblOutputTo.TextAlign = ContentAlignment.MiddleLeft; 

            this.lblBonusRetired.Location = new Point(44, 181);
            this.lblBonusRetired.Name = "lblBonusRetired";
            this.lblBonusRetired.Size = new Size(149, 23);
            this.lblBonusRetired.TabIndex = 7;
            this.lblBonusRetired.Text = "Bonus against \"retired\"";
            this.tbBonusRetired.LargeChange = 20;
            this.tbBonusRetired.Location = new Point(246, 181);
            this.tbBonusRetired.Maximum = 100;
            this.tbBonusRetired.Name = "tbBonusRetired";
            this.tbBonusRetired.Size = new Size(207, 45);
            this.tbBonusRetired.SmallChange = 5;
            this.tbBonusRetired.TabIndex = 6;
            this.tbBonusRetired.TickFrequency = 5;
            this.tbBonusRetired.Value = 75;
            this.tbBonusRetired.ValueChanged += new EventHandler(this.TbBonusValueChanged);
            this.lblBonusClubgame.Location = new Point(44, 30);
            this.lblBonusClubgame.Name = "lblBonusClubgame";
            this.lblBonusClubgame.Size = new Size(166, 23);
            this.lblBonusClubgame.TabIndex = 5;
            this.lblBonusClubgame.Text = "Bonus: hindered, club game";
            this.lblBonusUnexcused.Location = new Point(43, 131);
            this.lblBonusUnexcused.Name = "lblBonusUnexcused";
            this.lblBonusUnexcused.Size = new Size(149, 23);
            this.lblBonusUnexcused.TabIndex = 4;
            this.lblBonusUnexcused.Text = "Bonus: missing unexcused";
            this.lblBonusExcused.Location = new Point(43, 86);
            this.lblBonusExcused.Name = "lblBonusExcused";
            this.lblBonusExcused.Size = new Size(149, 23);
            this.lblBonusExcused.TabIndex = 3;
            this.lblBonusExcused.Text = "Bonus: missing excused";
            this.tbBonusHindered.LargeChange = 20;
            this.tbBonusHindered.Location = new Point(245, 30);
            this.tbBonusHindered.Maximum = 100;
            this.tbBonusHindered.Name = "tbBonusHindered";
            this.tbBonusHindered.Size = new Size(208, 45);
            this.tbBonusHindered.SmallChange = 5;
            this.tbBonusHindered.TabIndex = 2;
            this.tbBonusHindered.TickFrequency = 5;
            this.tbBonusHindered.Value = 65;
            this.tbBonusHindered.ValueChanged += new EventHandler(this.TbBonusValueChanged);
            this.tbBonusUnexcused.LargeChange = 20;
            this.tbBonusUnexcused.Location = new Point(245, 131);
            this.tbBonusUnexcused.Maximum = 100;
            this.tbBonusUnexcused.Name = "tbBonusUnexcused";
            this.tbBonusUnexcused.Size = new Size(207, 45);
            this.tbBonusUnexcused.SmallChange = 5;
            this.tbBonusUnexcused.TabIndex = 1;
            this.tbBonusUnexcused.TickFrequency = 5;
            this.tbBonusUnexcused.ValueChanged += new EventHandler(this.TbBonusValueChanged);
            this.tbBonusExcused.LargeChange = 20;
            this.tbBonusExcused.Location = new Point(245, 80);
            this.tbBonusExcused.Maximum = 100;
            this.tbBonusExcused.Name = "tbBonusExcused";
            this.tbBonusExcused.Size = new Size(207, 45);
            this.tbBonusExcused.SmallChange = 5;
            this.tbBonusExcused.TabIndex = 0;
            this.tbBonusExcused.TickFrequency = 5;
            this.tbBonusExcused.Value = 35;
            this.tbBonusExcused.ValueChanged += new EventHandler(this.TbBonusValueChanged);
            this.mnuMainmenu.Items.AddRange(new ToolStripItem[4]
            {
        (ToolStripItem) this.mnuTurnierstart,
        (ToolStripItem) this.mnuPaarungen,
        (ToolStripItem) this.mnuListen,
        (ToolStripItem) this.mnuHelp
            });
            this.mnuMainmenu.Location = new Point(0, 0);
            this.mnuMainmenu.Name = "mnuMainmenu";
            this.mnuMainmenu.Size = new Size(704, 24);
            this.mnuMainmenu.TabIndex = 1;
            this.mnuMainmenu.Text = "Program";
            this.mnuTurnierstart.DropDownItems.AddRange(new ToolStripItem[3]
            {
        (ToolStripItem) this.mnuStartStart,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.mnuStartLanguage
            });
            this.mnuTurnierstart.Name = "mnuTurnierstart";
            this.mnuTurnierstart.Size = new Size(52, 20);
            this.mnuTurnierstart.Text = "Start...";
            this.mnuStartStart.Name = "mnuStartStart";
            this.mnuStartStart.Size = new Size(218, 22);
            this.mnuStartStart.Text = "Start...";
            this.mnuStartStart.Click += new EventHandler(this.OpenStartTournamentToolStripMenuItemClick);
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(215, 6);
            this.mnuStartLanguage.Enabled = false;
            this.mnuStartLanguage.Name = "mnuStartLanguage";
            this.mnuStartLanguage.Size = new Size(218, 22);
            this.mnuStartLanguage.Text = "Language,Sprache,Spraak...";
            this.mnuStartLanguage.Click += new EventHandler(this.MnuStartLanguageClick);
            this.mnuPaarungen.DropDownItems.AddRange(new ToolStripItem[5]
            {
        (ToolStripItem) this.mnuPaarungNext,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.mnuPaarungManuell,
        (ToolStripItem) this.toolStripMenuItem3,
        (ToolStripItem) this.mnuPaarungDropLast
            });
            this.mnuPaarungen.Enabled = false;
            this.mnuPaarungen.Name = "mnuPaarungen";
            this.mnuPaarungen.Size = new Size(56, 20);
            this.mnuPaarungen.Text = "Pairing";
            this.mnuPaarungNext.Name = "mnuPaarungNext";
            this.mnuPaarungNext.Size = new Size(183, 22);
            this.mnuPaarungNext.Text = "Next round";
            this.mnuPaarungNext.Click += new EventHandler(this.MnuPairingNextRoundClick);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new Size(180, 6);
            this.mnuPaarungManuell.Name = "mnuPaarungManuell";
            this.mnuPaarungManuell.Size = new Size(183, 22);
            this.mnuPaarungManuell.Text = "Manual next round...";
            this.mnuPaarungManuell.Click += new EventHandler(this.MnuPairingManualClick);
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new Size(180, 6);
            this.mnuPaarungDropLast.Name = "mnuPaarungDropLast";
            this.mnuPaarungDropLast.Size = new Size(183, 22);
            this.mnuPaarungDropLast.Text = "Drop last round";
            this.mnuPaarungDropLast.Click += new EventHandler(this.MnuPairingDropLastClick);
            this.mnuListen.DropDownItems.AddRange(new ToolStripItem[3]
            {
        (ToolStripItem) this.mnuListenPairing,
        (ToolStripItem) this.mnuListenStanding,
        (ToolStripItem) this.mnuListenParticipants
            });
            this.mnuListen.Enabled = false;
            this.mnuListen.Name = "mnuListen";
            this.mnuListen.Size = new Size(42, 20);
            this.mnuListen.Text = "Lists";
            this.mnuListenPairing.Name = "mnuListenPairing";
            this.mnuListenPairing.Size = new Size(158, 22);
            this.mnuListenPairing.Text = "Pairings/Results";
            this.mnuListenPairing.Click += new EventHandler(this.MnuListenPairingClick);
            this.mnuListenStanding.Name = "mnuListenStanding";
            this.mnuListenStanding.Size = new Size(158, 22);
            this.mnuListenStanding.Text = "Standing";
            this.mnuListenStanding.Click += new EventHandler(this.MnuListenStandingClick);
            this.mnuListenParticipants.Name = "mnuListenParticipants";
            this.mnuListenParticipants.Size = new Size(158, 22);
            this.mnuListenParticipants.Text = "Participants";
            this.mnuListenParticipants.Click += new EventHandler(this.MnuListenParticipantsClick);
            this.mnuHelp.DropDownItems.AddRange(new ToolStripItem[]
            {
        (ToolStripItem) this.mnuHelpDocumentation,
        (ToolStripItem) this.mnuHelpFaq,
        (ToolStripItem) this.mnuHelpAbout,
            });
            this.mnuHelp.Enabled = false;
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new Size(44, 20);
            this.mnuHelp.Text = "Help";
            dlgOpenTournament.CheckFileExists = false;
            dlgOpenTournament.DefaultExt = "*.s3db";
            dlgOpenTournament.FileName = "turnier_1";
            dlgOpenTournament.Filter = "Tourn.|*.s3db";
            dlgOpenTournament.Title = "Open/Start Tournament";
            mnuHelpDocumentation.Name = "mnuHelpDocumentation";
            mnuHelpDocumentation.Size = new Size(157, 22);
            mnuHelpDocumentation.Text = "Documentation";
            mnuHelpDocumentation.Click += new EventHandler(MnuHelpDocumentationClick);
            mnuHelpFaq.Name = "mnuHelpFaq";
            mnuHelpFaq.Size = new Size(157, 22);
            mnuHelpFaq.Text = "FAQ";
            mnuHelpFaq.Click += new EventHandler(MnuHelpFAQClick);
            mnuHelpAbout.Name = "mnuHelpFaq";
            mnuHelpAbout.Size = new Size(157, 22);
            mnuHelpAbout.Text = "About";
            mnuHelpAbout.Click += new EventHandler(MnuAboutBoxClick);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(704, 425);
            this.Controls.Add((Control)this.tabMainWindow);
            this.Controls.Add((Control)this.mnuMainmenu);
            this.MainMenuStrip = this.mnuMainmenu;
            this.Name = nameof(frmMainform);
            this.Text = "KeizerForClubs";
            this.FormClosing += new FormClosingEventHandler(this.FrmMainformFormClosing);
            this.tabMainWindow.ResumeLayout(false);
            this.tabPlayer.ResumeLayout(false);
            ((ISupportInitialize)this.grdPlayers).EndInit();
            this.tabPairings.ResumeLayout(false);
            this.pnlPairingPanel.ResumeLayout(false);
            this.numRoundSelect.EndInit();
            ((ISupportInitialize)this.grdPairings).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.numRoundsGameRepeat.EndInit();
            this.tbBonusRetired.EndInit();
            this.tbBonusHindered.EndInit();
            this.tbBonusUnexcused.EndInit();
            this.tbBonusExcused.EndInit();
            this.mnuMainmenu.ResumeLayout(false);
            this.mnuMainmenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            try
            {
                if (Args.Length > 0)
                    OpenTournament(Args[0]);
                else if (File.Exists(ReadDBFileName()))
                    OpenTournament(ReadDBFileName());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, SQLiteIntf.fLocl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
            }
        }
    }
}
