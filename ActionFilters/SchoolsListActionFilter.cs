using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HeroesCup.ActionFilters;

public class SchoolsListAttribute: ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var clubs = context.HttpContext.RequestServices.GetService<IClubsService>();
        if (clubs != null) ((Controller)context.Controller).ViewBag.Schools = clubs.GetSchools().Result.OrderBy(s=>s);
        base.OnActionExecuting(context);
    }
}