using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using LeagueManager.Models;




namespace LeagueManager.Models
{
    public class LeagueManagerDbInitialiser : System.Data.Entity.DropCreateDatabaseAlways<LeagueManagerDB>
    {
        private ApplicationUserManager userManager;
        private RoleManager<IdentityRole> roleManager;
        private LeagueManagerDB context;

        protected override void Seed(LeagueManagerDB db)
        {
            // Initialise the private member varriables.
            context = db;
            userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Add Test data.
            AddTestUser();

            // Add Deno data.
            AddNewUser();

            // Add a Familiar League.
            AddPremierLeague();

            // Add an Administrator User.
            AddAdmin();

            base.Seed(context);
        }

        private void AddTestUser()
        {
            // Create the Test User.
            ApplicationUser newUser = new ApplicationUser { UserName = "TestUser", Email = "test@testing.com" };
            IdentityResult testResult = userManager.Create(newUser, "test12");

            // Create a Test League.
            League testLeague = new League();
            testLeague.Name = "Test League 1";
            testLeague.Public = true;
            testLeague.PointsForAWin = 3;
            testLeague.PointsForADraw = 1;
            testLeague.PointsForALoss = 0;
            testLeague.OwnerName = "TestUser";

            // Create some Test Sides.
            List<Side> testLeagueTeams = new List<Side>();
            testLeagueTeams.Add(new Side { Name = "Test Side 1", OwnerName = "TestUser" });
            testLeagueTeams.Add(new Side { Name = "Test Side 2", OwnerName = "TestUser" });
            testLeagueTeams.Add(new Side { Name = "Test Side 3", OwnerName = "TestUser" });
            testLeagueTeams.Add(new Side { Name = "Test Side 4", OwnerName = "TestUser" });

            // Add the Sides to the League.
            testLeague.Members = testLeagueTeams;

            // Add the League to the database.
            context.Leagues.Add(testLeague);

            // Save the changes.
            context.SaveChanges();

            // Add some Test matches.
            context.Matches.Add(new Match(testLeague, testLeague.Members.Find(t => t.Name == "Test Side 2"), 1,
                                            testLeague.Members.Find(t => t.Name == "Test Side 1"), 9));
            context.Matches.Add(new Match(testLeague, testLeague.Members.Find(t => t.Name == "Test Side 1"), 4,
                                            testLeague.Members.Find(t => t.Name == "Test Side 2"), 6));
            context.Matches.Add(new Match(testLeague, testLeague.Members.Find(t => t.Name == "Test Side 2"), 4,
                                            testLeague.Members.Find(t => t.Name == "Test Side 1"), 3));

            // Save again.
            context.SaveChanges();
        }

        private void AddNewUser()
        {
            // Create our Demo User
            ApplicationUser newUser = new ApplicationUser { UserName = "NewUser", Email = "newuser@myemail.com" };
            IdentityResult testResult = userManager.Create(newUser, "newuser12");

            // Create the Demo League.
            League newLeague = new League();
            newLeague.Name = "My New League";
            newLeague.Description = "My First League";
            newLeague.Public = true;
            newLeague.PointsForAWin = 3;
            newLeague.PointsForADraw = 1;
            newLeague.PointsForALoss = 0;
            newLeague.OwnerName = "NewUser";

            // Create the Leagues Teams.
            List<Side> newLeagueTeams = new List<Side>();
            newLeagueTeams.Add(new Side { Name = "Team 1", OwnerName = "NewUser" });
            newLeagueTeams.Add(new Side { Name = "Team 2", OwnerName = "NewUser" });
            newLeagueTeams.Add(new Side { Name = "Team 3", OwnerName = "NewUser" });
            newLeagueTeams.Add(new Side { Name = "Team 4", OwnerName = "NewUser" });

            // Add Teams to the League
            newLeague.Members = newLeagueTeams;

            // Add the Leagues to the database.
            context.Leagues.Add(newLeague);

            // Save to database.
            context.SaveChanges();

            // Add the Demo Matches.
            context.Matches.Add(new Match(newLeague, newLeague.Members.Find(t => t.Name == "Team 1"), 3,
                                            newLeague.Members.Find(t => t.Name == "Team 2"), 1));
            context.Matches.Add(new Match(newLeague, newLeague.Members.Find(t => t.Name == "Team 3"), 1,
                                            newLeague.Members.Find(t => t.Name == "Team 4"), 2));
            context.Matches.Add(new Match(newLeague, newLeague.Members.Find(t => t.Name == "Team 3"), 3,
                                            newLeague.Members.Find(t => t.Name == "Team 1"), 2));
            context.Matches.Add(new Match(newLeague, newLeague.Members.Find(t => t.Name == "Team 2"), 1,
                                            newLeague.Members.Find(t => t.Name == "Team 4"), 1));
            context.Matches.Add(new Match(newLeague, newLeague.Members.Find(t => t.Name == "Team 4"), 2,
                                            newLeague.Members.Find(t => t.Name == "Team 1"), 3));
            context.Matches.Add(new Match(newLeague, newLeague.Members.Find(t => t.Name == "Team 2"), 1,
                                            newLeague.Members.Find(t => t.Name == "Team 3"), 0));

            //  Save again.
            context.SaveChanges();
        }

