using itRoot.Models;

namespace itRoot.Repos.IRepos
{
    public interface IUserRepo : IGenaricRepo<user>
    {
        user? GetUserByUserName(string userName);
        user GetUserByEmail(string email);
    }
}
