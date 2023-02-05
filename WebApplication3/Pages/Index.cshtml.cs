using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ASassignment.Model;
using ASassignment.ViewModels;

namespace ASassignment.Pages
{
    [Authorize]

    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public IndexModel(UserManager<ApplicationUser> userManager,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

        }
        public ApplicationUser User { get; set; }

        public void OnGet()
        {
			var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
			var protector = dataProtectionProvider.CreateProtector("MySecretKey");

			var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = string.Format("Post ID {0} not found", userId);
                Redirect("/Register");
            }
            User = _userManager.FindByIdAsync(userId).Result;
            if (User.CreditCard != null)
            {
                User.CreditCard = protector.Unprotect(User.CreditCard);

            }
            else
            {
                Redirect("/Register");
            }


        }
	}
} 