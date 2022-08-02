using System.Globalization;
using Piranha;

namespace HeroesCup.Web.Common;

public interface IWebUtils
{
    string GetUrlBase(HttpContext httpContext);

    Task<CultureInfo> GetCulture(IApi api);
}