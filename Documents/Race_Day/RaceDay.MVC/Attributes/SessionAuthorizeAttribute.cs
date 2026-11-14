using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.MVC.Attributes;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    public string? Role { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var token = session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        if (!string.IsNullOrEmpty(Role))
        {
            var userRole = session.GetString("UserRole");
            if (userRole != Role)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
           
        }
        base.OnActionExecuting(context);
    }
}
