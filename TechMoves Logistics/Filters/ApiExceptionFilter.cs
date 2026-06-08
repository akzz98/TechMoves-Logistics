using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechMoves_Logistics.Services;

namespace TechMoves_Logistics.Filters
{
    // Handles API failures from HttpClient calls in MVC controllers.
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not ApiClientException apiException)
                return;

            // Session token expired or missing — send user back to login.
            if (apiException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                context.HttpContext.Session.Clear();
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    new { returnUrl = context.HttpContext.Request.Path });
                context.ExceptionHandled = true;
                return;
            }

            context.HttpContext.TempData["ErrorMessage"] = apiException.Message;
            context.Result = new RedirectToActionResult("Index", "Home", null);
            context.ExceptionHandled = true;
        }
    }
}
