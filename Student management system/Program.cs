using ApplicationStudentManagement.Interfaces;
using ApplicationStudentManagement.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.DapperRepositories;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories;
using StudentManagementSystem.Infrastructure.Repository;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(Student_management_system.Controllers.StudentController).Assembly)
    .AddApplicationPart(typeof(StudentManagementSystem.Controllers.AccountController).Assembly);





builder.Services.AddScoped<IStudentInterface, StudentService>();
builder.Services.AddScoped<IStudentInterface, NewStudentService>();
builder.Services.AddScoped<IStudentChild, ChildStudentService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ILogIn, LogInService>();
builder.Services.AddScoped<IForgotPassword, ForgotPasswordService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IDapperRegistrationRepository, DapperRegistrationRepository>();
// --- Dapper service: depends on the repository interface above ---
builder.Services.AddScoped<IDapperRegistrationService, DapperRegistrationService>();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/LogIn";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("Allowfrontend");
app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LogIn}/{id?}")
    .WithStaticAssets();

app.Run();