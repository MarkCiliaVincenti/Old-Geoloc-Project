using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartFleet.Core.Data;
using SmartFleet.Customer.Domain.Common.Dtos;
using SmartFleet.Data;
using SmartFleet.Web.Framework.DataTables;

namespace SmartFleet.Customer.Domain.Queries.Vehicles
{
    public class VehiclesQueriesHandler :
        IRequestHandler<GetVehiclesListQuery, DataTablesModel<VehicleDto>>,
        IRequestHandler<GetVehicleByMobileUnitImeiQuery , VehicleDto>
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IMapper _mapper;
        private readonly DataTablesLinqQueryBulider _queryBuilder;

        public VehiclesQueriesHandler(IDbContextScopeFactory dbContextScopeFactory, IMapper mapper, DataTablesLinqQueryBulider queryBuilder)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _mapper = mapper;
            _queryBuilder = queryBuilder;
        }
        public async Task<DataTablesModel<VehicleDto>> Handle(GetVehiclesListQuery request, CancellationToken cancellationToken)
        {
            using (var dbFactory = _dbContextScopeFactory.Create())
            {
                var db = dbFactory.DbContexts.Get<SmartFleetObjectContext>();
                var queryable =   db.Vehicles
                    .Include("Brand")
                    .Include("Model")
                    .Include("Customer");
                var query = _queryBuilder.BuildQuery(request.Request, queryable);
                var jsResult = new DataTablesModel<VehicleDto>
                {
                    recordsTotal = query.recordsTotal,
                    draw = query.draw,
                    recordsFiltered = query.recordsFiltered,
                    data = _mapper.Map<List<VehicleDto>>( await query.data.ToListAsync(cancellationToken).ConfigureAwait(false)),
                    lenght = query.length
                };
                return jsResult;

            }
        }

        public async Task<VehicleDto> Handle(GetVehicleByMobileUnitImeiQuery request, CancellationToken cancellationToken)
        {
            using (var dbFactory = _dbContextScopeFactory.Create())
            {
                var db = dbFactory.DbContexts.Get<SmartFleetObjectContext>();
                var query = await (from v in db.Vehicles
                    join dbBox in db.Boxes on v.Id equals dbBox.VehicleId into boxes
                    from box in boxes
                    where box.Imei == request.Imei
                    select new 
                    {
                        v.Id,
                        v.VehicleName,
                        v.CustomerId, 
                        v.VehicleType,
                        BoxeID=box.Id
                    }).FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                return query!= null ? new VehicleDto(query.VehicleName, query.Id, query.CustomerId.ToString(), query.VehicleType){MobileUnitId = query.BoxeID} : default;
            }

        }
    }
}
