using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 單位資料表 Unit
    /// </summary>
    [Table("Unit")]
    public class Unit
    {
        [Key]
        public int UnitSN { get; set; }                 // 序號，自動增加

        [Required]
        [StringLength(50)]
        public string UnitName { get; set; } = string.Empty;   // 單位名稱

        [StringLength(100)]
        public string? Remark { get; set; }             // 備註

        [Required]
        public bool IsDeleted { get; set; } = false;    // 是否刪除 0:否 1:是

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now; // 建立時間

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty;   // 建立者帳號

        public DateTime? ModifyTime { get; set; }       // 最後修改時間

        [StringLength(50)]
        public string? ModifyUser { get; set; }         // 最後修改者帳號

        public DateTime? DeleteTime { get; set; }       // 刪除時間

        [StringLength(50)]
        public string? DeleteUser { get; set; }         // 刪除者帳號

        
    }
}
