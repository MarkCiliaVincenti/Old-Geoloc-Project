using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using SmartFLEET.Web.Hubs;

[assembly: OwinStartup(typeof(SmartFLEET.Web.Startup))]
namespace SmartFLEET.Web
{

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuthentication(app);
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            app.MapSignalR();
        }

    
    }
}
