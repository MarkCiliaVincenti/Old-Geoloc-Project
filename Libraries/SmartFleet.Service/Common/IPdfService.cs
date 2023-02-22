using System.Collections.Generic;
using System.IO;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Service.Common
{
    public interface IPdfService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="vehicle"></param>
        /// <param name="report"></param>
        /// <param name="stream"></param>
        void CreatePdfReport(List<Position> positions, Vehicle vehicle, dynamic report,
            MemoryStream stream);
    }
}