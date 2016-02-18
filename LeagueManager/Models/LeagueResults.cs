using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeagueManager.Models
{
    public class LeagueResults
    {
        // This class is an ordred List of SideResults.
        public List<SideResults> Results;
        // So we know about the League the Results are for.
        public int LeagueId;
        public string LeagueName;
        public string LeagueDescription;

        public LeagueResults(LeagueManagerDB db, int leagueid)
        {
            Results = new List<SideResults>();

            // Find the League and save the details.
            League LeagueMatchesAreIn = db.Leagues.Find(leagueid);
            LeagueId = leagueid;
            LeagueName = LeagueMatchesAreIn.Name;
            LeagueDescription = LeagueMatchesAreIn.Description;

            // Get all the Matches in the League.
            List<Match> MatchesInLeague = db.Matches.Where(m => m.League.Id == LeagueId)
                                                    .Include(m => m.Side1)
                                                    .Include(m => m.Side2)
                                                    .ToList();

            //Populate our SideReults List from the Members of the League
            foreach(Side s in LeagueMatchesAreIn.Members)
            {
                SideResults sideresult = new SideResults(LeagueMatchesAreIn, s.Id, s.Name);
                Results.Add(sideresult);
            }

            //Enumerate through our Match List adding the the match to each side.
            foreach(Match match in MatchesInLeague)
            {
                //Find SideResult for Side1 and add the result.
                Results.Find(r => r.SideId == match.Side1Id).AddResult(match.Side1Score, match.Side2Score);

                //Find SideResult for Side2 and add the result.
                Results.Find(r => r.SideId == match.Side2Id).AddResult(match.Side2Score, match.Side1Score);
            }

            //Sort the List
            Results = Results.OrderByDescending(r => r.Points).ThenByDescending(r => r.TotalScoreDifference).ThenByDescending(r => r.TotalScoreFor).ToList();
        }
    }
}