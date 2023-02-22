using System.Collections.Generic;
using MediatR;

namespace SmartFleet.Customer.Domain.Queries.Customers
{
    public class GetCustomersListQuery :  IRequest<List<Core.Domain.Customers.Customer>>
    {
        
    }
}
