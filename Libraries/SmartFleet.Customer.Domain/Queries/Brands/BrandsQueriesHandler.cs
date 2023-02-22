using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Data;

namespace SmartFleet.Customer.Domain.Queries.Brands
{
    public class BrandsQueriesHandler :IRequestHandler<GetBrandsListQuery, List<Brand>>
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IMapper _mapper;

        public BrandsQueriesHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
        }
        public async Task<List<Brand>> Handle(GetBrandsListQuery request, CancellationToken cancellationToken)
        {
            using (var dbFactory = _dbContextScopeFactory.Create())
            {
                var db = dbFactory.DbContexts.Get<SmartFleetObjectContext>();
                return await db.Brands.ToListAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
