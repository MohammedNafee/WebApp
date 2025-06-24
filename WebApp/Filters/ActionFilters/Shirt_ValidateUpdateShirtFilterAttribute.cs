using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;

namespace WebApp.Filters.ActionFilters
{
    public class Shirt_ValidateUpdateShirtFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var id = context.ActionArguments["id"] as int?;

            var shirt = context.ActionArguments["shirt"] as Shirt;

            if (shirt != null && id != shirt.ShirtId)
            {
                var problemDetails = new ProblemDetails()
                {
                    Title = "Invalid Shirt ID",
                    Detail = "The provided shirt ID does not match the shirt being updated.",
                    Status = StatusCodes.Status400BadRequest
                };

                context.ModelState.AddModelError("ShirtId", "The provided shirt ID does not match the shirt being updated.");

                context.Result = new BadRequestObjectResult(problemDetails);
            }


        }
    }
}
