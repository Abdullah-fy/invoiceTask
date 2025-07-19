
using System.Collections.Generic;
using System.Linq.Expressions;
using itRoot.Models;
using itRoot.Repos.IRepos;
using Microsoft.EntityFrameworkCore;

namespace itRoot.Repos
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : class
    {
        private readonly dbContext _db;
        public DbSet<T> dbset;
        public GenaricRepo(dbContext db)
        {
            _db = db;
            this.dbset = _db.Set<T>();  
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }
        public void Remove(int id)
        {
            T? entity = dbset.Find(id);
            if(entity != null)
            {
                dbset.Remove(entity);
             }
         }
        public T FirstOrDefault(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {

            IQueryable<T> query = dbset;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var prop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }
            return query.FirstOrDefault();
        }

        

        public T GetById(int id)
        {
            T? entity = dbset.Find(id);
            return entity;
        }

        public void Insert(T item)
        {
            dbset.Add(item);
        }

        

        public void Update(T item)
        {
            dbset.Update(item);
        }

        public void save()
        {
            _db.SaveChanges();
        }
    }
}
