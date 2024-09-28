using System.ComponentModel.DataAnnotations;

namespace NuochoaHuxtah.Models.ViewModels
{
	public class LoginViewModel // Khi sử dụng UserModel để login thì sẽ bị yêu cầu email, trả về trực tiếp khhi nhập sai
	{
		
		public int Id { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
		public string Username { get; set; }
	
		[DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
