using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WTG.Core.Data;

namespace WTG.Core.DataObjects
{
    public class Record
    {
        // Class Members

        public int? RecordID;
        public int? TeamID;
        public int? Wins;
        public int? Losses;
        public int? Ties;

        public Record() { }

        public Record(int? recordID, int teamID, int wins, int losses, int ties)
        {
            this.RecordID = recordID;
            this.TeamID = teamID;
            this.Wins = wins;
            this.Losses = losses;
            this.Ties = ties;
        }

        /// <summary>
        /// Executes an Immediate Query to Add this Record to the DB. This object is used
        /// as a container of values when calling this method. All values are required, except
        /// for RecordID, which is ignored and assigned by the DB as part of this operation.
        /// </summary>
        public void DBInsert()
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.InsertRecord(this);
        }

        /// <summary>
        /// Executes an Immediate Query to retrieve all rows in the Records Table.
        /// </summary>
        /// <returns>A List of all Records in the DB</returns>
        public static List<Record> GetAllRecords()
        {
            DataAccess dataAccess = new DataAccess();
            return dataAccess.GetRecords();
        }

        /// <summary>
        /// Executes an Immediate Query to Delete a row from the Records Table
        /// </summary>
        /// <param name="recordID">The RecordID of the row to delete.</param>
        public static void DeleteRecord(int recordID)
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.DeleteRecord(recordID);
        }

    }
}
