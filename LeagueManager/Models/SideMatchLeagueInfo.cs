using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeagueManager.Models
{
    public class SideMatchLeagueInfo
    {
        // This class finds out if a Side is in a League and then records how many 
        // matches it is involved in if it is.

        // The Side we are investigating.
        public Side Side { get; set; }
        // The Id of the League we are investigating.
        public int LeagueId { get; set; }
        // Whether the Side is in League or not.
        public bool IsInLeague { get; set; }
        // The number of matches the Side is involved in, in the League.
        public int noOfMatches;

        public SideMatchLeagueInfo(Side side, League league, LeagueManagerDB db)
        {
            // Set the Side and the League Id.
            Side = side;
            LeagueId = league.Id;

            // Check if the side is in the League or not.
            if(league.Members.Contains(side))
            {
                //Our Side is in the league
                IsInLeague = true;

                // Get all of the Matches that involve our Side.
                List<Match> matchesSideIsInALeague = db.Matches.Where(m => m.LeagueId == league.Id &&
                                                                    (m.Side1Id == side.Id || m.Side2Id == side.Id))
                                                        .ToList();

                // Record how many matches in this League the Side is a part of.
                noOfMatches = matchesSideIsInALeague.Count;
            }

            else
            {
                // Our side is not in the League.
                IsInLeague = false;
            }
        }
    }
}