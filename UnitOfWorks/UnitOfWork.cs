using itRoot.Models;
using itRoot.Repos;
using itRoot.Repos.IRepos;
using itRoot.UnitOfWorks.IUnitOfWorks;

namespace itRoot.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly dbContext _db;
        public UnitOfWork(dbContext db)
        {
            _db = db;
        }

        private IUserRepo _userRepo;
        private IInvoiceItemRepo _invoiceItemRepo;
        private IInvoiceRepo _invoiceRepo;
        private IEmailConfirmationTokenRepo _emailConfirmationTokenRepo;

        public IUserRepo UserRepo => _userRepo ??= new UserRepo(_db);
        public IInvoiceItemRepo InvoiceItemRepo => _invoiceItemRepo ??= new InvoiceItemRepo(_db);
        public IInvoiceRepo InvoiceRepo => _invoiceRepo ??= new InvoiceRepo(_db);
        public IEmailConfirmationTokenRepo EmailConfirmationTokenRepo => _emailConfirmationTokenRepo ??= new EmailConfirmationTokenRepo(_db);
        public int Save()
        {
            return _db.SaveChanges();
        }

    }
}
