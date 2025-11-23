using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        public int SupplierSN { get; set; } // 序號，自動增加

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶SN

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

        [Required]
        public int SupplierType { get; set; } // 營業型態-1:飼料 2:魚苗 3:其他

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
    }
}
