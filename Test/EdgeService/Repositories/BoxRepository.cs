using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Data;

namespace EdgeService.Repositories
{
    public class BoxRepository
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;
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

        public BoxRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null)
                throw new ArgumentNullException("ambientDbContextLocator");
            _ambientDbContextLocator = ambientDbContextLocator;
        }

        public Box Get(Guid id)
        {
            return DbContext.Boxes.Find(id);
        }

        public Task<Box> GetAsync(Guid id)
        {
            return DbContext.Boxes.FindAsync(id);
        }

        public void Add(Box box)
        {
            DbContext?.Boxes.Add(box);

        }

        public IQueryable<Box> Where(Expression<Func<Box, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Box> SingleAsync(Expression<Func<Box, bool>> predicate)
        {
            return DbContext.Boxes.FirstOrDefaultAsync(predicate);
        }
    }
}