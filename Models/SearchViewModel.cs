using Microsoft.AspNetCore.Mvc;

namespace HeroesCup.Web.Models;

public class SearchViewModel
{
    [BindProperty(Name = "search")]
    public string SearchTerm { get; set; }
    [BindProperty(Name = "search-token")]
    public string Token { get; set; }
}