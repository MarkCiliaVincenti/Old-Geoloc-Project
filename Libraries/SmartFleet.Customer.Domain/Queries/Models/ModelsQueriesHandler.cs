using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Data;

namespace SmartFleet.Customer.Domain.Queries.Models
{
    public class ModelsQueriesHandler :IRequestHandler<GetModelsListQuery, List<Model>>
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IMapper _mapper;

        public ModelsQueriesHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
        }
        public async Task<List<Model>> Handle(GetModelsListQuery request, CancellationToken cancellationToken)
        {
            using (var dbFactory = _dbContextScopeFactory.Create())
            {
                var db = dbFactory.DbContexts.Get<SmartFleetObjectContext>();
                return await db.Models.ToListAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
