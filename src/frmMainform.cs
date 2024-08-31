using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AwiUtils;
using PuzzleKnocker;
using static KeizerForClubs.SqliteInterface;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using ToolTip = System.Windows.Forms.ToolTip;
using TrackBar = System.Windows.Forms.TrackBar;

namespace KeizerForClubs
{
    public partial class frmMainform : Form
    {
        private readonly SqliteInterface db = new SqliteInterface();
        private readonly RankingCalculator ranking;
        private string sTurniername = "", sFilename = "unknown";
        private int iPairingRekursionCnt;
        private int iPairingPlayerCntAll;
        private int iPairingPlayerCntAvailable;
        private int iPairingMinFreeCnt;
        private stPlayer[] pPairingPlayerList = new stPlayer[100];
        private stPairing[] pPairingList = new stPairing[50];
        private IContainer components;
        private ToolStripMenuItem mnuHelpDocumentation;
        private ToolStripMenuItem mnuHelpFaq, mnuHelp, mnuHelpAbout;

        private ToolStripMenuItem mnuListenParticipants;
        private ToolStripMenuItem mnuListenStanding, mnuListenStandingFull;
        private ToolStripMenuItem mnuListenPairing, mnuListenAll;
        private NumericUpDown numRoundsGameRepeat;
        private ComboBox ddlRatioFirst2Last, ddlFirstRoundRandom;
        private ToolTip tooltip;
        private Label lblRoundsGameRepeat, lblOutputTo, lblNiceName, lblRatioFirst2Last, lblFirstRoundRandom;
        private TextBox tbNiceName;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuStartStart;
        private CheckBox chkFreilosVerteilen, chkNovusRandomBoard;
        private CheckBox chkHtml, chkXml, chkTxt, chkCsv, chkWickerNormalization;
        private ToolStripMenuItem mnuStartLanguage;
        private ToolStripMenuItem mnuPlayers, mnuPlayersImport, mnuPlayersDeleteAll, mnuPlayersRebaseIds;
        private CheckBox chkPairingOnlyPlayed;
        private ToolStripMenuItem mnuPaarungDropLast;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuPaarungManuell, mnuPaarungThisMan;
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
        private Label lblBonusClubValue, lblBonusExcusedValue, lblBonusUnexcusedValue, lblBonusRetiredValue, 
            lblBonusFreilosValue, lblBonusVerlustValue;
        internal TrackBar tbBonusClub, tbBonusExcused, tbBonusUnexcused, tbBonusRetired, tbBonusFreilos, tbBonusVerlust;
        private Label lblBonusClub, lblBonusExcused, lblBonusUnexcused, lblBonusRetired, lblBonusFreilos, lblBonusVerlust;
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
        private Button btDonate1, btDonate2;
        private DonateButton donateButton1, donateButton2;
        private int numClicks;
        private readonly string[] Args;
        private readonly Random random = new Random();

        public frmMainform(string[] args)
        {
            Args = args;
            CopyCfgDocsExport();
            InitializeComponent();
            ranking = new RankingCalculator(db, this, false);
            donateButton1 = new DonateButton(btDonate1, ReadDonated(), () => numClicks, 50, () => true, 20);
            donateButton2 = new DonateButton(btDonate2, ReadDonated(), () => numClicks, 120, () => true, 20);
            IncNumClicks();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RestoreWindowSettings();
        }

        private void OpenStartTournamentToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (dlgOpenTournament.ShowDialog() != DialogResult.OK)
                return;
            string fileName = dlgOpenTournament.FileName;
            OpenTournament(fileName);
            db.SetConfigText("TournamentFile", fileName);
        }

        private void OpenTournament(string fileName)
        {
            if (File.Exists(fileName))
                db.OpenTournament(fileName);
            else
                db.OpenTournament(fileName);
            dlgOpenTournament.FileName = Path.GetFileName(fileName);
            dlgOpenTournament.InitialDirectory = Path.GetDirectoryName(fileName);
            SetTurnierAndFilename(db.GetConfigText("OPTION.NiceName"));
            if (db.LangCode == "")
                fSelectLanguage();
            ApplyLanguageText();
            tabMainWindow.Enabled = true;
            mnuListen.Enabled = true;
            mnuHelp.Enabled = mnuStartLanguage.Enabled = true;
            LoadPlayerlist();
            LoadPairingList();

            tbBonusClub.Value = db.GetConfigInt("BONUS.Clubgame");
            tbBonusExcused.Value = db.GetConfigInt("BONUS.Excused");
            tbBonusUnexcused.Value = db.GetConfigInt("BONUS.Unexcused");
            tbBonusRetired.Value = db.GetConfigInt("BONUS.Retired");
            tbBonusFreilos.Value = db.GetConfigInt("BONUS.Freilos", 50);
            tbBonusVerlust.Value = db.GetConfigInt("BONUS.Verlust", 0);

            chkFreilosVerteilen.Checked = db.GetConfigBool("OPTION.DistBye");
            chkNovusRandomBoard.Checked = db.GetConfigBool("OPTION.NovusRandom", false);
            chkPairingOnlyPlayed.Checked = db.GetConfigBool("OPTION.ShowOnlyPlayed");
            numRoundsGameRepeat.Value = (Decimal)db.GetConfigInt("OPTION.GameRepeat");

            var ratio = db.GetConfigFloat("OPTION.RatioFirst2Last", 3);
            var idx = (ddlRatioFirst2Last.DataSource as List<float>).IndexOf(ratio);
            ddlRatioFirst2Last.CreateControl();  // Without this CreateControl, the following SelectedIndex= crashes. God knows why. 
            if (idx != ddlRatioFirst2Last.SelectedIndex)
                ddlRatioFirst2Last.SelectedIndex = idx;
            var ttddl = @"In the Keizer-System, a win against the first-ranked player 
gets more points than a win against the other players. This value 
is the points you'd get for a win against the first ranked player 
divided by the points you'd get against the last ranked player. The 
lower this number, the closer are Keizer system and Swiss system. 
3 is the default.";
            this.tooltip.SetToolTip(this.ddlRatioFirst2Last, ttddl);
            this.tooltip.SetToolTip(this.lblRatioFirst2Last, ttddl);
            this.tooltip.AutomaticDelay = 2000;
            this.tooltip.InitialDelay = 200;

            var firstRoundRandom = db.GetConfigInt("OPTION.FirstRoundRandom", 0);
            var idxf = (ddlFirstRoundRandom.DataSource as List<int>).IndexOf(firstRoundRandom);
            ddlFirstRoundRandom.CreateControl();  // Without this CreateControl, the following SelectedIndex= crashes. God knows why. 
            if (idxf != ddlFirstRoundRandom.SelectedIndex)
                ddlFirstRoundRandom.SelectedIndex = idxf;
            var ttddlf = @"In the Keizer-System, the first round would be each time the same with the 
same players. When setting this value not to 0, the first round colours are random 
and a random number between 0 and the value is added or subtracted from each players rating 
for determining the first round pairings.";
            this.tooltip.SetToolTip(this.ddlFirstRoundRandom, ttddlf);
            this.tooltip.SetToolTip(this.lblFirstRoundRandom, ttddlf);

            chkHtml.Checked = db.GetConfigBool("OPTION.Html");
            chkXml.Checked = db.GetConfigBool("OPTION.Xml");
            chkTxt.Checked = db.GetConfigBool("OPTION.Txt");
            chkCsv.Checked = db.GetConfigBool("OPTION.Csv");
            chkWickerNormalization.Checked = db.GetConfigBool("OPTION.WickerNormalization");
            numClicks = db.GetConfigInt("INTERNAL.NumClicks");
            numRoundSelect.Value = (Decimal)db.GetMaxRound();
            tbNiceName.Text = db.GetConfigText("OPTION.NiceName");
            // Der Name des db-Files ist einem ini-File gemerkt, alle anderen Settings in 
            // der Config-Datenbank. Weil die Config-Db nur schwer geöffnet werden kann ohne die 
            // Haupt-Db. Weil die Config-Db eine an die Haupt-Db attached DB ist. 
            if (fileName != ReadDBFileName())
                SaveDBFileName(fileName);
        }

        readonly IniFile inifile = new IniFile("cfg", "KFC2.ini");
        private string ReadDBFileName() => inifile.ReadValue("A", "DBFile", "");
        private void SaveDBFileName(string filename) => inifile.WriteValue("A", "DBFile", filename);
        private string ReadDonated() => inifile.ReadValue("A", "Donated", "a");


        private void fSelectLanguage()
        {
            frmLangSelect frmLangSelect = new frmLangSelect(db.LangCode);
            int num1 = (int)frmLangSelect.ShowDialog();
            db.LangCode = frmLangSelect.radEnglisch.Checked ? "EN" :
                frmLangSelect.radDeutsch.Checked ? "DE" :
                frmLangSelect.radHollaendisch.Checked ? "NL" : "FR";
            db.SetConfigText("LANGCODE", db.LangCode);
        }

        void fShowAboutBox()
        {
            var f = new frmAboutBox();
            f.ShowDialog();
        }

        void IncNumClicks(int num = 1)
        {
            numClicks += num;
            donateButton1?.SetState();
            donateButton2?.SetState();
            if (numClicks % 3 == 0 || num >= 7)
                db.SetConfigInt("INTERNAL.NumClicks", numClicks);
        }

        private void MnuProgQuitClick(object sender, EventArgs e) => this.Close();

