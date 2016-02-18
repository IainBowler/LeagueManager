using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LeagueManager.Models
{
    public class LeagueManagerDB : IdentityDbContext<ApplicationUser>
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public LeagueManagerDB() : base("name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // So that each match must be a member of a League and
            // deleting a League will delete all matches in the League
            modelBuilder.Entity<Match>()
                              .HasRequired(m => m.League)
                              .WithMany()
                              .HasForeignKey(m => m.LeagueId);

            // So that both Side1 and Side 2 are required in a Match but we
            // cannot cascade delete as there are two references to table Side
            // in match. So we must handle it in the code.
            modelBuilder.Entity<Match>()
                              .HasRequired(m => m.Side1)
                              .WithMany()
                              .HasForeignKey(m => m.Side1Id)
                              .WillCascadeOnDelete(false);
            modelBuilder.Entity<Match>()
                                .HasRequired(m => m.Side2)
                                .WithMany()
                                .HasForeignKey(m => m.Side2Id)
                                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<LeagueManager.Models.League> Leagues { get; set; }
        public System.Data.Entity.DbSet<LeagueManager.Models.Side> Sides { get; set; }
        public System.Data.Entity.DbSet<LeagueManager.Models.Match> Matches { get; set; }
    }
}
