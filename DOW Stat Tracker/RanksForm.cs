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
    public partial class RanksForm : Form
    {
        public RanksForm()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            this.Icon = Properties.Resources.info1;
        }
    }
}
