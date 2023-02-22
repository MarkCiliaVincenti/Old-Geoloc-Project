using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Data;
using SmartFleet.Core.Geofence;
using SmartFleet.Data;
using SmartFleet.MobileUnit.Domain.Movements.Queries;
using SmartFleet.Web.Framework.DataTables;

namespace SmartFleet.MobileUnit.Domain.Movements.Handlers
{
    public class MovementsQueriesHandler  :IRequestHandler<GetLastPositionByMobileUnitIdQuery, GeofenceHelper.Position>
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IMapper _mapper;
        private readonly DataTablesLinqQueryBulider _queryBuilder;

        public MovementsQueriesHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper, DataTablesLinqQueryBulider queryBuilder)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
            _queryBuilder = queryBuilder;
        }
        public async Task<GeofenceHelper.Position> Handle(GetLastPositionByMobileUnitIdQuery request, CancellationToken cancellationToken)
        {
            using (var factory = _dbContextScopeFactory.Create())
            {
                var dbContext = factory.DbContexts.Get<SmartFleetObjectContext>();
                var lastPosition = await
                    dbContext.Positions
                        .Where(x => x.Box_Id == request.MobileUnitId)
                        .OrderByDescending(p => p.Timestamp)
                        .Select(x=> new {x.Long , x.Lat})
                        .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return lastPosition != null
                    ? new GeofenceHelper.Position
                    {
                        Latitude = lastPosition.Lat,
                        Longitude = lastPosition.Long
                    }
                    : default;
            }
        }
    }
}
