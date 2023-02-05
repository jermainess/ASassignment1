using ASassignment.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ASassignment.Model;

namespace ASassignment.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<LoginModel> _logger;
		private readonly int MAX_FAILED_ACCESS_ATTEMPTS = 3;
		public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            this.signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
		}
	
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
				var user = await _userManager.FindByEmailAsync(LModel.Email);
				if (user != null)
				{
					if (!await _userManager.IsLockedOutAsync(user))
					{
						var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
						LModel.RememberMe, false);
						if (identityResult.Succeeded)
						{
							var claims = new List<Claim>
							{
								new Claim(ClaimTypes.Name, user.UserName),
								new Claim(ClaimTypes.Email, user.Email),
								new Claim("Role", "User")
							};
							var i = new ClaimsIdentity(claims, "MyCookieAuth");
							ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
							await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
							return RedirectToPage("Index");
						}
						else
						{
							await _userManager.AccessFailedAsync(user);
							int accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
							if (accessFailedCount >= MAX_FAILED_ACCESS_ATTEMPTS)
							{
								await _userManager.SetLockoutEndDateAsync(user, new System.DateTimeOffset(System.DateTime.UtcNow.AddMinutes(5)));
								_logger.LogWarning("User account locked out.");
								ModelState.AddModelError("", "Your account is locked out");
								return Page();
							}
						}
					}
					else
					{
						_logger.LogWarning("User account locked out.");
						ModelState.AddModelError("", "Your account is locked out");
						return Page();
					}
				}
				ModelState.AddModelError("", "Username or Password incorrect");
			}
			return Page();
		}
	}
}