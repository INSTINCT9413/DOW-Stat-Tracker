using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOW_Stat_Tracker
{
    public partial class ChangelogForm : Form
    {
        public ChangelogForm()
        {
            InitializeComponent();
        }
        public void SetChangelogText(string text)
        {
            richTextBox1.Text = text;
        }
        private void ChangelogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
