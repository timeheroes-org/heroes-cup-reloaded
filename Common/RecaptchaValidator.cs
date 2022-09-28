using System.Net;
using HeroesCup.Web.Models;
using Newtonsoft.Json;

namespace HeroesCup.Web.Common;

public static class RecaptchaValidator
{
    public static async Task<bool> Verify(IConfiguration config, string token)
    {
        var url =
            $"https://www.google.com/recaptcha/api/siteverify?secret={config.GetSection("GoogleRecaptcha").GetSection("SecretKey").Value}&response={token}";

        using (var client = new HttpClient())
        {
            var httpResult = await client.GetAsync(url);
            if (httpResult.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var responseString = await httpResult.Content.ReadAsStringAsync();

            var googleResult = JsonConvert.DeserializeObject<GoogleReCaptchaResponseModel>(responseString);

            return googleResult.Success && googleResult.Score >= 0.5;
        }
    }
}