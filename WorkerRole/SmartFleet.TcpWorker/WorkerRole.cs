using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using Serilog;
using Serilog.Core;

namespace SmartFleet.TcpWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private static Logger _log;

        public WorkerRole()
        {
            InitLog();
        }
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MethodNameNotMeaningful
        public override void Run()
        {
            
            // init teltonika server 
            DependencyRegistrar.ResolveDependencies();
            var listener = DependencyRegistrar.StartListener();
            listener.Start();
            
        }

        private static void InitLog()
        {
            _log = new LoggerConfiguration()
                // .WriteTo.Console()
                .WriteTo.File("tcp-worker-role.txt")
                .CreateLogger();
        }
        public override bool OnStart()
        {
            // Définir le nombre maximum de connexions simultanées
            ServicePointManager.DefaultConnectionLimit = 12;

            // Pour plus d'informations sur la gestion des modifications de configuration
            // consultez la rubrique MSDN à l'adresse https://go.microsoft.com/fwlink/?LinkId=166357.
            bool result = base.OnStart();
           
           _log.Information("SmartFleet.TcpWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            _log.Information("SmartFleet.TcpWorker is stopping");
            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();
            base.OnStop();
            _log.Information("SmartFleet.TcpWorker  stopped");
        }

      
    }
}
