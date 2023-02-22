using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartFleet.Core.Domain.Gpsdevices;

namespace EdgeService.Repositories
{
    public interface IMicroSericeRepository<T>
    {
        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Box Get(Guid id);
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Adds the specified box.
        /// </summary>
        /// <param name="box">The box.</param>
        void Add(T box);
        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Singles the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);



    }
}