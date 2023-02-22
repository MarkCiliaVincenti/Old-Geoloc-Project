using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Service.Models;

namespace SmartFleet.Service.Tracking
{
    public interface IPositionService
    {
        Task<List<PositionViewModel>> GetLastVehiclePositionAsync(string userName);

        Task<List<Position>> GetVehiclePositionsByPeriodAsync(Guid vehicleId, DateTime startPeriod,
            DateTime endPeriod,string timeZoneInfo  = null);
    }
}