using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Data;
using WebAppMVC.Models;
namespace WebAppMVC.Controllers
{
    public class ShirtsController : Controller
    {
        private readonly IWebApiExecutor webApiExecutor;

        public ShirtsController(IWebApiExecutor webApiExecutor)
        {
            this.webApiExecutor = webApiExecutor;
        }

        public async Task<IActionResult> Index()
        {
            return View(await webApiExecutor.InvokeGet<List<Shirt>>("shirts"));
        }

        public IActionResult CreateShirt()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShirt(Shirt shirt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   await webApiExecutor.InvokePost("shirts", shirt);
                   return RedirectToAction(nameof(Index));
                }
                catch (WebApiException ex)
                {
                    HandleWebApiException(ex);
                }

            }
            return View(shirt);
        }

        public async Task<IActionResult> UpdateShirt(int shirtId)
        {
            try
            {
                var shirt = await webApiExecutor.InvokeGet<Shirt>($"shirts/{shirtId}");

                if (shirt != null)
                {
                    return View(shirt);
                }
            }
            catch(WebApiException ex) 
            {
                HandleWebApiException(ex);
                return View();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShirt(Shirt shirt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await webApiExecutor.InvokePut($"shirts/{shirt.ShirtId}", shirt);

                    return RedirectToAction(nameof(Index));
                }
                catch(WebApiException ex) 
                {
                    HandleWebApiException(ex);
                }
            }
            return View(shirt);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShirt([FromForm]int shirtId)
        {
            try
            {
                await webApiExecutor.InvokeDelete($"shirts/{shirtId}");

                return RedirectToAction(nameof(Index));
            }
            catch(WebApiException ex)
            { 
                HandleWebApiException(ex);
                return View(nameof(Index),
                    await webApiExecutor.InvokeGet<List<Shirt>>("shirts"));
            }
        }

        private void HandleWebApiException(WebApiException ex)
        {
            // Handle validation errors returned from the Web API.
            // Some errors are tied to specific model properties (e.g., "Brand", "Color") and will be 
            // displayed next to the corresponding input using asp-validation-for.
            // However, some errors may be general (e.g., empty key "" or in the "Title" field) and are not 
            // associated with a specific property. These must be added with an empty key ("") so they show 
            // up in the ValidationSummary section of the view.
            // Without this, general API errors would not be visible to the user.

            if (ex.ErrorResponse != null)
            {
                if (ex.ErrorResponse.Errors != null)
                {
                    foreach (var error in ex.ErrorResponse.Errors)
                    {
                        string key = error.Key;

                        foreach (var message in error.Value)
                        {
                            if (string.IsNullOrEmpty(key))
                            {
                                ModelState.AddModelError("", message);
                            }
                            else
                            {
                                ModelState.AddModelError(key, message);
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(ex.ErrorResponse.Title))
                {
                    ModelState.AddModelError("", ex.ErrorResponse.Title);
                }
                
            }
        }

    }
}
