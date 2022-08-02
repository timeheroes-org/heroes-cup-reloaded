using HeroesCup.Data.Models;
using System;
using System.Collections.Generic;

namespace HeroesCup.Web.ClubsModule.Models
{
    public class HeroEditModel
    {
        public Hero Hero { get; set; }

        public Guid ClubId { get; set; }

        public IEnumerable<Mission> Missions { get; set; }

        public IEnumerable<Club> Clubs { get; set; }
    }
}