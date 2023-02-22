using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SmartFleet.Core.Data;
using SmartFleet.Core.Geofence;
using SmartFleet.Data.Dbcontextccope.Implementations;

namespace SmartFLEET.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public static class SignalRHubManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static IHubCallerConnectionContext<dynamic> Clients { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string> Connections = new Dictionary<string, string>();
        /// <summary>
        /// 
        /// </summary>
        public static HubCallerContext Context { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static IGroupManager Group { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static IDbContextScopeFactory DbContextScopeFactory = new DbContextScopeFactory();
        /// <summary>
        /// 
        /// </summary>
        public static Semaphore Semaphore = new Semaphore(3, 3);
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, GeofenceHelper.Position> LastPosition = new Dictionary<string, GeofenceHelper.Position>();

        public static IMapper Mapper { get; set; }
        //public static IMediator Mediator  = new Mediator();
    }
}
