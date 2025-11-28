using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>魚種資料表</summary>
    [Table("FishVariety")]
    public class FishVariety
    {
        [Key]
        public int FVSN { get; set; }             // 序號，自動增加

        [Required]
        [StringLength(20)]
        public string FVName { get; set; } = "";  // 魚種名稱

        [StringLength(100)]
        public string? Remark { get; set; }       // 備註

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = "";

        public DateTime? ModifyTime { get; set; }

        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        [StringLength(50)]
        public string? DeleteUser { get; set; }

        // 導覽屬性：一個魚種可有多筆魚苗
        public ICollection<Fry> Fries { get; set; } = new List<Fry>();
        public ICollection<GrowTargetMain> GrowTargets { get; set; } = new List<GrowTargetMain>();
        public ICollection<FeedingPlanMain> FeedingPlans { get; set; } = new List<FeedingPlanMain>();
    }
}
