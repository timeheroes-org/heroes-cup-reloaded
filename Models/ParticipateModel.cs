using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace HeroesCup.Web.Models;

public class ParticipateModel
{
    [BindProperty(Name = "participant-name")]
    public string Name { get; set; }
    [BindProperty(Name = "participant-lastname")]
    public string LastName { get; set; }
    [BindProperty(Name = "participant-type")]
    public string Type { get; set; }
    [BindProperty(Name = "participant-school")]
    public string School { get; set; }
    [BindProperty(Name = "participant-grade")]
    public string Grade { get; set; }
    [BindProperty(Name = "participant-email")]
    public string Email { get; set; }
    [BindProperty(Name = "participant-more")]
    public string More { get; set; }
    [BindProperty(Name = "participant-location")]
    public string Location { get; set; }
    [BindProperty(Name = "participant-token")]
    public string Token { get; set; }


}