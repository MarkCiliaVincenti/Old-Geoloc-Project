using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using SmartFleet.Data;

namespace SmartFLEET.Web.Areas.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly SmartFleetObjectContext ObjectContext;
        protected readonly IMapper Mapper;

        public BaseApiController(SmartFleetObjectContext objectContext)
        {
            ObjectContext = objectContext;
        }
        public BaseApiController(SmartFleetObjectContext objectContext, IMapper mapper)
        {
            ObjectContext = objectContext;
            Mapper = mapper;
        }

        public BaseApiController()
        {
            
        }

    }
}
