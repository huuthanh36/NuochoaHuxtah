using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuochoaHuxtah.Models
{
	public class ReviewModel
	{
		[Key]
		public int Id { get; set; }
		public int ProductId { get; set; }
		[Required(ErrorMessage ="Yêu cầu nhập đánh giá")]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập email")]
		public string Email { get; set; }
		public string Rating { get; set; }
		[ForeignKey("ProductId")]
		public ProductModel Product { get; set; }
	}
}
