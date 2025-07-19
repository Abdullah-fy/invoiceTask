using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace itRoot.Models;

public partial class inVoice
{
    [Key]
    public int inVoiceId { get; set; }

    public int? userId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal totalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? inVoiceDate { get; set; }

    [InverseProperty("inVoice")]
    public virtual ICollection<inVoiceItem> inVoiceItems { get; set; } = new List<inVoiceItem>();

    [ForeignKey("userId")]
    [InverseProperty("inVoices")]
    public virtual user user { get; set; }
}
