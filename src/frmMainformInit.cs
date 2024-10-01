using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwiUtils;

namespace KeizerForClubs
{
    public partial class frmMainform
    {

        private void InitializeComponent()
        {
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.0");
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

            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.1");
            int xloc = 12;
            InitializeBonus(1, xloc, "Clubgame", ref lblBonusClub, ref tbBonusClub, ref lblBonusClubValue);
            InitializeBonus(2, xloc, "Excused", ref lblBonusExcused, ref tbBonusExcused, ref lblBonusExcusedValue);
            InitializeBonus(3, xloc, "Unexcused", ref lblBonusUnexcused, ref tbBonusUnexcused, ref lblBonusUnexcusedValue);
            InitializeBonus(4, xloc, "Retired", ref lblBonusRetired, ref tbBonusRetired, ref lblBonusRetiredValue);
            InitializeBonus(5, xloc, "Freilos", ref lblBonusFreilos, ref tbBonusFreilos, ref lblBonusFreilosValue);
            InitializeBonus(6, xloc, "Verlust", ref lblBonusVerlust, ref tbBonusVerlust, ref lblBonusVerlustValue);
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.2");

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

            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.3");
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
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.4");

            this.tabSettings.Controls.Add(this.lblRoundsGameRepeat);
            this.tabSettings.Controls.Add(this.lblRatioFirst2Last);
            this.tabSettings.Controls.Add(this.lblFirstRoundRandom);
            this.tabSettings.Controls.Add(this.numRoundsGameRepeat);
            this.tabSettings.Controls.Add(this.ddlRatioFirst2Last);
            this.tabSettings.Controls.Add(this.ddlFirstRoundRandom);
            this.tabSettings.Controls.Add(this.chkFreilosVerteilen);
            this.tabSettings.Controls.Add(this.chkNovusRandomBoard);
            this.tabSettings.Controls.Add(this.btDonate1);
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

            var yOutput = 180;
            xloc -= 4;

            this.chkFreilosVerteilen.CheckAlign = ContentAlignment.MiddleRight;
            this.chkFreilosVerteilen.TextAlign = ContentAlignment.MiddleRight;
            this.chkFreilosVerteilen.Checked = true;
            this.chkFreilosVerteilen.CheckState = CheckState.Checked;
            this.chkFreilosVerteilen.Location = new Point(xloc, yOutput);
            this.chkFreilosVerteilen.Name = "chkFreilosVerteilen";
            this.chkFreilosVerteilen.Size = new Size(171, 24);
            this.chkFreilosVerteilen.TabIndex = 8;
            this.chkFreilosVerteilen.UseVisualStyleBackColor = true;

            this.lblFirstRoundRandom.Location = new Point(xloc + 180, yOutput);
            this.lblFirstRoundRandom.Size = new Size(170, 23);
            this.lblFirstRoundRandom.TextAlign = ContentAlignment.MiddleRight;
            this.ddlFirstRoundRandom.Location = new Point(xloc + 350, yOutput);
            this.ddlFirstRoundRandom.Size = new Size(40, 21);
            var li = new List<int>(new int[] { 0, 10, 50, 100, 150, 200, 300, 400, 500 });
            this.ddlFirstRoundRandom.DataSource = li;

            this.chkNovusRandomBoard.CheckAlign = ContentAlignment.MiddleRight;
            this.chkNovusRandomBoard.Checked = true;
            this.chkNovusRandomBoard.CheckState = CheckState.Checked;
            this.chkNovusRandomBoard.Location = new Point(xloc + 395, yOutput);
            this.chkNovusRandomBoard.Name = "chkNovusRandomBoard";
            this.chkNovusRandomBoard.Size = new Size(126, 24);
            this.chkNovusRandomBoard.TabIndex = 9;
            this.chkNovusRandomBoard.UseVisualStyleBackColor = true;
            this.chkNovusRandomBoard.TextAlign = ContentAlignment.MiddleRight;
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.5");


            yOutput += 23;
            this.lblRoundsGameRepeat.Location = new Point(xloc - 5, yOutput);
            this.lblRoundsGameRepeat.Size = new Size(159, 23);
            this.lblRoundsGameRepeat.TabIndex = 10;
            this.lblRoundsGameRepeat.TextAlign = ContentAlignment.MiddleRight;
            this.numRoundsGameRepeat.Location = new Point(xloc + 158, yOutput + 2);
            this.numRoundsGameRepeat.Maximum = new Decimal(new int[4] { 50, 0, 0, 0 });
            this.numRoundsGameRepeat.Name = "numRoundsGameRepeat";
            this.numRoundsGameRepeat.Size = new Size(40, 21);
            this.numRoundsGameRepeat.TabIndex = 9;

            this.lblRatioFirst2Last.Location = new Point(xloc + 280, yOutput);
            this.lblRatioFirst2Last.Size = new Size(200, 23);
            this.lblRatioFirst2Last.TextAlign = ContentAlignment.MiddleRight;
            this.ddlRatioFirst2Last.Location = new Point(xloc + 481, yOutput);
            this.ddlRatioFirst2Last.Size = new Size(40, 21);
            List<float> list = new List<float>(new float[] { 4, 3.5f, 3, 2.5f, 2, 1.5f, 1.2f, 1.1f, 1.01f });
            this.ddlRatioFirst2Last.DataSource = list;


            yOutput += 23;
            this.lblOutputTo.Location = new Point(xloc, yOutput);
            this.lblOutputTo.Size = new Size(154, 23);
            this.lblOutputTo.TextAlign = ContentAlignment.MiddleRight;

            int dxOutput = 50, dx0 = xloc + 166;
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
            this.chkWickerNormalization.Location = new Point(xloc + 521 - chkWickerNormalization.Size.Width, yOutput);
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


            yOutput += 23;
            this.lblNiceName.Location = new Point(xloc, yOutput);
            this.lblNiceName.Size = new Size(154, 23);
            this.lblNiceName.TextAlign = ContentAlignment.MiddleRight;
            this.tbNiceName.Location = new Point(xloc + 158, yOutput + 2);
            this.tbNiceName.Size = new Size(362, 23);


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
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.6");

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
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 1.7");

            try
            {
                if (Args.Length > 0)
                    OpenTournament(Args[0]);
                else if (File.Exists(ReadDBFileName()))
                    OpenTournament(ReadDBFileName());
            }
            catch (Exception ex)
            {
                ExLogger.Instance.LogException("frmMainForm.IniComp 1.8", ex);
                MessageBox.Show(ex.Message, db.Locl_GetText("GUI_TEXT", "FehlerAufgetreten"), MessageBoxButtons.OK);
            }
            ExLogger.Instance.LogInfo("frmMainForm.IniComp 2.0");
        }

