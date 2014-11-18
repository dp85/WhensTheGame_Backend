using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WTG.Core.Data;
namespace WTG.Core.DataObjects
{
    public class League
    {
        // Class Members

        public int? LeagueID;
        public string ShortName;
        public string FullName;

        public League() { }
        public League(int? leagueID, string shortName, string fullName)
        {
            this.LeagueID = leagueID;
            this.ShortName = shortName;
            this.FullName = fullName;
        }

        /// <summary>
        /// Executes an Immediate Query to Add a League to the DB.
        /// </summary>
        /// <param name="shortName">The ShortName for the leauge, ex. NFL</param>
        /// <param name="fullName">The LongName for the league, ex. National Football League</param>
        /// <returns>The League object added to the DB</returns>
        public static League InsertLeague(string shortName, string fullName)
        {
            League league = new League();
            league.ShortName = shortName;
            league.FullName = fullName;

            DataAccess dataAccess = new DataAccess();
            dataAccess.InsertLeague(league);

            return league;
        }

        /// <summary>
        /// Executes an Immediate Query to Add this League to the DB. This league object
        /// is used as a container of values. All values are required, excpet for LeagueID,
        /// which is ignored and assigned as part of this operation.
        /// </summary>
        public void DBInsert()
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.InsertLeague(this);
        }

        /// <summary>
        /// Executes an Immediate Query to retrieve all rows in the League Table.
        /// </summary>
        /// <returns>A List of all Leagues in the DB</returns>
        public static List<League> GetAllLeagues()
        {
            DataAccess dataAccess = new DataAccess();
            return dataAccess.GetLeagues();
        }

        /// <summary>
        /// Executes an Immediate Query to Delete a row from the Leagues Table.
        /// </summary>
        /// <param name="leagueID">The LeagueID of the row to delete.</param>
        public static void DeleteLeague(int leagueID)
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.DeleteLeague(leagueID);
        }
    }
}
