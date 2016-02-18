using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LeagueManager.Models;
using Microsoft.AspNet.Identity;


namespace LeagueManager.Controllers
{
    // This secion all needs the user to be logged on as an Administrator
    [Authorize(Roles = "Admin")]
    public class AdministratorController : Controller
    {
        private LeagueManagerDB db = new LeagueManagerDB();

        // GET: Adminstrator
        // Displays the leagues belonging to the current user.
        public ActionResult Index()
        {
            return View(db.Leagues.ToList());
        }

        // GET: Adminstrator/LeagueDetails/5
        // Displays the details of the selected league.
        public ActionResult LeagueDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // GET: Adminstrator/CreateLeague
        // Set up for creating a new league.
        public ActionResult CreateLeague()
        {
            // now build the view model
            CreateEditLeagueViewModel createleague = new CreateEditLeagueViewModel(db, null, User.Identity.GetUserName());

            // pass the view model to the view
            return View(createleague);
        }

        // POST: Adminstrator/CreateLeague
        // Recieve the details for creating a new league and save it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLeague(CreateEditLeagueViewModel createleague)
        {
            if (ModelState.IsValid)
            {
                League league = new League();
                createleague.Initalise(league, User.Identity.GetUserName());

                //Add sides to league Members.
                foreach (int item in createleague.SelectedSides)
                {
                    Side s = db.Sides.Find(item);
                    league.Members.Add(s);
                }
                db.Leagues.Add(league);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(createleague);
        }

        // GET: Adminstrator/EditLeague/5
        // Display the details of a league to edit.
        public ActionResult EditLeague(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }

            // now build the view model
            CreateEditLeagueViewModel editleague = new CreateEditLeagueViewModel(db, id, User.Identity.GetUserName());

            // pass the view model to the view
            return View(editleague);
        }

        // POST: Adminstrator/EditLeague/5
        // Recieve the new details and save them to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLeague(CreateEditLeagueViewModel editleague)
        {
            if (ModelState.IsValid)
            {
                League league = db.Leagues.Find(editleague.League.Id);
                //Initalize our league with the data collected from our edit form, but keep OwnerName the same.
                editleague.Initalise(league, editleague.League.OwnerName);
                db.Entry(league).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(editleague);
        }

        // GET: Adminstrator/EditLeagueMembers/5
        // Editing which sides are members of the league.
        public ActionResult EditLeagueMembers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }

            //Initialise our view model
            EditLeagueMembersViewModel viewModel = new EditLeagueMembersViewModel(league, db, league.OwnerName);

            return View(viewModel);
        }

        // Called from EditLeagueMembers, we add the side then reload EditLeagueMembers.
        public ActionResult AddSideToLeague(int LeagueId, int SideId)
        {
            // Find the League and  the Side.
            League league = db.Leagues.Find(LeagueId);
            Side side = db.Sides.Find(SideId);

            // Add the Side to the League and save to the database.
            league.Members.Add(side);
            db.Entry(league).State = EntityState.Modified;
            db.SaveChanges();

            // Go back to Editing the League Members.
            return RedirectToAction("EditLeagueMembers", new { id = LeagueId });
        }

        // Called from EditLeagueMembers, we remove the side, delete associated matches and then reload EditLeagueMembers.
        public ActionResult RemoveSideFromLeague(int LeagueId, int SideId)
        {
            // Find the League and  the Side.
            League league = db.Leagues.Find(LeagueId);
            Side side = db.Sides.Find(SideId);

            // Remove the Side to the League and save to the database.
            league.Members.Remove(side);
            db.Entry(league).State = EntityState.Modified;

            // Find the Matches in the League that the Side is a part of.
            List<Match> matchesForSideInLeague = db.Matches.Where(m => m.LeagueId == LeagueId &&
                                                                    (m.Side1Id == SideId || m.Side2Id == SideId))
                                                        .ToList();

            // Delete all of the Matches and Save to the Database.
            foreach(Match match in matchesForSideInLeague)
            {
                db.Matches.Remove(match);
            }
            db.SaveChanges();

            return RedirectToAction("EditLeagueMembers", new { id = LeagueId });
        }

        // GET: Adminstrator/AddNewSideToLeague/5
        // Create a new side and add it to the specified League.
        public ActionResult AddNewSideToLeague(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }

            //Initialise our view model
            CreateEditSideViewModel viewModel = new CreateEditSideViewModel(db, null, league.OwnerName, id);

