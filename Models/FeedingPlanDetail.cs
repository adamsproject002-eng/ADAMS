using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 投餌規劃明細表
    /// </summary>
    [Table("FeedingPlanDetail")]
    public class FeedingPlanDetail
    {
        [Key]
        public int FPDSN { get; set; }          // 序號，自動增加

        [Required]
        public int FPMSN { get; set; }          // 對應主表序號

        [ForeignKey(nameof(FPMSN))]
        public FeedingPlanMain? Main { get; set; }

        [Required]
        public int FP_DOC { get; set; }         // DOC(days)

        [Column(TypeName = "decimal(10,2)")]
        public decimal FP_ABW { get; set; }     // ABW(g/pc)

        [Column(TypeName = "decimal(10,2)")]
        public decimal FP_FeedingRate { get; set; } // 投餌率(%)

        [StringLength(100)]
        public string? Remark { get; set; }

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
    }
}
