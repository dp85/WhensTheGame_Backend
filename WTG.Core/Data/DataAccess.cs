using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using WTG.Core.DataObjects;

namespace WTG.Core.Data
{
    internal class DataAccess
    {


        private const string _ConnectionString = @"Data Source=(localdb)\Projects;Initial Catalog=WTG;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
        //  System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"];

        #region Leagues

        // STATIC QUERY STRINGS
        private const string _SelectAllLeagues = "SELECT * FROM Leagues;";
        private const string _InsertIntoLeagues = "INSERT INTO Leagues(ShortName, FullName) " +
                                                 "OUTPUT INSERTED.LeagueID " +
                                                 "VALUES (@ShortName, @FullName)";
        private const string _DeleteFromLeagues = "DELETE FROM Leagues " +
                                                 "WHERE LeagueID = @LeagueID";
        public List<League> GetLeagues()
        {
            List<League> leagues = new List<League>();

            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _SelectAllLeagues;

                sqlConnection.Open();

                SqlDataReader sqlDataReader;
                sqlDataReader = sqlCommand.ExecuteReader();

                // Build collection of Leagues from Query Results.

                while (sqlDataReader.Read())
                { 
                    int leagueID = Convert.ToInt32(sqlDataReader[0]);
                    string shortName = Convert.ToString(sqlDataReader[1]);
                    string longName = Convert.ToString(sqlDataReader[2]);

                    League league = new League(leagueID, shortName, longName);
                    leagues.Add(league);
                }

                sqlDataReader.Close();

                sqlConnection.Close();
            }

            return leagues;
        }

        /// <summary>
        /// Add a row to the Leagues table. The Identifier assigned to the LeagueID object
        /// by the DB will be assigned to the leagueToInsert object.
        /// </summary>
        /// <param name="leagueToInsert">League to Insert. This object is used as a container for
        /// values. LeagueID is assigned automatically by the DB.</param>
        public void InsertLeague(League leagueToInsert)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _InsertIntoLeagues;

                sqlCommand.Parameters.AddWithValue("@ShortName", leagueToInsert.ShortName);
                sqlCommand.Parameters.AddWithValue("@FullName", leagueToInsert.FullName);

                sqlConnection.Open();

                // The LeagueID is an Identifier and is auto incremented on insertion.
                // The value is requested in the query and we get it by ExecuteScalar().
                int leagueID = (int)sqlCommand.ExecuteScalar();
                leagueToInsert.LeagueID = leagueID;

