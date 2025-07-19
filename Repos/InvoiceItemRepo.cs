using itRoot.Models;
using itRoot.Repos.IRepos;

namespace itRoot.Repos
{
    public class InvoiceItemRepo : GenaricRepo<inVoiceItem>, IInvoiceItemRepo
    {
        private readonly dbContext _db;
        public InvoiceItemRepo(dbContext db) : base(db)
        {
            _db = db;
        }
    }
}
