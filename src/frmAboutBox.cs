using System;
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
        public frmAboutBox()
        {
            InitializeComponent();
            this.Text = labelProductName.Text = $"KeizerForClubs v{AssemblyVersion}";
            labelCopyright.Text = GetCopyright();

            linkLabel1.Text = @"Originally written by T. Schlapp. He's got a whole website dedicated to the Keizer system, 
including a nice example tournament. Improved and put to Github by me. Contact me:
Dumuzy@Github aka Alakaluf@Lichess for questions, suggestions or bug reports.";
            AddLinkLabel(linkLabel1, "example tournament", "http://keizer.schlapp.name/index.php?id=beispielturnier");
            AddLinkLabel(linkLabel1, "Github", "https://github.com/Dumuzy/KeizerForClubs");
            AddLinkLabel(linkLabel1, "Dumuzy@Github", "https://github.com/Dumuzy");
            AddLinkLabel(linkLabel1, "Alakaluf@Lichess", "https://lichess.org/@/Alakaluf");

            linkLabel2.Text = @"If you like this software... I would be delighted if you'd buy me a coffee! :-)
I am   keizer@atlantis44.de   at Paypal.";
            AddLinkLabel(linkLabel2, "Paypal", "https://www.paypal.com");
        }

        void AddLinkLabel(LinkLabel ll, string linkText, string link, int delta = 0)
        {
            // In the LinkLabel it seems \r\n is counted as one char. 
            var lt = ll.Text.Replace("\r", ""); 
            var idx = lt.IndexOf(linkText);
            if (idx != -1)
                ll.Links.Add(idx - delta, linkText.Length, link);
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
