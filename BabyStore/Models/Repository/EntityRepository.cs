using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BabyStore.Extensions;
namespace BabyStore.Models
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        private readonly StoreContext context;

        public EntityRepository(StoreContext context)
        {
            this.context = context;
        }
        public virtual IQueryable<T> AllIncluding(
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = context.Set<T>();
            foreach (var includePropertie in includeProperties)
            {
                query = query.Include(includePropertie);
            }
            return query;
        }

        
        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = context.Set<T>();
            return query;
        }

        public virtual T GetSingle(int key)
        {
            return context.Set<T>().Find(key);
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate);
        }


        public virtual PaginatedList<T> Paginate<TKey>(
                    int pageIndex, int pageSize,
                    Expression<Func<T, TKey>> keySelector)
        {

            return Paginate(pageIndex, pageSize, keySelector, null);
        }
        public virtual PaginatedList<T> Paginate<TKey>(
           int pageIndex, int pageSize,
           Expression<Func<T, TKey>> keySelector,
           Expression<Func<T, bool>> predicate,
           params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = AllIncluding(includeProperties)
                .OrderBy(keySelector);

            query = predicate == null
                ? query
                : query.Where(predicate);

            return query.ToPaginatedList(pageIndex, pageSize);
        }



        public virtual void Add(T entity)
        {
            EntityEntry entityEntry = context.Entry<T>(entity);
            context.Set<T>().Add(entity);
        }
        public virtual void Edit(T entity)
        {
            EntityEntry entityEntry = context.Entry<T>(entity);
            context.Set<T>().Update(entity);
            // entityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            EntityEntry entityEntry = context.Entry<T>(entity);
            context.Set<T>().Remove(entity);
            // entityEntry.State = EntityState.Deleted;
        }
        public void Detache(T entity)
        {
            context.Entry(entity).State = EntityState.Detached;
        }
        public virtual void Save()
        {
            context.SaveChanges();
        }

       
    }
}
