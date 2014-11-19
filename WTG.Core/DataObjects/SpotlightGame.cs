using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WTG.Core.Data;

namespace WTG.Core.DataObjects
{
    public class SpotlightGame
    {
        // Class Member
        public int? SpotlightGameID;
        public DateTime? GameDate;
        public int? Status;
        // TODO Make sure Headline is added to the db
        public string Headline;
        public string Description;

        public SpotlightGame() { }

        public SpotlightGame(int? spotlightGameID, DateTime gameDate, int status, string headline, string description)
        {
            this.SpotlightGameID = spotlightGameID;
            this.GameDate = gameDate;
            this.Status = status;
            this.Headline = headline;
            this.Description = description;
        }

        /// <summary>
        /// Executes an Immediate Query to Add this SpotlightGame to the DB. This object is to be
        /// used as a container for values. All values are required, except for SpotlightGameID, which
        /// is ignored and assigned by the DB as part of this operation.
        /// </summary>
        public void DBInsert()
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.InsertSpotlightGame(this);
        }

        /// <summary>
        /// Executes an Immediate Query to retrieve all rows in the SpotlightGames Table.
        /// </summary>
        /// <returns>A List of all SpotlightGames in the DB</returns>
        public static List<SpotlightGame> GetAllSpotLightGames()
        {
            DataAccess dataAccess = new DataAccess();
            return dataAccess.GetSpotlightGames();
        }

        /// <summary>
        /// Executes an Immediate Query to Delete a row from the SpotLightGames Table.
        /// </summary>
        /// <param name="spotlightGameID">The SpotlightGameID of the row to delete.</param>
        public static void DeleteSpotlightGame(int spotlightGameID)
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.DeleteLeague(spotlightGameID);
        }

    }

}
