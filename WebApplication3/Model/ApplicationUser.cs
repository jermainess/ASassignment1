using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASassignment.Model
{
	public class ApplicationUser : IdentityUser
	{
		[Required]
		public string FullName { get; set; }

		[Required]
		public string CreditCard { get; set; }

		[Required, MaxLength(1)]
		public string Gender { get; set; } = string.Empty;


		[Required]
		public string Delivery { get; set; }


		[Required]
		public string AboutMe { get; set; }

		[RegularExpression(@"^\w+\.(jpg|jpeg)$")]
		[MaxLength(50) ]
        public string? ImageURL { get; set; }
    }
	
}

