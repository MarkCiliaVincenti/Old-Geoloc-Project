using System;
using System.Collections.Generic;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Service.Models;

namespace SmartFleet.Service.Report
{
    public interface IActivitiesReport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positionsOfVehicle"></param>
        /// <returns></returns>
        List<PositionViewModel> PositionViewModels(List<Position> positionsOfVehicle );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="startPeriod"></param>
        /// <param name="vehicleName"></param>
        /// <returns></returns>
        List<TargetViewModel> BuildDailyReport(List<Position> positions, DateTime startPeriod, string vehicleName);

        List<TargetViewModel> BuildReport(List<Position> positions, DateTime startPeriod, string vehicleName, List<Periods> periods);
    }
}