using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BabyStore.Models
{
    public interface IEntityRepository<T> where T : class
    {
        IQueryable<T> AllIncluding(
            params Expression<Func<T, object>>[] includeProperties
            );
         


        IQueryable<T> GetAll();
        T GetSingle(int key);
        IQueryable<T> FindBy(Expression<Func<T,bool>> predicate);

        PaginatedList<T> Paginate<TKey>(
        int pageIndex, int pageSize,
        Expression<Func<T, TKey>> keySelector);
           PaginatedList<T> Paginate<TKey>(
           int pageIndex, int pageSize,
           Expression<Func<T, TKey>> keySelector,
           Expression<Func<T, bool>> predicate,
           params Expression<Func<T, object>>[] includeProperties);
        



            void Add(T entity);
        void Edit(T entity);
        void Delete(T entity);
        void Detache(T entity);
        void Save();
    }
}
