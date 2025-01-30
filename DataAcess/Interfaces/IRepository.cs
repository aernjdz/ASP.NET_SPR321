using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //CRUD oerations
        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           params string[] includeProperties);
        TEntity GetByID(object id, params string[] includeProperties);
        void Insert(TEntity entity);
        void Delete(object id);
        void DeleteRange(List<object> ids);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
        void Save();
    }
}
