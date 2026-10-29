using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;

namespace Taxi.Infrastructure.Repos
{
    public interface IGenericRepository<T> where T : BaseEntity, new()
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(Guid id);
        void Delete(T entity);
        void Update(T entity);
        Guid Add(T entity);
        Task<T> GetByPredicate(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetManyByPredicate(Expression<Func<T, bool>> predicate);
    }
}
