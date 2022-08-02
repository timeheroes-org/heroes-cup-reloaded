using HeroesCup.Data.Models;
using System;
using System.Collections.Generic;

namespace HeroesCup.Web.ClubsModule.Models
{
    public class HeroListModel
    {
        public IEnumerable<HeroListItem> Heroes { get; set; }
    }

    public class HeroListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ClubId { get; set; }

        public string ClubName { get; set; }
    }
}