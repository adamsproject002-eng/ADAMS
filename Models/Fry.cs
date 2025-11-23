using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Fry")]
    public class Fry
    {
        [Key]
        public int FrySN { get; set; } // 序號，自動增加

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶SN

        [Required]
        [StringLength(50)]
        public string FryName { get; set; } = string.Empty; // 魚苗名稱

        [Required]
        public int SupplierSN { get; set; } // 供應商SN

        [Required]
        [StringLength(20)]
        public string FVName { get; set; } = string.Empty; // 魚種名稱

        [Required]
        [StringLength(50)]
        public string UnitName { get; set; } = string.Empty; // 單位名稱

        [StringLength(100)]
        public string? Remark { get; set; } // 備註

        [Required]
        public bool IsDeleted { get; set; } = false; // 是否刪除-0:否,1:是

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now; // 建立時間

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty; // 建立者的Account

        public DateTime? ModifyTime { get; set; } // 最後修改時間

        [StringLength(50)]
        public string? ModifyUser { get; set; } // 最後修改的Account

        public DateTime? DeleteTime { get; set; } // 刪除時間

        [StringLength(50)]
        public string? DeleteUser { get; set; } // 刪除的Account

        // 導覽屬性
        [ForeignKey("SupplierSN")]
        public Supplier Supplier { get; set; } = null!;
    }
}
