using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Authorization")]
    public class Authorization
    {
        [Key]
        public int AuthorizationSN { get; set; } // 主鍵，自動遞增

        [Required]
        public int AccGroupSN { get; set; } // 權限群組 SN（對應 AccountGroup.AccountGroupSN）

        [ForeignKey(nameof(AccGroupSN))]
        public AccountGroup? AccountGroup { get; set; }
        [Required]
        public int FunctionSN { get; set; } // 系統功能 SN（對應 Function.FunctionSN）

        [ForeignKey(nameof(FunctionSN))]
        public Function? Function { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false; // 是否刪除：0=否，1=是

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now; // 建立時間

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } // 建立者帳號

        public DateTime? ModifyTime { get; set; } // 最後修改時間

        [StringLength(50)]
        public string? ModifyUser { get; set; } // 最後修改者帳號

        public DateTime? DeleteTime { get; set; } // 刪除時間

        [StringLength(50)]
        public string? DeleteUser { get; set; } // 刪除者帳號
    }
}
