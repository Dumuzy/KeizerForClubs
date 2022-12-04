using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KeizerForClubs
{
    public class frmLangSelect : Form
    {
        private IContainer components;
        private Button btnOK;
        public RadioButton radHollaendisch;
        public RadioButton radDeutsch;
        public RadioButton radEnglisch;
        private readonly string currLang;


        public frmLangSelect(string currLang)
        {
            this.currLang = currLang;
            this.InitializeComponent();
        }

        private void BtnOKClick(object sender, EventArgs e) => this.Close();

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.radEnglisch = new RadioButton();
            this.radDeutsch = new RadioButton();
            this.radHollaendisch = new RadioButton();
            this.btnOK = new Button();
            this.SuspendLayout();
            this.radEnglisch.Location = new Point(44, 20);
            this.radEnglisch.Name = "radEnglisch";
            this.radEnglisch.Size = new Size(104, 24);
            this.radEnglisch.TabIndex = 0;
            this.radEnglisch.TabStop = true;
            this.radEnglisch.Text = "English";
            this.radEnglisch.UseVisualStyleBackColor = true;
            this.radDeutsch.Location = new Point(44, 55);
            this.radDeutsch.Name = "radDeutsch";
            this.radDeutsch.Size = new Size(104, 24);
            this.radDeutsch.TabIndex = 1;
            this.radDeutsch.Text = "Deutsch";
            this.radDeutsch.UseVisualStyleBackColor = true;
            this.radHollaendisch.Location = new Point(44, 90);
            this.radHollaendisch.Name = "radHollaendisch";
            this.radHollaendisch.Size = new Size(104, 24);
            this.radHollaendisch.TabIndex = 2;
            this.radHollaendisch.Text = "Nederlands";
            this.radHollaendisch.UseVisualStyleBackColor = true;
            this.btnOK.Location = new Point(191, 55);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.BtnOKClick);
            this.AcceptButton = (IButtonControl)this.btnOK;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(304, 172);
            this.ControlBox = false;
            this.Controls.Add((Control)this.btnOK);
            this.Controls.Add((Control)this.radHollaendisch);
            this.Controls.Add((Control)this.radDeutsch);
            this.Controls.Add((Control)this.radEnglisch);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = nameof(frmLangSelect);
            this.StartPosition = FormStartPosition.CenterParent;
            if (currLang == "NL")
                radHollaendisch.Checked = true;
            else if (currLang == "EN")
                radEnglisch.Checked = true;
            else if (currLang == "DE")
                radDeutsch.Checked = true;

            this.ResumeLayout(false);
        }
    }
}
