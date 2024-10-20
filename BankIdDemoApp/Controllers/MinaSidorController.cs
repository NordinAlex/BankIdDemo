using BankIdDemoApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankIdDemoApp.Controllers
{
    
    [BankIdAuthorize]
    public class MinaSidorController : Controller
    {
        // GET: MinaSidor/Index
        public IActionResult Index()
        {
            // Hämta completionData från sessionen.
            var completionDataJson = HttpContext.Session.GetString("CompletionData");

            // Om ingen data finns i sessionen, visa ett felmeddelande.
            if (string.IsNullOrEmpty(completionDataJson))
            {
                ViewData["ErrorMessage"] = "Ingen verifieringsdata hittades. Vänligen försök igen.";
                return View("Error");
            }

            // Skicka JSON-strängen till vyn.
            ViewBag.JsonData = completionDataJson;

            return View();
        }

        // GET: MinaSidor/Error        
        public IActionResult Error()
        {
            // Visar en enkel felvy om något går fel.
            return View();
        }
    }
}
