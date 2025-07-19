using itRoot.Models;
using itRoot.Repos.IRepos;

namespace itRoot.Repos
{
    public class UserRepo : GenaricRepo<user>, IUserRepo
    {
        private readonly dbContext _db;
        public UserRepo(dbContext db) : base(db)
        {
            _db = db;
        }

        public user GetUserByEmail(string email)
        {
            return _db.users.FirstOrDefault(a => a.email == email);
        }

        public user? GetUserByUserName(string userName)
        {
            var user = _db.users.FirstOrDefault( a => a.userName == userName);
            if(user != null)
            {
                return user;
            }
            return null;
        }

    }
}
