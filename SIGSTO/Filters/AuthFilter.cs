using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SIGSTO.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        private readonly string? _requiredRole;

        public AuthFilter(string? requiredRole = null)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (_requiredRole != null)
            {
                var role = context.HttpContext.Session.GetString("Role");
                if (role != _requiredRole)
                {
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
