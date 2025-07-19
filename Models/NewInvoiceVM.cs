using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using itRoot.ModelViews;

namespace itRoot.Models
{
    public class NewInvoiceVM
    {
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public List<InvoiceItemVM> Items { get; set; } = new List<InvoiceItemVM>();
        public decimal TotalAmount => Items.Sum(x => x.lineTotal);
    }
}
