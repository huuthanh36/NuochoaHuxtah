using NuochoaHuxtah.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuochoaHuxtah.Models
{
	public class ContactModel
	{
		
		public string Name { get; set; }
		public string Description { get; set; }
		public string LogoImg { get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; } // Có hay không không quan trọng 
	}
}
