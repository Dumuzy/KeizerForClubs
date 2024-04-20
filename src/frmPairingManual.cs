using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace KeizerForClubs
{
    public class frmPairingManual : Form
    {
        private IContainer components;
        public Button btnCancel;
        private Button btnOk;
        public DataGridViewTextBoxColumn colBlack;
        public DataGridViewTextBoxColumn colWhite;
        public DataGridView grdPaarungen;
        public ListBox lstNames;
        private SqliteInterface db;
        private int runde;


        public frmPairingManual(SqliteInterface db, int runde)
        {
            this.InitializeComponent();
            this.db = db;
            this.runde = runde;
        }

        private void GrdPaarungenCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.grdPaarungen.SelectedCells.Count <= 0)
                return;
            DataGridViewCell selectedCell = this.grdPaarungen.SelectedCells[0];
            if (selectedCell.Value == null)
            {
                int selectedIndex = this.lstNames.SelectedIndex;
                if (selectedIndex <= -1)
                    return;
                selectedCell.Value = this.lstNames.Items[selectedIndex];
                this.lstNames.Items.RemoveAt(selectedIndex);
                if (this.lstNames.Items.Count <= 0)
                    return;
                this.lstNames.SelectedIndex = 0;
            }
            else
            {
                this.lstNames.Items.Add(selectedCell.Value);
                selectedCell.Value = (object)null;
            }
        }

        private bool fCheckPairings()
        {
            int nAvailable = 0;
            foreach (string playerName in lstNames.Items)
            {
                if (playerName == "NN")
                    continue;
                var player = db.GetPlayer($" WHERE name == '{playerName}' ", "", runde);
                if (player.State == SqliteInterface.PlayerState.Available)
                    ++nAvailable;
            }
            if (nAvailable > 1)
                return false;
            foreach(DataGridViewRow row in this.grdPaarungen.Rows)
            {
                if (row.Cells[0].Value == null && row.Cells[1].Value != null || 
                    row.Cells[0].Value != null && row.Cells[1].Value == null)
                    return false;
            }
            return true;
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            if (!this.fCheckPairings())
                return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelClick(object sender, EventArgs e) => this.Close();

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lstNames = new ListBox();
            this.grdPaarungen = new DataGridView();
            this.colWhite = new DataGridViewTextBoxColumn();
            this.colBlack = new DataGridViewTextBoxColumn();
            this.btnOk = new Button();
            this.btnCancel = new Button();
            ((ISupportInitialize)this.grdPaarungen).BeginInit();
            this.SuspendLayout();
            this.lstNames.FormattingEnabled = true;
            this.lstNames.Location = new Point(12, 12);
            this.lstNames.Name = "lstNames";
            this.lstNames.Size = new Size(203, 238);
            this.lstNames.TabIndex = 0;
            this.grdPaarungen.AllowDrop = true;
            this.grdPaarungen.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPaarungen.Columns.AddRange((DataGridViewColumn)this.colWhite, (DataGridViewColumn)this.colBlack);
            this.grdPaarungen.Location = new Point(236, 12);
            this.grdPaarungen.Name = "grdPaarungen";
            this.grdPaarungen.Size = new Size(434, 197);
            this.grdPaarungen.TabIndex = 1;
            this.grdPaarungen.CellClick += new DataGridViewCellEventHandler(this.GrdPaarungenCellClick);
            this.colWhite.HeaderText = "White";
            this.colWhite.Name = "colWhite";
            this.colWhite.ReadOnly = true;
            this.colWhite.Width = 180;
            this.colBlack.HeaderText = "Black";
            this.colBlack.Name = "colBlack";
            this.colBlack.ReadOnly = true;
            this.colBlack.Width = 180;
            this.btnOk.Location = new Point(352, 215);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(76, 35);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new EventHandler(this.BtnOkClick);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(472, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.BtnCancelClick);
            this.AcceptButton = (IButtonControl)this.btnOk;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = (IButtonControl)this.btnCancel;
            this.ClientSize = new Size(692, 317);
            this.ControlBox = false;
            this.Controls.Add((Control)this.btnCancel);
            this.Controls.Add((Control)this.btnOk);
            this.Controls.Add((Control)this.grdPaarungen);
            this.Controls.Add((Control)this.lstNames);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Name = nameof(frmPairingManual);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Edit pairings";
            ((ISupportInitialize)this.grdPaarungen).EndInit();
            this.ResumeLayout(false);
        }
    }
}
