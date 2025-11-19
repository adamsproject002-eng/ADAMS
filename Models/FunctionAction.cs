using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    public class FunctionAction
    {
        [Key]
        public int FunctionActionSN { get; set; }

        [ForeignKey(nameof(Function))]
        public int FunctionSN { get; set; }

        public Function Function { get; set; } = null!;

        public string ActionCode { get; set; } = null!;
    }
}
