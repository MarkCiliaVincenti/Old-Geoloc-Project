using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartFleet.Core;
using SmartFleet.Core.Data;

namespace SmartFleet.Data
{
    public class EfScopeRepository<T>:IScopeRepository<T> where T : BaseEntity
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public EfScopeRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            _ambientDbContextLocator = ambientDbContextLocator;
        }
        private IDbSet<T> _entities;
        private SmartFleetObjectContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<SmartFleetObjectContext>();

                if (dbContext == null)
                    throw new InvalidOperationException("No ambient DbContext of type UserManagementDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }
        public T Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync(Guid id)
        {
            return Entities?.FirstOrDefaultAsync(e => e.Id == id);
        }

        public void Add(T entity)
        {
            if (entity==null)
                throw new ArgumentNullException(nameof(entity));
            if(DbContext != null)
                Entities?.Add(entity);
           // DbContext.SaveChanges();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Entities?.Where(predicate);
        }

        public Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
        {
            if (DbContext != null)
                return Entities?.SingleAsync(predicate);
            return null;
        }
        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = DbContext?.Set<T>();
                return _entities;
            }
        }
    }
}