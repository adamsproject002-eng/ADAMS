using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        public int SupplierSN { get; set; } // 序號，自動遞增

        [Required]
        public int TenantSN { get; set; }   // 所屬養殖戶 SN

        [Required]
        [StringLength(20)]
        public string SupplierNum { get; set; } = string.Empty; // 供應商代號

        [Required]
        [StringLength(50)]
        public string SupplierName { get; set; } = string.Empty; // 供應商名稱

        [StringLength(50)]
        public string? ContactName { get; set; } // 聯絡人姓名

        [StringLength(50)]
        public string? ContactPhone { get; set; } // 聯絡人電話

        /// <summary>
        /// 營業型態：1=飼料、2=魚苗、3=其他
        /// </summary>
        [Required]
        public int SupplierType { get; set; }

        [StringLength(100)]
        public string? Remark { get; set; } // 備註

        [Required]
        public bool IsDeleted { get; set; } = false; // 是否刪除：0=否，1=是

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty;

        public DateTime? ModifyTime { get; set; }

        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        [StringLength(50)]
        public string? DeleteUser { get; set; }
        public ICollection<Feed> Feeds { get; set; } = new List<Feed>();
    }
}