                sqlConnection.Close();
            }
        }


        /// <summary>
        /// Deletes a League from the DB with the specified LeagueID
        /// </summary>
        /// <param name="leagueID">ID of the League To Delete</param>
        public void DeleteLeague(int leagueID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _DeleteFromLeagues;

                sqlCommand.Parameters.AddWithValue("@LeagueID", leagueID);

                sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        #endregion Leagues

        #region Teams

        // STATIC QUERY STRINGS

        private const string _SelectAllTeams = "SELECT * FROM Teams;";
        private const string _InsertIntoTeams = "INSERT INTO Teams(LeagueID, City, TeamName, Latitude, Longitude) " +
                                                 "OUTPUT INSERTED.TeamID " +
                                                 "VALUES (@LeagueID, @City, @TeamName, @Latitude, @Longitude)";
        private const string _DeleteFromTeams = "DELETE FROM Teams " +
                                                 "WHERE TeamID = @TeamID";

        /// <summary>
        /// Gets all Teams from the database
        /// </summary>
        /// <returns>A collection of all Teams in the Teams db table.</returns>
        public List<Team> GetTeams()
        {
            List<Team> teams = new List<Team>();

            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _SelectAllTeams;

                sqlConnection.Open();

                SqlDataReader sqlDataReader;
                sqlDataReader = sqlCommand.ExecuteReader();

                // Build collection of Teams from Query Results.

                while (sqlDataReader.Read())
                {
                    int teamID = Convert.ToInt32(sqlDataReader[0]);
                    int leagueID = Convert.ToInt32(sqlDataReader[1]);
                    string city = Convert.ToString(sqlDataReader[2]);
                    string teamName = Convert.ToString(sqlDataReader[3]);
                    double latitude = Convert.ToDouble(sqlDataReader[4]);
                    double longitude = Convert.ToDouble(sqlDataReader[5]);

                    Team team = new Team(teamID, leagueID, city, teamName, latitude, longitude);
                    teams.Add(team);
                }

                sqlDataReader.Close();

                sqlConnection.Close();
            }

            return teams;
        }

        /// <summary>
        /// Add a row to the Teams table. The Identifier assigned to the TeamID object
        /// by the DB will be assigned to the teamToInsert object.
        /// </summary>
        /// <param name="teamToInsert">Team to Insert. This object is used as a container for
        /// values. TeamID is assigned automatically by the DB.</param>
        public void InsertTeam(Team teamToInsert)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _InsertIntoTeams;

                sqlCommand.Parameters.AddWithValue("@LeagueID", teamToInsert.LeagueID);
                sqlCommand.Parameters.AddWithValue("@City", teamToInsert.City);
                sqlCommand.Parameters.AddWithValue("@TeamName", teamToInsert.TeamName);
                sqlCommand.Parameters.AddWithValue("@Latitude", teamToInsert.Latitude);
                sqlCommand.Parameters.AddWithValue("@Longitude", teamToInsert.Longitude);

                sqlConnection.Open();

                // The LeagueID is an Identifier and is auto incremented on insertion.
                // The value is requested in the query and we get it by ExecuteScalar().
                int teamID = (int)sqlCommand.ExecuteScalar();
                teamToInsert.TeamID = teamID;
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Deletes a Team from the DB with the specified TeamID
        /// </summary>
        /// <param name="teamID">ID of the Team To Delete</param>
        public void DeleteTeam(int teamID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _DeleteFromTeams;

                sqlCommand.Parameters.AddWithValue("@TeamID", teamID);

                sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        #endregion Teams

        #region Records

        // STATIC QUERY STRINGS

        private const string _SelectAllRecords = "SELECT * FROM Records;";
        private const string _InsertIntoRecords = "INSERT INTO Records(TeamID, Wins, Losses, Ties) " +
                                                 "OUTPUT INSERTED.RecordID " +
                                                 "VALUES (@TeamID, @Wins, @Losses, @Ties)";
        private const string _DeleteFromRecords = "DELETE FROM Records " +
                                                 "WHERE RecordID = @RecordID";

        /// <summary>
        /// Gets all Records from the DB
        /// </summary>
        /// <returns>A collection of all Records from the Records table.</returns>
        public List<Record> GetRecords()
        {
            List<Record> records = new List<Record>();

            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _SelectAllRecords;

                sqlConnection.Open();

                SqlDataReader sqlDataReader;
                sqlDataReader = sqlCommand.ExecuteReader();

                // Build collection of Records from Query Results.

                while (sqlDataReader.Read())
                {
                    int recordID = Convert.ToInt32(sqlDataReader[0]);
                    int teamID = Convert.ToInt32(sqlDataReader[1]);
                    int wins = Convert.ToInt32(sqlDataReader[2]);
                    int losses = Convert.ToInt32(sqlDataReader[3]);
                    int ties = Convert.ToInt32(sqlDataReader[4]);

                    Record record = new Record(recordID, teamID, wins, losses, ties);
                    records.Add(record);
                }

                sqlDataReader.Close();

                sqlConnection.Close();
            }

            return records;
        }

        /// <summary>
        /// Add a row to the Records table. The Identifier assigned to the RecordID object
        /// by the DB will be assigned to the recordToInsert object.
        /// </summary>
        /// <param name="recordToInsert">Record to Insert. This object is used as a container for
        /// values. RecordID is assigned automatically by the DB.</param>
        public void InsertRecord(Record recordToInsert)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _InsertIntoRecords;

                sqlCommand.Parameters.AddWithValue("@TeamID", recordToInsert.TeamID);
                sqlCommand.Parameters.AddWithValue("@Wins", recordToInsert.Wins);
                sqlCommand.Parameters.AddWithValue("@Losses", recordToInsert.Losses);
                sqlCommand.Parameters.AddWithValue("@Ties", recordToInsert.Ties);

                sqlConnection.Open();

                // The RecordID is an Identifier and is auto incremented on insertion.
                // The value is requested in the query and we get it by ExecuteScalar().
                int recordID = (int)sqlCommand.ExecuteScalar();
                recordToInsert.RecordID = recordID;
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Deletes a Record from the DB with the specified RecordID
        /// </summary>
        /// <param name="recordID">ID of the Record To Delete</param>
        public void DeleteRecord(int recordID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _DeleteFromRecords;

                sqlCommand.Parameters.AddWithValue("@RecordID", recordID);

                sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        #endregion Records

        #region Games

        // STATIC QUERY STRINGS

        private const string _SelectAllGames = "SELECT * FROM Games;";
        private const string _SelectAllGamesOrderByDate = "SELECT * FROM Games " +
                                                          "ORDER BY GameDate;";
        private const string _InsertIntoGames = "INSERT INTO Games(HomeTeamID, VisitorTeamID, GameDate, HomeTeamBroadcast, VisitorTeamBroadcast, Status, HomeTeamScore, VisitorTeamScore) " +
                                                 "OUTPUT INSERTED.GameID " +
                                                 "VALUES (@HomeTeamID, @VisitorTeamID, @GameDate, @HomeTeamBroadcast, @VisitorTeamBroadcast, @Status, @HomeTeamScore, @VisitorTeamScore)";
        private const string _DeleteFromGames = "DELETE FROM Games " +
                                                 "WHERE GameID = @GameID";

        /// <summary>
        /// Gets all Games from the DB. Unordered.
        /// </summary>
        /// <returns>A collection of all Games from the Games table.</returns>
        public List<Game> GetGames()
        {
            return getGames(_SelectAllGames);
        }

        /// <summary>
        /// Gets all Games from the DB Ordered By GameDate
        /// </summary>
        /// <returns>A List of Games ordered by GameDate.</returns>
        public List<Game> GetGamesOrderedByGameDate()
        {
            return getGames(_SelectAllGamesOrderByDate);
        }

        private List<Game> getGames(string sqlQueryToUse)
        {
            List<Game> games = new List<Game>();

            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = sqlQueryToUse;

                sqlConnection.Open();

                SqlDataReader sqlDataReader;
                sqlDataReader = sqlCommand.ExecuteReader();

                // Build collection of Records from Query Results.

                while (sqlDataReader.Read())
                {
                    int gameID = Convert.ToInt32(sqlDataReader[0]);
                    int homeTeamID = Convert.ToInt32(sqlDataReader[1]);
                    int visitorTeamID = Convert.ToInt32(sqlDataReader[2]);
                    DateTime gameDate = Convert.ToDateTime(sqlDataReader[3]);
                    string homeTeamBroadcast = Convert.ToString(sqlDataReader[4]);
                    string visitorTeamBroadcast = Convert.ToString(sqlDataReader[5]);
                    int status = Convert.ToInt32(sqlDataReader[6]);
                    int homeTeamScore = Convert.ToInt32(sqlDataReader[7]);
                    int visitorTeamScore = Convert.ToInt32(sqlDataReader[8]);

                    Game game = new Game(gameID, homeTeamID, visitorTeamID, gameDate, homeTeamBroadcast, 
                        visitorTeamBroadcast, status, homeTeamScore, visitorTeamScore);
                    games.Add(game);
                }

                sqlDataReader.Close();

                sqlConnection.Close();
            }

            return games;
        }

        /// <summary>
        /// Add a row to the Games table. The Identifier assigned to the GameID object
        /// by the DB will be assigned to the gameToInsert object.
        /// </summary>
        /// <param name="gameToInsert">Record to Insert. This object is used as a container for
        /// values. GameID is assigned automatically by the DB.</param>
        public void InsertGame(Game gameToInsert)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _InsertIntoGames;

                sqlCommand.Parameters.AddWithValue("@HomeTeamID", gameToInsert.HomeTeamID);
                sqlCommand.Parameters.AddWithValue("@VisitorTeamID", gameToInsert.VisitorTeamID);
                sqlCommand.Parameters.AddWithValue("@GameDate", gameToInsert.GameDate);
                sqlCommand.Parameters.AddWithValue("@HomeTeamBroadcast", gameToInsert.HomeTeamBroadcast);
                sqlCommand.Parameters.AddWithValue("@VisitorTeamBroadcast", gameToInsert.VisitorTeamBroadcast);
                sqlCommand.Parameters.AddWithValue("@Status", gameToInsert.Status);
                sqlCommand.Parameters.AddWithValue("@HomeTeamScore", gameToInsert.HomeTeamScore);
                sqlCommand.Parameters.AddWithValue("@VisitorTeamScore", gameToInsert.VisitorTeamScore);


                sqlConnection.Open();

                // The GameID is an Identifier and is auto incremented on insertion.
                // The value is requested in the query and we get it by ExecuteScalar().
                int gameID= (int)sqlCommand.ExecuteScalar();
                gameToInsert.GameID = gameID;
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Deletes a Game from the DB with the specified GameID
        /// </summary>
        /// <param name="GameID">ID of the Game To Delete</param>
        public void DeleteGame(int gameID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _DeleteFromGames;

                sqlCommand.Parameters.AddWithValue("@GameID", gameID);

                sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        #endregion Games

        #region SpotlightGames

        // STATIC QUERY STRINGS

        private const string _SelectAllSpotlightGames = "SELECT * FROM SpotlightGames;";
        private const string _InsertIntoSpotlightGames = "INSERT INTO SpotlightGames(GameDate, Status, Headline, Description) " +
                                                 "OUTPUT INSERTED.SpotlightGameID " +
                                                 "VALUES (@GameDate, @Status, @Headline, @Description)";
        private const string _DeleteFromSpotlightGames = "DELETE FROM SpotlightGames " +
                                                 "WHERE SpotlightGameID = @SpotlightGameID";


        /// <summary>
        /// Gets all SpotlightGames from the DB
        /// </summary>
        /// <returns>A collection of Spotlight Games</returns>
        public List<SpotlightGame> GetSpotlightGames()
        {
            List<SpotlightGame> spotlightGames = new List<SpotlightGame>();

            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _SelectAllSpotlightGames;

                sqlConnection.Open();

                SqlDataReader sqlDataReader;
                sqlDataReader = sqlCommand.ExecuteReader();

                // Build collection of Records from Query Results.

                while (sqlDataReader.Read())
                {
                    int spotlightGameID = Convert.ToInt32(sqlDataReader[0]);
                    DateTime gameDate = Convert.ToDateTime(sqlDataReader[1]);
                    int status = Convert.ToInt32(sqlDataReader[2]);
                    string headline = Convert.ToString(sqlDataReader[3]);
                    string description = Convert.ToString(sqlDataReader[4]);

                    SpotlightGame spotlightGame = new SpotlightGame(spotlightGameID, gameDate, status, headline, description);
                    spotlightGames.Add(spotlightGame);
                }

                sqlDataReader.Close();

                sqlConnection.Close();
            }

            return spotlightGames;
        }

        /// <summary>
        /// Add a row to the SpotlightGames table. The Identifier assigned to the SpotlightGameID object
        /// by the DB will be assigned to the spotlightGameToInsert object.
        /// </summary>
        /// <param name="spotlightGameToInsert">Record to Insert. This object is used as a container for
        /// values. SpotlightGameID is assigned automatically by the DB.</param>
        public void InsertSpotlightGame(SpotlightGame spotlightGameToInsert)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _InsertIntoSpotlightGames;

                sqlCommand.Parameters.AddWithValue("@GameDate", spotlightGameToInsert.GameDate);
                sqlCommand.Parameters.AddWithValue("@Status", spotlightGameToInsert.Status);
                sqlCommand.Parameters.AddWithValue("@Headline", spotlightGameToInsert.Headline);
                sqlCommand.Parameters.AddWithValue("@Description", spotlightGameToInsert.Description);


                sqlConnection.Open();

                // The SpotlightGameID is an Identifier and is auto incremented on insertion.
                // The value is requested in the query and we get it by ExecuteScalar().
                int spotlightGameID = (int)sqlCommand.ExecuteScalar();
                spotlightGameToInsert.SpotlightGameID = spotlightGameID;
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Deletes a SpotlightGame from the DB with the specified SpotlightGameID
        /// </summary>
        /// <param name="SpotlightGameID">ID of the SpotlightGame To Delete</param>
        public void DeleteSpotlightGame(int spotlightGameID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _DeleteFromSpotlightGames;

                sqlCommand.Parameters.AddWithValue("@SpotlightGameID", spotlightGameID);

                sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }
        }

        #endregion SpotlightGames

    }
}
