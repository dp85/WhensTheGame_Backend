using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Caching;

using WTG.Core.DataObjects;

namespace WTG.Engine.Server
{
    public class WTGServer
    {

        private static readonly TimeSpan _GamesCacheExpiration = TimeSpan.FromMinutes(30.0);
        private const string _GamesCacheKey = "GamesCacheKey";
        private static object _GamesCacheLock = new object();
        private static readonly TimeSpan _TeamsCacheExpiration = TimeSpan.FromMinutes(70.0);
        private const string _TeamsCacheKey = "TeamsCacheKey";
        private static object _TeamsCacheLock = new object();
        private static readonly TimeSpan _RecordsCacheExpiration = TimeSpan.FromMinutes(40.0);
        private const string _RecordsCacheKey = "RecordsCacheKey";
        private static object _RecordsCacheLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TeamID"></param>
        /// <returns></returns>
        public TeamData GetTeamData(int TeamID)
        {
            TeamData teamData = new TeamData();

            // Get the current DateTime and save and reuse in searches to avoid
            // repeadetly asking the system.
            DateTime currentDateTime = DateTime.Now;

            // Get all of the Teams from the Cache.
            List<Team> allTeams = getCachedObject(_TeamsCacheKey, _TeamsCacheLock, _TeamsCacheExpiration) as List<Team>;


            // Get the Team object of the team that this method is currently concerned with
            // This is a linear time operation.
            Team requestedTeam = allTeams.Find(x => x.TeamID == TeamID);

            // Verify that the Team Requested Exists. allTeams.Find() will return null when
            // the team does not exist.
            if(requestedTeam == null)
            {
                // Return null when the Team is not in the system.
                return null;
            }
            
            teamData.TeamID = TeamID;

            // Get All Games from the Cache
            List<Game> allGames = getCachedObject(_GamesCacheKey, _GamesCacheLock, _GamesCacheExpiration) as List<Game>;

            // Get All Records from the cache
            List<Record> allRecords = getCachedObject(_RecordsCacheKey, _RecordsCacheLock, _RecordsCacheExpiration) as List<Record>;

            // BEGIN Logic to find the previous and next game the Team will play

            // We want to find the previous and next game that the team will play
            // For our purposes, any game currently in progress is considered
            // the next game. Live scoring will not be implemented for the
            // initial release.

            // Start at the beginning of the list and find a game of today's date or beyond
            // Logic here is pretty straight-forward. Start at the beginning of the list and
            // look for games that involve the Team. If a Game is found, check to see if the
            // GameDate is today or in the future. The first one we find is the next Game since
            // the list is sorted by Date. Also keep track of the previous Game that is found
            // that involves the team as the last one of these stored will be the most-recent
            // previous game.
            int i = 0;
            int nextGameIndex = 0;
            int previousGameIndex = 0;
            while(i < allGames.Count)
            {
                if ((allGames[i].HomeTeamID == TeamID) || (allGames[i].VisitorTeamID == TeamID) )
                {
                    if(allGames[i].GameDate.Date >= currentDateTime.Date)
                    {
                        // i is a Game that is today or in the future.
                        // it's possible that the game was today and already complete
                        if(allGames[i].IsComplete())
                        {
                            previousGameIndex = i;
                        }
                        else
                        {
                            nextGameIndex = i;
                            break;
                        }
                    }
                    else
	                {
                        previousGameIndex = i;
	                }
                }

                i++;
            }

            Game previousGame = allGames[previousGameIndex];
            Game nextGame = allGames[nextGameIndex];

            // END Logic to find the previous and next game the Team plays

            // Process the Previous Game
            processPreviousGame(teamData, previousGame, requestedTeam, allTeams, allRecords);

            // Process the Next Game
            processNextGame(teamData, nextGame, requestedTeam, allTeams, allRecords);

            // Get the requested Teams Record
            Record teamRecord = allRecords.Find(x => x.TeamID == requestedTeam.TeamID);
            teamData.TeamSeasonWins = teamRecord.Wins ?? -1;
            teamData.TeamSeasonLosses = teamRecord.Losses ?? -1;
            teamData.TeamSeasonTies = teamRecord.Ties ?? -1;
    

            return teamData;
        }

