using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Models;
using NuochoaHuxtah.Models.ViewModels;
using NuochoaHuxtah.Repository;

namespace NuochoaHuxtah.Controllers
{
	public class ProductController : Controller
    {
		private readonly DataContext _dataContext;
		public ProductController(DataContext context)
		{
			_dataContext = context;
		}
		public IActionResult Index()
        {
            return View();
        }
        public async  Task<IActionResult> Details( int Id )
        {
            if(Id ==null)
            {
                return RedirectToAction("Index");
            }
			var productsById = _dataContext.Products
                .Include(p=>p.Reviews)
                .Where(p => p.Id == Id).FirstOrDefault();

            var relatedProducts = await _dataContext.Products
                .Where(p=>p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
                .Take(4)
                .ToListAsync();
            
            ViewBag.RelatedProducts = relatedProducts;
            var viewModel = new ProductDetailsViewModel
            {
                ProductDetails = productsById,
                
            };
			return View(viewModel);
        }
        public async Task<IActionResult> CommentProduct(ReviewModel review)
        {
            if(ModelState.IsValid)
            {
                var reviewEntity = new ReviewModel
                {
                    ProductId = review.ProductId,
                    Name = review.Name,
                    Email = review.Email,
                    Comment = review.Comment,
                    Rating = review.Rating

                };

                _dataContext.Reviews.Add(reviewEntity);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);


            }
            else
            {
				TempData["error"] = "Model not ok";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						Console.WriteLine(error.ErrorMessage);
						errors.Add(error.ErrorMessage);
						
					}
				}
				string errorMessage = string.Join("\n", errors);
				return RedirectToAction("Details", new {id = review.ProductId});
            }
            return Redirect(Request.Headers["Referer"]);
        }
        // Tìm theo từ khóa
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();
            ViewBag.Keyword = searchTerm;
            return View(products);   
        }
    }
}
