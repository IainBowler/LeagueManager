using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeagueManager.Models
{
    public class CreateEditLeagueViewModel
    {
        //The league we are editing.
        public virtual League League { get; set; }
        //So the listbox knows which sides are part of the league.
        public virtual List<int> SelectedSides { get; set; }
        //All the sides to populate the list box.
        public virtual List<SelectListItem> Sides { get; set; }

        public CreateEditLeagueViewModel()
        {
            SelectedSides = new List<int>();
            Sides = new List<SelectListItem>();
            League = new League();
        }

        public CreateEditLeagueViewModel(LeagueManagerDB db, int? id, string username)
        {
            //The sides that are already in the league if we are editing.
            SelectedSides = new List<int>();
            //Listbox of sides to add to the league.
            Sides = new List<SelectListItem>();
            //List of sides to create our listbox from
            List<Side> sides;

            //If we are creating as opposed to editing id will be null
            if (id != null)
            {
                //Get the league we are editing
                League = db.Leagues.Find(id);

                //Populate the selected sides list.
                foreach (Side side in League.Members)
                {
                    SelectedSides.Add(side.Id);
                }
            }
            else
            {
                League = new League();
                if (username != "")
                {
                    // Set current user to be owner of the new League
                    League.OwnerName = username;
                }
            }
            
            //If we have a user name get that users sides, if not get all sides.
            if (username == "")
            {
                //Get all sides
                sides = db.Sides.ToList();
            }
            else
            {
                //Get only the sides this user has created.
                sides = db.Sides.Where(s => s.OwnerName == username).ToList();
            }

            //Populate a SelectList of all the sides.
            foreach (Side side in sides)
            {
                SelectListItem item = new SelectListItem();
                item.Value = side.Id.ToString();
                item.Text = side.Name;
                Sides.Add(item);
            }
        }

        public void Initalise(League newleague, string username)
        {
            //Set up a passed in new league and copies the values gained from the user.
            newleague.Name = League.Name;
            newleague.Description = League.Description;
            newleague.OwnerName = username;
            newleague.PointsForAWin = League.PointsForAWin;
            newleague.PointsForADraw = League.PointsForADraw;
            newleague.PointsForALoss = League.PointsForALoss;
            newleague.Public = League.Public;
        }
    }
}