        private void TabMainWindowSelectedIndexChanged(object sender, EventArgs e)
        {
            IncNumClicks();
            TabPage selectedTab = this.tabMainWindow.SelectedTab;
            TabPage tabPlayer = this.tabPlayer;
            this.mnuPaarungen.Enabled = this.tabMainWindow.SelectedTab == this.tabPairings;
            this.mnuListen.Enabled = this.tabMainWindow.SelectedTab != tabSettings;
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
            IncNumClicks();
            var lngs = new string[] { db.LangCode, "en", "de" };
            foreach (var lng in lngs)
            {
                var pp = pathWithPlaceholder.Replace("%LNG%", lng);
                foreach (var ext in new string[] { "pdf", "html" })
                {
                    var p = pp.Replace("%X%", ext);
                    if (File.Exists(p))
                    {
                        OpenWithDefaultApp(p);
                        goto end;
                    }
                }
            }
        end:
            return;
        }

        void MnuHelpDocumentationClick(object sender, EventArgs e) => OpenWithDefaultAppByLanguage("docs\\KeizerForClubs.%LNG%.%X%");

        void MnuHelpKeizerClick(object sender, EventArgs e) => OpenWithDefaultAppByLanguage("docs\\Keizer.%LNG%.%X%");

        void MnuHelpFAQClick(object sender, EventArgs e) => OpenWithDefaultAppByLanguage("docs\\KeizerForClubs.FAQ.%LNG%.%X%");

        void NumRoundSelectValueChanged(object sender, EventArgs e) => LoadPairingList();

        void MnuPairingNextRoundClick(object sender, EventArgs e)
        {
            IncNumClicks();
            DelDeletedPlayers();
            new ReportingUnit(sTurniername, sFilename, db).WriteCurrentTablesToDb();
            if (db.GetPairings_NoResult() == 0)
                ExecutePairing();
            else
                MessageBox.Show(db.Locl_GetText("GUI_TEXT", "Hinweis.RundeUnv"), "No....", MessageBoxButtons.OK);
            ApplyPlayerStateTexte();
        }

        void MnuPairingManualClick(object sender, EventArgs e)
        {
            IncNumClicks();
            DelDeletedPlayers();
            int maxRound = db.GetMaxRound();
            new ReportingUnit(sTurniername, sFilename, db).WriteCurrentTablesToDb();
            if (sender == mnuPaarungManuell)
            {
                if (db.GetPairings_NoResult() == 0)
                    ExecutePairingManual(maxRound + 1);
                else
                    MessageBox.Show(db.Locl_GetText("GUI_TEXT", "Hinweis.RundeUnv"), "No....", MessageBoxButtons.OK);
            }
            else
            {
                if (SelectedRound != maxRound)
                    MessageBox.Show(db.Locl_GetText("GUI_TEXT", "Hinweis.EdLetzteRd"), "No....", MessageBoxButtons.OK);
                else
                    ExecutePairingManual(maxRound);
            }
            ApplyPlayerStateTexte();
        }

        void MnuPairingDropLastClick(object sender, EventArgs e)
        {
            if (SelectedRound != 0)
            {
                IncNumClicks();
                int maxRound = db.GetMaxRound();
                if (SelectedRound != maxRound)
                {
                    int num = (int)MessageBox.Show(db.Locl_GetText("GUI_TEXT", "Hinweis.DelLetzteRd"), "No....", MessageBoxButtons.OK);
                }
                else
                {
                    db.DelPairings(maxRound);
                    db.DelCurrentStoredTablesWHeader(maxRound);
                    this.numRoundSelect.Value = (Decimal)(maxRound - 1);
                    this.LoadPairingList();
                }
                ApplyPlayerStateTexte();
            }
        }

        void MnuAboutBoxClick(object sender, EventArgs e) => fShowAboutBox();

        void MnuStartLanguageClick(object sender, EventArgs e)
        {
            this.fSelectLanguage();
            this.ApplyLanguageText();
            this.LoadPlayerlist();
            this.LoadPairingList();
        }

