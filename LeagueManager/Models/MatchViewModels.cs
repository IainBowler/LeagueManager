using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeagueManager.Models
{
    public class CreateEditMatchViewModel
    {
        //All the Leagues to populate the list box.
        public virtual List<SelectListItem> AllLeagues { get; set; }
        //The match we are editing.
        public virtual Match MatchToEdit { get; set; }
        public virtual int IdOfMatch { get; set; }
        //The League the match is in.
        private League pLeagueMatchIsIn;
        public virtual int IdOfLeagueMatchIsIn { get; set; }
        //To get the two selcted sides
        public virtual List<int> SelectedSide1 { get; set; }
        public virtual List<int> SelectedSide2 { get; set; }
        //To get the selected League.
        public virtual List<int> SelectedLeague { get; set; }
        //Two SelectLists for the sides in the League.
        public virtual List<SelectListItem> Side1SelectList { get; set; }
        public virtual List<SelectListItem> Side2SelectList { get; set; }

        //Public interface to the League the match is in.
        public virtual League LeagueMatchIsIn
        {
            get { return pLeagueMatchIsIn; }

            set
            {
                pLeagueMatchIsIn = value;

                IdOfLeagueMatchIsIn = pLeagueMatchIsIn.Id;

                //Now we have a league we can generate two select list of Sides.
                foreach (Side side in pLeagueMatchIsIn.Members)
                {
                    SelectListItem item1 = new SelectListItem();
                    SelectListItem item2 = new SelectListItem();
                    item1.Value = side.Id.ToString();
                    item2.Value = side.Id.ToString();
                    item1.Text = side.Name;
                    item2.Text = side.Name;
                    if(side.Id == SelectedSide1.FirstOrDefault())
                    {
                        item1.Selected = true;
                    }
                    if (side.Id == SelectedSide2.FirstOrDefault())
                    {
                        item2.Selected = true;
                    }
                    Side1SelectList.Add(item1);
                    Side2SelectList.Add(item2);
                }
            }
        }

        public CreateEditMatchViewModel()
        {
            AllLeagues = new List<SelectListItem>();
            SelectedSide1 = new List<int>();
            SelectedSide2 = new List<int>();
            SelectedLeague = new List<int>();
            Side1SelectList = new List<SelectListItem>();
            Side2SelectList = new List<SelectListItem>();
            MatchToEdit = new Match();
            LeagueMatchIsIn = new League();
        }

        public CreateEditMatchViewModel(LeagueManagerDB db, int? id)
        {
            AllLeagues = new List<SelectListItem>();
            SelectedSide1 = new List<int>();
            SelectedSide2 = new List<int>();
            SelectedLeague = new List<int>();
            Side1SelectList = new List<SelectListItem>();
            Side2SelectList = new List<SelectListItem>();
            MatchToEdit = new Match();
            LeagueMatchIsIn = new League();

            //If we are creating as opposed to editing id will be null
            if (id != null)
            {
                //Get the match we are editing
                MatchToEdit = db.Matches.Where(m => m.MatchId == id)
                                        .Include(m => m.League)
                                        .Include(m => m.Side1)
                                        .Include(m => m.Side2)
                                        .FirstOrDefault();

                //Record the Id so we can find the Match later.
                IdOfMatch = (int)id;

                //Set the selected sides.
                SelectedSide1.Add(MatchToEdit.Side1.Id);
                SelectedSide2.Add(MatchToEdit.Side2.Id);

                //Set the League - This will populate the side SelectLists
                LeagueMatchIsIn = MatchToEdit.League;
            }

            //Get all the leagues.
            List<League> leagues = db.Leagues.ToList();

            //Populate a SelectList of all the Leagues.
            foreach (League league in leagues)
            {
                SelectListItem item = new SelectListItem();
                item.Value = league.Id.ToString();
                item.Text = league.Name;
                AllLeagues.Add(item);
            }
        }
    }

    public class ListLeagueMatchesViewModel
    {
        public List<Match> ListOfMatches { get; set; }
        public string LeagueName { get; set; }
        public int LeagueId { get; set; }
    }
}