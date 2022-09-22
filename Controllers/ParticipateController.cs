using System.Net;
using HeroesCup.Web.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;

namespace HeroesCup.Web.ClubsModule.Controllers;

[Route("participate")]
public class ParticipateController : Controller
{
    private IConfiguration _config;

    public ParticipateController(IConfiguration config)
    {
        this._config = config;
    }


    private async Task<bool> Verify(string token)
    {
        var url =
            $"https://www.google.com/recaptcha/api/siteverify?secret={_config.GetSection("GoogleRecaptcha").GetSection("SecretKey").Value}&response={token}";

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

    [HttpPost]
    public async Task<IActionResult> Participate([FromForm] ParticipateModel model)
    {
        if (ModelState.IsValid)
        {
            var googleReCaptchaResult = await this.Verify(model.Token);
            if (googleReCaptchaResult)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_config.GetSection("OutgoingEmailSettings").GetSection("EmailFrom").Value, _config.GetSection("OutgoingEmailSettings").GetSection("EmailAccount").Value));
                emailMessage.To.Add(MailboxAddress.Parse("hello@timeheroes.org"));
                emailMessage.Subject = "Нова заявка за участие в Купата на героите";
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = $"Име {model.Name}, Фамилия {model.LastName}\n" +
                    $"Град: {model.Location}, Училище: {model.School}, Email: {model.Email}\n" +
                    $"Клас: {model.Grade}, Ти си: {model.Type}\n" +
                    $"Още за теб: {model.More}\n" };
                using (var client = new SmtpClient())
                {
                    try
                    {
                        client.Connect(_config.GetSection("OutgoingEmailSettings").GetSection("EmailServer").Value, Int32.Parse(_config.GetSection("OutgoingEmailSettings").GetSection("EmailPort").Value), SecureSocketOptions.Auto);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(_config.GetSection("OutgoingEmailSettings").GetSection("EmailAccount").Value,_config.GetSection("OutgoingEmailSettings").GetSection("EmailPassword").Value);
                        client.Send(emailMessage);
                        client.Disconnect(true);
                    }
                    catch
                    {
                        //log an error message or throw an exception or both.
                        throw;
                    }
                    finally
                    {
                        client.Disconnect(true);
                        client.Dispose();
                    }
                }

                return Ok();
            }
        }

        return BadRequest();
    }
}