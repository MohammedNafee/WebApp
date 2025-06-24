using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Data;

namespace WebApp.Filters.ActionFilters
{
    public class Shirt_ValidateShirtIdFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDBContext db;

        public Shirt_ValidateShirtIdFilterAttribute(ApplicationDBContext db)
        {
            this.db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirtId = context.ActionArguments["id"] as int?;

            if (shirtId.HasValue)
            {
                if (shirtId.Value <= 0)
                {
                    var problemDetails = new ProblemDetails()
                    {
                        Title = "Invalid shirtId",
                        Detail = "The provided shirtId must be a positive integer greater than zero.",
                        Status = StatusCodes.Status400BadRequest
                    };

                    context.ModelState.AddModelError("shirtId", problemDetails.Detail);

                    context.Result = new BadRequestObjectResult(problemDetails);
                }
                else
                {
                    var shirt = db.Shirts.Find(shirtId.Value);

                    if (shirt == null)
                    {
                        var problemDetails = new ProblemDetails()
                        {
                            Title = "Shirt Not Found",
                            Detail = $"No shirt found with id {shirtId.Value}.",
                            Status = StatusCodes.Status404NotFound
                        };

                        context.ModelState.AddModelError("shirtId", problemDetails.Detail);

                        context.Result = new NotFoundObjectResult(problemDetails);
                    }
                    else
                    {
                        context.HttpContext.Items["shirt"] = shirt;
                    }
                }
            }
        }
    }
}
