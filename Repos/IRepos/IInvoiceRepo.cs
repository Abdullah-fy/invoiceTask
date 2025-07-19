using itRoot.Models;

namespace itRoot.Repos.IRepos
{
    public interface IInvoiceRepo : IGenaricRepo<inVoice>
    {
        List<inVoice> GetInvoiceByUserId(int userId);
    }
}
