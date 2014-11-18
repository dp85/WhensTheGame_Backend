using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WTG.Core.DataObjects;

namespace WTG.Test.CoreTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Leagues();
            Teams();

            Console.ReadLine();
        }

        public static void Teams()
        {
             // InsertRecordIntoTeams(4, "Cleveland", "Browns", 41.4822, 81.6697);
            InsertRecordIntoTeams(4, "Pittsburgh", "Steelers", 40.4417, 80.0000);
            GetAllTeams();
        }



        public static void Leagues()
        {
            //  DeleteLeague(1);

          //  InsertRecordIntoLeague("NBA", "National Basketball Association");
            GetAllLeagues();
        }


        public static void InsertRecordIntoTeams(int leagueID, string city, string teamName, double latitude, double longitude)
        {
            Team teamToInsert = new Team(null, leagueID, city, teamName, latitude, longitude);
            teamToInsert.DBInsert();
            Console.WriteLine(teamToInsert.TeamID);
            Console.WriteLine(teamToInsert.LeagueID);
            Console.WriteLine(teamToInsert.City);
            Console.WriteLine(teamToInsert.TeamName);
            Console.WriteLine(teamToInsert.Latitude);
            Console.WriteLine(teamToInsert.Longitude);

        }

        public static void GetAllTeams()
        {
            List<Team> allTeams = Team.GetAllTeams();
            foreach(Team t in allTeams)
            {
                Console.WriteLine("{0, -4}{1, -4}{2, -20}{3, -20}{4, -10}{5, -10}", t.TeamID, t.LeagueID, t.City, t.TeamName, t.Latitude, t.Longitude);
            }
        }

        public static void DeleteTeam(int teamID)
        {
            Team.DeleteTeam(teamID);
        }

        public static void InsertRecordIntoLeague(string shortName, string fullName)
        {
            League leagueToInsert = new League(null, "NBA", "National Basketball Association");
            leagueToInsert.DBInsert();
            Console.WriteLine(leagueToInsert.LeagueID);
            Console.WriteLine(leagueToInsert.ShortName);
            Console.WriteLine(leagueToInsert.FullName);
        }


        public static void GetAllLeagues()
        {
            List<League> allLeagues = League.GetAllLeagues();
            foreach(League l in allLeagues)
            {
                Console.WriteLine("{0, -4}{1, -15}{2, -50}", l.LeagueID, l.ShortName, l.FullName);
            }
        }

        public static void DeleteLeague(int leagueID)
        {
            League.DeleteLeague(leagueID);
        }
    }
}
