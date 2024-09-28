using Microsoft.AspNetCore.Identity;

namespace NuochoaHuxtah.Models
{
	public class AppUserModel : IdentityUser
	{
		public string Occupation {  get; set; }
	}
}