        private void InitializeBonus(int num, int xloc, string name, ref Label lblText, ref TrackBar tb, ref Label lblValue)
        {
            var yloc = 15 + (num - 1) * 25;
            InitializeBonusText(num, xloc, yloc, name, ref lblText);
            InitializeBonusTrackbar(num, xloc + 155, yloc, name, ref tb);
            InitializeBonusValue(num, xloc + 360, yloc, name, ref lblValue);
        }

        private void InitializeBonusTrackbar(int num, int xloc, int yloc, string name, ref TrackBar tb)
        {
            tb = new TrackBar();
            tb.BeginInit();
            this.tabSettings.Controls.Add(tb);
            tb.LargeChange = 20;
            tb.Location = new Point(xloc, yloc);
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

        private void InitializeBonusText(int num, int xloc, int yloc, string name, ref Label lblText)
        {
            lblText = new Label();
            this.tabSettings.Controls.Add(lblText);
            lblText.Location = new Point(xloc, yloc);
            lblText.Name = "lblBonus" + name;
            lblText.Size = new Size(150, 23);
            lblText.TabIndex = 10 + num;
            lblText.TextAlign = ContentAlignment.MiddleRight;
        }

        private void InitializeBonusValue(int num, int xloc, int yloc, string name, ref Label lblValue)
        {
            lblValue = new Label();
            this.tabSettings.Controls.Add(lblValue);

            lblValue.Location = new Point(xloc, yloc);
            lblValue.Name = $"lblBonus{name}Value";
            lblValue.Size = new Size(100, 23);
            lblValue.TabIndex = 10 + num;
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
        }
    }
}
