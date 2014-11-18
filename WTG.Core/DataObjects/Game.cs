using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WTG.Core.Data;

namespace WTG.Core.DataObjects
{
    public class Game 
    {
        public int? GameID;
        public int HomeTeamID;
        public int VisitorTeamID;
        public DateTime GameDate;
        public string HomeTeamBroadcast;
        public string VisitorTeamBroadcast;
        public int Status;
        public int HomeTeamScore;
        public int VisitorTeamScore;


        public Game() { }

        public Game(int? gameID, int homeTeamID, int visitorTeamID, DateTime gameDate, string homeTeambroadcast, 
            string visitorTeamBroadcast, int status, int homeTeamScore, int visitorTeamScore)
        {
            this.GameID = gameID;
            this.HomeTeamID = homeTeamID;
            this.VisitorTeamID = visitorTeamID;
            this.GameDate = gameDate;
            this.HomeTeamBroadcast = homeTeambroadcast;
            this.VisitorTeamBroadcast = visitorTeamBroadcast;
            this.Status = status;
            this.HomeTeamScore = homeTeamScore;
            this.VisitorTeamScore = visitorTeamScore;
        }

        /// <summary>
        /// Returns True if the Game is complete. If true, means that this Game is not
        /// considered the Next Game. 
        /// </summary>
        /// <returns>True if the Game is Complete.</returns>
        public bool IsComplete()
        {
            throw new MissingMethodException();
        }

        /// <summary>
        /// Executes an Immediate Query to Add this Game to the DB. This object is to be
        /// used as a container for values. All values are required, except for GameID, which
        /// is ignored and assigned by the DB as part of this operation.
        /// </summary>
        public void DBInsert()
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.InsertGame(this);
        }

        /// <summary>
        /// Executes an Immediate Query to retrieve all rows in the Games Table.
        /// </summary>
        /// <returns>A List of all Games in the DB</returns>
        public static List<Game> GetAllGames()
        {
            DataAccess dataAccess = new DataAccess();
            return dataAccess.GetGames();
        }

        /// <summary>
        /// Executes an Immediate Query to retrieve all rows in the Games Table,
        /// sorted by GameDate
        /// </summary>
        /// <returns>A List of all Games in the DB sorted by GameDate</returns>
        public static List<Game> GetAllGamesSortedByGameDate()
        {
            DataAccess dataAccess = new DataAccess();
            return dataAccess.GetGamesOrderedByGameDate();
        }

        /// <summary>
        /// Executes an Immediate Query to Delete a row from the Games Table.
        /// </summary>
        /// <param name="gameID">The GameID of the row to delete.</param>
        public static void DeleteGame(int gameID)
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.DeleteGame(gameID);
        }

    }


}