        private void processNextGame(TeamData teamData, Game nextGame, Team requestedTeam, List<Team> allTeams, List<Record> allRecords)
        {
            teamData.NextGameDateTime = nextGame.GameDate.ToString();

            // Determine if the requested Team will be the home team or visitor during the next game
            bool isNextGameHome = nextGame.HomeTeamID == requestedTeam.TeamID;

            teamData.IsNextGameHome = isNextGameHome;

            int opponentID = isNextGameHome ? nextGame.VisitorTeamID : nextGame.HomeTeamID;
            Team opponent = allTeams.Find(x => x.TeamID == opponentID);

            teamData.NextOpponentCity = opponent.City;
            teamData.NextOpponentName = opponent.TeamName;
            teamData.NextGameBroadcast = isNextGameHome ? nextGame.HomeTeamBroadcast : nextGame.VisitorTeamBroadcast;

            Record opponentRecord = allRecords.Find(x => x.TeamID == opponent.TeamID);

            teamData.NextOpponentSeasonWins = opponentRecord.Wins ?? -1;
            teamData.NextOpponentSeasonLosses = opponentRecord.Losses ?? -1;
            teamData.NextOpponentSeasonTies = opponentRecord.Ties ?? -1;            
        }

        private void processPreviousGame(TeamData teamData, Game previousGame, Team requestedTeam, List<Team> allTeams, List<Record> allRecords)
        {
            teamData.PreviousGameDateTime = previousGame.GameDate.ToString();

            // Determine if the requested Team is the home team or visitor
            bool wasPreviousGameHome = previousGame.HomeTeamID == requestedTeam.TeamID;

            teamData.WasPreviousGameHome = wasPreviousGameHome;

            Team opponent;
            int requestedTeamScore;
            int opponentScore;

            if (wasPreviousGameHome)
            {
                opponent = allTeams.Find(x => x.TeamID == previousGame.VisitorTeamID);
                requestedTeamScore = previousGame.HomeTeamScore;
                opponentScore = previousGame.VisitorTeamScore;
            }
            else
            {
                opponent = allTeams.Find(x => x.TeamID == previousGame.HomeTeamID);
                requestedTeamScore = previousGame.VisitorTeamScore;
                opponentScore = previousGame.HomeTeamScore;
            }

            teamData.PreviousOpponentCity = opponent.City;
            teamData.PreviousOpponentName = opponent.TeamName;
            teamData.PreviousOpponentScore = opponentScore;
            teamData.PreviousTeamScore = requestedTeamScore;

            Record opponentRecord = allRecords.Find(x => x.TeamID == opponent.TeamID);

            teamData.PreviousOpponentSeasonWins = opponentRecord.Wins ?? -1;
            teamData.PreviousOpponentSeasonLosses = opponentRecord.Losses ?? -1;
            teamData.PreviousOpponentSeasonTies = opponentRecord.Ties ?? -1;

        }



        private object getCachedObject(string cacheKey, object cacheLock, TimeSpan cacheExpiration)
        {
            // Access the MemoryCache for the Cache Key we are looking for.
            // If the cache doesn't exist, we will get back null.
            // When an item in MemoryCache expires, it is removed from the cache
            // and null will be returned.
            object returnObj = MemoryCache.Default.Get(cacheKey);

            if (returnObj != null)
            {
                return returnObj;
            }

            // The cached object is not in the MemoryCache, let's built it!
            lock (cacheLock)
            {
                // Check to see if it's been loaded up while we were checking and possibly
                // waiting for the lock
                returnObj = MemoryCache.Default.Get(cacheKey);
                if (returnObj != null)
                {
                    return returnObj;
                }

                // It's still not there, grab the collection from the DB

                switch (cacheKey)
                {
                    case _GamesCacheKey:
                        returnObj = Game.GetAllGamesSortedByGameDate();
                        break;
                    case _TeamsCacheKey:
                        returnObj = Team.GetAllTeams();
                        break;
                    case _RecordsCacheKey:
                        returnObj = Record.GetAllRecords();
                        break;
                    default:
                        // We do not have logic to build the collection , return null.
                        return null;
                }

                // Add the collection to the cache.
                // Add a policy so the cached itme expires after CacheExpirationMinutes
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(cacheExpiration));

                MemoryCache.Default.Set(cacheKey, returnObj, cacheItemPolicy);

                return returnObj;
            }
        }


        // TODO - Delete these commented out getCached..() methods once the generic
        // has been tested.

        //private static List<T> getCachedCollection<T>(string cacheKey, object cacheLock, TimeSpan cacheExpiration)
        //{
        //    // Access the MemoryCache for the Cache Key we are looking for.
        //    // If the cache doesn't exist, we will get back null.
        //    // When an item in MemoryCache expires, it is removed from the cache
        //    // and null will be returned.
        //    List<T> returnList = MemoryCache.Default.Get(cacheKey) as List<T>;

        //    if (returnList != null)
        //    {
        //        return returnList;
        //    }

        //    // The cached list is not in the MemoryCache, let's built it!
        //    lock (cacheLock)
        //    {
        //        // Check to see if it's been loaded up while we were checking and possibly
        //        // waiting for the lock
        //        returnList = MemoryCache.Default.Get(cacheKey) as List<T>;
        //        if (returnList != null)
        //        {
        //            return returnList;
        //        }

