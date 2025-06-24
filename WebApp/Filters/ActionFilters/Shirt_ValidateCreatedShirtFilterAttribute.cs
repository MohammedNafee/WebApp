using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Filters.ActionFilters
{
    public class Shirt_ValidateCreatedShirtFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDBContext db;

        public Shirt_ValidateCreatedShirtFilterAttribute(ApplicationDBContext db)
        {
            this.db = db;
        }
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
            else
            {
               var existingShirt = db.Shirts.FirstOrDefault(s =>
                    !string.IsNullOrEmpty(shirt.Brand) &&
                    !string.IsNullOrEmpty(s.Brand) &&
                    s.Brand.ToLower() == shirt.Brand.ToLower() &&

                    !string.IsNullOrEmpty(shirt.Color) &&
                    !string.IsNullOrEmpty(s.Color) &&
                    s.Color.ToLower() == shirt.Color.ToLower() &&

                    !string.IsNullOrEmpty(shirt.Gender) &&
                    !string.IsNullOrEmpty(s.Gender) &&
                    s.Gender.ToLower() == shirt.Gender.ToLower() &&

                    shirt.Size == s.Size &&

                    shirt.Price == s.Price
               );

                if (existingShirt != null)
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
}
