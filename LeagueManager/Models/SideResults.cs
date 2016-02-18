using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeagueManager.Models
{
    public class SideResults
    {
        // Side results is a class that stores the results for a particular side in a league.
        // It keeps a running total for the LeagueResults class.

        // The League that the Side and matches are in.
        public League LeagueResultsAreFor { get; set; }

        // The Id of the Side the results are for.
        private int pSideId;
        public int SideId { get { return pSideId; } }
        // The Name of the Side the results are for.
        private string pSideName;
        public string SideName { get { return pSideName; } }

        // All the running total properties are set up as read only.
        // The number of Matches the Side has played.
        private int pMatchesPlayed;
        public int MatchesPlayed { get { return pMatchesPlayed; } }
        // The number of Matches the Side has won.
        private int pMatchesWon;
        public int MatchesWon { get { return pMatchesWon; } }
        // The number of Matches the Side has drawn.
        private int pMatchesDrawn;
        public int MatchesDrawn { get { return pMatchesDrawn; } }
        // The number of Matches the Side has lost.
        private int pMatchesLost;
        public int MatchesLost { get { return pMatchesLost; } }
        // The total score for the Side added up over all the matches.
        private int pTotalScoreFor;
        public int TotalScoreFor { get { return pTotalScoreFor; } }
        // The total score against the Side added up over all the matches.
        private int pTotalScoreAgainst;
        public int TotalScoreAgainst { get { return pTotalScoreAgainst; } }
        // The difference between the total score for and the total score against
        public int TotalScoreDifference { get { return pTotalScoreFor - pTotalScoreAgainst; } }
        // The Total points for the side from all the matches.
        private int pPoints;
        public int Points { get { return pPoints; } }

        // Constructor to set up the class.
        public SideResults(League league, int id, string name)
        {
            LeagueResultsAreFor = league;
            pSideId = id;
            pSideName = name;
        }

        // Add a result to our running totals.
        public void AddResult(int ScoreFor, int ScoreAgainst)
        {
            // Add 1 to number of matches played and add to the score running totals.
            pMatchesPlayed = pMatchesPlayed + 1;
            pTotalScoreFor = pTotalScoreFor + ScoreFor;
            pTotalScoreAgainst = pTotalScoreAgainst + ScoreAgainst;

            // Now work out if it's a win loss or draw add to the appropriate counter and add
            // the right number of points as described by the League.
            if(ScoreFor > ScoreAgainst)
            {
                pMatchesWon = pMatchesWon + 1;
                pPoints = pPoints + LeagueResultsAreFor.PointsForAWin;
            }
            else
            {
                if (ScoreFor < ScoreAgainst)
                {
                    pMatchesLost = pMatchesLost + 1;
                    pPoints = pPoints + LeagueResultsAreFor.PointsForALoss;
                }
                else
                {
                    pMatchesDrawn = pMatchesDrawn + 1;
                    pPoints = pPoints + LeagueResultsAreFor.PointsForADraw;
                }
            }
        }
    }
}