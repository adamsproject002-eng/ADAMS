using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Account")]
    public class Account
    {
        [Key]
        public int AccountSN { get; set; } // 主鍵，自動遞增

        [Required]
        [StringLength(50)]
        public string AccountName { get; set; } = string.Empty; // 帳號（命名避免與 class 同名）

        [Required]
        public string Password { get; set; } = string.Empty; // 密碼（加密後）

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶 SN

        [ForeignKey(nameof(TenantSN))]
        public Tenant? Tenant { get; set; }

        [Required]
        public int AccGroupSN { get; set; } // 權限群組 SN
        [ForeignKey(nameof(AccGroupSN))]
        public AccountGroup? AccGroup { get; set; }

        [StringLength(50)]
        public string? RealName { get; set; } // 姓名

        [StringLength(50)]
        public string? Phone { get; set; } // 電話，可空

        [StringLength(50)]
        public string? Email { get; set; } // Email，可空

        [StringLength(100)]
        public string? Remark { get; set; } // 備註，可空

        [Required]
        public bool IsEnable { get; set; } = true; // 是否啟用，預設啟用

        [Required]
        public bool IsDeleted { get; set; } = false; // 是否刪除

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now; // 建立時間

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty; // 建立人

        public DateTime? ModifyTime { get; set; } // 修改時間

        [StringLength(50)]
        public string? ModifyUser { get; set; } // 修改人

        public DateTime? DeleteTime { get; set; } // 刪除時間

        [StringLength(50)]
        public string? DeleteUser { get; set; } // 刪除人
    }
}
