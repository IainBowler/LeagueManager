using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeagueManager.Models
{
    public class League
    {
        // Key for the class.
        public virtual int Id { get; set; }
        // Display Name for League.
        [Required]
        public virtual string Name { get; set; }
        // Description for League.
        public virtual string Description { get; set; }
        // Which User owns the League.
        [Required]
        public virtual string OwnerName { get; set; }
        // The sides in the League.
        public virtual List<Side> Members { get; set; }
        // Whether everyone can see the League or not.
        public virtual bool Public { get; set; }
        // How points are allocated for the League Tables.
        public virtual int PointsForAWin { get; set; }
        public virtual int PointsForADraw { get; set; }
        public virtual int PointsForALoss { get; set; }

        // Default constructor.
        public League()
        {
            Members = new List<Side>();
            OwnerName = "No-one";
        }
    }
}