using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Function")]
    public class Function
    {
        [Key]
        public int FunctionSN { get; set; } // 主鍵，自動遞增

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty; // Controller 名稱或路由名稱

        [Required]
        [StringLength(50)]
        public string CName { get; set; } = string.Empty; // 中文名稱（顯示用）

        [Required]
        public int FLevel { get; set; } // 選單階層（1: 主選單，2: 次選單）

        [Required]
        public int UpperFunctionSN { get; set; } = 0; // 上層功能編號，頂層為 0

        [Required]
        public int Sort { get; set; } = 0; // 排序編號，數字越小越前面

        [Required]
        public bool IsDefault { get; set; } = false; // 是否預設權限

        [Required]
        public bool IsShow { get; set; } = true; // 是否顯示在選單中

        public ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();
    }
}
