
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
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
            public List<StatGroup> statGroups { get; set; }
            public List<LeaderboardStat> leaderboardStats { get; set; }
        }

        public class LeaderboardEntry
        {
            public int Rank { get; set; }
            public string Alias { get; set; }
            public string Country { get; set; }
            public int XP { get; set; }
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
            public int rating { get; set; }
            public int highestrating { get; set; }
        }
        public class LeaderboardPlayer
        {
            public string Alias { get; set; }
            public string Country { get; set; }
            public int XP { get; set; }
            public int Rating { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int APIRank { get; set; }
            public string CurrentRank { get; set; }
            public string NextRank { get; set; }
            public int XPToNextRank { get; set; }
            public int XPToNextPlayer { get; set; }
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
        public class MatchHistoryReportResults
        {
            public int xpgained { get; set; }
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
            public List<MatchHistoryReportResults> matchhistoryreportresults { get; set; }
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
            
            panel4v40.Bounds = this.ClientRectangle;
            panel4v40.Visible = true;
            pictureBox1.Visible = true;

           
            try
            {
                string username = txtUsername.Text.Trim();
                if (string.IsNullOrWhiteSpace(username))
                {
                    panel4v40.Visible = false;
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
                        panel4v40.Visible = false;
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
                        panel4v40.Visible = false;
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
                    label124.Text = $"Found {dgvProfiles.RowCount} games that were automatched (Work in progress)";
                    label125.Text = $"Breakdown of {dgvProfiles.RowCount} games played (Work in progress)";
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
                panel4v40.Visible = false;
                MessageBox.Show($"Error fetching data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            PopulateMainTab();
            panel4v40.Visible = false;
            //panel1.SendToBack();
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
    // Levels 0-10 (×0.5)
    new PlayerRank { Level = 0, MinXP = 0, RankName = "Recruit", RankIcon = Properties.Resources.Recruit },
    new PlayerRank { Level = 1, MinXP = 500, RankName = "Conscript", RankIcon = Properties.Resources.Conscript },
    new PlayerRank { Level = 2, MinXP = 750, RankName = "Guardsman", RankIcon = Properties.Resources.Guardsman },
    new PlayerRank { Level = 3, MinXP = 1125, RankName = "Corporal", RankIcon = Properties.Resources.Corporal },
    new PlayerRank { Level = 4, MinXP = 1687, RankName = "Sergeant", RankIcon = Properties.Resources.Sergeant },
    new PlayerRank { Level = 5, MinXP = 2531, RankName = "Battle Brother", RankIcon = Properties.Resources.BattleBrother },
    new PlayerRank { Level = 6, MinXP = 3796, RankName = "Lieutenant", RankIcon = Properties.Resources.Lieutenant },
    new PlayerRank { Level = 7, MinXP = 5694, RankName = "Tank Commander", RankIcon = Properties.Resources.TankCommander },
    new PlayerRank { Level = 8, MinXP = 8541, RankName = "Captain", RankIcon = Properties.Resources.Captain },
    new PlayerRank { Level = 9, MinXP = 12812, RankName = "Major", RankIcon = Properties.Resources.Major },
    new PlayerRank { Level = 10, MinXP = 19218, RankName = "Colonel", RankIcon = Properties.Resources.Colonel },

    // Levels 11-38 (×1.5 until 20, then perfect exponential)
    new PlayerRank { Level = 11, MinXP = 28827, RankName = "Commissar", RankIcon = Properties.Resources.Commissar },
    new PlayerRank { Level = 12, MinXP = 43241, RankName = "Colonel Commissar", RankIcon = Properties.Resources.ColonelCommissar },
    new PlayerRank { Level = 13, MinXP = 64861, RankName = "Castellan", RankIcon = Properties.Resources.Castellan },
    new PlayerRank { Level = 14, MinXP = 97291, RankName = "Codicier", RankIcon = Properties.Resources.Codicier },
    new PlayerRank { Level = 15, MinXP = 145936, RankName = "Librarian", RankIcon = Properties.Resources.Librarian },
    new PlayerRank { Level = 16, MinXP = 218904, RankName = "Chief Librarian", RankIcon = Properties.Resources.ChiefLibrarian },
    new PlayerRank { Level = 17, MinXP = 328356, RankName = "Chapter Master", RankIcon = Properties.Resources.ChapterMaster },
    new PlayerRank { Level = 18, MinXP = 492534, RankName = "Major General", RankIcon = Properties.Resources.MajorGeneral },
    new PlayerRank { Level = 19, MinXP = 738801, RankName = "Lieutenant General", RankIcon = Properties.Resources.LieutenantGeneral },
    new PlayerRank { Level = 20, MinXP = 1108201, RankName = "Marshal", RankIcon = Properties.Resources.Marshal },

    // Levels 21-38: smooth exponential (r ≈ 1.198)
    new PlayerRank { Level = 21, MinXP = 1328679, RankName = "General", RankIcon = Properties.Resources.General },
    new PlayerRank { Level = 22, MinXP = 1592863, RankName = "Inquisitor", RankIcon = Properties.Resources.Inquisitor },
    new PlayerRank { Level = 23, MinXP = 1909111, RankName = "Inquisitor Lord", RankIcon = Properties.Resources.InquisitorLord },
    new PlayerRank { Level = 24, MinXP = 2288362, RankName = "Master", RankIcon = Properties.Resources.Master },
    new PlayerRank { Level = 25, MinXP = 2740925, RankName = "Grandmaster", RankIcon = Properties.Resources.Grandmaster },
    new PlayerRank { Level = 26, MinXP = 3281084, RankName = "Lord General", RankIcon = Properties.Resources.LordGeneral },
    new PlayerRank { Level = 27, MinXP = 3926263, RankName = "Lord General Militant", RankIcon = Properties.Resources.LordGeneralMilitant },
    new PlayerRank { Level = 28, MinXP = 4704882, RankName = "Warmaster", RankIcon = Properties.Resources.Warmaster },
    new PlayerRank { Level = 29, MinXP = 5633985, RankName = "Lord Commander", RankIcon = Properties.Resources.LordCommander },
    new PlayerRank { Level = 30, MinXP = 6749468, RankName = "Lord Commander Militant", RankIcon = Properties.Resources.LordCommanderMilitant },
    new PlayerRank { Level = 31, MinXP = 8087713, RankName = "Commandant", RankIcon = Properties.Resources.Commandant },
    new PlayerRank { Level = 32, MinXP = 9686716, RankName = "Lord Constable", RankIcon = Properties.Resources.LordConstable },
    new PlayerRank { Level = 33, MinXP = 10601427, RankName = "Captain-General", RankIcon = Properties.Resources.Captain_General },
    new PlayerRank { Level = 34, MinXP = 12899419, RankName = "Chief Commandant", RankIcon = Properties.Resources.ChiefCommandant },
    new PlayerRank { Level = 35, MinXP = 14644580, RankName = "Primarch", RankIcon = Properties.Resources.Primarch },
	
    // Levels 36–38: perfectly smooth exponential to reach 20,000,000
	new PlayerRank { Level = 36, MinXP = 16644580, RankName = "Imperial Regent", RankIcon = Properties.Resources.ImperialRegent },
    new PlayerRank { Level = 37, MinXP = 18716768, RankName = "High Lords of Terra", RankIcon = Properties.Resources.HighLordsofTerra },
    new PlayerRank { Level = 38, MinXP = 20000000, RankName = "Emperor of Mankind", RankIcon = Properties.Resources.EmperorofMankind }

};

        private async Task LoadLeaderboard1v1()
        {
            string url = "https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=1&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                dg1v1.DataSource = withNextPlayer;

                // Nice headers
                dg1v1.Columns["XP"].HeaderText = "Total XP";
                dg1v1.Columns["Rating"].HeaderText = "Rating";
                dg1v1.Columns["Wins"].HeaderText = "Wins";
                dg1v1.Columns["Losses"].HeaderText = "Losses";
                dg1v1.Columns["APIRank"].HeaderText = "API Rank";
                dg1v1.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg1v1.Columns["NextRank"].HeaderText = "Next Rank";
                dg1v1.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg1v1.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }

        private async Task LoadLeaderboard2v2()
        {
            string url = "https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=2&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                dg2v2.DataSource = withNextPlayer;

                // Nice headers
                dg2v2.Columns["XP"].HeaderText = "Total XP";
                dg2v2.Columns["Rating"].HeaderText = "Rating";
                dg2v2.Columns["Wins"].HeaderText = "Wins";
                dg2v2.Columns["Losses"].HeaderText = "Losses";
                dg2v2.Columns["APIRank"].HeaderText = "API Rank";
                dg2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg2v2.Columns["NextRank"].HeaderText = "Next Rank";
                dg2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard3v3()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=3&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                dg3v3.DataSource = withNextPlayer;

                dg3v3.Columns["XP"].HeaderText = "Total XP";
                dg3v3.Columns["Rating"].HeaderText = "Rating";
                dg3v3.Columns["Wins"].HeaderText = "Wins";
                dg3v3.Columns["Losses"].HeaderText = "Losses";
                dg3v3.Columns["APIRank"].HeaderText = "API Rank";
                dg3v3.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg3v3.Columns["NextRank"].HeaderText = "Next Rank";
                dg3v3.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg3v3.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard4v4()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=4&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                dg4v4.DataSource = withNextPlayer;

                dg4v4.Columns["XP"].HeaderText = "Total XP";
                dg4v4.Columns["Rating"].HeaderText = "Rating";
                dg4v4.Columns["Wins"].HeaderText = "Wins";
                dg4v4.Columns["Losses"].HeaderText = "Losses";
                dg4v4.Columns["APIRank"].HeaderText = "API Rank";
                dg4v4.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg4v4.Columns["NextRank"].HeaderText = "Next Rank";
                dg4v4.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg4v4.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard5v5()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=5&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                     .Select((x, index) => new
                     {
                         APIRank = x.APIRank,
                         x.Alias,
                         x.Country,
                         x.Rating,
                         x.Wins,
                         x.Losses,
                         x.XP,
                         XPToNextPlayer = index == 0
             ? 0
             : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                         CurrentRank = x.CurrentRank,
                         NextRank = x.NextRank,
                         XPToNextRank = x.XPToNextRank

                     })
                     .ToList();

                dg5v5.DataSource = withNextPlayer;

                dg5v5.Columns["XP"].HeaderText = "Total XP";
                dg5v5.Columns["Rating"].HeaderText = "Rating";
                dg5v5.Columns["Wins"].HeaderText = "Wins";
                dg5v5.Columns["Losses"].HeaderText = "Losses";
                dg5v5.Columns["APIRank"].HeaderText = "API Rank";
                dg5v5.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg5v5.Columns["NextRank"].HeaderText = "Next Rank";
                dg5v5.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg5v5.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard6v6()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=6&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank
 
                    })
                    .ToList();

                dg6v6.DataSource = withNextPlayer;

                dg6v6.Columns["XP"].HeaderText = "Total XP";
                dg6v6.Columns["Rating"].HeaderText = "Rating";
                dg6v6.Columns["Wins"].HeaderText = "Wins";
                dg6v6.Columns["Losses"].HeaderText = "Losses";
                dg6v6.Columns["APIRank"].HeaderText = "API Rank";
                dg6v6.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg6v6.Columns["NextRank"].HeaderText = "Next Rank";
                dg6v6.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg6v6.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard7v7()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=7&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                     .Select((x, index) => new
                     {
                         APIRank = x.APIRank,
                         x.Alias,
                         x.Country,
                         x.Rating,
                         x.Wins,
                         x.Losses,
                         x.XP,
                         XPToNextPlayer = index == 0
             ? 0
             : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                         CurrentRank = x.CurrentRank,
                         NextRank = x.NextRank,
                         XPToNextRank = x.XPToNextRank

                     })
                     .ToList();

                dg7v7.DataSource = withNextPlayer;

                dg7v7.Columns["XP"].HeaderText = "Total XP";
                dg7v7.Columns["Rating"].HeaderText = "Rating";
                dg7v7.Columns["Wins"].HeaderText = "Wins";
                dg7v7.Columns["Losses"].HeaderText = "Losses";
                dg7v7.Columns["APIRank"].HeaderText = "API Rank";
                dg7v7.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg7v7.Columns["NextRank"].HeaderText = "Next Rank";
                dg7v7.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg7v7.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard8v8()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=8&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                     .Select((x, index) => new
                     {
                         APIRank = x.APIRank,
                         x.Alias,
                         x.Country,
                         x.Rating,
                         x.Wins,
                         x.Losses,
                         x.XP,
                         XPToNextPlayer = index == 0
             ? 0
             : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                         CurrentRank = x.CurrentRank,
                         NextRank = x.NextRank,
                         XPToNextRank = x.XPToNextRank

                     })
                     .ToList();

                dg8v8.DataSource = withNextPlayer;

                dg8v8.Columns["XP"].HeaderText = "Total XP";
                dg8v8.Columns["Rating"].HeaderText = "Rating";
                dg8v8.Columns["Wins"].HeaderText = "Wins";
                dg8v8.Columns["Losses"].HeaderText = "Losses";
                dg8v8.Columns["APIRank"].HeaderText = "API Rank";
                dg8v8.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg8v8.Columns["NextRank"].HeaderText = "Next Rank";
                dg8v8.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg8v8.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard9v9()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=9&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                dg9v9.DataSource = withNextPlayer;

                dg9v9.Columns["XP"].HeaderText = "Total XP";
                dg9v9.Columns["Rating"].HeaderText = "Rating";
                dg9v9.Columns["Wins"].HeaderText = "Wins";
                dg9v9.Columns["Losses"].HeaderText = "Losses";
                dg9v9.Columns["APIRank"].HeaderText = "API Rank";
                dg9v9.Columns["CurrentRank"].HeaderText = "DOW Rank";
                dg9v9.Columns["NextRank"].HeaderText = "Next Rank";
                dg9v9.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                dg9v9.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard10v10()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=10&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                chaos2v2.DataSource = withNextPlayer;

                chaos2v2.Columns["XP"].HeaderText = "Total XP";
                chaos2v2.Columns["Rating"].HeaderText = "Rating";
                chaos2v2.Columns["Wins"].HeaderText = "Wins";
                chaos2v2.Columns["Losses"].HeaderText = "Losses";
                chaos2v2.Columns["APIRank"].HeaderText = "API Rank";
                chaos2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                chaos2v2.Columns["NextRank"].HeaderText = "Next Rank";
                chaos2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                chaos2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard11v11()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=11&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                darkeldar2v2.DataSource = withNextPlayer;

                darkeldar2v2.Columns["XP"].HeaderText = "Total XP";
                darkeldar2v2.Columns["Rating"].HeaderText = "Rating";
                darkeldar2v2.Columns["Wins"].HeaderText = "Wins";
                darkeldar2v2.Columns["Losses"].HeaderText = "Losses";
                darkeldar2v2.Columns["APIRank"].HeaderText = "API Rank";
                darkeldar2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                darkeldar2v2.Columns["NextRank"].HeaderText = "Next Rank";
                darkeldar2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                darkeldar2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard12v12()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=12&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                eldar2v2.DataSource = withNextPlayer;

                eldar2v2.Columns["XP"].HeaderText = "Total XP";
                eldar2v2.Columns["Rating"].HeaderText = "Rating";
                eldar2v2.Columns["Wins"].HeaderText = "Wins";
                eldar2v2.Columns["Losses"].HeaderText = "Losses";
                eldar2v2.Columns["APIRank"].HeaderText = "API Rank";
                eldar2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                eldar2v2.Columns["NextRank"].HeaderText = "Next Rank";
                eldar2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                eldar2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard13v13()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=13&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                imperialguard2v2.DataSource = withNextPlayer;

                imperialguard2v2.Columns["XP"].HeaderText = "Total XP";
                imperialguard2v2.Columns["Rating"].HeaderText = "Rating";
                imperialguard2v2.Columns["Wins"].HeaderText = "Wins";
                imperialguard2v2.Columns["Losses"].HeaderText = "Losses";
                imperialguard2v2.Columns["APIRank"].HeaderText = "API Rank";
                imperialguard2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                imperialguard2v2.Columns["NextRank"].HeaderText = "Next Rank";
                imperialguard2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                imperialguard2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard14v14()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=14&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                necrons2v2.DataSource = withNextPlayer;

                necrons2v2.Columns["XP"].HeaderText = "Total XP";
                necrons2v2.Columns["Rating"].HeaderText = "Rating";
                necrons2v2.Columns["Wins"].HeaderText = "Wins";
                necrons2v2.Columns["Losses"].HeaderText = "Losses";
                necrons2v2.Columns["APIRank"].HeaderText = "API Rank";
                necrons2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                necrons2v2.Columns["NextRank"].HeaderText = "Next Rank";
                necrons2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                necrons2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard15v15()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=15&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                orks2v2.DataSource = withNextPlayer;

                orks2v2.Columns["XP"].HeaderText = "Total XP";
                orks2v2.Columns["Rating"].HeaderText = "Rating";
                orks2v2.Columns["Wins"].HeaderText = "Wins";
                orks2v2.Columns["Losses"].HeaderText = "Losses";
                orks2v2.Columns["APIRank"].HeaderText = "API Rank";
                orks2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                orks2v2.Columns["NextRank"].HeaderText = "Next Rank";
                orks2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                orks2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard16v16()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=16&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                sistersofbattle2v2.DataSource = withNextPlayer;

                sistersofbattle2v2.Columns["XP"].HeaderText = "Total XP";
                sistersofbattle2v2.Columns["Rating"].HeaderText = "Rating";
                sistersofbattle2v2.Columns["Wins"].HeaderText = "Wins";
                sistersofbattle2v2.Columns["Losses"].HeaderText = "Losses";
                sistersofbattle2v2.Columns["APIRank"].HeaderText = "API Rank";
                sistersofbattle2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                sistersofbattle2v2.Columns["NextRank"].HeaderText = "Next Rank";
                sistersofbattle2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                sistersofbattle2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard17v17()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=17&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                spacemarines2v2.DataSource = withNextPlayer;

                spacemarines2v2.Columns["XP"].HeaderText = "Total XP";
                spacemarines2v2.Columns["Rating"].HeaderText = "Rating";
                spacemarines2v2.Columns["Wins"].HeaderText = "Wins";
                spacemarines2v2.Columns["Losses"].HeaderText = "Losses";
                spacemarines2v2.Columns["APIRank"].HeaderText = "API Rank";
                spacemarines2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                spacemarines2v2.Columns["NextRank"].HeaderText = "Next Rank";
                spacemarines2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                spacemarines2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private async Task LoadLeaderboard18v18()
        {
            string url = $"https://dow-api.reliclink.com/community/leaderboard/getleaderboard2?count=200&leaderboard_id=18&start=1&sortBy=1&title=dow1-de";
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(url);

                var data = JsonConvert.DeserializeObject<Root>(json);

                // Build leaderboard with both API stats + WH40k custom rank system
                var leaderboard = data.statGroups
                    .Where(g => g.members != null && g.members.Any())
                    .Select(g =>
                    {
                        var m = g.members.First();
                        var stats = data.leaderboardStats.FirstOrDefault(s => s.statgroup_id == g.id);

                        // Use your GetRankProgress method
                        var progress = GetRankProgress(m.xp);

                        return new
                        {
                            Alias = m.alias,
                            Country = m.country.ToUpper(),
                            XP = m.xp,

                            // API Stats
                            Rating = stats?.rating ?? 0,
                            Wins = stats?.wins ?? 0,
                            Losses = stats?.losses ?? 0,
                            APIRank = stats?.rank ?? 0,

                            // WH40k Rank System
                            CurrentRank = progress.CurrentRank,
                            NextRank = progress.NextRank,
                            XPToNextRank = progress.XPToNextRank
                        };
                    })
                    .OrderByDescending(x => x.Rating) // ✅ rank by Rating
                    .ToList();

                // Add XP-to-Next-Player after sorting
                var withNextPlayer = leaderboard
                    .Select((x, index) => new
                    {
                        APIRank = x.APIRank,
                        x.Alias,
                        x.Country,
                        x.Rating,
                        x.Wins,
                        x.Losses,
                        x.XP,
                        XPToNextPlayer = index == 0
            ? 0
            : Math.Max(0, leaderboard[index - 1].XP - x.XP),

                        CurrentRank = x.CurrentRank,
                        NextRank = x.NextRank,
                        XPToNextRank = x.XPToNextRank

                    })
                    .ToList();

                tau2v2.DataSource = withNextPlayer;

                tau2v2.Columns["XP"].HeaderText = "Total XP";
                tau2v2.Columns["Rating"].HeaderText = "Rating";
                tau2v2.Columns["Wins"].HeaderText = "Wins";
                tau2v2.Columns["Losses"].HeaderText = "Losses";
                tau2v2.Columns["APIRank"].HeaderText = "API Rank";
                tau2v2.Columns["CurrentRank"].HeaderText = "DOW Rank";
                tau2v2.Columns["NextRank"].HeaderText = "Next Rank";
                tau2v2.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
                tau2v2.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
            }
        }
        private void dg1v1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg2v2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg3v3_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
            private void dg4v4_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    ); }
            private void dg5v5_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    ); }
            private void dg6v6_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    ); }
            private void dg7v7_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    ); }
            private void dg8v8_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    ); }
            private void dg9v9_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg10v10_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg11v11_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg12v12_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg13v13_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg14v14_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg15v15_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg16v16_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg17v17_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void dg18v18_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private void top100Players_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewProgressHelper.HandleCellPainting(
        sender, e,
        "XP",               // column with XP
        "XPToNextRank",     // column with XP to next rank
        "XPToNextPlayer",   // column with XP to next player
        Ranks               // pass in your rank list
    );
        }
        private List<LeaderboardPlayer> BuildGlobalTop100FromGrids()
        {
            var allPlayers = new List<LeaderboardPlayer>();

            allPlayers.AddRange(ExtractPlayersFromGrid(dg1v1));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg3v3));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg4v4));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg5v5));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg6v6));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg7v7));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg8v8));
            allPlayers.AddRange(ExtractPlayersFromGrid(dg9v9));
            allPlayers.AddRange(ExtractPlayersFromGrid(chaos2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(darkeldar2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(eldar2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(imperialguard2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(necrons2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(orks2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(sistersofbattle2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(spacemarines2v2));
            allPlayers.AddRange(ExtractPlayersFromGrid(tau2v2));
            
            var top100 = allPlayers
                .OrderByDescending(p => p.Rating)
                .Take(100)
                .ToList();

            return top100;
        }

        private List<LeaderboardPlayer> ExtractPlayersFromGrid(DataGridView grid)
        {
            var players = new List<LeaderboardPlayer>();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue; // skip empty row

                try
                {
                    var player = new LeaderboardPlayer
                    {
                        APIRank = Convert.ToInt32(row.Cells["APIRank"].Value ?? 0),
                        Alias = row.Cells["Alias"].Value?.ToString(),
                        Country = row.Cells["Country"].Value?.ToString(),
                        Rating = Convert.ToInt32(row.Cells["Rating"].Value ?? 0),
                        Wins = Convert.ToInt32(row.Cells["Wins"].Value ?? 0),
                        Losses = Convert.ToInt32(row.Cells["Losses"].Value ?? 0),
                        XP = Convert.ToInt32(row.Cells["XP"].Value ?? 0),
                        CurrentRank = row.Cells["CurrentRank"].Value?.ToString(),
                        XPToNextPlayer = Convert.ToInt32(row.Cells["XPToNextPlayer"].Value ?? 0),
                        NextRank = row.Cells["NextRank"].Value?.ToString(),
                        XPToNextRank = Convert.ToInt32(row.Cells["XPToNextRank"].Value ?? 0)
                    };

                    players.Add(player);
                }
                catch
                {
                    // Ignore malformed rows
                }
            }

            return players;
        }

        // Generic helper you can reuse (adjust names if you already have a helper)
        public static class DataGridViewProgressHelper
        {
            public static void HandleCellPainting(
                object sender,
                DataGridViewCellPaintingEventArgs e,
                string xpColumnName,
                string xpToNextRankColumn,
                string xpToNextPlayerColumn,
                List<PlayerRank> ranks)
            {
                if (e.RowIndex < 0) return;
                if (!(sender is DataGridView grid)) return;

                // ---------- XP to Next Rank (green) ----------
                if (!string.IsNullOrEmpty(xpToNextRankColumn)
                    && grid.Columns.Contains(xpToNextRankColumn)
                    && e.ColumnIndex == grid.Columns[xpToNextRankColumn].Index)
                {
                    e.PaintBackground(e.CellBounds, true);

                    int xpToNextRank = SafeInt(e.Value);
                    int currXP = SafeInt(grid.Rows[e.RowIndex].Cells[xpColumnName].Value);

                    int totalGap = GetTotalGapToNextRank(currXP, ranks);
                    float percentRank = totalGap > 0 ? (float)(totalGap - xpToNextRank) / totalGap : 1f;

                    string textRank = $"{xpToNextRank:N0} XP ({percentRank:P0})";
                    DrawProgress(e, percentRank, textRank, Brushes.LightGreen);
                }

                // ---------- XP to Next Player (blue) ----------
                if (!string.IsNullOrEmpty(xpToNextPlayerColumn)
                    && grid.Columns.Contains(xpToNextPlayerColumn)
                    && e.ColumnIndex == grid.Columns[xpToNextPlayerColumn].Index)
                {
                    e.PaintBackground(e.CellBounds, true);

                    int xpToNextPlayer = SafeInt(e.Value); // remaining to catch the player above
                    int currXP = SafeInt(grid.Rows[e.RowIndex].Cells[xpColumnName].Value);
                    int aboveXP = e.RowIndex > 0
                        ? SafeInt(grid.Rows[e.RowIndex - 1].Cells[xpColumnName].Value)
                        : currXP;

                    // percent = how close currXP is to aboveXP (1.0 means equal or higher)
                    float percentPlayer = aboveXP > 0 ? (float)currXP / (float)aboveXP : 1f;

                    // Ensure percent is clamped between 0 and 1
                    percentPlayer = Math.Min(Math.Max(percentPlayer, 0f), 1f);

                    // Text shows remaining XP and percent (percent computed from curr/above)
                    string textPlayer = $"{xpToNextPlayer:N0} XP ({percentPlayer:P0})";

                    DrawProgress(e, percentPlayer, textPlayer, Brushes.LightCyan);
                }
            }

            private static void DrawProgress(DataGridViewCellPaintingEventArgs e, float percent, string text, Brush fillBrush)
            {
                percent = Math.Min(Math.Max(percent, 0f), 1f); // clamp

                Rectangle inner = new Rectangle(
                    e.CellBounds.X + 2,
                    e.CellBounds.Y + 2,
                    (int)((e.CellBounds.Width - 4) * percent),
                    e.CellBounds.Height - 4);

                // Background (light gray)
                using (Brush back = new SolidBrush(Color.FromArgb(240, 240, 240)))
                    e.Graphics.FillRectangle(back, e.CellBounds.X + 2, e.CellBounds.Y + 2, e.CellBounds.Width - 4, e.CellBounds.Height - 4);

                // Filled portion
                e.Graphics.FillRectangle(fillBrush, inner);

                // Border
                e.Graphics.DrawRectangle(Pens.Black, e.CellBounds.X + 2, e.CellBounds.Y + 2,
                                         e.CellBounds.Width - 4, e.CellBounds.Height - 4);

                // Make text white if bar is dark enough for readability
                Color textColor = percent > 0.6f ? Color.Black : e.CellStyle.ForeColor;
                TextRenderer.DrawText(e.Graphics, text, e.CellStyle.Font, e.CellBounds, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }

            private static int GetTotalGapToNextRank(int currentXP, List<PlayerRank> ranks)
            {
                var current = ranks.Where(r => currentXP >= r.MinXP).OrderByDescending(r => r.MinXP).FirstOrDefault();
                var next = ranks.Where(r => currentXP < r.MinXP).OrderBy(r => r.MinXP).FirstOrDefault();
                if (current != null && next != null)
                    return next.MinXP - current.MinXP;
                return 0;
            }

            private static int SafeInt(object o)
            {
                if (o == null) return 0;
                if (o is int i) return i;
                int.TryParse(o.ToString(), out int v);
                return v;
            }
        }


        public (string CurrentRank, string NextRank, int XPToNextRank) GetRankProgress(int xp)
        {
            // Find current rank
            var current = Ranks
                .Where(r => xp >= r.MinXP)
                .OrderByDescending(r => r.MinXP)
                .FirstOrDefault();

            // Find next rank
            var next = Ranks
                .Where(r => r.MinXP > (current?.MinXP ?? 0))
                .OrderBy(r => r.MinXP)
                .FirstOrDefault();

            if (next != null)
            {
                return (current?.RankName ?? "Unranked", next.RankName, next.MinXP - xp);
            }
            else
            {
                // Already at max rank
                return (current?.RankName ?? "Unranked", "Max Rank", 0);
            }
        }
        public static class DataGridViewSearchHelper
        {
            /// <summary>
            /// Searches for a given alias in a DataGridView and highlights or selects matching rows.
            /// </summary>
            public static void SearchAndHighlight(DataGridView grid, string aliasColumn, string searchText, bool selectRow = true)
            {
                if (string.IsNullOrWhiteSpace(searchText)) return;

                string lowered = searchText.Trim().ToLower();

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.Cells[aliasColumn].Value == null) continue;

                    string alias = row.Cells[aliasColumn].Value.ToString().ToLower();

                    if (alias.Contains(lowered))
                    {
                        if (selectRow)
                        {
                            row.Selected = true;
                            grid.FirstDisplayedScrollingRowIndex = row.Index; // auto scroll to match
                        }
                        else
                        {
                            // Example: highlight by changing background color
                            row.DefaultCellStyle.BackColor = Color.Yellow;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }
                    }
                    else if (!selectRow)
                    {
                        // Reset style if highlight mode
                        row.DefaultCellStyle.BackColor = grid.DefaultCellStyle.BackColor;
                        row.DefaultCellStyle.ForeColor = grid.DefaultCellStyle.ForeColor;
                    }
                }
            }
        }

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
                    int chaostotal = chaos != null ? chaos.wins + chaos.losses : 0;
                    label136.Text = $"Total Games Played: {chaostotal}";
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
                    label134.Text = chaos != null? $"Current Rating: {chaos.rating}" : "Current Rating: N/A";
                    label135.Text = chaos != null ? $"Highest Rating: {chaos.highestrating}" : "Highest Rating: N/A";

                    //Dark Eldar Stats
                    int darkeldartotal = darkEldar != null ? darkEldar.wins + darkEldar.losses : 0;
                    label139.Text = $"Total Games Played: {darkeldartotal}";
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
                    label137.Text = darkEldar != null ? $"Current Rating: {darkEldar.rating}" : "Current Rating: N/A";
                    label138.Text = darkEldar != null ? $"Highest Rating: {darkEldar.highestrating}" : "Highest Rating: N/A";

                    //Eldar Stats
                    int eldartotal = eldar != null ? eldar.wins + eldar.losses : 0;
                    label142.Text = $"Total Games Played: {eldartotal}";
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
                    label140.Text = eldar != null ? $"Current Rating: {eldar.rating}" : "Current Rating: N/A";
                    label141.Text = eldar != null ? $"Highest Rating: {eldar.highestrating}" : "Highest Rating: N/A";

                    //Imperial Guard Stats
                    int imperialguardtotal = imperialGuard != null ? imperialGuard.wins + imperialGuard.losses : 0;
                    label145.Text = $"Total Games Played: {imperialguardtotal}";
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
                    label143.Text = imperialGuard != null ? $"Current Rating: {imperialGuard.rating}" : "Current Rating: N/A";
                    label144.Text = imperialGuard != null ? $"Highest Rating: {imperialGuard.highestrating}" : "Highest Rating: N/A";

                    //Necrons Stats
                    int necrontotal = necrons != null ? necrons.wins + necrons.losses : 0;
                    label148.Text = $"Total Games Played: {necrontotal}";
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
                    label146.Text = necrons != null ? $"Current Rating: {necrons.rating}" : "Current Rating: N/A";
                    label147.Text = necrons != null ? $"Highest Rating: {necrons.highestrating}" : "Highest Rating: N/A";


                    //Orks Stats
                    int orkstotal = orks != null ? orks.wins + orks.losses : 0;
                    label151.Text = $"Total Games Played: {orkstotal}";
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
                    label149.Text = orks != null ? $"Current Rating: {orks.rating}" : "Current Rating: N/A";
                    label150.Text = orks != null ? $"Highest Rating: {orks.highestrating}" : "Highest Rating: N/A";

                    //Sisters of Battle Stats
                    int sisterstotal = sisters != null ? sisters.wins + sisters.losses : 0;
                    label154.Text = $"Total Games Played: {sisterstotal}";  
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
                    label152.Text = sisters != null ? $"Current Rating: {sisters.rating}" : "Current Rating: N/A";
                    label153.Text = sisters != null ? $"Highest Rating: {sisters.highestrating}" : "Highest Rating: N/A";


                    //Space Marines Stats
                    int spacemarinestotal = spaceMarines != null ? spaceMarines.wins + spaceMarines.losses : 0;
                    label157.Text = $"Total Games Played: {spacemarinestotal}";
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
                    label155.Text = spaceMarines != null ? $"Current Rating: {spaceMarines.rating}" : "Current Rating: N/A";
                    label156.Text = spaceMarines != null ? $"Highest Rating: {spaceMarines.highestrating}" : "Highest Rating: N/A";


                    //Tau Stats
                    int tautotal = tau != null ? tau.wins + tau.losses : 0;
                    label160.Text = $"Total Games Played: {tautotal}";
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
                    label158.Text = tau != null ? $"Current Rating: {tau.rating}" : "Current Rating: N/A";
                    label159.Text = tau != null ? $"Highest Rating: {tau.highestrating}" : "Highest Rating: N/A";

                    int totalGames = totalWins + totalLosses;
                    // Example: set to labels
                    
                    var raceTotals = new Dictionary<string, int>
                    {
                        { "Chaos", chaostotal },
                        { "Space Marines", spacemarinestotal },
                        { "Tau", tautotal },
                        { "Orks", orkstotal },
                        { "Eldar", eldartotal },
                        { "Dark Eldar", darkeldartotal },
                        { "Necrons", necrontotal },
                        { "Imperial Guard", imperialguardtotal },
                        { "Sisters of Battle", sisterstotal }
                    };
                    
                    var favoriteRace = raceTotals.OrderByDescending(r => r.Value).FirstOrDefault();
                    
                    if(favoriteRace.Value > 0)
                    {
                        lblFavoriteRace.Text = $"Favorite Race: {favoriteRace.Key} ({favoriteRace.Value} games)";
                    }
                    else
                    {
                        lblFavoriteRace.Text = "No Games Played Yet";
                    }
                    
                    
                    
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
                        label123.Text = $"Next Rank: {nextRank.RankName}\n\r(at {playerXP} of {nextRank.MinXP} XP)";
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
            lblFavoriteRecRace.Text = favorite != null ? $"Recently Played Race: {favorite.RaceName} ({favorite.MatchesPlayed} matches)" : "N/A";

            // Best Race by WinRate
            var bestWinRate = raceStats.OrderByDescending(x => x.WinRate).FirstOrDefault();
            lblBestRaceWinRate.Text = bestWinRate != null ? $"Best Win Rate: {bestWinRate.RaceName} ({bestWinRate.WinRate:F1}%)" : "N/A";

            // Best Race by Avg Rating
            var bestRating = raceStats.OrderByDescending(x => x.AvgRating).FirstOrDefault();
            lblBestRaceRating.Text = bestRating != null ? $"Best AVG Rating: {bestRating.RaceName} ({bestRating.AvgRating:F0})" : "N/A";
        }
        private async Task LoadTop100()
        {
            var top100 = BuildGlobalTop100FromGrids();
            top100Players.DataSource = top100;

            top100Players.Columns["XP"].HeaderText = "Total XP";
            top100Players.Columns["Rating"].HeaderText = "Rating";
            top100Players.Columns["Wins"].HeaderText = "Wins";
            top100Players.Columns["Losses"].HeaderText = "Losses";
            top100Players.Columns["APIRank"].HeaderText = "API Rank";
            top100Players.Columns["CurrentRank"].HeaderText = "DOW Rank";
            top100Players.Columns["NextRank"].HeaderText = "Next Rank";
            top100Players.Columns["XPToNextRank"].HeaderText = "XP to Next Rank";
            top100Players.Columns["XPToNextPlayer"].HeaderText = "XP to Next Player";
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            this.Text = "DOW Stat Tracker by INSTINCT";
            label118.Text = "v" + Application.ProductVersion;
            this.Icon = Properties.Resources.logo1;
            dg1v1.CellPainting += dg1v1_CellPainting;
            dg2v2.CellPainting += dg2v2_CellPainting;
            dg3v3.CellPainting += dg3v3_CellPainting;
            dg4v4.CellPainting += dg4v4_CellPainting;
            dg5v5.CellPainting += dg5v5_CellPainting;
            dg6v6.CellPainting += dg6v6_CellPainting;
            dg7v7.CellPainting += dg7v7_CellPainting;
            dg8v8.CellPainting += dg8v8_CellPainting;
            dg9v9.CellPainting += dg9v9_CellPainting;
            chaos2v2.CellPainting += dg10v10_CellPainting;
            darkeldar2v2.CellPainting += dg11v11_CellPainting;
            eldar2v2.CellPainting += dg12v12_CellPainting;
            imperialguard2v2.CellPainting += dg13v13_CellPainting;
            necrons2v2.CellPainting += dg14v14_CellPainting;
            orks2v2.CellPainting += dg15v15_CellPainting;
            sistersofbattle2v2.CellPainting += dg16v16_CellPainting;
            spacemarines2v2.CellPainting += dg17v17_CellPainting;
            tau2v2.CellPainting += dg18v18_CellPainting;
            top100Players.CellPainting += top100Players_CellPainting;
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
            await LoadLeaderboard1v1();
            await LoadLeaderboard2v2();
            await LoadLeaderboard3v3();
            await LoadLeaderboard4v4();
            await LoadLeaderboard5v5();
            await LoadLeaderboard6v6();
            await LoadLeaderboard7v7();
            await LoadLeaderboard8v8();
            await LoadLeaderboard9v9();
            await LoadLeaderboard10v10();
            await LoadLeaderboard11v11();
            await LoadLeaderboard12v12();
            await LoadLeaderboard13v13();
            await LoadLeaderboard14v14();
            await LoadLeaderboard15v15();
            await LoadLeaderboard16v16();
            await LoadLeaderboard17v17();
            await LoadLeaderboard18v18();
            await LoadTop100();
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
            Form3 settings;
            settings = new Form3();
            settings.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnLoadJson.PerformClick();
        }

        private void label129_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            panel1v1.Location = new Point(dg1v1.Location.X, dg1v1.Location.Y);
            panel1v1.Bounds = dg1v1.Bounds;
            panel1v1.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel1v1.Visible = true;
            await LoadLeaderboard1v1();
            panel1v1.Visible = false;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            panel2v2.Location = new Point(dg2v2.Location.X, dg2v2.Location.Y);
            panel2v2.Bounds = dg2v2.Bounds;
            panel2v2.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel2v2.Visible = true;
            await LoadLeaderboard2v2();
            panel2v2.Visible = false;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            panel3v3.Location = new Point(dg2v2.Location.X, dg2v2.Location.Y);
            panel3v3.Bounds = dg3v3 .Bounds;
            panel3v3.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel3v3.Visible = true;
            await LoadLeaderboard3v3();
            panel3v3.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string searchAlias = textBox1.Text;
            DataGridViewSearchHelper.SearchAndHighlight(dg1v1, "Alias", searchAlias, selectRow: true);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string searchAlias = textBox3.Text;
            DataGridViewSearchHelper.SearchAndHighlight(dg3v3, "Alias", searchAlias, selectRow: true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string searchAlias = textBox2.Text;
            DataGridViewSearchHelper.SearchAndHighlight(dg2v2, "Alias", searchAlias, selectRow: true);
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            panel4v4.Location = new Point(dg4v4.Location.X, dg4v4.Location.Y);
            panel4v4.Bounds = dg4v4.Bounds;
            panel4v4.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel4v4.Visible = true;
            await LoadLeaderboard4v4();
            panel4v4.Visible = false;
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            panel5v5.Location = new Point(dg5v5.Location.X, dg5v5.Location.Y);
            panel5v5.Bounds = dg5v5.Bounds;
            panel5v5.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel5v5.Visible = true;
            await LoadLeaderboard5v5();
            panel5v5.Visible = false;

        }

        private async void button12_Click(object sender, EventArgs e)
        {
            panel6v6.Location = new Point(dg6v6.Location.X, dg6v6.Location.Y);
            panel6v6.Bounds = dg6v6.Bounds;
            panel6v6.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel6v6.Visible = true;
            await LoadLeaderboard6v6();
            panel6v6.Visible = false;
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            panel7v7.Location = new Point(dg7v7.Location.X, dg7v7.Location.Y);
            panel7v7.Bounds = dg7v7.Bounds;
            panel7v7.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel7v7.Visible = true;
            await LoadLeaderboard7v7();
            panel7v7.Visible = false;
        }

        private async void button16_Click(object sender, EventArgs e)
        {
            panel8v8.Location = new Point(dg8v8.Location.X, dg8v8.Location.Y);
            panel8v8.Bounds = dg8v8.Bounds;
            panel8v8.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel8v8.Visible = true;
            await LoadLeaderboard8v8();
            panel8v8.Visible = false;
        }

        private async void button18_Click(object sender, EventArgs e)
        {
            panel9v9.Location = new Point(dg9v9.Location.X, dg9v9.Location.Y);
            panel9v9.Bounds = dg9v9.Bounds;
            panel9v9.BackColor = Color.FromArgb(128, Color.Gray); // semi-transparent black
            panel9v9.Visible = true;
            await LoadLeaderboard9v9();
            panel9v9.Visible = false;
        }

        private async void button19_Click(object sender, EventArgs e)
        {
            panel4v40.Bounds = this.ClientRectangle;
            panel4v40.Visible = true;
            pictureBox1.Visible = true;
            await LoadLeaderboard1v1();
            await LoadLeaderboard2v2();
            await LoadLeaderboard3v3();
            await LoadLeaderboard4v4();
            await LoadLeaderboard5v5();
            await LoadLeaderboard6v6();
            await LoadLeaderboard7v7();
            await LoadLeaderboard8v8();
            await LoadLeaderboard9v9();
            await LoadLeaderboard10v10();
            await LoadLeaderboard11v11();
            await LoadLeaderboard12v12();
            await LoadLeaderboard13v13();
            await LoadLeaderboard14v14();
            await LoadLeaderboard15v15();
            await LoadLeaderboard16v16();
            await LoadLeaderboard17v17();
            await LoadLeaderboard18v18();
            btnLoadJson.PerformClick();
        }
    }
}