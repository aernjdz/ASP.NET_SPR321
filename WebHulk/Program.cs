using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using DataAcess.Data;
using DataAcess.Data.Entities.Identity;
using DataAcess.Services;
using DataAcess.Interfaces;
//
using BusinessLogic;

//
using BusinessLogic.Basic.Interfaces;
//
using BusinessLogic.Admin.Services;
using BusinessLogic.Admin.Interfaces;
using BusinessLogic.Basic.Services;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;



var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddDbContext<HulkDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));*/

builder.Services.AddDbContext<HulkDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("WebConnection")));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllersWithViews();


builder.Services.AddSingleton(new Cloudinary(new Account(
             builder.Configuration["Cloudinary:CloudName"],

             builder.Configuration["Cloudinary:ApiKey"],
             builder.Configuration["Cloudinary:ApiSecret"]
         )));


builder.Services.AddAutoMapper(typeof(AppMapProfile));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductServiceAdmin, ProductServiceAdmin>();
builder.Services.AddScoped<ICategoryServiceAdmin , CategoryServiceAdmin>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IImageWorker, ImageWorker>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddSession(x =>
{
    x.Cookie.HttpOnly = true; 
    x.Cookie.IsEssential = true;
});
builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.AllowedForNewUsers = true;

    //options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<HulkDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

string dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
if (!Directory.Exists(dirSave))
    Directory.CreateDirectory(dirSave);

app.UseRouting();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dirSave),
    RequestPath = "/images"
});

//
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Main}/{action=Index}/{id?}");

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "admin_area",
        areaName: "Admin",
        pattern: "admin/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Main}/{action=Index}/{id?}");        
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

using (var scope = app.Services.CreateScope())
{

    var dbContext = scope.ServiceProvider.GetRequiredService<HulkDbContext>();
    //dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedProducts();
    await seeder.SeedRolesAndUsers();
}

app.Run();