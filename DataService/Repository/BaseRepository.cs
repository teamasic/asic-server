using AsicServer.Core.Constant;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
        Task<TEntity> FindAsync(object Id);
        IEnumerable<TEntity> GetAll();

    }

    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected DbContext dbContext { get; set; }

        public BaseRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            return query.ToList();
        }

        public async Task AddAsync(TEntity entity)
        {
            await dbContext.AddAsync(entity);
            dbContext.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
            dbContext.SaveChanges();
        }

        public virtual async Task<TEntity> FindAsync(object Id)
        {
            return await dbContext.Set<TEntity>().FindAsync(Id);
        }

        public void Update(TEntity entityToUpdate)
        {
            dbContext.Set<TEntity>().Attach(entityToUpdate);
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        IEnumerable<TEntity> IBaseRepository<TEntity>.GetAll()
        {
            return dbContext.Set<TEntity>().Take(Constants.MAX_GET_ALL_RECORD).ToList(); 
        }

    }
}
