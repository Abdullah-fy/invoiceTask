using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace itRoot.Models
{
    public class EmailConfirmationToken
    {
        [Key]
        public int tokenId { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        [StringLength(255)]
        public string token { get; set; } = string.Empty;

        [Required]
        public DateTime createdAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime expiresAt { get; set; }

        public bool isUsed { get; set; } = false;

        [ForeignKey("userId")]
        public virtual user User { get; set; } = null!;
    }
}
