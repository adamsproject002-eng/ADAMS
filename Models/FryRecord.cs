using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("FryRecord")]
    public class FryRecord
    {
        [Key]
        public int FryRecordSN { get; set; } // 序號，自動增加

        [Required]
        public int PondSN { get; set; } // 對應 Pond

        [Required]
        public int FarmingNum { get; set; } // 放養次數

        [Required]
        [StringLength(20)]
        public string FarmingCode { get; set; } = string.Empty; // 放養碼

        [Required]
        public int FrySN { get; set; } // 對應 Fry

        [Required]
        public DateTime FarmingDate { get; set; } // 放苗日期

        [Required]
        public int FarmingPCS { get; set; } // 放苗尾數

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FryAge { get; set; } // 苗齡(克/尾)

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PondArea { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FarmingDensity { get; set; } // 放養密度

        [Required]
        [StringLength(50)]
        public string ManageAccount { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Remark { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TTL_Weight { get; set; }

        public int? TTL_PCS { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SurvivalRate { get; set; }

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
        [ForeignKey("PondSN")]
        public Pond Pond { get; set; } = null!;
    }
}
