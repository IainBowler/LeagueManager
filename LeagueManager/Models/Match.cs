using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeagueManager.Models
{
    public class Match
    {
        // Key for the class.
        public virtual int MatchId { get; set; }
        // Foreign Key for League the Match is in. Required see OnModelCreating
        private int pLeagueId;
        public int LeagueId { get { if (League != null) { return League.Id; } else { return pLeagueId; } } set { pLeagueId = value; } }
        // League the Match is in.
        public virtual League League { get; set; }
        // Foreign Key for the first Side in the Match. Required see OnModelCreating
        private int pSide1Id;
        public int Side1Id { get { if (Side1 != null) { return Side1.Id; } else { return pSide1Id; } } set { pSide1Id = value; } }
        // The first Side in the Match.
        public virtual Side Side1 { get; set; }
        // Foreign Key for the second Side in the Match. Required see OnModelCreating
        private int pSide2Id;
        public int Side2Id { get { if (Side2 != null) { return Side2.Id; } else { return pSide2Id; } } set { pSide2Id = value; } }
        // The second Side in the Match.
        public virtual Side Side2 { get; set; }
        // Score for the first Side
        public virtual int Side1Score { get; set; }
        // Score for the second Side
        public virtual int Side2Score { get; set; }

        // Default constructor
        public Match()
        {
        }

        // Complete constructor.
        public Match(League league, Side side1, int side1score, Side side2, int side2score)
        {
            League = league;
            Side1 = side1;
            Side1Score = side1score;
            Side2 = side2;
            Side2Score = side2score;
        }
    }
}