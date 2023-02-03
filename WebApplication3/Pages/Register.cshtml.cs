using WebApplication3.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WebApplication3.Model;
using Microsoft.Extensions.Hosting;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
		private IWebHostEnvironment _environment;

		[BindProperty]
		public IFormFile? Upload { get; set; }

		[BindProperty]
        public Register RModel { get; set; }


        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
			_environment = environment;
		}
        public void OnGet()
        { 
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
			{
                if (Upload!= null)
                {
                    if (Upload.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                        return Page();
                    }

                    var uploadsFolder = "uploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                    var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                    using var fileStream = new FileStream(imagePath, FileMode.Create);
                    await Upload.CopyToAsync(fileStream);
                    RModel.ImageUrl = string.Format("/{0}/{1}", uploadsFolder, imageFile);
                }
                var user = new ApplicationUser()
                {
                    UserName= RModel.Email,
                    FullName = RModel.FullName,
                    Email = RModel.Email,
                    PhoneNumber= RModel.PhoneNumber,
                    Delivery = RModel.Delivery,
                    CreditCard= RModel.CreditCard,
                    Gender= RModel.Gender,
                    AboutMe= RModel.AboutMe,
                    ImageURL = RModel.ImageUrl

                };

                // Create the User/Admin role if NOT exist (1)
                // By running the application, It creates a role of "user" if the role is not found 
                // If role is not found when trying to add them into the role (2)
                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role user failed");
                    }
                }


                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {

                    //Add users to Admin Role / User Role (2)
                    result = await userManager.AddToRoleAsync(user, "Admin");

                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

    }
}
