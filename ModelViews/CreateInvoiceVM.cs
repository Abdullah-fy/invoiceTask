namespace itRoot.ModelViews
{
    public class CreateInvoiceVM
    {
        public DateTime invoiceDate { get; set; } = DateTime.Now;
        public List<InvoiceItemVM> items { get; set; } = new List<InvoiceItemVM>();
        public decimal totalAmount => items.Sum(x => x.lineTotal);
    }
}
