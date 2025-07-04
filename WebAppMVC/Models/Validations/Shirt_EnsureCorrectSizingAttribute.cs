﻿using System.ComponentModel.DataAnnotations;

namespace WebAppMVC.Models.Validations
{
    public class Shirt_EnsureCorrectSizingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var shirt = validationContext.ObjectInstance as Shirt;

            if (shirt != null && !String.IsNullOrWhiteSpace(shirt.Gender))
            {
                if (shirt.Gender.Equals("mens", StringComparison.OrdinalIgnoreCase) && shirt.Size < 8 )
                    
                    return new ValidationResult("For men's shirts, the size has to be greater or equal to 8.");
                
                else if (shirt.Gender.Equals("womens", StringComparison.OrdinalIgnoreCase) && shirt.Size < 6)
                    
                    return new ValidationResult("For women's shirts, the size has to be greater or equal to 6.");
            } 

            return ValidationResult.Success; // Valid case, no error message

        }
    }
}
