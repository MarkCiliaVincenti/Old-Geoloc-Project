using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using SmartFleet.Data;
using SmartFLEET.Web.Areas.Administrator.Models;

namespace SmartFLEET.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly SmartFleetObjectContext ObjectContext;
        /// <summary>
        /// 
        /// </summary>
        protected readonly IMediator Mediator;
        /// <summary>
        /// 
        /// </summary>
        protected readonly IMapper Mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectContext"></param>
        public BaseController(SmartFleetObjectContext objectContext)
        {
            ObjectContext = objectContext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectContext"></param>
        /// <param name="mediator"></param>
        public BaseController(SmartFleetObjectContext objectContext, IMediator mediator)
        {
            ObjectContext = objectContext;
            Mediator = mediator;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectContext"></param>
        /// <param name="mapper"></param>
        public BaseController(SmartFleetObjectContext objectContext, IMapper mapper)
        {
            ObjectContext = objectContext;
            Mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="mapper"></param>
        public BaseController(IMediator mediator, IMapper mapper)
        {
            Mediator = mediator;
            Mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public BaseController( IMapper mapper)
        {
            Mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ValidationViewModel ValidationViewModel()
        {
            ValidationViewModel validationModel;
            var errors = (from modelStateValue in ModelState.Values
                from error in modelStateValue.Errors
                select error.ErrorMessage).ToList();
            validationModel = new ValidationViewModel(errors, "Validation errors");
            return validationModel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected List<string> ValidationViewModel(ValidationResult result)
        {
            var errors = new List<string>();
            if (result?.Errors != null)
                foreach (var error in result.Errors)
                    errors.Add(error.ErrorMessage);
            return errors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        protected Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return Mediator.Send(request);
        }
        
    }
}