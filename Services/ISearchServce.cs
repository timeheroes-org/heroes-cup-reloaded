using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public interface ISearchServce
{
    Task<SearchResponseModel> Search(string searchTerm);
}