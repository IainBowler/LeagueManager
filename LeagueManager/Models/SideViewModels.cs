using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeagueManager.Models
{
    public class CreateEditSideViewModel
    {
        //The side we are editing.
        public virtual Side Side { get; set; }
        //If we are creating a new Side the Id of the League it is to be added to.
        public virtual int IdOfLeagueToAddTo { get; set; }

        public CreateEditSideViewModel()
        {
            Side = new Side();
        }

        public CreateEditSideViewModel(LeagueManagerDB db, int? id, string username)
        {
            //If we are creating as opposed to editing id will be null
            if(id != null)
            {
                //Get the Side we are editing
                Side = db.Sides.Find(id);
            }

            else
            {
                Side = new Side();
                Side.OwnerName = username;
            }
        }

        public CreateEditSideViewModel(LeagueManagerDB db, int? id, string username, int? leagueToAddTo) : this(db, id, username)
        {
            if(leagueToAddTo != null)
            {
                IdOfLeagueToAddTo = (int)leagueToAddTo;
            }
        }

        public void Initalise(Side side)
        {
            side.Name = Side.Name;
            side.OwnerName = Side.OwnerName;
        }
    }

    public class EditLeagueMembersViewModel
    {
        //Collection of Information about the sides
        public List<SideMatchLeagueInfo> SideInfo { get; set; }
        public League League { get; set; }

        public EditLeagueMembersViewModel(League league, LeagueManagerDB db, string username)
        {
            League = league;

            //We need to create a list of all the sides for this user with information on the sides.
            string userName = username;
            List<Side> UserSides = db.Sides.Where(s => s.OwnerName == userName).ToList();

            // Find out about whether it's in the League and how many league matches are associated with it.
            SideInfo = new List<SideMatchLeagueInfo>();

            foreach (Side s in UserSides)
            {
                SideMatchLeagueInfo sideInfo = new SideMatchLeagueInfo(s, league, db);
                SideInfo.Add(sideInfo);
            }
        }
    }
}