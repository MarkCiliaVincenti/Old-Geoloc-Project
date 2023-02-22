using System.Collections.Generic;
using MediatR;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Customer.Domain.Queries.Brands
{
    public class GetBrandsListQuery :IRequest<List<Brand>>
    {
    }
}
