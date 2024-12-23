using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebQLDASinhVien.Data;
using WebQLDASinhVien.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WebQLDASinhVienContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebQLDASinhVienContext")
        ?? throw new InvalidOperationException("Connection string 'WebQLDASinhVienContext' not found.")));

// Sử dụng Identity với vai trò
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;       // Không yêu cầu số
    options.Password.RequireLowercase = false;   // Không yêu cầu chữ thường
    options.Password.RequireUppercase = false;   // Không yêu cầu chữ hoa
    options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
    options.Password.RequiredLength = 6;         // Độ dài tối thiểu của mật khẩu
})
.AddEntityFrameworkStores<WebQLDASinhVienContext>()
.AddDefaultTokenProviders();

// Sử dụng Identity với vai trò
builder.Services.AddAuthentication().AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Tạo vai trò mặc định khi khởi động
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Teacher", "Student" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Teachers}/{action=Index}/{id?}");

app.Run();
