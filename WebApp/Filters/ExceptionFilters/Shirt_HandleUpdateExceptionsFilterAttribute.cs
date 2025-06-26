using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models.Repositories;

namespace WebApp.Filters.ExceptionFilters
{
    public class Shirt_HandleUpdateExceptionsFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            var strShirtId = context.RouteData.Values["id"] as string;

            if (int.TryParse(strShirtId, out int shirtId))
            {
                if (!ShirtRepository.ShirtExists(shirtId))
                {
                    var problemDetails = new ProblemDetails()
                    {
                        Title = "Shirt Not Found",
                        Detail = $"Shirt with ID {shirtId} does not exist.",
                        Status = StatusCodes.Status404NotFound
                    };

                    context.ModelState.AddModelError(strShirtId, problemDetails.Detail);

                    context.Result = new NotFoundObjectResult(problemDetails);
                }
            }
        }
    }
}
