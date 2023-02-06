using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using ASassignment.Model;
using AspNetCore.ReCaptcha;
using ASassignment.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<GoogleCaptchaConfig>(builder.Configuration.GetSection("GoogleRecaptcha"));
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.ConfigureApplicationCookie(Config =>
{
	Config.LoginPath = "/Login";
});
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
	options.Cookie.Name = "MyCookieAuth";
	options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("MustBelongToAdmin",
		policy => policy.RequireClaim("Role", "Admin"));
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});


builder.Services.Configure<IdentityOptions>(options =>
{
	// Default Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 12;
	options.Password.RequiredUniqueChars = 1;
});
builder.Services.AddDataProtection();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
 
app.UseStatusCodePagesWithRedirects("/errors/{0}");



app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
