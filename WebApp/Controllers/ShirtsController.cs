using Microsoft.AspNetCore.Mvc;
using WebAPIApp.Attributes;
using WebAPIApp.Filters.AuthFilters;
using WebApp.Data;
using WebApp.Filters.ActionFilters;
using WebApp.Filters.ExceptionFilters;
using WebApp.Models;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [JwtTokenAuthFilter] // This filter will be applied to all actions in this controller 
    public class ShirtsController : ControllerBase
    {
        private readonly ApplicationDBContext db;

        public ShirtsController(ApplicationDBContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirts.ToList());
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirtsById(int id)
        {
            return Ok(HttpContext.Items["shirt"]);
        }

        [HttpPost]
        [TypeFilter(typeof(Shirt_ValidateCreatedShirtFilterAttribute))]
        [RequiredClaim("write", "true")]
        public IActionResult CreateShirt([FromBody] Shirt shirt)
        { 
            db.Shirts.Add(shirt);
            db.SaveChanges();

            return Created();
        } 
        
        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        [Shirt_ValidateUpdateShirtFilter]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("write", "true")]
        public IActionResult UpdateShirt(int id, [FromBody] Shirt shirt)
        {
            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt;

            shirtToUpdate.Brand = shirt.Brand;
            shirtToUpdate.Color = shirt.Color;
            shirtToUpdate.Size = shirt.Size;
            shirtToUpdate.Gender = shirt.Gender;
            shirtToUpdate.Price = shirt.Price;

            db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("delete", "true")]
        public IActionResult DeleteShirt(int id)
        {
            var shirtToDelete = HttpContext.Items["shirt"] as Shirt;

            db.Shirts.Remove(shirtToDelete);   

            db.SaveChanges();

            return Ok(shirtToDelete);
        }

    }
}