        void MnuPlayersImportClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = false;
            ofd.DefaultExt = "*.csv";
            ofd.FileName = "turnier_1";
            ofd.Filter = "CSV|*.csv";
            ofd.Title = "Import Players";
            ofd.InitialDirectory = Path.GetDirectoryName(dlgOpenTournament.FileName);

            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            string fileName = ofd.FileName;
            var lines = File.ReadAllLines(fileName).ToLi();
            ImportPlayers(lines);
        }

        void ImportPlayers(Li<string> lines)
        {
            var first = lines.FirstOrDefault();
            if (first != null && first.ToLowerInvariant() == "testscript")
                ExecTestScript(lines);
            else if (first != null)
            {
                var idxSplitter = first.IndexOfAny(";:,".ToCharArray());
                var splitter = idxSplitter != -1 ? first[idxSplitter] : '+';
                var firstParts = first.Split(splitter).ToLi();
                var idxName = firstParts.FindIndex(0, p => p.ToUpperInvariant() == "NAME");
                var idxRating = firstParts.FindIndex(0, p => p.ToUpperInvariant() == "RATING");
                if (idxRating != -1 && idxName != -1)
                {
                    lines = lines.Skip(1).ToLi();
                    var players = new Li<Tuple<string, int>>();
                    foreach (var li in lines)
                    {
                        var parts = li.Split(splitter).ToLi();
                        var name = parts[idxName].Trim();
                        var rating = Helper.ToInt(parts[idxRating].Trim());
                        players.Add(new Tuple<string, int>(name, rating));
                    }

                    if (db.CntPlayers() == 0)
                        db.ResetPlayerBaseIdTa();

                    Li<string> alreadyExistingAndChanged = new Li<string>();
                    players = players.OrderByDescending(p => p.Item2).ToLi();
                    foreach (var p in players)
                    {
                        (string name, int rating) = p;
                        int playerId = db.GetPlayerID(name);
                        while (playerId != -1)
                        {
                            name = "++" + name;
                            playerId = db.GetPlayerID(name);
                        }
                        if (name != p.Item1)
                            alreadyExistingAndChanged.Add($"{p.Item1} ({rating}) -> {name}");
                        db.InsPlayerNew(name, rating);
                    }
                    LoadPlayerlist();
                    if (alreadyExistingAndChanged.Any())
                    {

                        string t = db.Locl_GetText("GUI_TEXT", "NamesChanged") + "\n\n";
                        t += string.Join('\n', alreadyExistingAndChanged);
                        MessageBox.Show(t, Text);
                    }
                }
                else
                    MessageBox.Show(db.Locl_GetText("GUI_TEXT", "WrongCsvFileFormat"), Text);
            }
        }

        void MnuPlayersDeleteAllClickTa(object sender, EventArgs e)
        {
            if (db.GetMaxRound() == 0)
            {
                var t = db.Locl_GetText("GUI_TEXT", "ReallyDeleteAll");
                var res = MessageBox.Show(t, Text, MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    db.DeleteAllPlayersTa();
                    db.ResetPlayerBaseIdTa();
                    LoadPlayerlist();
                }
            }
            else
                MessageBox.Show(db.Locl_GetText("GUI_TEXT", "OnlyBefore1"), Text);
        }

        void MnuPlayersRebaseIdsClickTa(object sender, EventArgs e)
        {
            if (db.GetMaxRound() == 0)
            {
                db.RebasePlayerIdsTa();
                db.ResetPlayerBaseIdTa();
                LoadPlayerlist();
            }
            else
                MessageBox.Show(db.Locl_GetText("GUI_TEXT", "OnlyBefore1"), Text);
        }

        private void GrdPlayersDataError(object sender, DataGridViewDataErrorEventArgs e) => e.Cancel = false;

        private void ChkPairingOnlyPlayedCheckedChanged(object sender, EventArgs e)
        {
            IncNumClicks();
            this.LoadPairingList();
            db.SetConfigBool("OPTION.ShowOnlyPlayed", chkPairingOnlyPlayed.Checked);
        }

        private void FrmMainformFormClosing(object sender, FormClosingEventArgs e)
        {
            IncNumClicks();
            SaveSettings();
            db.DelDeletedPlayers();
            e.Cancel = false;
        }

        private void SaveSettings()
        {
            if (this.tabMainWindow.Enabled)
            {
                db.SetConfigInt("BONUS.Clubgame", this.tbBonusClub.Value);
                db.SetConfigInt("BONUS.Excused", this.tbBonusExcused.Value);
                db.SetConfigInt("BONUS.Unexcused", this.tbBonusUnexcused.Value);
                db.SetConfigInt("BONUS.Retired", this.tbBonusRetired.Value);
                db.SetConfigInt("BONUS.Freilos", this.tbBonusFreilos.Value);
                db.SetConfigInt("BONUS.Verlust", this.tbBonusVerlust.Value);

                db.SetConfigBool("OPTION.DistBye", this.chkFreilosVerteilen.Checked);
                db.SetConfigBool("OPTION.NovusRandom", this.chkNovusRandomBoard.Checked);
                db.SetConfigInt("OPTION.GameRepeat", (int)Convert.ToInt16(this.numRoundsGameRepeat.Value));
                db.SetConfigFloat("OPTION.RatioFirst2Last", Helper.ToSingle(ddlRatioFirst2Last.SelectedValue));
                db.SetConfigInt("OPTION.FirstRoundRandom", Helper.ToInt(ddlFirstRoundRandom.SelectedValue));
                db.SetConfigBool("OPTION.Html", this.chkHtml.Checked);
                db.SetConfigBool("OPTION.Xml", this.chkXml.Checked);
                db.SetConfigBool("OPTION.Txt", this.chkTxt.Checked);
                db.SetConfigBool("OPTION.Csv", this.chkCsv.Checked);
                db.SetConfigBool("OPTION.WickerNormalization", this.chkWickerNormalization.Checked);
                var oldNiNa = db.GetConfigText("OPTION.NiceName");
                if (oldNiNa != this.tbNiceName.Text)
                {
                    db.SetConfigText("OPTION.NiceName", this.tbNiceName.Text);
                    SetTurnierAndFilename(this.tbNiceName.Text);
                }
            }
            SaveWindowSettings();
        }

        private void SetTurnierAndFilename(string niceName)
        {
            sFilename = (dlgOpenTournament?.FileName?.Replace(".s3db", "") ?? "unknown");
            sTurniername = string.IsNullOrWhiteSpace(niceName) ? sFilename : niceName;
            this.Text = "KeizerForClubs  -  " + sTurniername;
        }

        private void SaveWindowSettings()
        {
            Point loc = RestoreBounds.Location;
            Size sz = RestoreBounds.Size;
            bool isMax = false, isMin = false;
            if (WindowState == FormWindowState.Maximized)
                isMax = true;
            else if (WindowState == FormWindowState.Normal)
            {
                loc = Location;
                sz = Size;
            }
            else
                isMin = true;
            db.SetConfigBool("Win.isMax", isMax);
            db.SetConfigBool("Win.isMin", isMin);
            db.SetConfigInt("WIN.locX", loc.X);
            db.SetConfigInt("WIN.locY", loc.Y);
            db.SetConfigInt("WIN.szWid", sz.Width);
            db.SetConfigInt("WIN.szHei", sz.Height);

            db.SetConfigInt("WIN.colPairingNameWhite.wid", colPairingNameWhite.Width);
            db.SetConfigInt("WIN.colPairingNameBlack.wid", colPairingNameBlack.Width);
            db.SetConfigInt("WIN.colPlayerName.wid", colPlayerName.Width);
            db.SetConfigInt("WIN.chkPairingOnlyPlayed.locX", chkPairingOnlyPlayed.Location.X);
        }

        private void RestoreWindowSettings()
        {
            try
            {
                var loc = new Point(db.GetConfigInt("WIN.locX"), db.GetConfigInt("WIN.locY"));
                var sz = new Size(db.GetConfigInt("WIN.szWid"), db.GetConfigInt("WIN.szHei"));
                if (IsWindowPartVisible(loc, sz))
                {
                    Location = loc;
                    Size = sz;
                }
                if (db.GetConfigBool("Win.isMax"))
                    WindowState = FormWindowState.Maximized;
                else if (db.GetConfigBool("Win.isMin"))
                    WindowState = FormWindowState.Minimized;

                colPairingNameWhite.Width = db.GetConfigInt("WIN.colPairingNameWhite.wid");
                colPairingNameBlack.Width = db.GetConfigInt("WIN.colPairingNameBlack.wid");
                colPlayerName.Width = db.GetConfigInt("WIN.colPlayerName.wid");
                chkPairingOnlyPlayed.Location = new Point(db.GetConfigInt("WIN.chkPairingOnlyPlayed.locX"),
                        chkPairingOnlyPlayed.Location.Y);
            }
            catch (Exception) {  /* For first start - there'll be no saved values. */ }
        }

        private bool IsWindowPartVisible(Point loc, Size sz)
        {
            Rectangle rect = new Rectangle(loc, sz);
            return Screen.AllScreens.Any(scr => scr.Bounds.IntersectsWith(rect));
        }

        int SelectedRound => Helper.ToInt(numRoundSelect.Value);

        private void MnuListenAllClick(object sender, EventArgs e)
        {
            RecalcIfNeeded();
            IncNumClicks(db.GetPlayerCount());
            var ru = new ReportingUnit(sTurniername, sFilename, db);
            ru.fReport_Paarungen(SelectedRound);
            ru.fReport_Tabellenstand(SelectedRound);
            ru.fReport_TabellenstandVoll(SelectedRound);
            ru.fReport_Teilnehmer(SelectedRound);
        }
        private void RecalcIfNeeded()
        {
            if (SelectedRound >= db.GetMaxRound())
                this.ranking.AllPlayersAllRoundsCalculateTa();
        }

        private void MnuListenPairingClick(object sender, EventArgs e)
        {
            IncNumClicks(db.GetPlayerCount() / 2);
            new ReportingUnit(sTurniername, sFilename, db).fReport_Paarungen(SelectedRound);
        }

        private void MnuListenStandingClick(object sender, EventArgs e)
        {
            RecalcIfNeeded();
            IncNumClicks(db.GetPlayerCount());
            if (sender == mnuListenStanding)
                new ReportingUnit(sTurniername, sFilename, db).fReport_Tabellenstand(SelectedRound);
            else if (sender == mnuListenStandingFull)
                new ReportingUnit(sTurniername, sFilename, db).fReport_TabellenstandVoll(SelectedRound);
        }

        private void MnuListenParticipantsClick(object sender, EventArgs e) =>
            new ReportingUnit(sTurniername, sFilename, db).fReport_Teilnehmer(SelectedRound);

        private void TbBonusValueChanged(object sender, EventArgs e)
        {
            this.lblBonusClubValue.Text = this.tbBonusClub.Value.ToString();
            this.lblBonusExcusedValue.Text = this.tbBonusExcused.Value.ToString();
            this.lblBonusUnexcusedValue.Text = this.tbBonusUnexcused.Value.ToString();
            this.lblBonusRetiredValue.Text = this.tbBonusRetired.Value.ToString();
            this.lblBonusFreilosValue.Text = this.tbBonusFreilos.Value.ToString();
            this.lblBonusVerlustValue.Text = this.tbBonusVerlust.Value.ToString();
        }

        private void BtDonateClick(object sender, EventArgs e) => new frmAboutBox(true).ShowDialog();

        private void TabSettingsLeave(object sender, EventArgs e) => SaveSettings();

        private void TabPlayerLeave(object sender, EventArgs e) => DelDeletedPlayers();

        private void GrdPlayersCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            grdPlayers.Rows[e.RowIndex].ErrorText = "";
            if (e.ColumnIndex == 1)  // Name
            {
                if (IsInvalidName(e))
                {
                    e.Cancel = true;
                    grdPlayers.Rows[e.RowIndex].ErrorText = db.Locl_GetText("GUI_TEXT", "Hinweis.NameZuKurz");
                }
                else if (IsDuplicateName(e))
                {
                    e.Cancel = true;
                    grdPlayers.Rows[e.RowIndex].ErrorText = db.Locl_GetText("GUI_TEXT", "Hinweis.NameDoppelt");
                }
            }
            else if (e.ColumnIndex == 2)  // Rating
            {
                if (IsInvalidRating(e))
                {
                    e.Cancel = true;
                    grdPlayers.Rows[e.RowIndex].ErrorText = db.Locl_GetText("GUI_TEXT", "Hinweis.InvalidRating");
                }
            }
            if (e.Cancel)
                MessageBox.Show(grdPlayers.Rows[e.RowIndex].ErrorText, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private bool IsInvalidRating(DataGridViewCellValidatingEventArgs e)
        {
            var newRating = e.FormattedValue.ToString();
            newRating = newRating?.Trim() ?? "";
            bool isInvalid = !Regex.IsMatch(newRating, @"^\d*$");
            isInvalid |= Helper.ToInt(newRating) >= 30000;
            return isInvalid;
        }

        private bool IsInvalidName(DataGridViewCellValidatingEventArgs e)
        {
            bool isInvalid = !grdPlayers.Rows[e.RowIndex].IsNewRow;
            // Darf nicht Invalid in neuer Zeile sein, sonst kommt man aus dem
            // neuen Namensfeld nie mehr raus, wenn man aus Versehn reingeklickt hat. 
            if (isInvalid)
            {
                var newPlayerName = e.FormattedValue.ToString();
                newPlayerName = Regex.Replace(newPlayerName?.Trim() ?? "", @"(\s+)", "");
                isInvalid = newPlayerName.Length <= 1;
            }
            return isInvalid;
        }

        private bool IsDuplicateName(DataGridViewCellValidatingEventArgs e)
        {
            bool isDuplicate = false;
            var newPlayerName = e.FormattedValue.ToString();
            newPlayerName = Regex.Replace(newPlayerName?.Trim() ?? "", @"(\s\s+)", " ");
            if (newPlayerName != "")
            {
                var row = this.grdPlayers.Rows[e.RowIndex];
                int gridPlayerId = row.Cells[0].Value != null ? Convert.ToInt16(row.Cells[0].Value) : -1;
                bool isNewPlayer = gridPlayerId == -1;
                int nPlayersWithName = db.CntPlayerNames(newPlayerName);
                isDuplicate = nPlayersWithName > 0 && isNewPlayer;
                if (!isNewPlayer)
                    if (newPlayerName != db.GetPlayerName(gridPlayerId))  // it's a name change.
                        isDuplicate = nPlayersWithName > 0;
            }
            return isDuplicate;
        }

        private void GrdPlayersCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.grdPlayers.Rows[e.RowIndex];
            if (row.Cells[1].Value == null)  // kein Name
                return;
            string newPlayerName = row.Cells[1].Value.ToString().Trim();
            newPlayerName = Regex.Replace(newPlayerName, @"(\s\s+)", " ");
            // Paragraph sign is not allowed in name, as it is used as splitting char. 
            newPlayerName = Regex.Replace(newPlayerName, "§", "$");
            if (string.IsNullOrEmpty(newPlayerName))  // kein Name
                return;
            int gridPlayerId = row.Cells[0].Value != null ? Convert.ToInt16(row.Cells[0].Value) : -1;
            bool isNewPlayer = gridPlayerId == -1;

            if (isNewPlayer)
            {
                if (row.Cells[2].Value == null)
                    row.Cells[2].Value = 0;
                if (row.Cells[grdPlayersStateCol].Value == null)
                    row.Cells[grdPlayersStateCol].Value = db.Locl_GetPlayerStateText(SqliteInterface.PlayerState.Available);

                db.InsPlayerNew(newPlayerName, Convert.ToInt16(row.Cells[2].Value));
                row.Cells[0].Value = db.GetPlayerID(newPlayerName);
                db.UpdPlayer(Helper.ToInt(row.Cells[0].Value), newPlayerName,
                            Helper.ToInt(row.Cells[2].Value),
                            db.Locl_GetPlayerState(row.Cells[grdPlayersStateCol].Value.ToString()));
            }
            else
                db.UpdPlayer(gridPlayerId, newPlayerName,
                    Helper.ToInt((row.Cells[2].Value?.ToString() ?? "").Trim()),
                    db.Locl_GetPlayerState(row.Cells[grdPlayersStateCol].Value.ToString()));
            row.Cells[1].Value = newPlayerName;
        }

        private void GrdPlayersDirty(object sender, EventArgs e)
        {
            if (grdPlayers.IsCurrentCellDirty)
                if (grdPlayers.CurrentCellAddress.X == grdPlayersStateCol)
                {
                    grdPlayers.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    grdPlayers.EndEdit();
                }
        }
        const int grdPlayersStateCol = 3, grdPairingsResultCol = 7;

        private void GrdPairingsCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var rawval = grdPairings.Rows[e.RowIndex].Cells[grdPairingsResultCol].Value;
                if (rawval != null)
                {
                    var value = rawval?.ToString() ?? "";
                    SqliteInterface.Results gameResult = db.Locl_GetGameResult(value);
                    int pid1 = Convert.ToInt16(grdPairings.Rows[e.RowIndex].Cells[1].Value);
                    int pid2 = Convert.ToInt16(grdPairings.Rows[e.RowIndex].Cells[4].Value);
                    db.UpdPairingResult(Convert.ToInt16(numRoundSelect.Value), pid1, pid2, gameResult);
                }
            }
        }

        private void GrdPairingsDirty(object sender, EventArgs e)
        {
            if (grdPairings.IsCurrentCellDirty)
            {
                grdPairings.CommitEdit(DataGridViewDataErrorContexts.Commit);
                grdPairings.EndEdit();
            }
        }

        private void ApplyPlayerStateTexte()
        {
            int m = db.GetMaxRound();
            string condition = m != 0 ? " AND key not in ('2', 'A') " : " AND key<>'2' ";
            string[] texte = new string[20];
            int topicTexte1 = db.Locl_GetTopicTexte("PLAYERSTATE", condition, ref texte);

            colPlayerState.Items.Clear();
            for (int index = 0; index < topicTexte1; ++index)
                colPlayerState.Items.Add((object)texte[index]);
        }

        private void ApplyPairingResultTexte()
        {
            string[] texte = new string[20];
            colPairingResult.Items.Clear();
            int topicTexte2 = db.Locl_GetTopicTexte("GAMERESULT", "", ref texte);

            var tt = texte.Take(topicTexte2).ToLi();
            tt.Insert(0, SqliteInterface.ColPairingUndefinedText);

            foreach (var text in tt)
                colPairingResult.Items.Add(text);
        }

        private void ApplyLanguageText()
        {
            ApplyPlayerStateTexte();
            ApplyPairingResultTexte();
            colPlayerState.HeaderText = db.Locl_GetText("GUI_COLS", "Sp.Status");
            colRating.HeaderText = db.Locl_GetText("GUI_COLS", "Sp.Rating");
            colPlayerName.HeaderText = db.Locl_GetText("GUI_COLS", "Sp.Name");
            colPairgBoard.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.Brett");
            colPairingNameWhite.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.Weiss");
            colPairingWhiteAddinfo.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.WeissAdd");
            colPairingNameBlack.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.Schwarz");
            colPairingAddInfoB.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.SchwarzAdd");
            colPairingResult.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.Ergebnis");
            mnuPlayers.Text = db.Locl_GetText("GUI_MENU", "PlayersMenu");
            mnuPlayersImport.Text = db.Locl_GetText("GUI_MENU", "ImportPlayers");
            mnuPlayersDeleteAll.Text = db.Locl_GetText("GUI_MENU", "DeletePlayers");
            mnuPlayersRebaseIds.Text = db.Locl_GetText("GUI_MENU", "RebasePlayerIds");

            mnuPaarungen.Text = db.Locl_GetText("GUI_MENU", "Paarungen");
            mnuPaarungNext.Text = db.Locl_GetText("GUI_MENU", "Paarung.Next");
            mnuPaarungManuell.Text = db.Locl_GetText("GUI_MENU", "Paarung.NextMan");
            mnuPaarungThisMan.Text = db.Locl_GetText("GUI_MENU", "Paarung.ThisMan");
            mnuPaarungDropLast.Text = db.Locl_GetText("GUI_MENU", "Paarung.Drop");
            mnuListen.Text = db.Locl_GetText("GUI_MENU", "Listen");
            mnuListenStanding.Text = db.Locl_GetText("GUI_MENU", "Listen.Calc");
            mnuListenStandingFull.Text = db.Locl_GetText("GUI_MENU", "Listen.CalcFull");
            mnuListenPairing.Text = db.Locl_GetText("GUI_MENU", "Listen.Paarungen");
            mnuListenAll.Text = db.Locl_GetText("GUI_MENU", "Listen.Alle");
            mnuListenParticipants.Text = db.Locl_GetText("GUI_MENU", "Listen.Teilnehmer");
            mnuHelp.Text = db.Locl_GetText("GUI_MENU", "Hilfe");
            mnuHelpDocumentation.Text = db.Locl_GetText("GUI_MENU", "Hilfe.Doku");
            mnuHelpAbout.Text = db.Locl_GetText("GUI_MENU", "Hilfe.About");
            tabPlayer.Text = db.Locl_GetText("GUI_TABS", "Spieler");
            tabPairings.Text = db.Locl_GetText("GUI_TABS", "Paarungen");
            tabSettings.Text = db.Locl_GetText("GUI_TABS", "Einstellungen");

            lblBonusExcused.Text = db.Locl_GetText("GUI_LABEL", "Bonus entschuldigt");
            lblBonusUnexcused.Text = db.Locl_GetText("GUI_LABEL", "Bonus unentschuldigt");
            lblBonusClub.Text = db.Locl_GetText("GUI_LABEL", "Bonus verhindert");
            lblBonusRetired.Text = db.Locl_GetText("GUI_LABEL", "Bonus Rueckzug");
            lblBonusFreilos.Text = db.Locl_GetText("GUI_LABEL", "Bonus Freilos");
            lblBonusVerlust.Text = db.Locl_GetText("GUI_LABEL", "Bonus Verlust");

            chkPairingOnlyPlayed.Text = db.Locl_GetText("GUI_LABEL", "Nur gespielte");
            chkFreilosVerteilen.Text = db.Locl_GetText("GUI_LABEL", "FreilosVerteilen");
            chkNovusRandomBoard.Text = db.Locl_GetText("GUI_LABEL", "NovusRandom");
            lblRunde.Text = db.Locl_GetText("GUI_LABEL", "Runde");
            numRoundSelect.Text = db.Locl_GetText("GUI_LABEL", "Runde");
            lblRoundsGameRepeat.Text = db.Locl_GetText("GUI_LABEL", "NumRundeWdh");
            lblRatioFirst2Last.Text = db.Locl_GetText("GUI_LABEL", "First2Last");
            lblFirstRoundRandom.Text = db.Locl_GetText("GUI_LABEL", "FirstRoundRandom");
            lblOutputTo.Text = db.Locl_GetText("GUI_LABEL", "OutputTo");
            lblNiceName.Text = db.Locl_GetText("GUI_LABEL", "NiceName");
            chkWickerNormalization.Text = db.Locl_GetText("GUI_LABEL", "WickerNorm");
            btDonate1.Text = btDonate2.Text = db.Locl_GetText("GUI_TEXT", "Donate");
        }

        private void DelDeletedPlayers()
        {
            int n = db.DelDeletedPlayers();
            if (n > 0)
                LoadPlayerlist();
        }

        /// <summary> Erzeugt die Spielerliste auf dem Spieler-Tab.</summary>
        private void LoadPlayerlist()
        {
            var players = db.GetPlayerLi("", " ORDER BY ID ", db.GetMaxRound() + 1);
            this.grdPlayers.Rows.Clear();
            for (int i = 0; i < players.Count; ++i)
            {
                grdPlayers.Rows.Add();
                grdPlayers.Rows[i].Cells[0].Value = players[i].Id;
                grdPlayers.Rows[i].Cells[1].Value = players[i].Name;
                grdPlayers.Rows[i].Cells[2].Value = players[i].Rating.ToString();
                grdPlayers.Rows[i].Cells[grdPlayersStateCol].Value =
                        db.Locl_GetPlayerStateText(players[i].State);
            }
        }

        private void LoadPairingList()
        {
            var pairingList = db.GetPairingLi(" WHERE rnd=" + this.numRoundSelect.Value.ToString(), " ORDER BY board ");
            this.grdPairings.Rows.Clear();
            for (int idx = 0; idx < pairingList.Count; ++idx)
            {
                var pair = pairingList[idx];
                if (!this.chkPairingOnlyPlayed.Checked || pair.Board < stPairing.FirstNonPlayingBoard)
                {
                    this.grdPairings.Rows.Add();
                    this.grdPairings.Rows[idx].Cells[0].Value = pair.Board.ToString();
                    this.grdPairings.Rows[idx].Cells[1].Value = pair.IdW.ToString();
                    this.grdPairings.Rows[idx].Cells[2].Value = db.GetPlayerName(pair.IdW);
                    this.grdPairings.Rows[idx].Cells[4].Value = pair.IdB.ToString();
                    this.grdPairings.Rows[idx].Cells[5].Value = db.GetPlayerName(pair.IdB);
                    var cbcell = (DataGridViewComboBoxCell)grdPairings.Rows[idx].Cells[grdPairingsResultCol];
                    cbcell.Value = db.Locl_GetGameResultText(pair.Result);
                    if (SqliteInterface.IsBoardResult(pair.Result))
                        foreach (var res in SqliteInterface.NonBoardResults)
                            cbcell.Items.Remove(db.Locl_GetGameResultText(res));
                    else if (SqliteInterface.IsNonBoardResult(pair.Result))
                        cbcell.ReadOnly = true;
                }
            }
        }

        private void fSetFirstRoundRandomRating(int currRunde, int iPairingPlayerCntAvailable)
        {
            if (currRunde != 1)
                return;
            var firstRoundRandom = db.GetConfigInt("OPTION.FirstRoundRandom", 0);
            for (int rwd = 0, index = 0; index < this.iPairingPlayerCntAvailable; ++index)
            {
                if (firstRoundRandom == 0)
                    rwd = 0;
                else
                    rwd = random.Next(-firstRoundRandom, firstRoundRandom) + this.pPairingPlayerList[index].Rating;
                this.pPairingPlayerList[index].RatingWDelta = rwd;
                db.UpdPlayerRatingWDelta(this.pPairingPlayerList[index].Id, rwd);
            }
        }

        private bool ExecutePairing()
        {
            Stopwatches.Start("ExecutePairing-1");
            var currRunde = db.GetMaxRound() + 1;  // currRunde ist die aktuell ausgeloste Runde. 
            this.iPairingPlayerCntAvailable = db.GetPlayerList_Available(ref this.pPairingPlayerList, "", currRunde);
            this.iPairingMinFreeCnt = 999;
            for (int index = 0; index < this.iPairingPlayerCntAvailable; ++index)
            {
                if (this.pPairingPlayerList[index].FreeCnt < this.iPairingMinFreeCnt)
                    this.iPairingMinFreeCnt = this.pPairingPlayerList[index].FreeCnt;
            }
            Stopwatches.Next("ExecutePairing-2");
            fSetFirstRoundRandomRating(currRunde, iPairingPlayerCntAvailable);
            this.ranking.AllPlayersAllRoundsCalculateTa();
            Stopwatches.Next("ExecutePairing-3");
            db.BeginTransaction();
            this.iPairingPlayerCntAll = db.GetPlayerList_NotDropped(ref this.pPairingPlayerList, " ORDER BY rank ", currRunde);
            this.iPairingRekursionCnt = 0;
            Stopwatches.Stop("ExecutePairing-3");
            if (this.RunPairingRecursion(0, currRunde))
            {
                this.numRoundSelect.Value = (Decimal)currRunde;
                for (int index = 0; index < this.iPairingPlayerCntAvailable / 2; ++index)
                    db.InsPairingNew(currRunde, index + 1, this.pPairingList[index].IdW, this.pPairingList[index].IdB);
                this.PairingInsertNoPlaying();
                db.EndTransaction();
                if (ShallUseNovusRandomBoard)
                    ShuffleBoardsOfPairings(currRunde);
                this.LoadPairingList();
                Stopwatches.Debug("");
                return true;
            }
            db.EndTransaction();
            int num = (int)MessageBox.Show("No success; adjust options or try manually", "Error");
            return false;
        }

        private bool ShallUseNovusRandomBoard => chkNovusRandomBoard.Checked;

        /// <summary> This is needed for the Latvian Novus game. Details see Issue #66 in GitHub. </summary>
        private void ShuffleBoardsOfPairings(int currRunde)
        {
            var pairs = db.GetClPairingLi($" WHERE Rnd={currRunde} AND board < {stPairing.FirstNonPlayingBoard} ", "");
            Li<int> boards = pairs.Select(p => p.P.Board).ToLi();
            foreach (var p in pairs)
            {
                var idx = random.Next(boards.Count);
                p.P.Board = boards[idx];
                boards.RemoveAt(idx);
            }
            db.BeginTransaction();
            db.ChangeBoards(currRunde, pairs);
            db.EndTransaction();
        }

        private void DealFreilos()
        {
            if (iPairingPlayerCntAvailable % 2 == 1)
                for (int index = this.iPairingPlayerCntAll - 1; index >= 0; --index)
                {
                    if ((!this.chkFreilosVerteilen.Checked || pPairingPlayerList[index].FreeCnt <= this.iPairingMinFreeCnt)
                        &&
                        pPairingPlayerList[index].State == SqliteInterface.PlayerState.Available)
                    {
                        pPairingPlayerList[index].State = SqliteInterface.PlayerState.Freilos;
                        break;
                    }
                }
        }

        private bool RunPairingRecursion(int brett, int currRunde)
        {
            // Debug.WriteLine($"fPairingRekursion brett:{brett}");
            if (brett == 0)
                DealFreilos();
            int minrunde = currRunde - Convert.ToInt16(this.numRoundsGameRepeat.Value);
            if (this.iPairingRekursionCnt++ > 100000)
                return false;
            if (brett * 2 + 1 >= this.iPairingPlayerCntAvailable)
                return true;
            for (int index1 = 0; index1 < this.iPairingPlayerCntAll; ++index1)
            {
                ref SqliteInterface.stPlayer player1 = ref this.pPairingPlayerList[index1];
                if (player1.State == SqliteInterface.PlayerState.Available)
                {
                    player1.State = SqliteInterface.PlayerState.Paired;
                    for (int index2 = 0; index2 < this.iPairingPlayerCntAll; ++index2)
                    {
                        ref SqliteInterface.stPlayer player2 = ref this.pPairingPlayerList[index2];

                        if (index1 != index2 && player2.State == SqliteInterface.PlayerState.Available
                            && db.CountPairingVorhandenSince(minrunde, player1.Id, player2.Id) == 0)
                        {
                            Stopwatches.Start("RunPairingRecursion-1");
                            player2.State = SqliteInterface.PlayerState.Paired;
                            int p1WeissPlus = db.GetPlayerWeissUeberschuss(player1.Id);
                            int p2WeissPlus = db.GetPlayerWeissUeberschuss(player2.Id);
                            if (p1WeissPlus > p2WeissPlus)
                                SetPairing2List(brett, player2, player1);
                            else if (p1WeissPlus < p2WeissPlus)
                                SetPairing2List(brett, player1, player2);
                            else
                            {  // WeissPlus Gleichheit. 
                                var nplayed = db.CountPairingVorhandenSince(0, player1.Id, player2.Id);
                                // Falls die zwei noch nie oder eine grade Anzahl Male gegeneinander gspielt haben, ...
                                if (nplayed % 2 == 0)
                                {
                                    // falls erste Runde und FirstRoundRandom, wird die Farbe ausgelost, 
                                    if (currRunde == 1 && db.GetConfigInt("OPTION.FirstRoundRandom") != 0)
                                    {
                                        if (random.NextSingle() > 0.5f)
                                            SetPairing2List(brett, player2, player1);
                                        else
                                            SetPairing2List(brett, player1, player2);
                                    }
                                    else
                                        // sonst kriegt der weiter vorne in der Rangliste schwarz. 
                                        SetPairing2List(brett, player2, player1);
                                }
                                else
                                {
                                    // Falls die zwei eine ungrade Anzahl Male gegeneinander gspielt haben, 
                                    // werden die Farben vertauscht.
                                    var cntPlayer1W = db.CountPairingVorhandenSinceOneWay(0, player1.Id, player2.Id);
                                    var cntPlayer2W = nplayed - cntPlayer1W;
                                    if (cntPlayer1W > cntPlayer2W)
                                        SetPairing2List(brett, player2, player1);
                                    else
                                        SetPairing2List(brett, player1, player2);
                                }
                            }
                            Stopwatches.Stop("RunPairingRecursion-1");
                            // Debug.WriteLine($"fPairingRekursion brett:{brett} paired:{player1.Name} vs {player2.Name}");
                            if (this.RunPairingRecursion(brett + 1, currRunde))
                                return true;
                            player2.State = SqliteInterface.PlayerState.Available;
                            if (this.iPairingRekursionCnt > 100000)
                                return false;
                        }
                    }
                    player1.State = SqliteInterface.PlayerState.Available;
                }
            }
            return false;
        }

        private void SetPairing2List(int brett, SqliteInterface.stPlayer playerWhite,
            SqliteInterface.stPlayer playerBlack)
        {
            this.pPairingList[brett].IdW = playerWhite.Id;
            this.pPairingList[brett].IdB = playerBlack.Id;
        }

        private bool PairingInsertNoPlaying()
        {
            int maxRound = db.GetMaxRound();
            int num = stPairing.FirstNonPlayingBoard;
            for (int i = 0; i < this.iPairingPlayerCntAll; ++i)
            {
                if (this.pPairingPlayerList[i].State.IsContainedIn(new PlayerState[] { SqliteInterface.PlayerState.Freilos,
                        SqliteInterface.PlayerState.Hindered, SqliteInterface.PlayerState.Excused,
                        SqliteInterface.PlayerState.Unexcused }))
                {
                    db.DelPairings(maxRound, this.pPairingPlayerList[i].Id);
                    db.InsPairingNew(maxRound, num++, this.pPairingPlayerList[i].Id, -1);

                    if (this.pPairingPlayerList[i].State == SqliteInterface.PlayerState.Freilos)
                        db.UpdPairingResult(maxRound, this.pPairingPlayerList[i].Id, -1, SqliteInterface.Results.FreeWin);
                    else if (this.pPairingPlayerList[i].State == SqliteInterface.PlayerState.Hindered)
                        db.UpdPairingResult(maxRound, this.pPairingPlayerList[i].Id, -1, SqliteInterface.Results.Hindered);
                    else if (this.pPairingPlayerList[i].State == SqliteInterface.PlayerState.Excused)
                        db.UpdPairingResult(maxRound, this.pPairingPlayerList[i].Id, -1, SqliteInterface.Results.Excused);
                    else if (this.pPairingPlayerList[i].State == SqliteInterface.PlayerState.Unexcused)
                        db.UpdPairingResult(maxRound, this.pPairingPlayerList[i].Id, -1, SqliteInterface.Results.Unexcused);
                }
            }
            return true;
        }

        private bool ExecutePairingManual(int currRunde)
        {
            bool isAlreadyExistingRound = currRunde != db.GetMaxRound() + 1;
            // Falls isAlreadyExistingRound, ist currRunde die nächste, noch nicht ausgeloste Runde. 
            // Andernfalls ist currRunde eine aktuell bereits ausgeloste, möglicherweise auch schon gespielte Runde. 

            frmPairingManual frmPairingManual = new frmPairingManual(db, currRunde);
            frmPairingManual.btnCancel.Text = db.Locl_GetText("GUI_TEXT", "Abbruch");
            frmPairingManual.colWhite.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.Weiss");
            frmPairingManual.colBlack.HeaderText = db.Locl_GetText("GUI_COLS", "Pa.Schwarz");
            var currPairings = db.GetPairingLi($" WHERE Rnd={currRunde} ", " ORDER BY board");
            var availPlayers = db.GetPlayerLi_Available(" ORDER BY rank ", currRunde);
            if (isAlreadyExistingRound)
            {
                for (int i = 0; i < currPairings.Count; ++i)
                {
                    var p = currPairings[i];
                    if (p.Board < stPairing.FirstNonPlayingBoard)
                    {
                        if (availPlayers.Any(pla => pla.Id == p.IdW) &&
                            availPlayers.Any(pla => pla.Id == p.IdB))
                        {
                            var iRow = frmPairingManual.grdPaarungen.Rows.Add();
                            var row = frmPairingManual.grdPaarungen.Rows[iRow];
                            row.Cells[0].Value = db.GetPlayerName(p.IdW);
                            row.Cells[1].Value = db.GetPlayerName(p.IdB);
                            availPlayers.RemoveAll(pla => pla.Id == p.IdW || pla.Id == p.IdB);
                        }
                    }
                }
            }

            foreach (var player in availPlayers)
                if (player.State == SqliteInterface.PlayerState.Available)
                    frmPairingManual.lstNames.Items.Add(player.Name);
            for (int i = 0; i < frmPairingManual.lstNames.Items.Count; i += 2)
                frmPairingManual.grdPaarungen.Rows.Add();



            this.iPairingPlayerCntAll = db.GetPlayerList_NotDropped(ref this.pPairingPlayerList, " ORDER BY rank ", currRunde);
            if (frmPairingManual.ShowDialog() == DialogResult.OK)
            {
                int runde = currRunde;
                this.numRoundSelect.Value = (Decimal)runde;
                int brett = 1;
                for (int i = 0; i < frmPairingManual.grdPaarungen.Rows.Count; ++i)
                {
                    if (frmPairingManual.grdPaarungen.Rows[i].Cells[0].Value != null && frmPairingManual.grdPaarungen.Rows[i].Cells[1].Value != null)
                    {
                        string sName1 = frmPairingManual.grdPaarungen.Rows[i].Cells[0].Value.ToString();
                        string sName2 = frmPairingManual.grdPaarungen.Rows[i].Cells[1].Value.ToString();
                        int playerId1 = db.GetPlayerID(sName1);
                        int playerId2 = db.GetPlayerID(sName2);
                        bool shallInsert = !isAlreadyExistingRound ||
                            !currPairings.Any(pp => pp.IdW == playerId1 && pp.IdB == playerId2);
                        if (shallInsert)
                        {
                            if (isAlreadyExistingRound)
                            {
                                db.DelPairings(currRunde, playerId1, playerId2);
                                brett = db.GetNextFreeBrettOfRound(currRunde, false);
                            }
                            else ++brett;
                            db.InsPairingNew(runde, brett, playerId1, playerId2);
                            for (int j = 0; j < this.iPairingPlayerCntAll; ++j)
                            {
                                if (this.pPairingPlayerList[j].Id == playerId1 || this.pPairingPlayerList[j].Id == playerId2)
                                    this.pPairingPlayerList[j].State = SqliteInterface.PlayerState.Paired;
                            }
                        }
                    }
                }

                foreach (var name in frmPairingManual.lstNames.Items)
                {
                    int pid = db.GetPlayerID((string)name);
                    var idx = Array.FindIndex(this.pPairingPlayerList, p => p.Id == pid);
                    if (idx >= 0 && idx < iPairingPlayerCntAll)
                        this.pPairingPlayerList[idx].State = SqliteInterface.PlayerState.Freilos;
                }

                this.PairingInsertNoPlaying();
            }
            this.LoadPairingList();
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

            InitializeBonus(1, "Clubgame", ref lblBonusClub, ref tbBonusClub, ref lblBonusClubValue);
            InitializeBonus(2, "Excused", ref lblBonusExcused, ref tbBonusExcused, ref lblBonusExcusedValue);
            InitializeBonus(3, "Unexcused", ref lblBonusUnexcused, ref tbBonusUnexcused, ref lblBonusUnexcusedValue);
            InitializeBonus(4, "Retired", ref lblBonusRetired, ref tbBonusRetired, ref lblBonusRetiredValue);
            InitializeBonus(5, "Freilos", ref lblBonusFreilos, ref tbBonusFreilos, ref lblBonusFreilosValue);
            InitializeBonus(6, "Verlust", ref lblBonusVerlust, ref tbBonusVerlust, ref lblBonusVerlustValue);

            this.lblRoundsGameRepeat = new Label();
            this.lblRatioFirst2Last = new Label();
            this.lblFirstRoundRandom = new Label();
            this.lblOutputTo = new Label();
            this.lblNiceName = new Label();
            this.tbNiceName = new TextBox();
            this.numRoundsGameRepeat = new NumericUpDown();
            this.ddlRatioFirst2Last = new ComboBox();
            this.ddlFirstRoundRandom = new ComboBox();
            this.tooltip = new ToolTip();
            this.chkFreilosVerteilen = new CheckBox();
            this.chkNovusRandomBoard = new CheckBox();
            this.chkHtml = new CheckBox();
            this.chkXml = new CheckBox();
            this.chkTxt = new CheckBox();
            this.chkCsv = new CheckBox();
            this.chkWickerNormalization = new CheckBox();

            this.btDonate1 = new Button();
            this.btDonate2 = new Button();
            this.mnuMainmenu = new MenuStrip();
            this.mnuTurnierstart = new ToolStripMenuItem();
            this.mnuStartStart = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripSeparator();
            this.mnuStartLanguage = new ToolStripMenuItem();

            this.mnuPlayers = new ToolStripMenuItem();
            this.mnuPlayersImport = new ToolStripMenuItem();
            this.mnuPlayersDeleteAll = new ToolStripMenuItem();
            this.mnuPlayersRebaseIds = new ToolStripMenuItem();
            this.mnuPaarungen = new ToolStripMenuItem();
            this.mnuPaarungNext = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripSeparator();
            this.mnuPaarungManuell = new ToolStripMenuItem();
            this.mnuPaarungThisMan = new ToolStripMenuItem();
            this.toolStripMenuItem3 = new ToolStripSeparator();
            this.mnuPaarungDropLast = new ToolStripMenuItem();
            this.mnuListen = new ToolStripMenuItem();
            this.mnuListenPairing = new ToolStripMenuItem();
            this.mnuListenAll = new ToolStripMenuItem();
            this.mnuListenStanding = new ToolStripMenuItem();
            this.mnuListenStandingFull = new ToolStripMenuItem();
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
            this.tabPlayer.Controls.Add(this.grdPlayers);
            this.tabPlayer.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.tabPlayer.Location = new Point(4, 4);
            this.tabPlayer.Name = "tabPlayer";
            this.tabPlayer.Padding = new Padding(3);
            this.tabPlayer.Size = new Size(696, 373);
            this.tabPlayer.TabIndex = 0;
            this.tabPlayer.Text = "Player";
            this.tabPlayer.UseVisualStyleBackColor = true;
            this.tabPlayer.Leave += TabPlayerLeave;

            this.grdPlayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPlayers.Columns.AddRange((DataGridViewColumn)this.colPlayerID, (DataGridViewColumn)this.colPlayerName, (DataGridViewColumn)this.colRating, (DataGridViewColumn)this.colPlayerState);
            this.grdPlayers.Dock = DockStyle.Fill;
            this.grdPlayers.Location = new Point(3, 3);
            this.grdPlayers.Name = "grdPlayers";
            this.grdPlayers.Size = new Size(690, 367);
            this.grdPlayers.TabIndex = 0;
            this.grdPlayers.CellEndEdit += new DataGridViewCellEventHandler(this.GrdPlayersCellEndEdit);
            this.grdPlayers.CurrentCellDirtyStateChanged += new EventHandler(this.GrdPlayersDirty);
            this.grdPlayers.DataError += new DataGridViewDataErrorEventHandler(this.GrdPlayersDataError);
            this.grdPlayers.CellValidating += new DataGridViewCellValidatingEventHandler(this.GrdPlayersCellValidating);
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
            this.tabPairings.Controls.Add(this.pnlPairingPanel);
            this.tabPairings.Controls.Add(this.grdPairings);
            this.tabPairings.Location = new Point(4, 4);
            this.tabPairings.Name = "tabPairings";
            this.tabPairings.Padding = new Padding(3);
            this.tabPairings.Size = new Size(696, 373);
            this.tabPairings.TabIndex = 1;
            this.tabPairings.Text = "Pairings";
            this.tabPairings.UseVisualStyleBackColor = true;
            this.btDonate2.Location = new Point(18, 10);
            this.btDonate2.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.btDonate2.Size = new Size(108, 30);
            this.btDonate2.TabIndex = 30;
            this.btDonate2.Click += new EventHandler(this.BtDonateClick);
            this.pnlPairingPanel.Controls.Add(this.chkPairingOnlyPlayed);
            this.pnlPairingPanel.Controls.Add(this.numRoundSelect);
            this.pnlPairingPanel.Controls.Add(this.lblRunde);
            this.pnlPairingPanel.Controls.Add(this.btDonate2);
            this.pnlPairingPanel.Dock = DockStyle.Top;
            this.pnlPairingPanel.Location = new Point(3, 3);
            this.pnlPairingPanel.Name = "pnlPairingPanel";
            this.pnlPairingPanel.Size = new Size(690, 51);
            this.pnlPairingPanel.TabIndex = 1;
            this.chkPairingOnlyPlayed.Checked = true;
            this.chkPairingOnlyPlayed.CheckState = CheckState.Checked;
            this.chkPairingOnlyPlayed.Location = new Point(380, 18);
            this.chkPairingOnlyPlayed.Name = "chkPairingOnlyPlayed";
            this.chkPairingOnlyPlayed.Size = new Size(164, 24);
            this.chkPairingOnlyPlayed.TabIndex = 2;
            this.chkPairingOnlyPlayed.Text = "only played games";
            this.chkPairingOnlyPlayed.UseVisualStyleBackColor = true;
            this.chkPairingOnlyPlayed.CheckedChanged += new EventHandler(this.ChkPairingOnlyPlayedCheckedChanged);
            this.numRoundSelect.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.numRoundSelect.Location = new Point(237, 16);
            this.numRoundSelect.Maximum = new Decimal(new int[4] { 298, 0, 0, 0 });
            this.numRoundSelect.Name = "numRoundSelect";
            this.numRoundSelect.Size = new Size(59, 26);
            this.numRoundSelect.TabIndex = 1;
            this.numRoundSelect.ValueChanged += new EventHandler(this.NumRoundSelectValueChanged);
            this.lblRunde.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.lblRunde.Location = new Point(129, 17);
            this.lblRunde.Name = "lblRunde";
            this.lblRunde.Size = new Size(93, 23);
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
            this.grdPairings.CellEndEdit += new DataGridViewCellEventHandler(GrdPairingsCellEndEdit);
            this.grdPairings.CurrentCellDirtyStateChanged += new EventHandler(GrdPairingsDirty);
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
            this.colPairingNameWhite.Width = 230;
            this.colPairingWhiteAddinfo.HeaderText = "AddInfo";
            this.colPairingWhiteAddinfo.Name = "colPairingWhiteAddinfo";
            this.colPairingWhiteAddinfo.ReadOnly = true;
            this.colPairingWhiteAddinfo.Width = 50;
            this.colPairingWhiteAddinfo.Visible = false;
            this.colPairingIDB.HeaderText = "ID_B";
            this.colPairingIDB.Name = "colPairingIDB";
            this.colPairingIDB.ReadOnly = true;
            this.colPairingIDB.Visible = false;
            this.colPairingIDB.Width = 40;
            this.colPairingNameBlack.HeaderText = "Black";
            this.colPairingNameBlack.Name = "colPairingNameBlack";
            this.colPairingNameBlack.ReadOnly = true;
            this.colPairingNameBlack.Width = 230;
            this.colPairingAddInfoB.HeaderText = "AddInfo";
            this.colPairingAddInfoB.Name = "colPairingAddInfoB";
            this.colPairingAddInfoB.ReadOnly = true;
            this.colPairingAddInfoB.Width = 50;
            this.colPairingAddInfoB.Visible = false;
            this.colPairingResult.HeaderText = "Result";
            this.colPairingResult.Name = "colPairingResult";
            this.colPairingResult.Width = 140;

            this.tabSettings.Controls.Add((Control)this.lblRoundsGameRepeat);
            this.tabSettings.Controls.Add((Control)this.lblRatioFirst2Last);
            this.tabSettings.Controls.Add((Control)this.lblFirstRoundRandom);
            this.tabSettings.Controls.Add((Control)this.numRoundsGameRepeat);
            this.tabSettings.Controls.Add((Control)this.ddlRatioFirst2Last);
            this.tabSettings.Controls.Add((Control)this.ddlFirstRoundRandom);
            this.tabSettings.Controls.Add((Control)this.chkFreilosVerteilen);
            this.tabSettings.Controls.Add((Control)this.chkNovusRandomBoard);
            this.tabSettings.Controls.Add((Control)this.btDonate1);
            this.tabSettings.Controls.Add(this.chkHtml);
            this.tabSettings.Controls.Add(this.chkXml);
            this.tabSettings.Controls.Add(this.chkTxt);
            this.tabSettings.Controls.Add(this.chkCsv);
            this.tabSettings.Controls.Add(this.chkWickerNormalization);
            this.tabSettings.Controls.Add(this.lblOutputTo);
            this.tabSettings.Controls.Add(this.lblNiceName);
            this.tabSettings.Controls.Add(this.tbNiceName);

            this.tabSettings.Location = new Point(4, 4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new Padding(3);
            this.tabSettings.Size = new Size(696, 373);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            this.tabSettings.Leave += TabSettingsLeave;

            var yOutput = 230;

            this.chkFreilosVerteilen.CheckAlign = ContentAlignment.MiddleRight;
            this.chkFreilosVerteilen.TextAlign = ContentAlignment.MiddleRight;
            this.chkFreilosVerteilen.Checked = true;
            this.chkFreilosVerteilen.CheckState = CheckState.Checked;
            this.chkFreilosVerteilen.Location = new Point(44, yOutput);
            this.chkFreilosVerteilen.Name = "chkFreilosVerteilen";
            this.chkFreilosVerteilen.Size = new Size(215, 24);
            this.chkFreilosVerteilen.TabIndex = 8;
            this.chkFreilosVerteilen.Text = "Assign bye's even";
            this.chkFreilosVerteilen.UseVisualStyleBackColor = true;

            this.lblFirstRoundRandom.Location = new Point(270, yOutput);
            this.lblFirstRoundRandom.Size = new Size(170, 23);
            this.lblFirstRoundRandom.Text = "# First round random";
            this.lblFirstRoundRandom.TextAlign = ContentAlignment.MiddleRight;
            this.ddlFirstRoundRandom.Location = new Point(440, yOutput);
            this.ddlFirstRoundRandom.Size = new Size(40, 21);
            var li = new List<int>(new int[] { 0, 10, 50, 100, 150, 200, 300, 400, 500 });
            this.ddlFirstRoundRandom.DataSource = li;

            this.chkNovusRandomBoard.CheckAlign = ContentAlignment.MiddleRight;
            this.chkNovusRandomBoard.Checked = true;
            this.chkNovusRandomBoard.CheckState = CheckState.Checked;
            this.chkNovusRandomBoard.Location = new Point(485, yOutput);
            this.chkNovusRandomBoard.Name = "chkNovusRandomBoard";
            this.chkNovusRandomBoard.Size = new Size(125, 24);
            this.chkNovusRandomBoard.TabIndex = 9;
            this.chkNovusRandomBoard.Text = "Novuss";
            this.chkNovusRandomBoard.UseVisualStyleBackColor = true;
            this.chkNovusRandomBoard.TextAlign = ContentAlignment.MiddleRight;


            yOutput += 23;
            this.lblRoundsGameRepeat.Location = new Point(44, yOutput);
            this.lblRoundsGameRepeat.Size = new Size(200, 23);
            this.lblRoundsGameRepeat.TabIndex = 10;
            this.lblRoundsGameRepeat.Text = "# Rounds before paired again";
            this.lblRoundsGameRepeat.TextAlign = ContentAlignment.MiddleRight;
            this.numRoundsGameRepeat.Location = new Point(246, yOutput);
            this.numRoundsGameRepeat.Maximum = new Decimal(new int[4] { 50, 0, 0, 0 });
            this.numRoundsGameRepeat.Name = "numRoundsGameRepeat";
            this.numRoundsGameRepeat.Size = new Size(40, 21);
            this.numRoundsGameRepeat.TabIndex = 9;

            this.lblRatioFirst2Last.Location = new Point(370, yOutput);
            this.lblRatioFirst2Last.Size = new Size(200, 23);
            this.lblRatioFirst2Last.Text = "# Rounds before paired again";
            this.lblRatioFirst2Last.TextAlign = ContentAlignment.MiddleRight;
            this.ddlRatioFirst2Last.Location = new Point(570, yOutput);
            this.ddlRatioFirst2Last.Size = new Size(40, 21);
            List<float> list = new List<float>(new float[] { 4, 3.5f, 3, 2.5f, 2, 1.5f, 1.2f, 1.1f, 1.01f });
            this.ddlRatioFirst2Last.DataSource = list;


            yOutput += 23;
            int dxOutput = 50, dx0 = 246;
            this.chkHtml.CheckAlign = chkHtml.TextAlign = ContentAlignment.MiddleRight;
            this.chkHtml.Location = new Point(dx0, yOutput);
            this.chkHtml.Name = "chkHtml";
            this.chkHtml.Size = new Size(45, 24);
            this.chkHtml.TabIndex = 10;
            this.chkHtml.Text = "Html";
            this.chkHtml.UseVisualStyleBackColor = true;

            this.chkXml.CheckAlign = chkXml.TextAlign = ContentAlignment.MiddleRight;
            this.chkXml.Location = new Point(dx0 + dxOutput, yOutput);
            this.chkXml.Name = "chkXml";
            this.chkXml.Size = new Size(44, 24);
            this.chkXml.TabIndex = 11;
            this.chkXml.Text = "Xml";
            this.chkXml.UseVisualStyleBackColor = true;

            this.chkTxt.CheckAlign = chkTxt.TextAlign = ContentAlignment.MiddleRight;
            this.chkTxt.Location = new Point(dx0 + 2 * dxOutput, yOutput);
            this.chkTxt.Name = "chkTxt";
            this.chkTxt.Size = new Size(44, 24);
            this.chkTxt.TabIndex = 12;
            this.chkTxt.Text = "Txt";
            this.chkTxt.UseVisualStyleBackColor = true;

            this.chkCsv.CheckAlign = chkCsv.TextAlign = ContentAlignment.MiddleRight;
            this.chkCsv.Location = new Point(dx0 + 3 * dxOutput, yOutput);
            this.chkCsv.Name = "chkCsv";
            this.chkCsv.Size = new Size(44, 24);
            this.chkCsv.TabIndex = 13;
            this.chkCsv.Text = "Csv";
            this.chkCsv.UseVisualStyleBackColor = true;

            this.chkWickerNormalization.CheckAlign = chkWickerNormalization.TextAlign = ContentAlignment.MiddleRight;
            this.chkWickerNormalization.Size = new Size(150, 24);
            this.chkWickerNormalization.Location = new Point(610 - chkWickerNormalization.Size.Width, yOutput);
            this.chkWickerNormalization.TabIndex = 14;
            this.chkWickerNormalization.UseVisualStyleBackColor = true;
            var ttwn = @"If this checkbox is checked, all the Keizer points are normalized so 
that a win against the last in the rank counts 1 Keizer point. 
This doesn't really change anything. But it somehow makes the 
Keizer points much more graspable.

(In the original Keizer system the given Keizer points usually are big whole numbers.
 Originally, this has probably been the case to make the caclculations easier. 
 It's easier to calculate with whole numbers if you don't have a computer.

 Nowadays, all the calculation is done by the computer and so we can use much 
 smaller but broken numbers.)";
            this.tooltip.SetToolTip(chkWickerNormalization, ttwn);

            this.lblOutputTo.Location = new Point(44, yOutput);
            this.lblOutputTo.Size = new Size(200, 23);
            this.lblOutputTo.Text = "# Rounds before paired again";
            this.lblOutputTo.TextAlign = ContentAlignment.MiddleRight;


            yOutput += 23;
            this.lblNiceName.Location = new Point(44, yOutput);
            this.lblNiceName.Size = new Size(200, 23);
            this.lblNiceName.Text = "Tournament name";
            this.lblNiceName.TextAlign = ContentAlignment.MiddleRight;
            this.tbNiceName.Location = new Point(246, yOutput + 2);
            this.tbNiceName.Size = new Size(400, 23);
            this.tbNiceName.Text = "Tournament name";




            this.btDonate1.Location = new Point(43, 330);
            this.btDonate1.Name = "lblDonate";
            this.btDonate1.Size = new Size(149, 23);
            this.btDonate1.TabIndex = 30;
            this.btDonate1.Click += new EventHandler(this.BtDonateClick);

            this.mnuMainmenu.Items.AddRange(new ToolStripItem[]
                { this.mnuTurnierstart, this.mnuPaarungen, this.mnuListen, this.mnuPlayers, this.mnuHelp });
            this.mnuMainmenu.Location = new Point(0, 0);
            this.mnuMainmenu.Name = "mnuMainmenu";
            this.mnuMainmenu.Size = new Size(704, 24);
            this.mnuMainmenu.TabIndex = 1;
            this.mnuMainmenu.Text = "Program";
            this.mnuTurnierstart.DropDownItems.AddRange(new ToolStripItem[]
                { this.mnuStartStart, this.toolStripMenuItem1, this.mnuStartLanguage });
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
            this.mnuStartLanguage.Text = "Language, Sprache, Spraak, Langue...";
            this.mnuStartLanguage.Click += new EventHandler(this.MnuStartLanguageClick);

            this.mnuPlayersImport.Size = new Size(218, 22);
            this.mnuPlayersImport.Click += new EventHandler(this.MnuPlayersImportClick);

            this.mnuPlayersDeleteAll.Size = new Size(218, 22);
            this.mnuPlayersDeleteAll.Click += new EventHandler(this.MnuPlayersDeleteAllClickTa);

            this.mnuPlayersRebaseIds.Size = new Size(218, 22);
            this.mnuPlayersRebaseIds.Click += new EventHandler(this.MnuPlayersRebaseIdsClickTa);

            this.mnuPlayers.DropDownItems.AddRange(new ToolStripItem[]
                    { mnuPlayersImport, mnuPlayersDeleteAll, mnuPlayersRebaseIds});


            this.mnuPaarungen.DropDownItems.AddRange(new ToolStripItem[]
                { mnuPaarungNext, toolStripMenuItem2, mnuPaarungManuell, mnuPaarungThisMan, toolStripMenuItem3, mnuPaarungDropLast });
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
            this.mnuPaarungManuell.Click += new EventHandler(this.MnuPairingManualClick);
            this.mnuPaarungThisMan.Name = "mnuPaarungManuell";
            this.mnuPaarungThisMan.Size = new Size(183, 22);
            this.mnuPaarungThisMan.Click += new EventHandler(this.MnuPairingManualClick);
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new Size(180, 6);
            this.mnuPaarungDropLast.Name = "mnuPaarungDropLast";
            this.mnuPaarungDropLast.Size = new Size(183, 22);
            this.mnuPaarungDropLast.Text = "Drop last round";
            this.mnuPaarungDropLast.Click += new EventHandler(this.MnuPairingDropLastClick);
            this.mnuListen.DropDownItems.AddRange(new ToolStripItem[]
                 {  mnuListenAll, mnuListenPairing, mnuListenStanding, mnuListenStandingFull, mnuListenParticipants });
            this.mnuListen.Enabled = false;
            this.mnuListen.Name = "mnuListen";
            this.mnuListen.Size = new Size(42, 20);
            this.mnuListen.Text = "Lists";
            this.mnuListenAll.Name = "mnuListenAll";
            this.mnuListenAll.Size = new Size(158, 22);
            this.mnuListenAll.Text = "All";
            this.mnuListenAll.Click += new EventHandler(this.MnuListenAllClick);
            this.mnuListenPairing.Name = "mnuListenPairing";
            this.mnuListenPairing.Size = new Size(158, 22);
            this.mnuListenPairing.Text = "Pairings/Results";
            this.mnuListenPairing.Click += new EventHandler(this.MnuListenPairingClick);
            this.mnuListenStanding.Name = "mnuListenStanding";
            this.mnuListenStanding.Size = new Size(158, 22);
            this.mnuListenStanding.Text = "Standing";
            this.mnuListenStanding.Click += new EventHandler(this.MnuListenStandingClick);
            this.mnuListenStandingFull.Name = "mnuListenStandingFull";
            this.mnuListenStandingFull.Size = new Size(158, 22);
            this.mnuListenStandingFull.Text = "Standing Full";
            this.mnuListenStandingFull.Click += new EventHandler(this.MnuListenStandingClick);
            this.mnuListenParticipants.Name = "mnuListenParticipants";
            this.mnuListenParticipants.Size = new Size(158, 22);
            this.mnuListenParticipants.Text = "Participants";
            this.mnuListenParticipants.Click += new EventHandler(this.MnuListenParticipantsClick);
            this.mnuHelp.DropDownItems.AddRange(new ToolStripItem[]
                { mnuHelpDocumentation, mnuHelpFaq, mnuHelpAbout, });
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
                MessageBox.Show(ex.Message, db.Locl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
            }
        }

        private void InitializeBonus(int num, string name, ref Label lblText, ref TrackBar tb, ref Label lblValue)
        {
            var yloc = 15 + (num - 1) * 25;
            InitializeBonusText(num, yloc, name, ref lblText);
            InitializeBonusValue(num, yloc, name, ref lblValue);
            InitializeBonusTrackbar(num, yloc, name, ref tb);
        }

        private void InitializeBonusTrackbar(int num, int yloc, string name, ref TrackBar tb)
        {
            tb = new TrackBar();
            tb.BeginInit();
            this.tabSettings.Controls.Add(tb);
            tb.LargeChange = 20;
            tb.Location = new Point(245, yloc);
            tb.Maximum = 100;
            tb.Name = $"tbBonus" + name;
            tb.AutoSize = false;
            tb.Size = new Size(200, 20);
            tb.SmallChange = 5;
            tb.TabIndex = num;
            tb.TickFrequency = 5;
            tb.ValueChanged += new EventHandler(this.TbBonusValueChanged);
            tb.EndInit();
        }

        private void InitializeBonusText(int num, int yloc, string name, ref Label lblText)
        {
            lblText = new Label();
            this.tabSettings.Controls.Add(lblText);
            lblText.Location = new Point(90, yloc);
            lblText.Name = "lblBonus" + name;
            lblText.Size = new Size(150, 23);
            lblText.TabIndex = 10 + num;
            lblText.Text = "...";
            lblText.TextAlign = ContentAlignment.MiddleRight;
        }

        private void InitializeBonusValue(int num, int yloc, string name, ref Label lblValue)
        {
            lblValue = new Label();
            this.tabSettings.Controls.Add(lblValue);

            lblValue.Location = new Point(450, yloc);
            lblValue.Name = $"lblBonus{name}Value";
            lblValue.Size = new Size(100, 23);
            lblValue.TabIndex = 10 + num;
            lblValue.Text = "...";
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
        }

    }
}
