using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using AwiUtils;

namespace PuzzleKnocker
{
    internal class DonateButton
    {
        /// <summary> Creates a DonateButton wrapper for a button. The donate button is only shown under certain 
        /// circumstances - and then in different colors. </summary>
        /// <param name="bt"> The button. </param>
        /// <param name="iniDonated"> A string from the ini-File. If == MAC-address, button is not shown. </param>
        /// <param name="getNumClicks"> A func which returns num clicks made. </param>
        /// <param name="minNumClicks"> If getNumCLicks { minNumClicks, button is not shown. </param>
        /// <param name="shallShowNow"> Only if shallShowNow returns true, the button is shown. </param>
        public DonateButton(Button bt, string iniDonated, Func<int> getNumClicks, int minNumClicks,
            Func<bool> shallShowNow, int clicksTillColorChange = 20)
        {
            this.btDonate = bt;
            this.btDonate.Visible = false;
            this.iniDonated = iniDonated.Replace("-", "").Replace(":", "").Trim();
            this.getNumClicks = getNumClicks;
            this.minNumClicksForShowing = minNumClicks;
            this.shallShowNow = shallShowNow;
            this.clicksTillColorChange = clicksTillColorChange;
        }

        public string Text => btDonate.Text;

        public void SetState()
        {
            var numClicks = getNumClicks();
            if (shallShowNow() && numClicks >= minNumClicksForShowing && !HasDonated())
            {
                var colors = new Color[] { Color.FromArgb(192, 0, 0), Color.AliceBlue, Color.Black,
                    Color.FromArgb(0, 192, 0), Color.CornflowerBlue, Color.MediumVioletRed, Color.DarkOrange };
                btDonate.Visible = true;
                var divisor = clicksTillColorChange * colors.Length;
                btDonate.BackColor = colors[(numClicks % divisor) / clicksTillColorChange];
                if (btDonate.BackColor.GetBrightness() > 0.4)
                    btDonate.ForeColor = Color.Black;
                else
                    btDonate.ForeColor = Color.White;
            }
            else
                btDonate.Visible = false;
        }

        /// <summary> Gibt false zrück, falls iniDonated eine MAC-Adresse des Geräts enthält. Das ist 
        /// ein Zeichen dafür, daß der Donate-Button gar nicht mehr gezeigt werden soll. Weil schon 
        /// gespendet wurde. </summary>
        bool HasDonated()
        {
            if (!hasDonated.HasValue)
                hasDonated = !string.IsNullOrEmpty(iniDonated) &&
                    GetMACAddresses().ContainsIgnoreCase(iniDonated);
            return hasDonated.Value;
        }

        Li<string> GetMACAddresses()
        {
            Li<string> macAddresses = new Li<string>();
            //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            //    macAddresses.Add(nic.GetPhysicalAddress().ToString());
            // 
            // Getting the mac adresses might be the problem for the virus scanner programs. 
            // And so maybe the reason for issues #13 and #82.
            // I'll try it without them. And use the current (year*2+11) in hex instead. 
            var currYear = Helper.ToInt(DateTime.Now.ToString("yyyy"));
            for (int i = 2; i >= 0; --i)
            {
                int y = currYear - i;
                var dateStr = (y * 2 + 11).ToString("X4");
                macAddresses.Add(dateStr);
            }
            return macAddresses;
        }

        bool? hasDonated;
        readonly Button btDonate;
        readonly string iniDonated;
        readonly Func<int> getNumClicks;
        readonly int minNumClicksForShowing, clicksTillColorChange;
        readonly Func<bool> shallShowNow;
    }
}
