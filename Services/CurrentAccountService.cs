using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ADAMS.Services
{
    public class CurrentAccountService : ICurrentAccountService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentAccountService(IHttpContextAccessor http)
        {
            _http = http;
        }

        // 1. 基礎核心：取得 User Principal
        public ClaimsPrincipal? Account => _http.HttpContext?.User;

        // 2. 常用屬性實作

        public string Name =>
            Account?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        public int TenantSN
        {
            get
            {
                var value = Account?.FindFirst("TenantSN")?.Value;
                return int.TryParse(value, out var sn) ? sn : 0;
            }
        }

        public int AccGroupSN
        {
            get
            {
                var value = Account?.FindFirst("AccGroupSN")?.Value;
                return int.TryParse(value, out var sn) ? sn : 0;
            }
        }

        public string TenantName =>
            Account?.FindFirst("TenantName")?.Value ?? string.Empty;

        public string GroupName =>
            Account?.FindFirst("GroupName")?.Value ?? string.Empty;

        // 3. 邏輯判斷

        public bool IsOperationCompany =>
            TenantSN == 0 && AccGroupSN == 0;

        ///// <summary>
        ///// 檢查是否擁有該功能權限
        ///// </summary>
        //public bool HasFunction(string functionName)
        //{
        //    // 使用 HasClaim 來檢查是否有包含該值的 "Function" Claim
        //    return Account?.HasClaim(c => c.Type == "Function" && c.Value == functionName) ?? false;
        //}
    }
}