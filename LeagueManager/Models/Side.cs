using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;

namespace LeagueManager.Models
{
    public class Side
    {
        // Key for the class.
        public virtual int Id { get; set; }
        // Display Name for the Side.
        [Required]
        public virtual string Name { get; set; }
        // The user that owns this side.
        [Required]
        public virtual string OwnerName { get; set; }
        // List of Leagues that the side is in.
        public virtual List<League> LeaguesIn { get; set; }

        // Default constructor.
        public Side()
        {
            LeaguesIn = new List<League>();
        }
    }
}