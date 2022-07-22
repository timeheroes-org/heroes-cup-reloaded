using Piranha;

namespace HeroesCup.Web.Common
{
    public class WebUtils : IWebUtils
    {
        private readonly IConfiguration _configuration;

        public WebUtils(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GetUrlBase(HttpContext httpContext)
        {
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        }

        public async Task<System.Globalization.CultureInfo> GetCulture(IApi api)
        {
            var defaultLanguage = await api.Languages.GetDefaultAsync();
            var siteCulture = defaultLanguage.Culture;
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(siteCulture);
            return culture;
        }
    }
}
