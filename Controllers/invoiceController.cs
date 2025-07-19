using System.Security.Claims;
using AspNetCoreGeneratedDocument;
using itRoot.Models;
using itRoot.ModelViews;
using itRoot.Repos;
using itRoot.Repos.IRepos;
using itRoot.UnitOfWorks.IUnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace itRoot.Controllers
{
    [Authorize]
    public class invoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public invoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var user = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var invoices = _unitOfWork.InvoiceRepo.GetInvoiceByUserId(user);
            return View(invoices);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateInvoiceVM();
             if (model.items == null)
            {
                model.items = new List<InvoiceItemVM>();
            }
            model.items.Add(new InvoiceItemVM());
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(CreateInvoiceVM createInvoiceVM)
        {
            if (createInvoiceVM == null)
            {
                createInvoiceVM = new CreateInvoiceVM();
            }
            if (createInvoiceVM.items == null)
            {
                createInvoiceVM.items = new List<InvoiceItemVM>();
            }
            if (createInvoiceVM.items.Count == 0)
            {
                createInvoiceVM.items.Add(new InvoiceItemVM());
            }
            if (!ModelState.IsValid)
            {
                return View(createInvoiceVM);
            }
            var invoice = new inVoice
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                totalAmount = createInvoiceVM.totalAmount,
                inVoiceDate = createInvoiceVM.invoiceDate
            };
            _unitOfWork.InvoiceRepo.Insert(invoice);
            _unitOfWork.Save();

            foreach(var item in createInvoiceVM.items)
            {
                var invoiceitem = new inVoiceItem
                {
                    inVoiceId = invoice.inVoiceId,
                    productName = item.productName,
                    quantity = item.quantity,
                    price = item.price
                };
                _unitOfWork.InvoiceItemRepo.Insert(invoiceitem);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

    }
}
