using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Feed")]
    public class Feed
    {
        [Key]
        public int FeedSN { get; set; }          // 序號，自動增加

        [Required]
        public int TenantSN { get; set; }        // 所屬養殖戶 SN

        [ForeignKey(nameof(TenantSN))]
        public Tenant? Tenant { get; set; }

        [Required]
        [StringLength(50)]
        public string FeedName { get; set; } = string.Empty;   // 飼料名稱

        [Required]
        public int SupplierSN { get; set; }      // 供應商 SN

        [ForeignKey(nameof(SupplierSN))]
        public Supplier? Supplier { get; set; }

        [Required]
        [StringLength(50)]
        public string UnitName { get; set; } = "公斤";        // 單位名稱（預設公斤）

        [StringLength(100)]
        public string? Remark { get; set; }      // 備註

        [Required]
        public bool IsDeleted { get; set; } = false;

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

        public ICollection<FeedingRecord> FeedingRecords { get; set; } = new List<FeedingRecord>();
    }
}
