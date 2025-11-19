using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ADAMS.Data;

namespace ADAMS.Attributes
{
    /// <summary>
    /// 功能權限驗證屬性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class FunctionPermissionAttribute : ActionFilterAttribute
    {
        private readonly int _functionSN;

        public FunctionPermissionAttribute(int functionSN)
        {
            _functionSN = functionSN;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            // 檢查是否登入
            if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "" });
                return;
            }

            try
            {
                // 從 Claims 取得使用者功能權限名稱
                var userFunctions = httpContext.User.Claims
                    .Where(c => c.Type == "Function")
                    .Select(c => c.Value)
                    .ToList();

                // 從資料庫取得所需功能資訊
                var db = httpContext.RequestServices.GetRequiredService<AppDbContext>();

                var requiredFunction = await db.Function
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FunctionSN == _functionSN);

                if (requiredFunction == null)
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        StatusCode = 500
                    };
                    return;
                }

                // 檢查使用者是否有此功能權限
                if (!userFunctions.Contains(requiredFunction.CName))
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/AccessDenied.cshtml",
                        StatusCode = 403
                    };
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                var logger = httpContext.RequestServices
                    .GetRequiredService<ILogger<FunctionPermissionAttribute>>();
                logger.LogError(ex, "權限驗證失敗");

                context.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml",
                    StatusCode = 500
                };
            }
        }
    }

    /// <summary>
    /// 功能 SN 常數定義
    /// </summary>
    public static class FunctionSNs
    {
        // === 基礎資料管理 ===
        public const int BasicDataManagement = 1;
        public const int AreaPondManagement = 7;
        public const int SupplierDataManagement = 8;
        public const int SeedlingDataManagement = 9;
        public const int FeedDataManagement = 10;
        public const int TimeSegmentSetting = 11;
        public const int GrowthTargetReference = 12;
        public const int FeedingPlanManagement = 13;
        public const int FarmingTypeManagement = 14;
        public const int UnitDataManagement = 15;

        // === 養殖池總覽 ===
        public const int PondOverview = 2;
        public const int PondOverviewPage = 16;

        // === 環境監測資訊 ===
        public const int EnvironmentMonitoringInfo = 3;
        public const int RealtimeMonitoringInfo = 17;
        public const int HistoryMonitoringInfo = 18;
        public const int MonitoringAlertSetting = 19;
        public const int MonitoringAlertRecord = 20;

        // === 作業管理 ===
        public const int OperationManagement = 4;
        public const int StockingRecordQuery = 21;
        public const int FeedingRecordQuery = 22;
        public const int SamplingRecordQuery = 23;
        public const int HarvestRecordQuery = 24;
        public const int PowerUsageRecordQuery = 25;
        public const int ExpenseRecordQuery = 26;
        public const int AbnormalityReportQuery = 27;
        public const int OtherEnvironmentDataQuery = 28;

        // === 統計報表管理 ===
        public const int StatisticsReportManagement = 5;
        public const int FarmingPerformanceSummary = 29;
        public const int EnvironmentRecordReport = 30;
        public const int FeedingRecordReport = 31;
        public const int ABWCurveChart = 32;

        // === 帳號權限管理 ===
        public const int AccountPermissionManagement = 6;
        public const int UserAccountManagement = 33;
        public const int GroupPermissionManagement = 34;
        public const int FarmerManagement = 35;
    }
}