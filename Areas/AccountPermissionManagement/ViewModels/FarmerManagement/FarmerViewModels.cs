using ADAMS.Areas.Models;
using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.AccountPermissionManagement.ViewModels.FarmerManagement
{
    // ==================== 列表頁 ViewModel ====================
    public class FarmerListViewModel
    {
        public List<FarmerViewModel> Farmers { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string? StatusFilter { get; set; }

        public string? Keyword { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    // ==================== 列表項目 ViewModel ====================
    public class FarmerViewModel
    {
        public int SN { get; set; }

        [Display(Name = "養殖戶編號")]
        public string TenantNum { get; set; } = string.Empty;

        [Display(Name = "名稱")]
        public string TenantName { get; set; } = string.Empty;

        [Display(Name = "負責人")]
        public string? ResponName { get; set; }

        [Display(Name = "負責人電話")]
        public string? ResponPhone { get; set; }

        [Display(Name = "負責人信箱")]
        public string? ResponEmail { get; set; }

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        [Display(Name = "功能")]
        public bool IsEnable { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        // 統計資訊
        public int AccountCount { get; set; }
    }

    // ==================== 新增 ViewModel ====================
    public class FarmerCreateViewModel
    {
        [Required(ErrorMessage = "養殖戶編號為必填")]
        [StringLength(50, ErrorMessage = "養殖戶編號不可超過50字元")]
        [Display(Name = "養殖戶編號")]
        public string TenantNum { get; set; } = string.Empty;

        [Required(ErrorMessage = "養殖戶名稱為必填")]
        [StringLength(50, ErrorMessage = "養殖戶名稱不可超過50字元")]
        [Display(Name = "養殖戶名稱")]
        public string TenantName { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "養殖戶地址不可超過150字元")]
        [Display(Name = "養殖戶地址")]
        public string? TenantAddr { get; set; }

        [StringLength(50, ErrorMessage = "負責人姓名不可超過50字元")]
        [Display(Name = "負責人姓名")]
        public string? ResponName { get; set; }

        [StringLength(50, ErrorMessage = "負責人電話不可超過50字元")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        [Display(Name = "負責人電話")]
        public string? ResponPhone { get; set; }

        [StringLength(100, ErrorMessage = "負責人Email不可超過100字元")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email地址")]
        [Display(Name = "負責人Email")]
        public string? ResponEmail { get; set; }

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        // === 註冊帳號資訊 ===
        [Required(ErrorMessage = "使用者帳號為必填")]
        [StringLength(50)]
        [Display(Name = "帳號")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "使用者密碼為必填")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string? Password { get; set; }

        [StringLength(100)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不符")]
        [Display(Name = "確認密碼")]
        public string? ConfirmPassword { get; set; }

        [StringLength(50)]
        [Display(Name = "電話")]
        public string? AccountPhone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? AccountEmail { get; set; }

        [Display(Name = "帳號狀態")]
        public bool AccountIsEnable { get; set; } = true;

        [Display(Name = "帳號備註")]
        public string? AccountRemark { get; set; }
    }

    // ==================== 編輯 ViewModel ====================
    public class FarmerEditViewModel
    {
        public int SN { get; set; }

        [Required(ErrorMessage = "養殖戶編號為必填")]
        [StringLength(50, ErrorMessage = "養殖戶編號不可超過50字元")]
        [Display(Name = "養殖戶編號")]
        public string TenantNum { get; set; } = string.Empty;

        [Required(ErrorMessage = "養殖戶名稱為必填")]
        [StringLength(50, ErrorMessage = "養殖戶名稱不可超過50字元")]
        [Display(Name = "養殖戶名稱")]
        public string TenantName { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "養殖戶地址不可超過150字元")]
        [Display(Name = "養殖戶地址")]
        public string? TenantAddr { get; set; }

        [StringLength(50, ErrorMessage = "負責人姓名不可超過50字元")]
        [Display(Name = "負責人姓名")]
        public string? ResponName { get; set; }

        [StringLength(50, ErrorMessage = "負責人電話不可超過50字元")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼")]
        [Display(Name = "負責人電話")]
        public string? ResponPhone { get; set; }

        [StringLength(100, ErrorMessage = "負責人Email不可超過100字元")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email地址")]
        [Display(Name = "負責人Email")]
        public string? ResponEmail { get; set; }

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        [Display(Name = "啟用狀態")]
        public bool IsEnable { get; set; }
    }

    // ==================== 詳細頁 ViewModel ====================
    public class FarmerDetailViewModel
    {
        public int SN { get; set; }

        [Display(Name = "養殖戶編號")]
        public string TenantNum { get; set; } = string.Empty;

        [Display(Name = "養殖戶名稱")]
        public string TenantName { get; set; } = string.Empty;

        [Display(Name = "養殖戶地址")]
        public string? TenantAddr { get; set; }

        [Display(Name = "負責人姓名")]
        public string? ResponName { get; set; }

        [Display(Name = "負責人電話")]
        public string? ResponPhone { get; set; }

        [Display(Name = "負責人Email")]
        public string? ResponEmail { get; set; }

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        [Display(Name = "啟用狀態")]
        public bool IsEnable { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "建立者")]
        public string CreateUser { get; set; } = string.Empty;

        [Display(Name = "最後修改時間")]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "最後修改者")]
        public string? ModifyUser { get; set; }

        // 關聯資料
        public List<AccountSummary> Accounts { get; set; } = new();
        public List<AccountGroupSummary> AccountGroups { get; set; } = new();
    }

    // ==================== 關聯資料摘要 ====================
    public class AccountSummary
    {
        public string AccountName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsEnable { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }

    public class AccountGroupSummary
    {
        public string GroupName { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public bool IsEnable { get; set; }
    }

    // ==================== 下拉選單 DTO ====================
    public class FarmerDropdownDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}