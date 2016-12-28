using LeagueManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LeagueManager.Controllers
{
    public class PublicLeaguesController : Controller
    {
        private LeagueManagerDB db = new LeagueManagerDB();

        // GET: PublicLeagues
        public ActionResult Index()
        {
            return View(db.Leagues.Where(l => l.Public == true).ToList());
        }

        public ActionResult ViewLeagueTable(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LeagueResults results = new LeagueResults(db, (int)id);

            return View(results);
        }
    }
}
