using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace itRoot.Models;

[Table("user")]
[Index("userName", Name = "UQ__user__66DCF95CA95E63A3", IsUnique = true)]
[Index("email", Name = "UQ__user__AB6E61642D76C50D", IsUnique = true)]
public partial class user
{
    [Key]
    public int userId { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string userName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string email { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string fullName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string password { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? phone { get; set; }

    public bool? isConfirmed { get; set; }

    [InverseProperty("user")]
    public virtual ICollection<inVoice> inVoices { get; set; } = new List<inVoice>();
}
