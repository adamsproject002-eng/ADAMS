using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ADAMS.Data;
using ADAMS.Areas.OperationManagement.Services.StockingRecordQuery;
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
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
//builder.Services.AddControllersWithViews()
//    .AddRazorOptions(options =>
//    {
//        // �]�w Area �� View ���|
//        options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
//        options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
//    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
// {
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.Migrate();  // �۰ʰ��� migrations
//    DbSeeder.Seed(db, forceReset: true);  // ��l�Ƹ�Ʈw�P�ؤl���
// }

// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }
if (app.Environment.IsDevelopment())
{

    Console.WriteLine("In Development environment");
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Area ���ѳ]�w`
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

    // Repository
    var repositoryTypes = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract)
        .Where(t => t.Name.EndsWith("Repository"));

    foreach (var type in repositoryTypes)
    {
        var iface = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
        if (iface != null)
        {
            // 如果有 interface，就用 interface 註冊
            services.AddScoped(iface, type);
        }
        else
        {
            // 沒有 interface 就直接註冊實體
            services.AddScoped(type);
        }
    }

    // Service
    var serviceTypes = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract)
        .Where(t => t.Name.EndsWith("Service"));

    foreach (var type in serviceTypes)
    {
        var iface = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
        if (iface != null)
        {
            services.AddScoped(iface, type);
        }
        else
        {
            services.AddScoped(type);
        }
    }

    
}


