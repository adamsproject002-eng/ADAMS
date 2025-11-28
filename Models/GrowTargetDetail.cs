using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 成長目標參考明細
    /// </summary>
    [Table("GrowTargetDetail")]
    public class GrowTargetDetail
    {
        [Key]
        public int GTDSN { get; set; }             // 序號，自動增加

        [Required]
        public int GTMSN { get; set; }             // 主表序號

        [ForeignKey(nameof(GTMSN))]
        public GrowTargetMain? Main { get; set; }

        [Required]
        public int GT_DOC { get; set; }            // DOC(days)

        [Column(TypeName = "decimal(10,2)")]
        public decimal GT_ABW { get; set; }        // ABW(g/pc)

        [Column(TypeName = "decimal(10,2)")]
        public decimal GT_DailyGrow { get; set; }  // DailyGrow(g/pc/day)

        [StringLength(100)]
        public string? Remark { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required, StringLength(50)]
        public string CreateUser { get; set; } = "";

        public DateTime? ModifyTime { get; set; }

        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        [StringLength(50)]
        public string? DeleteUser { get; set; }
    }
}
