using ADAMS.Areas.Models;
using ADAMS.Models;
using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.AccountPermissionManagement.ViewModels.GroupPermissionManagement
{
    // ==================== 列表頁 ViewModel ====================
    public class GroupPermissionListViewModel
    {
        public List<GroupPermissionViewModel> GroupPermissions { get; set; } = new();
        public List<TenantOption> TenantOptions { get; set; } = new List<TenantOption>();
        public bool IsOperationCompany { get; set; }
        public int CurrentTenantSN { get; set; }
        public PaginationInfo Pagination { get; set; } = new();

    }
    // ==================== 列表項目 ViewModel ====================
    public class GroupPermissionViewModel
    {
        public int AccountGroupSN { get; set; }
        [Display(Name = "群組名稱")]
        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        [Display(Name = "備註")]
        public string? Remark { get; set; }
    }

    // 養殖戶選項
    public class TenantOption
    {
        public int SN { get; set; }
        public string? TenantName { get; set; }
    }

    //使用者清單
    public class GroupPermissionAccountListViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Account> Accounts { get; set; } = new();
    }

    // ==================== 權限明細 ViewModel（新增） ====================
    public class GroupPermissionDetailViewModel
    {
        public int AccountGroupSN { get; set; }

        [Display(Name = "群組名稱")]
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// 所有功能清單（含階層、排序、是否擁有權限）
        /// </summary>
        public List<FunctionPermissionItemViewModel> Functions { get; set; } = new();
    }

    public class FunctionPermissionItemViewModel
    {
        public int FunctionSN { get; set; }

        /// <summary>中文名稱（顯示用，如：魚塭監測資訊）</summary>
        public string CName { get; set; } = string.Empty;

        /// <summary>功能階層：1=主選單、2=次選單</summary>
        public int FLevel { get; set; }

        /// <summary>上層功能編號（FLevel=1 時為 0）</summary>
        public int UpperFunctionSN { get; set; }

        /// <summary>排序用</summary>
        public int Sort { get; set; }

        /// <summary>該群組是否勾選此功能</summary>
        public bool HasPermission { get; set; }
    }

    // ==================== 新增/修改群組用 ViewModel ====================
    public class GroupPermissionEditViewModel
    {
        /// <summary>群組 SN，新增時為 null</summary>
        public int? AccountGroupSN { get; set; }

        /// <summary>所屬養殖戶 SN</summary>
        [Required]
        public int TenantSN { get; set; }

        /// <summary>所屬養殖戶名稱（顯示用）</summary>
        [Display(Name = "養殖戶")]
        public string TenantName { get; set; } = string.Empty;

        /// <summary>群組名稱</summary>
        [Required]
        [Display(Name = "群組名稱")]
        [StringLength(50)]
        //[RegularExpression(@"^(?i)(?!admin$).+$",
        //ErrorMessage = "群組名稱不得為 Admin，請改用其他名稱。")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }

        /// <summary>所有功能清單 + 是否勾選</summary>
        public List<FunctionPermissionEditItemViewModel> Functions { get; set; } = new();
        public bool IsOperationCompany { get; set; }

        /// <summary>營運帳號新增時使用的養殖戶下拉選項</summary>
        public List<TenantOption> TenantOptions { get; set; } = new();
    }

    /// <summary>
    /// 新增 / 編輯畫面中的功能項目
    /// </summary>
    public class FunctionPermissionEditItemViewModel
    {
        public int FunctionSN { get; set; }
        public string CName { get; set; } = string.Empty;
        public int FLevel { get; set; }
        public int UpperFunctionSN { get; set; }
        public int Sort { get; set; }

        /// <summary>
        /// 是否被勾選，代表該群組擁有這個功能的權限
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
