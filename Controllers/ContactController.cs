using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProjektLABDetailing.Models;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ProjektLABDetailing.Controllers
{
    public class ContactController : Controller
    {
        private readonly IConfiguration _configuration;

        public ContactController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitContactForm(ContactFormModel model)
        {
            var errors = ValidateContactForm(model);
            if (errors.Count == 0)
            {
                try
                {
                    var fromAddress = new MailAddress(_configuration["Smtp:Username"], "Nowy Klient - Formularz kontaktowy");
                    var toAddress = new MailAddress("detailingfirma@outlook.com", "Firma Detailing");
                    const string subject = "Nowa wiadomość od potencjalnego klienta!";
                    string body = $"<strong>Imię i Nazwisko:</strong> {model.FullName}<br /><br />" +
                                  $"<strong>Email:</strong> {model.Email}<br /><br />" +
                                  $"<strong>Numer Telefonu:</strong> {model.PhoneNumber}<br /><br />" +
                                  $"<strong>Usługa:</strong> {model.SelectedService}<br /><br />" +
                                  $"<strong>Wiadomość od użytkownika:</strong> {model.Message}";

                    var smtp = new SmtpClient
                    {
                        Host = _configuration["Smtp:Host"],
                        Port = int.Parse(_configuration["Smtp:Port"]),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"])
                    };

                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(message);
                    }

                    ViewBag.Message = "Twoja wiadomość została wysłana!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Wystąpił błąd podczas wysyłania wiadomości: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Errors = errors;
            }

            return View("Index", model);
        }

        private List<string> ValidateContactForm(ContactFormModel model)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                errors.Add("Imię i nazwisko jest wymagane.");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                errors.Add("Email jest wymagany.");
            }
            else if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add("Nieprawidłowy format adresu email.");
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                errors.Add("Numer telefonu jest wymagany.");
            }

            if (string.IsNullOrWhiteSpace(model.SelectedService))
            {
                errors.Add("Usługa jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(model.Message))
            {
                errors.Add("Treść wiadomości jest wymagana.");
            }

            if (!model.AcceptTerms)
            {
                errors.Add("Musisz wyrazić zgodę na przetwarzanie danych osobowych.");
            }

            return errors;
        }
    }
}
