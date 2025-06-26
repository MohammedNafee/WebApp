using Microsoft.AspNetCore.Mvc;
using WebApp.Filters;
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
        public IActionResult CreateShirt([FromBody] Shirt shirt)
        {
            return Ok("Creating a new shirt with id: {shirt.ShirtId}, Brand: {shirt.Brand} ");
        } 
        
        [HttpPut("{id}")]
        [Shirt_ValidateShirtIdFilter]
        public IActionResult UpdateShirt(int id, [FromQuery] string Color)
        {

            if (String.IsNullOrWhiteSpace(Color))
                return BadRequest("Color cannot be null or empty.");

            var shirt = ShirtRepository.GetShirtById(id);   

            if (shirt != null)
            {
                shirt.Color = Color;
                return Ok($"Updating shirt with id: {id}, New Color: {shirt.Color}");
            }
            
            return BadRequest("Shirt not found.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteShirt(int id)
        {
            return Ok("Deleting shirt with id: {id}");
        }

    }
}
