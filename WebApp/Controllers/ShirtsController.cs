using Microsoft.AspNetCore.Mvc;
using WebApp.Filters.ActionFilters;
using WebApp.Filters.ExceptionFilters;
using WebApp.Models;
using WebApp.Models.Repositories;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShirtsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetShirts()
        {
            return Ok(ShirtRepository.GetAllShirts());
        }

        [HttpGet("{id}")]
        [Shirt_ValidateShirtIdFilter]
        public IActionResult GetShirtsById(int id)
        {
            return Ok(ShirtRepository.GetShirtById(id));
        }

        [HttpPost]
        [Shirt_ValidateCreatedShirtFilter]
        public IActionResult CreateShirt([FromBody] Shirt shirt)
        {
            ShirtRepository.AddShirt(shirt);

            return Created();
        } 
        
        [HttpPut("{id}")]
        [Shirt_HandleUpdateExceptionsFilter]
        [Shirt_ValidateUpdateShirtFilter]
        [Shirt_ValidateShirtIdFilter]
        public IActionResult UpdateShirt(int id, [FromBody] Shirt shirt)
        {
            ShirtRepository.UpdateShirt(shirt);
  
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Shirt_ValidateShirtIdFilter]
        public IActionResult DeleteShirt(int id)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);

            ShirtRepository.DeleteShirt(id);

            return Ok(existingShirt);
        }

    }
}
