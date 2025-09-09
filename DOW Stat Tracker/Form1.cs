
using DOW_Stat_Tracker.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DOW_Stat_Tracker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Classes that match the JSON structure
        public class Root
        {
            public int code { get; set; }
            public string message { get; set; }
            public List<MatchHistoryStat> matchHistoryStats { get; set; }
        }
        public class PlayerRank
        {
            public int Level { get; set; }
            public int MinXP { get; set; }
            public string RankName { get; set; }
            public Image RankIcon { get; set; }
        }
        public class PersonalRoot
        {
            public int code { get; set; }
            public string message { get; set; }
            public List<StatGroup> statGroups { get; set; }
            public List<LeaderboardStat> leaderboardStats { get; set; }
        }

        public class StatGroup
        {
            public int id { get; set; }
            public string name { get; set; }
            public int type { get; set; }
            public List<Member> members { get; set; }

        }

        public class Member
        {
            public int profile_id { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
            public int personal_statgroup_id { get; set; }
            public int xp { get; set; }
            public int level { get; set; }
            public int leaderboardregion_id { get; set; }
            public string country { get; set; }
        }

        public class LeaderboardStat
        {
            public int statgroup_id { get; set; }
            public int leaderboard_id { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int streak { get; set; }
            public int disputes { get; set; }
            public int drops { get; set; }
            public int rank { get; set; }
            public int ranktotal { get; set; }
            public int ranklevel { get; set; }
            public int regionrank { get; set; }
            public int regionranktotal { get; set; }
            public long lastmatchdate { get; set; }
            public int highestrank { get; set; }
            public int highestranklevel { get; set; }
        }

        public class MatchHistoryStat
        {
            public int id { get; set; }
            public int creator_profile_id { get; set; }
            public string mapname { get; set; }
            public int maxplayers { get; set; }
            public int matchtype_id { get; set; }
            public string options { get; set; }
            public string slotinfo { get; set; }
            public string description { get; set; }
            public long startgametime { get; set; }
            public long completiontime { get; set; }
            public int observertotal { get; set; }
            public List<MatchHistoryMember> matchhistorymember { get; set; }
        }

        public class MatchHistoryMember
        {
            public int matchhistory_id { get; set; }
            public int profile_id { get; set; }
            public int race_id { get; set; }
            public int statgroup_id { get; set; }
            public int teamid { get; set; }
            public int wins { get; set; }
            public int losses { get; set; }
            public int streak { get; set; }
            public int arbitration { get; set; }
            public int outcome { get; set; } // 1 = win, 0 = loss
            public int oldrating { get; set; }
            public int newrating { get; set; }
            public int reporttype { get; set; }
        }

        // Dictionary for race names
        public static readonly Dictionary<int, string> Races = new Dictionary<int, string>
        {
    { 0, "Chaos" },
    { 1, "Dark Eldar" },
    { 2, "Eldar" },
    { 3, "Imperial Guard" },
    { 4, "Necrons" },
    { 5, "Orks" },
    { 6, "Sisters of Battle" },
    { 7, "Space Marines" },
    { 8, "Tau" }
        };
        public static readonly Dictionary<int, string> Leaderboards = new Dictionary<int, string>
{
    { 1, "Chaos" },
    { 2, "Dark Eldar" },
    { 3, "Eldar" },
    { 4, "Imperial Guard" },
    { 5, "Necrons" },
    { 6, "Orks" },
    { 7, "Sisters of Battle" },
    { 8, "Space Marines" },
    { 9, "Tau" },
    { 14, "2v2 Ranked" },
    { 18, "3v3 Ranked" }
};

        private Root data;
        private int profileId = 11019565; // TODO: replace with your profile ID
        private string steamID = "";

        private async void btnLoadJson_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            try
            {
                string username = txtUsername.Text.Trim();
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Please enter a username first.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Save username in settings
                var settings = Properties.Settings.Default;
                typeof(Settings).GetProperty("LastUsername")?.SetValue(settings, username);
                settings.Save();
                await LoadPersonalStats(username);
                string getRecentMatches = $"https://dow-api.reliclink.com/community/leaderboard/getRecentMatchHistory?title=dow1-de&aliases=[{username}]";
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(getRecentMatches);
                    data = JsonConvert.DeserializeObject<Root>(json);

                    if (data == null || data.matchHistoryStats == null || !data.matchHistoryStats.Any())
                    {
                        MessageBox.Show("No data found for this player.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // ✅ Only take automatched games
                    var automatches = data.matchHistoryStats
                        .Where(m => !string.IsNullOrEmpty(m.description) &&
                m.description.ToUpper().Contains("AUTOMATCH"))
                        .ToList();

                    if (!automatches.Any())
                    {
                        MessageBox.Show("No automatched games found for this player.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Flatten all match members
                    var players = automatches
                        .SelectMany(m => m.matchhistorymember)
                        .Select(p => new
                        {
                            p.profile_id,
                            Race = Races.ContainsKey(p.race_id) ? Races[p.race_id] : $"Unknown ({p.race_id})",
                            p.teamid,
                            p.wins,
                            p.losses,
                            Outcome = p.outcome == 1 ? "Win" : "Loss",
                            p.newrating,
                            p.oldrating
                        })
                        .ToList();

                    dgvProfiles.DataSource = players;
                    label124.Text = $"Found {dgvProfiles.RowCount} games that were automatched";
                    label125.Text = $"Breakdown of {dgvProfiles.RowCount} games played";
                    // Compute race breakdown
                    var raceBreakdown = players
                        .GroupBy(p => p.Race)
                        .Select(g => new
                        {
                            Race = g.Key,
                            Wins = g.Count(x => x.Outcome == "Win"),
                            Losses = g.Count(x => x.Outcome == "Loss"),
                            WinRate = g.Any() ? (g.Count(x => x.Outcome == "Win") * 100.0 / g.Count()).ToString("F1") + "%" : "0%"
                        })
                        .OrderByDescending(x => x.Wins + x.Losses)
                        .ToList();

                    dgvRaces.DataSource = raceBreakdown;

                    // Build per-race stats only from automatches
                    var raceStats = new Dictionary<int, (int wins, int losses)>();
                    foreach (var match in automatches)
                    {
                        foreach (var member in match.matchhistorymember)
                        {
                            if (member.profile_id == profileId)
                            {
                                if (!raceStats.ContainsKey(member.race_id))
                                    raceStats[member.race_id] = (0, 0);

                                if (member.outcome == 1)
                                    raceStats[member.race_id] = (raceStats[member.race_id].wins + 1, raceStats[member.race_id].losses);
                                else
                                    raceStats[member.race_id] = (raceStats[member.race_id].wins, raceStats[member.race_id].losses + 1);
                            }
                        }
                    }

                    var raceResults = raceStats.Select(r => new
                    {
                        Race = Races.ContainsKey(r.Key) ? Races[r.Key] : $"Unknown ({r.Key})",
                        Wins = r.Value.wins,
                        Losses = r.Value.losses,
                        WinRate = (r.Value.wins + r.Value.losses) > 0
                            ? (r.Value.wins * 100.0 / (r.Value.wins + r.Value.losses)).ToString("F1") + "%"
                            : "0%"
                    }).ToList();

                    dataGridView1.DataSource = raceResults;

                    // Profile summary (only automatch totals)
                    int totalWins = raceStats.Sum(r => r.Value.wins);
                    int totalLosses = raceStats.Sum(r => r.Value.losses);
                    double overallWR = (totalWins + totalLosses) > 0 ? totalWins * 100.0 / (totalWins + totalLosses) : 0;
                    Username.Text = "Alias: " + txtUsername.Text.Trim() + "\n\rID: " + profileId;
                    //Wins.Text = "Wins: " + totalWins.ToString();
                    //Losses.Text = "Losses: " + totalLosses.ToString();
                    //Overall.Text = "Winrate: " + overallWR.ToString("F1") + "%";
                }

                pictureBox1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            PopulateMainTab();
        }
        public string GetRankName(int xp)
        {
            // Return the highest rank where MinXP <= player's XP
            var rank = Ranks.Where(r => xp >= r.MinXP)
                            .OrderByDescending(r => r.MinXP)
                            .FirstOrDefault();
            return rank != null ? rank.RankName : "Unranked";
        }

        List<PlayerRank> Ranks = new List<PlayerRank>
{
    new PlayerRank { Level = 1, MinXP = 0, RankName = "Recruit", RankIcon = Properties.Resources.Recruit },
    new PlayerRank { Level = 2, MinXP = 10000, RankName = "Scout", RankIcon = Properties.Resources.Scout },
    new PlayerRank { Level = 3, MinXP = 50000, RankName = "Veteran Scout", RankIcon = Properties.Resources.VeteranScout },
    new PlayerRank { Level = 4, MinXP = 150000, RankName = "Trooper", RankIcon = Properties.Resources.Trooper },
    new PlayerRank { Level = 5, MinXP = 300000, RankName = "Stormtrooper", RankIcon = Properties.Resources.StormTrooper },
    new PlayerRank { Level = 6, MinXP = 600000, RankName = "Veteran Stormtrooper", RankIcon = Properties.Resources.VeteranStormTrooper },
    new PlayerRank { Level = 7, MinXP = 1000000, RankName = "Sergeant", RankIcon = Properties.Resources.Sergeant },
    new PlayerRank { Level = 8, MinXP = 1500000, RankName = "Lieutenant", RankIcon = Properties.Resources.Lieutenant },
    new PlayerRank { Level = 9, MinXP = 2200000, RankName = "Captain", RankIcon = Properties.Resources.Captain },
    new PlayerRank { Level = 10, MinXP = 3600000, RankName = "Major", RankIcon = Properties.Resources.Major },
    new PlayerRank { Level = 11, MinXP = 4200000, RankName = "Chapter Master", RankIcon = Properties.Resources.Chapter_Master },
    new PlayerRank { Level = 12, MinXP = 6000000, RankName = "Lord Commander", RankIcon = Properties.Resources.LordCommader },
    new PlayerRank { Level = 13, MinXP = 8000000, RankName = "Inquisitor", RankIcon = Properties.Resources.Inquisitor },
    new PlayerRank { Level = 14, MinXP = 9700000, RankName = "Emperor", RankIcon = Properties.Resources.Emperor }
    // add more as you see fit
};
        private async Task LoadPersonalStats(string username)
        {
            try
            {
                string url = $"https://dow-api.reliclink.com/community/leaderboard/getPersonalStat?title=dow1-de&aliases=[{username}]";

                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    var personalData = JsonConvert.DeserializeObject<PersonalRoot>(json);

                    if (personalData == null || personalData.statGroups == null || personalData.statGroups.Count == 0)
                    {
                        MessageBox.Show("No stats found for this player.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Get profile info
                    var profile = personalData.statGroups[0].members.FirstOrDefault();
                    if (profile == null)
                    {
                        MessageBox.Show("Could not find profile info.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // Navigate: Root → statGroups[0] → members[0] → leaderboardStats
                    var stats = personalData?.leaderboardStats;

                    // Fix for the error CS1061: Replace 'stats.leaderboardStat' with 'stats' as 'leaderboardStat' is not a property of List<Form1.LeaderboardStat>.
                    int totalWins = stats.Sum(s => s.wins);
                    int totalLosses = stats.Sum(s => s.losses);

                    // Extract specific leaderboard stats
                    var spaceMarines = stats.FirstOrDefault(s => s.leaderboard_id == 8);
                    var chaos = stats.FirstOrDefault(s => s.leaderboard_id == 1);
                    var eldar = stats.FirstOrDefault(s => s.leaderboard_id == 3);
                    var orks = stats.FirstOrDefault(s => s.leaderboard_id == 6);
                    var imperialGuard = stats.FirstOrDefault(s => s.leaderboard_id == 4);
                    var necrons = stats.FirstOrDefault(s => s.leaderboard_id == 5);
                    var sisters = stats.FirstOrDefault(s => s.leaderboard_id == 7);
                    var darkEldar = stats.FirstOrDefault(s => s.leaderboard_id == 2);
                    var tau = stats.FirstOrDefault(s => s.leaderboard_id == 9);

                    //Chaos Stats
                    label1.Text = chaos != null ? $"Total Wins: {chaos.wins}" : "Total Wins: N/A";
                    label2.Text = chaos != null ? $"Total Losses: {chaos.losses}" : "Total Losses: N/A";
                    label3.Text = chaos != null ? $"Current Streak: {chaos.streak}" : "Current Streak: N/A";
                    label4.Text = chaos != null ? $"Disputes: {chaos.disputes}" : "Disputes: N/A";
                    label5.Text = chaos != null ? $"Drops: {chaos.drops}" : "Drops: N/A";
                    label6.Text = chaos != null ? $"Rank: {chaos.rank}" : "Rank: N/A";
                    label7.Text = chaos != null ? $"Rank Total: {chaos.ranktotal}" : "Rank Total: N/A";
                    label8.Text = chaos != null ? $"Rank Level: {chaos.ranklevel}" : "Rank Level: N/A";
                    label9.Text = chaos != null ? $"Region Rank: {chaos.regionrank}" : "Region Rank: N/A";
                    label10.Text = chaos != null ? $"Region Rank Total: {chaos.regionranktotal}" : "Region Rank Total: N/A";
                    label11.Text = chaos != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(chaos.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label12.Text = chaos != null ? $"Highest Rank: {chaos.highestrank}" : "Highest Rank: N/A";
                    label13.Text = chaos != null ? $"Highest Rank Level: {chaos.highestranklevel}" : "Highest Rank Level: N/A";
                    //Dark Eldar Stats
                    label26.Text = darkEldar != null ? $"Total Wins: {darkEldar.wins}" : "Total Wins: N/A";
                    label25.Text = darkEldar != null ? $"Total Losses: {darkEldar.losses}" : "Total Losses: N/A";
                    label24.Text = darkEldar != null ? $"Current Streak: {darkEldar.streak}" : "Current Streak: N/A";
                    label23.Text = darkEldar != null ? $"Disputes: {darkEldar.disputes}" : "Disputes: N/A";
                    label22.Text = darkEldar != null ? $"Drops: {darkEldar.drops}" : "Drops: N/A";
                    label21.Text = darkEldar != null ? $"Rank: {darkEldar.rank}" : "Rank: N/A";
                    label20.Text = darkEldar != null ? $"Rank Total: {darkEldar.ranktotal}" : "Rank Total: N/A";
                    label19.Text = darkEldar != null ? $"Rank Level: {darkEldar.ranklevel}" : "Rank Level: N/A";
                    label18.Text = darkEldar != null ? $"Region Rank: {darkEldar.regionrank}" : "Region Rank: N/A";
                    label17.Text = darkEldar != null ? $"Region Rank Total: {darkEldar.regionranktotal}" : "Region Rank Total: N/A";
                    label16.Text = darkEldar != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(darkEldar.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label15.Text = darkEldar != null ? $"Highest Rank: {darkEldar.highestrank}" : "Highest Rank: N/A";
                    label14.Text = darkEldar != null ? $"Highest Rank Level: {darkEldar.highestranklevel}" : "Highest Rank Level: N/A";

                    //Eldar Stats
                    label39.Text = eldar != null ? $"Total Wins: {eldar.wins}" : "Total Wins: N/A";
                    label38.Text = eldar != null ? $"Total Losses: {eldar.losses}" : "Total Losses: N/A";
                    label37.Text = eldar != null ? $"Current Streak: {eldar.streak}" : "Current Streak: N/A";
                    label36.Text = eldar != null ? $"Disputes: {eldar.disputes}" : "Disputes: N/A";
                    label35.Text = eldar != null ? $"Drops: {eldar.drops}" : "Drops: N/A";
                    label34.Text = eldar != null ? $"Rank: {eldar.rank}" : "Rank: N/A";
                    label33.Text = eldar != null ? $"Rank Total: {eldar.ranktotal}" : "Rank Total: N/A";
                    label32.Text = eldar != null ? $"Rank Level: {eldar.ranklevel}" : "Rank Level: N/A";
                    label31.Text = eldar != null ? $"Region Rank: {eldar.regionrank}" : "Region Rank: N/A";
                    label30.Text = eldar != null ? $"Region Rank Total: {eldar.regionranktotal}" : "Region Rank Total: N/A";
                    label29.Text = eldar != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(eldar.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label28.Text = eldar != null ? $"Highest Rank: {eldar.highestrank}" : "Highest Rank: N/A";
                    label27.Text = eldar != null ? $"Highest Rank Level: {eldar.highestranklevel}" : "Highest Rank Level: N/A";

                    //Imperial Guard Stats
                    label52.Text = imperialGuard != null ? $"Total Wins: {imperialGuard.wins}" : "Total Wins: N/A";
                    label51.Text = imperialGuard != null ? $"Total Losses: {imperialGuard.losses}" : "Total Losses: N/A";
                    label50.Text = imperialGuard != null ? $"Current Streak: {imperialGuard.streak}" : "Current Streak: N/A";
                    label49.Text = imperialGuard != null ? $"Disputes: {imperialGuard.disputes}" : "Disputes: N/A";
                    label48.Text = imperialGuard != null ? $"Drops: {imperialGuard.drops}" : "Drops: N/A";
                    label47.Text = imperialGuard != null ? $"Rank: {imperialGuard.rank}" : "Rank: N/A";
                    label46.Text = imperialGuard != null ? $"Rank Total: {imperialGuard.ranktotal}" : "Rank Total: N/A";
                    label45.Text = imperialGuard != null ? $"Rank Level: {imperialGuard.ranklevel}" : "Rank Level: N/A";
                    label44.Text = imperialGuard != null ? $"Region Rank: {imperialGuard.regionrank}" : "Region Rank: N/A";
                    label43.Text = imperialGuard != null ? $"Region Rank Total: {imperialGuard.regionranktotal}" : "Region Rank Total: N/A";
                    label42.Text = imperialGuard != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(imperialGuard.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label41.Text = eldar != null ? $"Highest Rank: {imperialGuard.highestrank}" : "Highest Rank: N/A";
                    label40.Text = imperialGuard != null ? $"Highest Rank Level: {imperialGuard.highestranklevel}" : "Highest Rank Level: N/A";

                    //Necrons Stats
                    label65.Text = necrons != null ? $"Total Wins: {necrons.wins}" : "Total Wins: N/A";
                    label64.Text = necrons != null ? $"Total Losses: {necrons.losses}" : "Total Losses: N/A";
                    label63.Text = necrons != null ? $"Current Streak: {necrons.streak}" : "Current Streak: N/A";
                    label62.Text = necrons != null ? $"Disputes: {necrons.disputes}" : "Disputes: N/A";
                    label61.Text = necrons != null ? $"Drops: {necrons.drops}" : "Drops: N/A";
                    label60.Text = necrons != null ? $"Rank: {necrons.rank}" : "Rank: N/A";
                    label59.Text = necrons != null ? $"Rank Total: {necrons.ranktotal}" : "Rank Total: N/A";
                    label58.Text = necrons != null ? $"Rank Level: {necrons.ranklevel}" : "Rank Level: N/A";
                    label57.Text = necrons != null ? $"Region Rank: {necrons.regionrank}" : "Region Rank: N/A";
                    label56.Text = necrons != null ? $"Region Rank Total: {necrons.regionranktotal}" : "Region Rank Total: N/A";
                    label55.Text = necrons != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(necrons.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label54.Text = necrons != null ? $"Highest Rank: {necrons.highestrank}" : "Highest Rank: N/A";
                    label53.Text = necrons != null ? $"Highest Rank Level: {necrons.highestranklevel}" : "Highest Rank Level: N/A";

                    //Orks Stats
                    label78.Text = orks != null ? $"Total Wins: {orks.wins}" : "Total Wins: N/A";
                    label77.Text = orks != null ? $"Total Losses: {orks.losses}" : "Total Losses: N/A";
                    label76.Text = orks != null ? $"Current Streak: {orks.streak}" : "Current Streak: N/A";
                    label75.Text = orks != null ? $"Disputes: {orks.disputes}" : "Disputes: N/A";
                    label74.Text = orks != null ? $"Drops: {orks.drops}" : "Drops: N/A";
                    label73.Text = orks != null ? $"Rank: {orks.rank}" : "Rank: N/A";
                    label72.Text = orks != null ? $"Rank Total: {orks.ranktotal}" : "Rank Total: N/A";
                    label71.Text = orks != null ? $"Rank Level: {orks.ranklevel}" : "Rank Level: N/A";
                    label70.Text = orks != null ? $"Region Rank: {orks.regionrank}" : "Region Rank: N/A";
                    label69.Text = orks != null ? $"Region Rank Total: {orks.regionranktotal}" : "Region Rank Total: N/A";
                    label68.Text = orks != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(orks.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label67.Text = orks != null ? $"Highest Rank: {orks.highestrank}" : "Highest Rank: N/A";
                    label66.Text = orks != null ? $"Highest Rank Level: {orks.highestranklevel}" : "Highest Rank Level: N/A";

                    //Sisters of Battle Stats
                    label91.Text = sisters != null ? $"Total Wins: {sisters.wins}" : "Total Wins: N/A";
                    label90.Text = sisters != null ? $"Total Losses: {sisters.losses}" : "Total Losses: N/A";
                    label89.Text = sisters != null ? $"Current Streak: {sisters.streak}" : "Current Streak: N/A";
                    label88.Text = sisters != null ? $"Disputes: {sisters.disputes}" : "Disputes: N/A";
                    label87.Text = sisters != null ? $"Drops: {sisters.drops}" : "Drops: N/A";
                    label86.Text = sisters != null ? $"Rank: {sisters.rank}" : "Rank: N/A";
                    label85.Text = sisters != null ? $"Rank Total: {sisters.ranktotal}" : "Rank Total: N/A";
                    label84.Text = sisters != null ? $"Rank Level: {sisters.ranklevel}" : "Rank Level: N/A";
                    label83.Text = sisters != null ? $"Region Rank: {sisters.regionrank}" : "Region Rank: N/A";
                    label82.Text = sisters != null ? $"Region Rank Total: {sisters.regionranktotal}" : "Region Rank Total: N/A";
                    label81.Text = sisters != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(sisters.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label80.Text = sisters != null ? $"Highest Rank: {sisters.highestrank}" : "Highest Rank: N/A";
                    label79.Text = sisters != null ? $"Highest Rank Level: {sisters.highestranklevel}" : "Highest Rank Level: N/A";

                    //Space Marines Stats
                    label104.Text = spaceMarines != null ? $"Total Wins: {spaceMarines.wins}" : "Total Wins: N/A";
                    label103.Text = spaceMarines != null ? $"Total Losses: {spaceMarines.losses}" : "Total Losses: N/A";
                    label102.Text = spaceMarines != null ? $"Current Streak: {spaceMarines.streak}" : "Current Streak: N/A";
                    label101.Text = spaceMarines != null ? $"Disputes: {spaceMarines.disputes}" : "Disputes: N/A";
                    label100.Text = spaceMarines != null ? $"Drops: {spaceMarines.drops}" : "Drops: N/A";
                    label99.Text = spaceMarines != null ? $"Rank: {spaceMarines.rank}" : "Rank: N/A";
                    label98.Text = spaceMarines != null ? $"Rank Total: {spaceMarines.ranktotal}" : "Rank Total: N/A";
                    label97.Text = spaceMarines != null ? $"Rank Level: {spaceMarines.ranklevel}" : "Rank Level: N/A";
                    label96.Text = spaceMarines != null ? $"Region Rank: {spaceMarines.regionrank}" : "Region Rank: N/A";
                    label95.Text = spaceMarines != null ? $"Region Rank Total: {spaceMarines.regionranktotal}" : "Region Rank Total: N/A";
                    label94.Text = spaceMarines != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(spaceMarines.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label93.Text = spaceMarines != null ? $"Highest Rank: {spaceMarines.highestrank}" : "Highest Rank: N/A";
                    label92.Text = spaceMarines != null ? $"Highest Rank Level: {spaceMarines.highestranklevel}" : "Highest Rank Level: N/A";

                    //Tau Stats
                    label117.Text = tau != null ? $"Total Wins: {tau.wins}" : "Total Wins: N/A";
                    label116.Text = tau != null ? $"Total Losses: {tau.losses}" : "Total Losses: N/A";
                    label115.Text = tau != null ? $"Current Streak: {tau.streak}" : "Current Streak: N/A";
                    label114.Text = tau != null ? $"Disputes: {tau.disputes}" : "Disputes: N/A";
                    label113.Text = tau != null ? $"Drops: {tau.drops}" : "Drops: N/A";
                    label112.Text = tau != null ? $"Rank: {tau.rank}" : "Rank: N/A";
                    label111.Text = tau != null ? $"Rank Total: {tau.ranktotal}" : "Rank Total: N/A";
                    label110.Text = tau != null ? $"Rank Level: {tau.ranklevel}" : "Rank Level: N/A";
                    label109.Text = tau != null ? $"Region Rank: {tau.regionrank}" : "Region Rank: N/A";
                    label108.Text = tau != null ? $"Region Rank Total: {tau.regionranktotal}" : "Region Rank Total: N/A";
                    label107.Text = tau != null
    ? $"Last Match: {DateTimeOffset.FromUnixTimeSeconds(tau.lastmatchdate).LocalDateTime:g}"
    : "Last Match: N/A";

                    label106.Text = tau != null ? $"Highest Rank: {tau.highestrank}" : "Highest Rank: N/A";
                    label105.Text = tau != null ? $"Highest Rank Level: {tau.highestranklevel}" : "Highest Rank Level: N/A";
                    int totalGames = totalWins + totalLosses;
                    // Example: set to labels
                    label129.Text = $"Total Games: {totalGames}";
                    Wins.Text = $"Total Wins: {totalWins}";
                    Losses.Text = $"Total Losses: {totalLosses}";
                    Overall.Text = totalWins + totalLosses > 0
                        ? $"Overall Winrate: {(totalWins * 100.0 / (totalWins + totalLosses)):F1}%"
                        : "Overall Winrate: N/A";
                    profileId = profile.profile_id; // save globally
                    Username.Text = $"Player: {profile.alias} (ID: {profileId})";

                    label119.Text = $"Country: " + (string.IsNullOrEmpty(profile.country.ToUpper()) ? "N/A" : profile.country.ToUpper());
                    label120.Text = $"Level: {profile.level}";
                    label121.Text = $"XP: {profile.xp.ToString()}";
                    int playerXP = profile.xp;
                    string rankName = GetRankName(playerXP);
                    label122.Text = $"Rank: {rankName}";
                    // Fix for CS1061: Replace 'rankName.RankIcon' with 'rank.RankIcon' as 'rankName' is a string and does not have a 'RankIcon' property.  
                    var rank = Ranks.FirstOrDefault(r => r.RankName == rankName);
                    if (rank != null)
                    {
                        pictureBox12.Image = rank.RankIcon;
                    }
                    else
                    {
                        pictureBox12.Image = null; // Or set a default/fallback image if needed  
                    }

                    var nextRank = Ranks.Where(r => r.MinXP > playerXP).OrderBy(r => r.MinXP).FirstOrDefault();
                    if (nextRank != null)
                    {
                        double progress = (double)(playerXP - Ranks.Where(r => r.MinXP <= playerXP).Max(r => r.MinXP)) /
                                          (nextRank.MinXP - Ranks.Where(r => r.MinXP <= playerXP).Max(r => r.MinXP));
                        label123.Text = $"Next Rank: {nextRank.RankName} (at {playerXP} of {nextRank.MinXP} XP)";
                        progressBar1.Value = (int)(progress * 100);
                    }


                    // 🔹 Extract SteamID64
                    string steamId = profile.name.Replace("/steam/", "").Trim();
                    steamID = steamId.ToString();
                    // 🔹 Fetch Steam profile page
                    string profileUrl = $"https://steamcommunity.com/profiles/{steamId}/?xml=1";
                    string xml = await client.GetStringAsync(profileUrl);

                    // 🔹 Regex for avatar URL
                    var match = XDocument.Parse(xml);

                    string avatarUrl = match.Descendants("avatarFull").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(avatarUrl))
                    {


                        try
                        {
                            using (var avatarStream = await client.GetStreamAsync(avatarUrl))
                            {
                                pictureBox11.Image = Image.FromStream(avatarStream);
                            }
                        }
                        catch
                        {
                            pictureBox11.Image = Properties.Resources.DoWSSTauEmpireHouseMark; // fallback image in resources
                        }
                    }
                    else
                    {
                        pictureBox11.Image = Properties.Resources.DoWSSTauEmpireHouseMark; // fallback image if regex fails
                    }
                }
            }
            // 🔹 Populate leaderboard stats


            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching stats: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //pictureBox11.Image = Properties.Resources.DoWSSTauEmpireHouseMark; // global fallback
            }
        }



        private void btnShowProfile_Click(object sender, EventArgs e)
        {

        }

        private void btnShowProfile_Click_1(object sender, EventArgs e)
        {
            if (data == null)
            {
                MessageBox.Show("Please load a JSON file first.");
                return;
            }

            var raceStats = new Dictionary<int, (int wins, int losses)>();

            foreach (var match in data.matchHistoryStats)
            {
                foreach (var member in match.matchhistorymember)
                {
                    if (member.profile_id == profileId)
                    {
                        if (!raceStats.ContainsKey(member.race_id))
                            raceStats[member.race_id] = (0, 0);

                        if (member.outcome == 1)
                            raceStats[member.race_id] = (raceStats[member.race_id].wins + 1, raceStats[member.race_id].losses);
                        else
                            raceStats[member.race_id] = (raceStats[member.race_id].wins, raceStats[member.race_id].losses + 1);
                    }
                }
            }

            var raceResults = raceStats.Select(r => new
            {
                Race = Races.ContainsKey(r.Key) ? Races[r.Key] : $"Unknown ({r.Key})",
                Wins = r.Value.wins,
                Losses = r.Value.losses,
                WinRate = (r.Value.wins + r.Value.losses) > 0
                    ? (r.Value.wins * 100.0 / (r.Value.wins + r.Value.losses)).ToString("F1") + "%"
                    : "0%"
            }).ToList();

            dataGridView1.DataSource = raceResults;

            // Profile summary
            int totalWins = raceStats.Sum(r => r.Value.wins);
            int totalLosses = raceStats.Sum(r => r.Value.losses);
            double overallWR = (totalWins + totalLosses) > 0 ? totalWins * 100.0 / (totalWins + totalLosses) : 0;
            Username.Text = txtUsername.Text.Trim() + " " + profileId;
            MessageBox.Show(
                $"Profile ID: {profileId}\nTotal Wins: {totalWins}\nTotal Losses: {totalLosses}\nWinrate: {overallWR:F1}%",
                "Profile Summary",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void PopulateMainTab()
        {
            if (data == null || data.matchHistoryStats == null) return;

            var allMembers = data.matchHistoryStats.SelectMany(m => m.matchhistorymember).ToList();

            var raceStats = allMembers
                .GroupBy(p => p.race_id)
                .Select(g => new
                {
                    RaceID = g.Key,
                    RaceName = Races.ContainsKey(g.Key) ? Races[g.Key] : $"Unknown ({g.Key})",
                    MatchesPlayed = g.Count(),
                    Wins = g.Count(x => x.outcome == 1),
                    Losses = g.Count(x => x.outcome == 0),
                    WinRate = g.Count() > 0 ? g.Count(x => x.outcome == 1) * 100.0 / g.Count() : 0,
                    AvgRating = g.Any() ? g.Average(x => x.newrating) : 0
                })
                .ToList();

            // Favorite Race by matches played
            var favorite = raceStats.OrderByDescending(x => x.MatchesPlayed).FirstOrDefault();
            lblFavoriteRace.Text = favorite != null ? $"Favorite Race: {favorite.RaceName} ({favorite.MatchesPlayed} recent matches)" : "N/A";

            // Best Race by WinRate
            var bestWinRate = raceStats.OrderByDescending(x => x.WinRate).FirstOrDefault();
            lblBestRaceWinRate.Text = bestWinRate != null ? $"Best Win Rate: {bestWinRate.RaceName} ({bestWinRate.WinRate:F1}%)" : "N/A";

            // Best Race by Avg Rating
            var bestRating = raceStats.OrderByDescending(x => x.AvgRating).FirstOrDefault();
            lblBestRaceRating.Text = bestRating != null ? $"Best AVG Rating: {bestRating.RaceName} ({bestRating.AvgRating:F0})" : "N/A";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "DOW Stat Tracker by INSTINCT";
            this.Icon = Properties.Resources.logo1;
            txtUsername.Text = Properties.Settings.Default.LastUsername;
            if (Properties.Settings.Default.AutoRefresh == true)
            {
                short refreshInterval = Properties.Settings.Default.AutoRefreshTime;
                if (refreshInterval > 0)
                {
                    timer1.Interval = refreshInterval * 60 * 1000;
                    timer1.Enabled = true;
                    timer1.Start();
                }
                else
                {
                    timer1.Stop();
                    timer1.Enabled = false;
                }
            }
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                btnLoadJson.PerformClick();
            }
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            Process.Start("https://steamcommunity.com/profiles/" + steamID);
        }
        private void pictureBox11_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(pictureBox11, "Go to Steam Profile" + "\n\r" + "ID: " + steamID);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://steamcommunity.com/profiles/" + steamID);
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Form2 rankSystemInfo;
            rankSystemInfo = new Form2();
            rankSystemInfo.Show();
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            settings.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnLoadJson.PerformClick();
        }

        private void label129_Click(object sender, EventArgs e)
        {

        }
    }
}