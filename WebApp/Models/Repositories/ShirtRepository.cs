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

    }
}
