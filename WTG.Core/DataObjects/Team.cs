using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WTG.Core.Data;

namespace WTG.Core.DataObjects
{
    public class Team
    {
        // Class Members

        public int? TeamID;
        public int? LeagueID;
        public string City;
        public string TeamName;
        public double Latitude;
        public double Longitude;

        public Team() { }

        public Team(int? teamID, int leagueID, string city, string teamName, double latitude, double longitude)
        {
            this.TeamID = teamID;
            this.LeagueID = leagueID;
            this.City = city;
            this.TeamName = teamName;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Executes an Immediate Query to Add this Team to the DB. This object is to be
        /// used as a container for values. All values are required, except for TeamID, which
        /// is ignored and assigned by the DB as part of this operation.
        /// </summary>
        public void DBInsert()
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.InsertTeam(this);
        }

        /// <summary>
        /// Executes an Immediate Query to retrieve all rows in the Teams Table.
        /// </summary>
        /// <returns>A List of all Teams in the DB</returns>
        public static List<Team> GetAllTeams()
        {
            DataAccess dataAccess = new DataAccess();
            return dataAccess.GetTeams();
        }

        /// <summary>
        /// Executes an Immediate Query to Delete a row from the Teams Table.
        /// </summary>
        /// <param name="teamID">The TeamID of the row to delete.</param>
        public static void DeleteTeam(int teamID)
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.DeleteTeam(teamID);
        }


    }
}