            return View(viewModel);
        }

        // POST: Adminstrator/AddNewSideToLeague/5
        // Recieve the details for creating a new side and save it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewSideToLeague(CreateEditSideViewModel createside)
        {
            if (ModelState.IsValid)
            {
                //Create the new side and initialise it.
                Side side = new Side();
                createside.Initalise(side);
                //Find the League we want to to put the side in and add it.
                League l = db.Leagues.Find(createside.IdOfLeagueToAddTo);
                side.LeaguesIn.Add(l);
                // Add the side to the database and save.
                db.Sides.Add(side);
                db.SaveChanges();
                //Go back to the League view/
                return RedirectToAction("Index");
            }

            return View(createside);
        }


        // GET: Adminstrator/DeleteLeague/5
        // Show the details of the league to confirm the delete.
        public ActionResult DeleteLeague(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            League league = db.Leagues.Find(id);

            if (league == null)
            {
                return HttpNotFound();
            }

            // Find out how many Matches are in the League and tell the View.
            List<Match> sidematches = db.Matches.Where(m => m.Side1Id == id || m.Side2Id == id)
                                    .ToList();
            ViewBag.noOfMatches = sidematches.Count;

            return View(league);
        }

        // POST: Adminstrator/DeleteLeague/5
        // Delete this league from the database, this will delete all the Matches as well.
        [HttpPost, ActionName("DeleteLeague")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            League league = db.Leagues.Find(id);
            db.Leagues.Remove(league);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Adminstrator/ViewLeagueTable/5
        // Show the league table for the selected league.
        public ActionResult ViewLeagueTable(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LeagueResults results = new LeagueResults(db, (int)id);

            return View(results);
        }

        // GET: Adminstrator/ViewSides
        // View all the sides.
        public ActionResult ViewSides()
        {
            return View(db.Sides.ToList());
        }

        // GET: Adminstrator/SideDetails/5
        // Show the details for the selected side.
        public ActionResult SideDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Side side = db.Sides.Find(id);
            if (side == null)
            {
                return HttpNotFound();
            }
            return View(side);
        }

        // GET: Adminstrator/CreateSide
        // Set up for creating a new side.
        public ActionResult CreateSide()
        {
            // now build the view model
            CreateEditSideViewModel createside = new CreateEditSideViewModel(db, null, User.Identity.GetUserName());

            // pass the view model to the view
            return View(createside);
        }

        // POST: Adminstrator/CreateSide
        // Recieve the details for creating a new side and save it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSide(CreateEditSideViewModel createside)
        {
            if (ModelState.IsValid)
            {
                Side side = new Side();
                side.Name = createside.Side.Name;
                side.OwnerName = User.Identity.GetUserName(); 
                db.Sides.Add(side);
                db.SaveChanges();
                return RedirectToAction("ViewSides");
            }

            return View(createside);
        }

        // GET: Adminstrator/EditSide/5
        // Display the details of a side to edit.
        public ActionResult EditSide(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Side side = db.Sides.Where(s => s.Id == id)
                                .Include("LeaguesIn")
                                .FirstOrDefault();
            if (side == null)
            {
                return HttpNotFound();
            }

            // now build the view model
            CreateEditSideViewModel editside = new CreateEditSideViewModel(db, id, User.Identity.GetUserName());

            // pass the view model to the view
            return View(side);
        }

        // POST: Adminstrator/EditSide/5
        // Recieve the new details and save them to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSide(Side editside)
        {
            if (ModelState.IsValid)
            {
                Side side = db.Sides.Find(editside.Id);
                side.Name = editside.Name;
                side.OwnerName = editside.OwnerName;
 
                db.Entry(side).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewSides");
            }
            return View(editside);
        }

        // GET: Adminstrator/DeleteSide/5
        // Show the details of the side to confirm the delete.
        public ActionResult DeleteSide(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Side side = db.Sides.Find(id);
            if (side == null)
            {
                return HttpNotFound();
            }
            return View(side);
        }

        // POST: Adminstrator/DeleteSide/5
        // Delete this side from the database.
        [HttpPost, ActionName("DeleteSide")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSideConfirmed(int id)
        {
            //Find out if there are any matches for this side.
            List<Match> sidematches = db.Matches.Where(m => m.Side1Id == id || m.Side2Id == id)
                                                .ToList();
            if (sidematches.Count != 0)
            {
                // We have matches for this side go back to the user to confirm the delete.
                return RedirectToAction("ConfirmSideAndMatchesDelete", new { id = id });
            }
            else
            {
                //We have no matches so just delete the side.
                Side side = db.Sides.Find(id);
                db.Sides.Remove(side);
                db.SaveChanges();
                return RedirectToAction("ViewSides");
            }
        }

        // GET: Adminstrator/ConfirmSideAndMatchesDelete/5
        // User has selected a Side to delete that has matches associated with it.
        // Confirm with user that they want to delete the side and assocoiated matches.
        public ActionResult ConfirmSideAndMatchesDelete(int? id )
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<Match> sidematches = db.Matches.Where(m => m.Side1Id == id || m.Side2Id == id)
                                                .ToList();

            ViewBag.noOfMatches = sidematches.Count;
            Side side = db.Sides.Find(id);

            if (side == null)
            {
                return HttpNotFound();
            }
            return View(side);
        }

        // POST: Adminstrator/ConfirmSideAndMatchesDelete/5
        // Delete this side from the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmSideAndMatchesDelete(Side deleteside)
        {
            //Delete has been confirmed
            //Find all of the matches for this side.
            List<Match> sidematches = db.Matches.Where(m => m.Side1Id == deleteside.Id || m.Side2Id == deleteside.Id)
                                                .ToList();
            //Loop round the list and remove them all.
            foreach(Match match in sidematches)
            {
                db.Matches.Remove(match);
            }
            //Now we can delete the side.
            Side side = db.Sides.Find(deleteside.Id);
            db.Sides.Remove(side);
            db.SaveChanges();
            return RedirectToAction("ViewSides");
        }


        // GET: Adminstrator/ViewLeagueMatches/5
        // View the matches in the selected league.
        public ActionResult ViewLeagueMatches(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ListLeagueMatchesViewModel model = new ListLeagueMatchesViewModel();
            League league = db.Leagues.Find(id);
            model.LeagueName = league.Name;
            model.LeagueId = league.Id;
            model.ListOfMatches = db.Matches.Where(l => l.League.Id == id)
                                          .Include(m => m.League)
                                          .Include(m => m.Side1)
                                          .Include(m => m.Side2)
                                          .ToList();
            return View(model);
        }

        // GET: Adminstrator/MatchDetails/5
        // Show the details for the selected match.
        public ActionResult MatchDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Match match = db.Matches.Where(m => m.MatchId == id)
                                    .Include(m => m.League)
                                    .Include(m => m.Side1)
                                    .Include(m => m.Side2)
                                    .FirstOrDefault();
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }


        // GET: Adminstrator/CreateMatch
        // Set up for creating a new match.
        public ActionResult CreateMatch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // now build the view model
            CreateEditMatchViewModel creatematch = new CreateEditMatchViewModel(db, null);

            creatematch.LeagueMatchIsIn = db.Leagues.Find(id);

            // pass the view model to the view
            return View(creatematch);
        }

        // POST: Adminstrator/CreateMatch
        // Recieve the details for creating a new match and save it to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMatch(CreateEditMatchViewModel creatematch)
        {
            if (ModelState.IsValid)
            {
                //Create a new Match and populate it with the data from the post.
                Match match = new Match();
                match.League = db.Leagues.Find(creatematch.IdOfLeagueMatchIsIn);
                match.Side1 = db.Sides.Find(creatematch.SelectedSide1.FirstOrDefault());
                match.Side1Score = creatematch.MatchToEdit.Side1Score;
                match.Side2 = db.Sides.Find(creatematch.SelectedSide2.FirstOrDefault());
                match.Side2Score = creatematch.MatchToEdit.Side2Score;

                //Add and save the match and return to the list view of matches.
                db.Matches.Add(match);
                db.SaveChanges();
                return RedirectToAction("ViewLeagueMatches", new { id = creatematch.IdOfLeagueMatchIsIn });
            }

            return View(creatematch);
        }

        // GET: Adminstrator/EditMatch/5
        // Display the details of a match to edit.
        public ActionResult EditMatch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreateEditMatchViewModel editmatch = new CreateEditMatchViewModel(db, id);

            if (editmatch.MatchToEdit == null)
            {
                return HttpNotFound();
            }
            return View(editmatch);
        }

        // POST: Adminstrator/EditMatch/5
        // Recieve the new details and save them to the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMatch(CreateEditMatchViewModel editmatch)
        {
            if (ModelState.IsValid)
            {
                //Find our match and change it with data from the post.
                Match match = db.Matches.Find(editmatch.IdOfMatch);
                match.League = db.Leagues.Find(editmatch.IdOfLeagueMatchIsIn);
                match.Side1 = db.Sides.Find(editmatch.SelectedSide1.FirstOrDefault());
                match.Side1Score = editmatch.MatchToEdit.Side1Score;
                match.Side2 = db.Sides.Find(editmatch.SelectedSide2.FirstOrDefault());
                match.Side2Score = editmatch.MatchToEdit.Side2Score;

                //Add and save the match and return to the list view of matches.
                db.Entry(match).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewLeagueMatches", new { id = editmatch.IdOfLeagueMatchIsIn });
            }
            return View(editmatch);
        }

        // GET: Adminstrator/DeleteMatch/5
        // Show the details of the match to confirm the delete.
        public ActionResult DeleteMatch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Where(m => m.MatchId == id)
                                    .Include(m => m.League)
                                    .Include(m => m.Side1)
                                    .Include(m => m.Side2)
                                    .FirstOrDefault();
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Adminstrator/DeleteMatch/5
        // Delete this match from the database.
        [HttpPost, ActionName("DeleteMatch")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMatchConfirmed(int id)
        {
            try
            {
                Match match = db.Matches.Where(m => m.MatchId == id)
                                        .First();
                db.Matches.Remove(match);
                db.SaveChanges();
                return RedirectToAction("ViewLeagueMatches", new { id = match.LeagueId });

            }
            catch
            {
                //Could not find the match to delete.
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
