using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;
using WebApp.Models.Repositories;

namespace WebApp.Filters.ActionFilters
{
    public class Shirt_ValidateCreatedShirtFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirt = context.ActionArguments["shirt"] as Shirt;

            if (shirt == null)
            {
                var problemDetails = new ProblemDetails()
                {
                    Title = "Invalid Shirt Object",
                    Detail = "The provided shirt object cannot be null.",
                    Status = StatusCodes.Status400BadRequest
                };

                context.ModelState.AddModelError("shirt", problemDetails.Detail);

                context.Result = new BadRequestObjectResult(problemDetails);
            }
            else if ( ShirtRepository.GetShirtByProperties(shirt.Brand ,shirt.Color, shirt.Size, shirt.Gender) != null)
            {
               var problemDetails = new ProblemDetails()
                {
                    Title = "Shirt Already Exists",
                    Detail = "A shirt with the same properties already exists.",
                    Status = StatusCodes.Status400BadRequest
                };

                context.ModelState.AddModelError("shirt", problemDetails.Detail);

                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }
    }
}
