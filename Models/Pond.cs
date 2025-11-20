using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Pond")]
    public class Pond
    {
        [Key]
        public int PondSN { get; set; } // 序號，自動增加

        [Required]
        public int AreaSN { get; set; } // 場區序號，對應 Area

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶SN

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PondWidth { get; set; } // 最寬邊(m)

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PondLength { get; set; } // 長/直徑(m)

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PondArea { get; set; } // 水面積

        [StringLength(20)]
        public string? GPSX { get; set; }

        [StringLength(20)]
        public string? GPSY { get; set; }

        [StringLength(100)]
        public string? Remark { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty;

        public DateTime? ModityTime { get; set; }
        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }
        [StringLength(50)]
        public string? DeleteUser { get; set; }

        // 導覽屬性
        [ForeignKey("AreaSN")]
        public Area Area { get; set; } = null!;
        public ICollection<FryRecord> FryRecords { get; set; } = new List<FryRecord>();
    }
}