        //        // It's still not there, grab the collection from the DB

        //        switch (cacheKey)
        //        {
        //            case _GamesCacheKey:
        //                returnList = Game.GetAllGamesSortedByGameDate() as List<T>;
        //                break;
        //            case _TeamsCacheKey:
        //                returnList = Team.GetAllTeams() as List<T>;
        //                break;
        //            case _RecordsCacheKey:
        //                returnList = Record.GetAllRecords() as List<T>;
        //                break;
        //            default:
        //                // We do not have logic to build the collection , return null.
        //                return null;
        //        }

        //        // Add the collection to the cache.
        //        // Add a policy so the cached itme expires after CacheExpirationMinutes
        //        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
        //        cacheItemPolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(cacheExpiration));

        //        MemoryCache.Default.Set(cacheKey, returnList, cacheItemPolicy);

        //        return returnList;

        //    }
        //}

        //private static List<Game> getCachedGamesByGameDate()
        //{
        //    // Access the MemoryCache for the Cache Key we are looking for.
        //    // If the cache doesn't exist, we will get back null.
        //    // When an item in MemoryCache expires, it is removed from the cache
        //    // and null will be returned.
        //    List<Game> returnList = MemoryCache.Default.Get(_GamesCacheKey) as List<Game>;

        //    if(returnList != null)
        //    {
        //        return returnList;
        //    }

        //    // The cached list is not in the MemoryCache, let's build it!

        //    lock(_GamesCacheLock)
        //    {
        //        // Check to see if it's been loaded up while we were checking and possibly
        //        // waiting for the lock
        //        returnList = MemoryCache.Default.Get(_GamesCacheKey) as List<Game>;
        //        if(returnList != null)
        //        {
        //            return returnList;
        //        }

        //        // It's still not there, grab the collection from the DB.

        //        returnList = Game.GetAllGamesSortedByGameDate();

        //        // Add a policy so the cached itme expires after CacheExpirationMinutes
        //        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
        //        cacheItemPolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(_GamesCacheExpiration));

        //        MemoryCache.Default.Set(_GamesCacheKey, returnList, cacheItemPolicy);

        //        return returnList;
        //    }
        //}

        //private static List<Team> getCachedTeams()
        //{
        //    // Access the MemoryCache for the Cache Key we are looking for.
        //    // If the cache doesn't exist, we will get back null.
        //    // When an item in MemoryCache expires, it is removed from the cache
        //    // and null will be returned.
        //    List<Team> returnList = MemoryCache.Default.Get(_TeamsCacheKey) as List<Team>;

        //    if(returnList != null)
        //    {
        //        return returnList;
        //    }

        //    // The cached list is not in the MemoryCache, let's build it!
        //    lock(_TeamsCacheLock)
        //    {
                
        //        // Check to see if it's been loaded up while we were checking and possibly
        //        // waiting for the lock
        //        returnList = MemoryCache.Default.Get(_TeamsCacheKey) as List<Team>;
        //        if(returnList != null)
        //        {
        //            return returnList;
        //        }

        //        // It's still not there, grab the collection from the DB.

        //        returnList = Team.GetAllTeams();

        //        // Add a policy so the cached itme expires after CacheExpirationMinutes
        //        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
        //        cacheItemPolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(_TeamsCacheExpiration));

        //        MemoryCache.Default.Set(_TeamsCacheKey, returnList, cacheItemPolicy);

        //        return returnList;
        //    }
        //}

        //private static List<Record> getCachedRecords()
        //{
        //    // Access the MemoryCache for the Cache Key we are looking for.
        //    // If the cache doesn't exist, we will get back null.
        //    // When an item in MemoryCache expires, it is removed from the cache
        //    // and null will be returned.
        //    List<Record> returnList = MemoryCache.Default.Get(_RecordsCacheKey) as List<Record>;

        //    if (returnList != null)
        //    {
        //        return returnList;
        //    }

        //    // The cached list is not in the MemoryCache, let's build it!
        //    lock (_RecordsCacheLock)
        //    {

        //        // Check to see if it's been loaded up while we were checking and possibly
        //        // waiting for the lock
        //        returnList = MemoryCache.Default.Get(_RecordsCacheKey) as List<Record>;
        //        if (returnList != null)
        //        {
        //            return returnList;
        //        }

        //        // It's still not there, grab the collection from the DB.

        //        returnList = Record.GetAllRecords();

        //        // Add a policy so the cached itme expires after CacheExpirationMinutes
        //        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
        //        cacheItemPolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(_RecordsCacheExpiration));

        //        MemoryCache.Default.Set(_RecordsCacheKey, returnList, cacheItemPolicy);

        //        return returnList;
        //    }

        //}




    }
}
