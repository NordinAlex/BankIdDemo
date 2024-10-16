using BankIdDemoApp.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BankIdDemoApp.Extensions
{
    public class BankIdAuthorizeAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var bankIdService = (IBankIdService)context.HttpContext.RequestServices.GetService(typeof(IBankIdService));

            // Kontrollera om användaren har genomgått BankID-verifiering (kanske genom session eller token)
            var completionData = context.HttpContext.Session.GetString("CompletionData");

            if (string.IsNullOrEmpty(completionData))
            {
                //// Om användaren inte är verifierad, omdirigera till BankID-login
                //context.Result = new RedirectToActionResult("Login", "Auth", null);
                // Om användaren inte är verifierad, visa 401-sidan
                context.Result = new ViewResult
                {
                    ViewName = "Unauthorized",
                };
                return;
            }
            
            // Om allt är bra, fortsätt med åtgärden
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
