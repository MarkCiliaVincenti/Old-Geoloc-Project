using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SmartFleet.Data;
using SmartFleet.Service.Authentication;
using SmartFleet.Service.Models;
using SmartFleet.Service.Report;
using SmartFleet.Service.Tracking;
using SmartFleet.Service.Vehicles;

namespace SmartFLEET.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PositionController : BaseController
    {
        private readonly IPositionService _positionService;
        private readonly IVehicleService _vehicleService;
        private readonly IAuthenticationService _authentication;

        // GET: Position
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult GetCurrentPosition()
        {
            return PartialView("_CurrentPosition");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetCurrentDayPosition(string vehicleId)
        {
            var id = Guid.Parse(vehicleId);
            //var start = 
            var endPeriod = DateTime.Now.ToUniversalTime();
            var startPeriod = DateTime.Now.Date.ToUniversalTime();
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id).ConfigureAwait(false);
            var positions = await _positionService.GetVehiclePositionsByPeriodAsync(id, startPeriod, endPeriod).ConfigureAwait(false);
            if (!positions.Any())
                return Json(new List<TargetViewModel>(), JsonRequestBehavior.AllowGet);
            var gpsCollection = positions.Select(x =>
                new { Latitude = x.Lat, Longitude = x.Long, GpsStatement = x.Timestamp.ToString("O") });
            var positionReport = new ActivitiesReport();
            return Json(new { Periods = positionReport.BuildDailyReport(positions, startPeriod, vehicle.VehicleName), GpsCollection = gpsCollection }, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GetPositionByDate(string  vehicleId, DateTime start,  DateTime? end )
        {
            var id = Guid.Parse(vehicleId);
            // var endPeriod = DateTime.Now;
             var endPeriod = end?.ToUniversalTime() ?? start.Date.AddDays(1).AddTicks(-1).ToUniversalTime();
            var vehicle = await ObjectContext.Vehicles.FindAsync(id).ConfigureAwait(false);
            var user = await ObjectContext.UserAccounts
                .FirstOrDefaultAsync(x=>x.UserName == User.Identity.Name).ConfigureAwait(false);
            var positions = await _positionService
                .GetVehiclePositionsByPeriodAsync(id, start.ToUniversalTime(), endPeriod, user?.TimeZoneInfo)
                .ConfigureAwait(false);
            if (!positions.Any()) return Json(new List<TargetViewModel>(), JsonRequestBehavior.AllowGet);
            var gpsCollection = positions.OrderBy(x => x.Timestamp)
                .Select(x => new {Latitude = x.Lat, Longitude = x.Long, GpsStatement = x.Timestamp.ToString("O")});
            var positionReport = new ActivitiesReport();
            var result = positionReport.BuildDailyReport(positions.OrderBy(x => x.Timestamp).ToList(), start,
                vehicle.VehicleName);
            var distance = result.Where(x => x.MotionStatus == "Moving").Sum(x => x.Distance);
            return Json(
                new
                {
                    Vehiclename = vehicle?.VehicleName, Distance = Math.Round(distance, 2), Periods = result,
                    GpsCollection = gpsCollection.Distinct()
                }, JsonRequestBehavior.AllowGet);
        }

        public PositionController(SmartFleetObjectContext objectContext, IVehicleService vehicleService) : base(objectContext)
        {
            _vehicleService = vehicleService;
        }

        public PositionController(SmartFleetObjectContext objectContext, IMapper mapper, IPositionService positionService, IVehicleService vehicleService, IAuthenticationService authentication) : base(objectContext, mapper)
        {
            _positionService = positionService;
            _vehicleService = vehicleService;
            _authentication = authentication;
        }
    }
}