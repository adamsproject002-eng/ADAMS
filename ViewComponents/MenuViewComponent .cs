using Microsoft.AspNetCore.Mvc;
using ADAMS.Data;
using System.Security.Claims;


namespace ADAMS.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;
        public MenuViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var user = HttpContext.User as ClaimsPrincipal;   // ← 直接拿 ClaimsPrincipal
            if (user?.Identity?.IsAuthenticated != true)
                return Content("");

            var functionNames = user
                .FindAll("Function")                          // ← OK
                .Select(c => c.Value)
                .ToList();

            var functions = _db.Function
                .Where(f => functionNames.Contains(f.CName) && f.IsShow)
                .OrderBy(f => f.Sort)
                .ToList();

            var parents = functions.Where(f => f.FLevel == 1).ToList();
            ViewBag.AllFunctions = functions;
            return View(parents);
        }
    }
}
