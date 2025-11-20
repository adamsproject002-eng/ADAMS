using System.Security.Claims;

namespace ADAMS.Services
{
    public interface ICurrentAccountService
    {
        /// <summary>
        /// 取得當前的 ClaimsPrincipal
        ClaimsPrincipal? Account { get; }

        /// <summary>
        /// 登入者帳號名稱
        /// </summary>
        string Name { get; }

        int TenantSN { get; }

        int AccGroupSN { get; }

        string TenantName { get; }

        string GroupName { get; } 

        /// <summary>
        /// 判斷是否為營運公司 (TenantSN=0 && AccGroupSN=0)
        /// </summary>
        bool IsOperationCompany { get; }

        /// <summary>
        /// 檢查是否有特定功能權限 (對應 Claim: Function)
        /// </summary>
        //bool HasFunction(string functionName);
    }
}