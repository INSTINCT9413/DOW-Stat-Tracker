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
using System.Net.Http;

namespace DOW_Stat_Tracker
{
    public partial class Form3 : Form
    {
        string updater = Application.StartupPath + "updater.exe";
        public Form3()
        {
            InitializeComponent();
            label4.Text = "Installed Version: " + Application.ProductVersion;
        }
        public static class AppInfo
        {
            public static string CurrentVersion = Application.ProductVersion; // update per release
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

        private async void button1_Click(object sender, EventArgs e)
        {
            await DownloadExtractor();
            await CheckForUpdates();
        }
        private async Task DownloadExtractor()
        {
            string url = "https://github.com/INSTINCT9413/DOW-Stat-Tracker/raw/master/INSTINCT%20Extractor.exe"; // URL of your EXE
            string tempFile = Path.Combine(Application.StartupPath, "INSTINCT Extractor.exe");

            //Console.WriteLine("Downloading updater...");

            using (HttpClient client = new HttpClient())
            {
                var data = await client.GetByteArrayAsync(url);
                File.WriteAllBytes(tempFile, data);
            }

            //Console.WriteLine("Download complete. Launching updater...");

            // Launch the downloaded EXE
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true // ensures it opens like a normal app
            };

            //Process.Start(psi);

            //Console.WriteLine("Updater launched. Exiting main program.");

        }
        private async Task CheckForUpdates()
        {
            string updateUrl = "https://raw.githubusercontent.com/INSTINCT9413/DOW-Stat-Tracker/master/updates.txt";

            using (HttpClient client = new HttpClient())
            {
                string content = await client.GetStringAsync(updateUrl);

                var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(line => line.Split('='))
                                   .Where(parts => parts.Length == 2) // ✅ ignore bad lines
                                   .ToDictionary(parts => parts[0].Trim().ToLower(), parts => parts[1].Trim());

                // ✅ Safe lookups
                if (!lines.TryGetValue("version", out string latestVersion) ||
                    !lines.TryGetValue("changelog", out string changelog) ||
                    !lines.TryGetValue("url", out string downloadUrl))
                {
                    MessageBox.Show("Update file is missing required fields.", "Update Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                changelog = changelog.Replace(";", Environment.NewLine);

                if (latestVersion != AppInfo.CurrentVersion)
                {
                    DialogResult result = MessageBox.Show(
                        $"A new version {latestVersion} is available!\n\nChangelog:\n{changelog}\n\nDo you want to download it now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        Properties.Settings.Default.UpgradeRequired = true;
                        Properties.Settings.Default.Save();
                        await DownloadAndUpdate(downloadUrl, latestVersion);
                    }
                }
                else
                {
                    MessageBox.Show("You are running the latest version.", "No Updates",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async Task DownloadAndUpdate(string downloadUrl, string newVersion)
        {
            // Temp zip location
            string tempFile = Path.Combine(Application.StartupPath, $"update.zip");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            MessageBox.Show($"Update {newVersion} has been downloaded to:\n{tempFile}\n\nThe updater will now launch.",
                "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Path to your extractor app
            string extractorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "INSTINCT Extractor.exe");

            if (File.Exists(extractorPath))
            {
                // Launch extractor and pass the downloaded zip as an argument
                Process.Start(new ProcessStartInfo
                {
                    FileName = extractorPath,
                    Arguments = $"\"{tempFile}\"", // Pass zip path
                    UseShellExecute = true
                });

                // Exit the current app so the extractor can replace files
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Extractor app not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\License.txt")) 
            { 
                Process.Start(Application.StartupPath + @"\License.txt");
            }
            else
            {
                MessageBox.Show("License file not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}