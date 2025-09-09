using DOW_Stat_Tracker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Drawing;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace DOW_Stat_Tracker
{
    public partial class Form3 : Form
    {
        string updater = Application.StartupPath + "updater.exe";
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            this.Icon = Properties.Resources.gear1;
            if (Properties.Settings.Default.AutoRefresh == true)
            {
                numericUpDown1.Value = Properties.Settings.Default.AutoRefreshTime;
                checkBox1.Checked = true;
            }
            else if (Properties.Settings.Default.AutoRefresh == false)
            {
                numericUpDown1.Value = Properties.Settings.Default.AutoRefreshTime;
                checkBox1.Checked = false;
            }
            if (File.Exists(updater))
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://steamcommunity.com/id/INSTINCTxTV/");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                numericUpDown1.Enabled = true;
                var settings = Properties.Settings.Default;
                typeof(Settings).GetProperty("AutoRefresh")?.SetValue(settings, true);
                typeof(Settings).GetProperty("AutoRefreshTime")?.SetValue(settings, (short)numericUpDown1.Value);
                settings.Save();
            }
            else
            {
                numericUpDown1.Enabled = false;
                var settings = Properties.Settings.Default;
                typeof(Settings).GetProperty("AutoRefresh")?.SetValue(settings, false);
                typeof(Settings).GetProperty("AutoRefreshTime")?.SetValue(settings, (short)numericUpDown1.Value);
                settings.Save();
            }

        }
        private void OpenSettingsFolder()
        {
            //string folderPath = Application.UserAppDataPath;
            string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

            if (File.Exists(configPath))
            {
                Process.Start("explorer.exe", $"/select,\"{configPath}\"");
            }
            else
            {
                MessageBox.Show("Settings folder not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            OpenSettingsFolder();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (File.Exists(updater))
            {
                button1.Enabled = true;
                Process.Start(updater);
            }
            else
            {
                button1.Enabled = false;
            }
        }
    }
}