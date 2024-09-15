using System.ComponentModel.DataAnnotations;

namespace NuochoaHuxtah.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên thương hiệu")]
		public string Name { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả thương hiệu")]
		public string Description { get; set; }

        public int Slug { get; set;}

        public string Status { get; set;}
    }
}
