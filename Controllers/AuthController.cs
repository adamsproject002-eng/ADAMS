using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADAMS.Data;
using ADAMS.ViewModels;
using System.Security.Claims;

namespace ADAMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string account, string password)
        {
            // 將密碼 Hash 後比對
            var hash = AppDbContextExtensions.Sha256(password);

            var user = _db.Account
                .Include(a => a.Tenant)
                .Include(a => a.AccGroup)
                    .ThenInclude(g => g.Authorizations)
                        .ThenInclude(au => au.Function)
                .Where(a => a.AccountName == account && a.Password == hash && !a.IsDeleted)
                .FirstOrDefault();

            //var user = await (
            //         from a in _db.Account
            //         where a.AccountName == account && a.Password == password && !a.IsDeleted
            //         join t in _db.Tenant on a.TenantSN equals t.SN into tj
            //         from t in tj.DefaultIfEmpty()
            //         join g in _db.AccountGroup on a.AccGroupSN equals g.AccountGroupSN into gj
            //         from g in gj.DefaultIfEmpty()
            //         join au in _db.Authorization on g.AccountGroupSN equals au.AccGroupSN into auj
            //         from au in auj.DefaultIfEmpty()
            //         join f in _db.Function on au.FunctionSN equals f.FunctionSN into fj
            //         from f in fj.DefaultIfEmpty()
            //         select new
            //         {
            //             Account = a,
            //             Tenant = t,
            //             AccGroup = g,
            //             Authorizations = au,
            //             Function = f
            //         }).FirstOrDefaultAsync();

            if (user == null)
            {
                ViewBag.Error = "帳號或密碼錯誤";
                return View();
            }

            
            var vm = new UserProfileViewModel
            {
                AccountName = user.AccountName,
                TenantName = user.Tenant?.TenantName,
                GroupName = user.AccGroup?.Name,
                FunctionNames = user.AccGroup?.Authorizations?
                    .Where(a => a.Function != null)
                    .Select(a => a.Function!.CName)
                    .OrderBy(n => n)
                    .ToList() ?? new List<string>()
            };

            //建立Claims（登入者的身份資訊與權限）
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.AccountName),
        new Claim("TenantSN", user.TenantSN.ToString()),
        new Claim("TenantName", user.Tenant?.TenantName ?? ""),
        new Claim("GroupName", user.AccGroup?.Name ?? "")
    };

            //把功能權限也加進 Claims
            foreach (var f in vm.FunctionNames)
            {
                claims.Add(new Claim("Function", f));
            }

            //建立身份 (ClaimsIdentity)
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //寫入 Cookie（保持登入狀態）
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true, // 是否持久登入
                    ExpiresUtc = DateTime.UtcNow.AddHours(8) // 登入有效時間
                });


            
            return RedirectToAction("Profile", "Auth");
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync("cookie");
            //return RedirectToAction(nameof(Login));

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Denied() => View();
    }

    public static class AppDbContextExtensions
    {
        public static string Sha256(string s)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s)));
        }
    }
}
