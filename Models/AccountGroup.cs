using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ADAMS.Models
{
    [Table("AccountGroup")]
    public class AccountGroup
    {
        [Key]
        public int AccountGroupSN { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } // 群組名稱

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶 SN

        [ForeignKey(nameof(TenantSN))]
        public Tenant? Tenant { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();

        [StringLength(100)]
        public string? Remark { get; set; } // 備註，可為 Null

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
