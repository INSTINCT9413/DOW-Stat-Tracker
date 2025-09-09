using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOW_Stat_Tracker
{
    internal class MatchGetSet
    {
    }

    public class Root
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<MatchHistoryStat> matchHistoryStats { get; set; }
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

}
