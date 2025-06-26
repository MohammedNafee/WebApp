using System.ComponentModel.DataAnnotations;
using WebApp.Models.Validations;

namespace WebApp.Models
{
    public class Shirt
    {
        public int ShirtId { get; set; }
        
        [Required(ErrorMessage = "Brnd is required.")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        public string? Color { get; set; }

        [Shirt_EnsureCorrectSizing]
        public int Size { get; set; }

        [Required]
        public string? Gender { get; set; }
        
    }
}
