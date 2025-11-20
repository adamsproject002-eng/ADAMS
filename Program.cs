using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ADAMS.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

RegisterRepositoriesFromAssembly(builder.Services);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        //options.AccessDeniedPath = "/Auth/Denied";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddControllersWithViews()
//    .AddRazorOptions(options =>
//    {
//        // 設定 Area 的 View 路徑
//        options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
//        options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
//    });
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.Migrate();  // 自動執行 migrations
//    DbSeeder.Seed(db, forceReset: true);  // 初始化資料庫與種子資料
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Area 路由設定
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void RegisterRepositoriesFromAssembly(IServiceCollection services)
{
    var assembly = typeof(Program).Assembly;

    // 自動掃描所有實作 IRepository 的類別
    var repositoryTypes = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract)
        .Where(t => t.Name.EndsWith("Repository"))
        .Select(t => new
        {
            Implementation = t,
            Interface = t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}")
        })
        .Where(x => x.Interface != null);

    foreach (var type in repositoryTypes)
    {
        services.AddScoped(type.Interface!, type.Implementation);
    }

    // 自動掃描所有實作 IService 的類別
    var serviceTypes = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract)
        .Where(t => t.Name.EndsWith("Service"))
        .Select(t => new
        {
            Implementation = t,
            Interface = t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}")
        })
        .Where(x => x.Interface != null);

    foreach (var type in serviceTypes)
    {
        services.AddScoped(type.Interface!, type.Implementation);
    }
}


