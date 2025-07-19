using System.Linq.Expressions;

namespace itRoot.Repos.IRepos
{
    public interface IGenaricRepo<T> where T : class 
    {
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        public T GetById(int id);
        public void Insert(T item);
        public void Update(T item);
        public void Remove(int id);
        public T FirstOrDefault(Expression<Func<T, bool>>? filter,  string? includeProperties = null);
        public void save();
    }
}
