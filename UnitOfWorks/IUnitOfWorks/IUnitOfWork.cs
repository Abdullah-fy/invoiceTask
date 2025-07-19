using itRoot.Repos.IRepos;

namespace itRoot.UnitOfWorks.IUnitOfWorks
{
    public interface IUnitOfWork
    {
        public IUserRepo UserRepo { get; }
        public IInvoiceRepo InvoiceRepo { get; }
        public IInvoiceItemRepo InvoiceItemRepo { get; }
        public IEmailConfirmationTokenRepo EmailConfirmationTokenRepo { get; }

        public int Save();
    }
 }
    

