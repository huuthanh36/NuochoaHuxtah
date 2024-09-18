using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Models;

namespace NuochoaHuxtah.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if(!_context.Products.Any() )
			{
				CategoryModel macbook = new CategoryModel{ Name = "Macbook", Slug = "macbook", Description="Macbook number one",Status=1 };
				CategoryModel pc = new CategoryModel{ Name = "pc", Slug = "pc", Description="pc number one",Status=1 };

				BrandModel apple = new BrandModel { Name = "Apple", Slug = "Apple", Description = "Apple number one",Status=1 };
				BrandModel samsung = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung number one", Status = 1 };
				_context.Products.AddRange(
					new ProductModel { Name = "Macbook", Slug = "macbook", Description = "Macbook number one", Image="1.jpg", Category=macbook, Brand=apple ,Price=1233},
					new ProductModel { Name = "Pc", Slug = "pc", Description = "Pc number one", Image="1.jpg", Category=pc , Brand=samsung ,Price=1233}

				);
				_context.SaveChanges();
			}
		}
	}
}