        private void AddAdmin()
        {
            // Create the Admin user.
            ApplicationUser newUser = new ApplicationUser { UserName = "Admin", Email = "admin@jokedb.net" };
            IdentityResult adminResult = userManager.Create(newUser, "admin12");

            // Create the Admin Role and add the Admin user to the role.
            if (newUser != null)
            {
                string role = "Admin";
                if (!roleManager.RoleExists(role))
                {
                    var roleResult = roleManager.Create(new IdentityRole(role));
                }
                if (!userManager.IsInRole<ApplicationUser, string>(newUser.Id, role))
                {
                    userManager.AddToRole<ApplicationUser, string>(newUser.Id, role);
                }
            }

            // Save user to database.
            context.SaveChanges();
        }

        private void AddPremierLeague()
        {
            // Create the user for the Premier League.
            ApplicationUser newUser = new ApplicationUser { UserName = "PremierLeagueManager", Email = "premier@jokedb.net" };
            IdentityResult premResult = userManager.Create(newUser, "prem12");

            // Create the League.
            League premLeague = new League();
            premLeague.Name = "Premier League";
            premLeague.Public = true;
            premLeague.OwnerName = "PremierLeagueManager";
            premLeague.PointsForAWin = 3;
            premLeague.PointsForADraw = 1;
            premLeague.PointsForALoss = 0;

            // Add the Teams to the League.
            AddPremierLeagueTeams(premLeague);

            // Add the League to the database.
            context.Leagues.Add(premLeague);

            // Save the changes.
            context.SaveChanges();

            // Add the matches to the League.
            AddAugustResults(premLeague);

            // Save again.
            context.SaveChanges();
        }

        private void AddPremierLeagueTeams(League premleague)
        {
            //Create a list of Sides and add each Premier League Team.
            List<Side> PremLeagueTeams = new List<Side>();
            PremLeagueTeams.Add(new Side { Name = "Arsenal", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Aston Villa", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Bournemouth", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Chelsea", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Crystal Palace", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Everton", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Leicester", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Liverpool", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Man City", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Man Utd", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Newcastle", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Norwich", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Southampton", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Spurs", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Stoke", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Sunderland", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Swansea", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "Watford", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "West Brom", OwnerName = "PremierLeagueManager" });
            PremLeagueTeams.Add(new Side { Name = "West Ham", OwnerName = "PremierLeagueManager" });

            // Now add the list to the League.
            premleague.Members = PremLeagueTeams;
        }

        private void AddAugustResults(League premleague)
        {
            // 08/08/15
            context.Matches.Add(new Match(premleague, premleague.Members.Find(t => t.Name == "Man Utd"), 1,
                                                        premleague.Members.Find(t => t.Name == "Spurs"), 0));
            context.Matches.Add(new Match(premleague, premleague.Members.Find(t => t.Name == "Norwich"), 1,
                                                        premleague.Members.Find(t => t.Name == "Crystal Palace"), 3));
            context.Matches.Add(new Match(premleague, premleague.Members.Find(t => t.Name == "Leicester"), 4,
                                                        premleague.Members.Find(t => t.Name == "Sunderland"), 2));
            context.Matches.Add(new Match(premleague, premleague.Members.Find(t => t.Name == "Everton"), 2,
                                                        premleague.Members.Find(t => t.Name == "Watford"), 2));
            context.Matches.Add(new Match(premleague, premleague.Members.Find(t => t.Name == "Bournemouth"), 0,
                                                        premleague.Members.Find(t => t.Name == "Aston Villa"), 1));
            context.Matches.Add(new Match(premleague, premleague.Members.Find(t => t.Name == "Chelsea"), 2,
                                                        premleague.Members.Find(t => t.Name == "Swansea"), 2));

        }
    }
}