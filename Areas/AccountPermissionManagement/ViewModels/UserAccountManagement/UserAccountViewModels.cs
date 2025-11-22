using ADAMS.Areas.Models;
using ADAMS.Models;
using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.AccountPermissionManagement.ViewModels.UserAccountManagement
{
    // 列表用項目
    public class UserAccountListItemViewModel
    {
        public int AccountSN { get; set; }

        [Display(Name = "帳號")]
        public string AccountName { get; set; } = string.Empty;

        [Display(Name = "帳號群組")]
        public string GroupName { get; set; } = string.Empty;

        [Display(Name = "姓名")]
        public string? RealName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "狀態")]
        public bool IsEnable { get; set; }

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        /// <summary>是否為預設帳號（Admin）</summary>
        public bool IsDefaultAccount { get; set; }
    }

    // 列表頁 ViewModel
    public class UserAccountListViewModel
    {
        public bool IsOperationCompany { get; set; }

        public int CurrentTenantSN { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();

        public string? Keyword { get; set; }

        /// <summary>0=全部,1=啟用,2=停用</summary>
        public int Status { get; set; }

        public List<UserAccountListItemViewModel> Accounts { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new();
    }

    // 新增/修改頁 ViewModel
    public class UserAccountEditViewModel
    {
        public int? AccountSN { get; set; }

        [Required]
        public int TenantSN { get; set; }

        [Display(Name = "養殖戶")]
        public string TenantName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "帳號")]
        [StringLength(50)]
        public string AccountName { get; set; } = string.Empty;

        [Display(Name = "姓名")]
        [StringLength(50)]
        public string? RealName { get; set; }

        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "電話")]
        [StringLength(50)]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }

        [Display(Name = "帳號狀態")]
        public bool IsEnable { get; set; } = true;

        [Display(Name = "帳號群組")]
        public int AccGroupSN { get; set; }

        public bool IsDefaultAccount { get; set; }

        public bool IsOperationCompany { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();

        public List<AccountGroupOption> AccountGroupOptions { get; set; } = new();
    }

    public class AccountGroupOption
    {
        public int AccountGroupSN { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
