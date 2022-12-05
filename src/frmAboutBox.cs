using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeizerForClubs;

namespace KeizerForClubs
{
    partial class frmAboutBox : Form
    {
        public frmAboutBox()
        {
            InitializeComponent();
            this.Text = labelProductName.Text = $"KeizerForClubs v{AssemblyVersion}";
            labelCopyright.Text = copyright;

            linkLabel1.Text = @"Originally written by T. Schlapp. Improved and put to Github by me. Contact me:
Dumuzy@Github aka Alakaluf@Lichess for questions, suggestions or bug reports.";
            AddLinkLabel(linkLabel1, "T. Schlapp", "http://keizer.schlapp.name/");
            AddLinkLabel(linkLabel1, "Github", "https://github.com/Dumuzy/KeizerForClubs");
            AddLinkLabel(linkLabel1, "Dumuzy@Github", "https://github.com/Dumuzy", 1);
            AddLinkLabel(linkLabel1, "Alakaluf@Lichess", "https://lichess.org/@/Alakaluf", 1);
        }

        void AddLinkLabel(LinkLabel ll, string linkText, string link, int delta = 0)
        {
            var idx = ll.Text.IndexOf(linkText);
            if (idx != -1)
                ll.Links.Add(idx - delta, linkText.Length, link);
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
