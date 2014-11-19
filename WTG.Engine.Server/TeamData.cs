using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WTG.Engine.Server
{
    /// <summary>
    /// Container of Values to display for a Team.
    /// </summary>
    public class TeamData
    {
        public int TeamID;
        public int TeamSeasonWins;
        public int TeamSeasonLosses;
        public int TeamSeasonTies;
        public string NextGameDateTime;
        public string NextGameBroadcast;
        public string NextOpponentName;
        public string NextOpponentCity;
        public int NextOpponentSeasonWins;
        public int NextOpponentSeasonLosses;
        public int NextOpponentSeasonTies;
        public bool IsNextGameHome;
        public string PreviousGameDateTime;
        public string PreviousOpponentName;
        public string PreviousOpponentCity;
        public int PreviousTeamScore;
        public int PreviousOpponentScore;
        public bool WasPreviousGameHome;
        public int PreviousOpponentSeasonWins;
        public int PreviousOpponentSeasonLosses;
        public int PreviousOpponentSeasonTies;
    }
}
