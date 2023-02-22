using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Data;
using SmartFleet.Data;

namespace SmartFleet.Customer.Domain.Queries.Customers
{
    public class CustomersQueriesHandler : IRequestHandler<GetCustomersListQuery,List<Core.Domain.Customers.Customer>>
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IMapper _mapper;

        public CustomersQueriesHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
        }
        public async Task<List<Core.Domain.Customers.Customer>> Handle(GetCustomersListQuery request, CancellationToken cancellationToken)
        {
            using (var dbFactory = _dbContextScopeFactory.Create())
            {
                var db = dbFactory.DbContexts.Get<SmartFleetObjectContext>();
                return await db.Customers.ToListAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
