using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Tenant")]
    public class Tenant
    {
        [Key]
        public int SN { get; set; } // 主鍵，自動遞增

        [Required]
        [StringLength(50)]
        public string TenantNum { get; set; } = string.Empty; // 養殖戶編號

        [Required]
        [StringLength(50)]
        public string TenantName { get; set; } = string.Empty; // 養殖戶名稱

        [StringLength(150)]
        public string? TenantAddr { get; set; } // 養殖戶地址

        [StringLength(50)]
        public string? ResponName { get; set; } // 負責人姓名

        [StringLength(50)]
        public string? ResponPhone { get; set; } // 負責人電話

        [StringLength(100)]
        public string? ResponEmail { get; set; } // 負責人 Email

        [Required]
        public bool IsEnable { get; set; } = true; // 是否啟用，預設為 1

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now; // 建立時間

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty; // 建立者帳號

        public DateTime? ModifyTime { get; set; } // 最後修改時間

        [StringLength(50)]
        public string? ModifyUser { get; set; } // 最後修改者帳號

        [StringLength(500)]
        public string? Remark { get; set; }  // 備註

        // 導覽屬性
        public ICollection<AccountGroup> AccountGroups { get; set; } = new List<AccountGroup>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Area> Areas { get; set; } = new List<Area>();
        public ICollection<Pond> Ponds { get; set; } = new List<Pond>();
        public ICollection<Fry> Fries { get; set; } = new List<Fry>();
        public ICollection<Feed> Feeds { get; set; } = new List<Feed>();
    }
}
