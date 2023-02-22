using System.Web;
using MediatR;
using SmartFleet.Customer.Domain.Common.Dtos;

namespace SmartFleet.Customer.Domain.Queries.Vehicles
{
    public class GetVehiclesListQuery : IRequest<DataTablesModel<VehicleDto>>
    {
        public HttpRequestBase Request { get; set; }
    }

}
