using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace itRoot.Models;

[Table("inVoiceItem")]
public partial class inVoiceItem
{
    [Key]
    public int itemId { get; set; }

    public int inVoiceId { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string productName { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal price { get; set; }

    [Column(TypeName = "decimal(21, 4)")]
    public decimal? lineTotal { get; set; }

    [ForeignKey("inVoiceId")]
    [InverseProperty("inVoiceItems")]
    public virtual inVoice inVoice { get; set; }
}
