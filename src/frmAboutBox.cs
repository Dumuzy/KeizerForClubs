﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeizerForClubs;

namespace KeizerForClubs
{
    partial class frmAboutBox : Form
    {
        public frmAboutBox(bool isDonate = false)
        {
            InitializeComponent();
            this.Text = labelProductName.Text = $"KeizerForClubs v{AssemblyVersion}";
            labelCopyright.Text = GetCopyright();
            linkLabel1.Text = TrimTextForLinkLabel(GetRawText(isDonate));

            if (!isDonate)
            {
                AddLink2Label(linkLabel1, "example tournament", "http://keizer.schlapp.name/index.php?id=beispielturnier");
                AddLink2Label(linkLabel1, "Github", "https://github.com/Dumuzy/KeizerForClubs");
                AddLink2Label(linkLabel1, "Dumuzy@Github", "https://github.com/Dumuzy");
                AddLink2Label(linkLabel1, "Alakaluf@Lichess", "https://lichess.org/@/Alakaluf");
                AddLink2Label(linkLabel1, "pascalg@Lichess", "https://lichess.org/@/pascalg");
            }

            linkLabel2.Text = TrimTextForLinkLabel(@"If you like this software and want to support its maintainenance ... 
I would be delighted if you'd buy me a coffee! :-)
I am   keizer@atlantis44.de   at Paypal.");

            AddLink2Label(isDonate ? linkLabel1 : linkLabel2, "Paypal", "https://www.paypal.com");
            if (isDonate)
            {
                AddLink2Label(linkLabel1, "Alakaluf at Lichess", "https://lichess.org/@/Alakaluf");
                linkLabel2.Visible = false;
                labelCopyright.Visible = false;
                labelProductName.Font = linkLabel1.Font = new System.Drawing.Font("Segoe UI", 14F,
                    System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)(0));
                tableLayoutPanel.RowStyles[0] = new RowStyle(SizeType.Absolute, 20);
                tableLayoutPanel.RowStyles[1] = new RowStyle(SizeType.Percent, 60);
                tableLayoutPanel.RowStyles[2] = new RowStyle(SizeType.Absolute, 0);
            }
        }

        string GetRawText(bool isDonate)
        {
            string t;
            if (!isDonate)
                t = @"Originally written by T. Schlapp. He's got a whole website dedicated to the Keizer system, 
including a nice example tournament. Improved and put to Github by me. Contact me:
Dumuzy@Github aka Alakaluf@Lichess for questions, suggestions, bug reports or chess lessons for players with a Lichess speed rating below 1750. Many thanks to pascalg@Lichess for testing 
and translation.";
            else
                t = @"

If you like this software and want to support its maintainenance ...   You would be delighted if you'd buy me a coffee! :-)
I am   keizer@atlantis44.de   at Paypal. And I would be very thankful.

Also, I'm giving chess lessons for players with a Lichess rapid rating below 1750. 
Only 10 Eu per hour in deutsch (or english with a heavy german accent). Contact Alakaluf at Lichess.";
            return t;
        }

        void AddLink2Label(LinkLabel ll, string linkText, string link, int delta = 0)
        {
            var idx = ll.Text.IndexOf(linkText);
            if (idx != -1)
                ll.Links.Add(idx - delta, linkText.Length, link);
        }

        string TrimTextForLinkLabel(string text)
        {
            // Replace single linebreaks by space, but not double line breaks. 
            // A double line breaks represents the end of a paragraph, this shall be kept.
            // A single line break shall be removed, so that the label's word wrap function does all 
            // the line breaking. 
            var t = Regex.Replace(text, @"(?<!\r\n) *\r\n\b(?!\r\n)", " ");
            // Replace \r\ by \n for that counting of characters in AddLink2Label works. 
            t = t.Replace("\r\n", "\n");
            return t;
        }

        string GetCopyright()
        {
            var c = Regex.Replace(copyright, @" +", " ");
            // Replace single linebreaks by space, but not double line breaks. 
            // A double line breaks represents the end of a paragraph, this shall be kept.
            // A single line break shall be removed, so that the label's word wrap function does all 
            // the line breaking. 
            c = Regex.Replace(c, @"(?<!\r\n) *\r\n\b(?!\r\n)", " ");
            return c;
        }

        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Determine which link was clicked within the LinkLabel.
            this.linkLabel1.Links[linkLabel1.Links.IndexOf(e.Link)].Visited = true;

            // Display the appropriate link based on the value of the 
            // LinkData property of the Link object.
            string target = e.Link.LinkData as string;

            if (null != target && (target.StartsWith("www") || target.StartsWith("http")))
                frmMainform.OpenWithDefaultApp(target);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            if (null != target && (target.StartsWith("www") || target.StartsWith("http")))
                frmMainform.OpenWithDefaultApp(target);
        }

        const string copyright = @"Permission is hereby granted, free of charge, to any person or organization
obtaining a copy of the software and accompanying documentation covered by
this license (the ""Software"") to use, reproduce, display, distribute,
execute, and transmit the Software, and to permit third-parties to whom the 
Software is furnished to do so, all subject to the following:

The copyright notices in the Software and this entire statement, including
the above license grant, this restriction and the following disclaimer,
must be included in all copies of the Software, in whole or in part.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.";

    }
}
