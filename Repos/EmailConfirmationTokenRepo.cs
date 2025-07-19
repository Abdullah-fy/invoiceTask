using itRoot.Models;
using itRoot.Repos.IRepos;

namespace itRoot.Repos
{
    public class EmailConfirmationTokenRepo : GenaricRepo<EmailConfirmationToken>, IEmailConfirmationTokenRepo
    {
        private readonly dbContext _db;
        public EmailConfirmationTokenRepo(dbContext db) : base(db) 
        {
            
        }
    }
}
