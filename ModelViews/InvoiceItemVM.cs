using itRoot.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace itRoot.ModelViews
{
    public class InvoiceItemVM
    {

        [Required]
        [StringLength(50)]
        public string productName { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal price { get; set; }
        public decimal lineTotal => price * quantity;
    }
}
