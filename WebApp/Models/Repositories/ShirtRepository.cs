using System.Numerics;

namespace WebApp.Models.Repositories
{
    public static class ShirtRepository
    {
        private static List<Shirt> shirts = new List<Shirt>
        {
            new Shirt { ShirtId = 1, Brand = "Nike", Color = "Red", Size = 10, Gender = "mens" },
            new Shirt { ShirtId = 2, Brand = "Adidas", Color = "Blue", Size = 12, Gender = "womens" },
            new Shirt { ShirtId = 3, Brand = "Puma", Color = "Green", Size = 9, Gender = "mens" }
        };

        public static bool ShirtExists(int id)
        {
            return shirts.Any(s => s.ShirtId == id);
        }

        public static Shirt? GetShirtById(int id)
        {
            return shirts.FirstOrDefault(s => s.ShirtId == id);
        }

        public static List<Shirt> GetAllShirts()
        {
            return shirts;
        }

        public static Shirt? GetShirtByProperties(string? brand, string? color, int size, string? gender)
        {
            return shirts.FirstOrDefault(s =>
                !string.IsNullOrEmpty(brand) &&
                !string.IsNullOrEmpty(s.Brand) &&
                s.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&

                !string.IsNullOrEmpty(color) &&
                !string.IsNullOrEmpty(s.Color) &&
                s.Color.Equals(color, StringComparison.OrdinalIgnoreCase) &&

                !string.IsNullOrEmpty(gender) &&
                !string.IsNullOrEmpty(s.Gender) &&
                s.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase) &&

                size == s.Size
            );
        }
        public static void AddShirt(Shirt shirt)
        {
            int maxId = shirts.Max(s => s.ShirtId);

            shirt.ShirtId = ++maxId;

            shirts.Add(shirt);
        }

        public static void UpdateShirt(Shirt shirt)
        {
            var existingShirt = shirts.First(s => s.ShirtId == shirt.ShirtId);

            existingShirt.Brand = shirt.Brand;
            existingShirt.Color = shirt.Color;
            existingShirt.Size = shirt.Size;
            existingShirt.Gender = shirt.Gender;
        }

        public static void DeleteShirt(int ShirtId)
        {
            var shirt = GetShirtById(ShirtId);

            if (shirt != null)
            {
                shirts.Remove(shirt);
            }
        }
    }
}
