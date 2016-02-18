using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using LeagueManager.Models;

namespace LeagueManager.Controllers
{
    public class HomeController : Controller
    {
        private LeagueManagerDB db = new LeagueManagerDB();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Guide()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "For any enquiries contact:";

            return View();
        }

        [Authorize (Roles = "Admin")]
        public ActionResult Administrator()
        {
            return View();
        }

    }
}