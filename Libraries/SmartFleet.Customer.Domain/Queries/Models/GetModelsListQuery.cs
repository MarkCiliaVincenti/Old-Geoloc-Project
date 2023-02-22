using System.Collections.Generic;
using MediatR;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Customer.Domain.Queries.Models
{
    public class GetModelsListQuery :IRequest<List<Model>>
    {
    }
}
