using itRoot.Models;
using itRoot.Repos.IRepos;

namespace itRoot.Repos
{
    public class InvoiceRepo : GenaricRepo<inVoice>, IInvoiceRepo
    {
        private readonly dbContext _db;
        public InvoiceRepo(dbContext db) : base(db)
        {
            _db = db;
        }
        public List<inVoice> GetInvoiceByUserId(int userId)
        {
            return _db.inVoices.Where(a => a.userId == userId).ToList();
        }

    }
}
