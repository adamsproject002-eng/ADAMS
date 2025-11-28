using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 成長目標參考主表
    /// </summary>
    [Table("GrowTargetMain")]
    public class GrowTargetMain
    {
        [Key]
        public int GTMSN { get; set; }              // 序號，自動增加

        [Required]
        [StringLength(50)]
        public string GTMName { get; set; } = "";   // 成長目標名稱（例如：白蝦成長目標）

        [Required]
        public int FVSN { get; set; }               // 魚種序號，對應 FishVariety

        [ForeignKey(nameof(FVSN))]
        public FishVariety? FishVariety { get; set; }

        [StringLength(100)]
        public string? Remark { get; set; }         // 備註

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

        public ICollection<GrowTargetDetail> Details { get; set; } = new List<GrowTargetDetail>();
    }
}
