using System;
using System.Configuration;
using Autofac;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using Microsoft.ServiceBus;
using SmartFLEET.Web.Hubs;

namespace SmartFLEET.Web.Helpers
{
    public class AzureServiceBusModule :Module
    {
        private readonly System.Reflection.Assembly[] _assembliesToScan;

        public AzureServiceBusModule(params System.Reflection.Assembly[] assembliesToScan)
        {
            _assembliesToScan = assembliesToScan;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Creates our bus from the factory and registers it as a singleton against two interfaces
            builder.Register(c => Bus.Factory.CreateUsingAzureServiceBus(sbc =>
                {
                    var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", ConfigurationManager.AppSettings["AzureSbNamespace"], ConfigurationManager.AppSettings["AzureSbPath"]);

                    var host = ServiceBusBusFactoryConfiguratorExtensions.Host(sbc, serviceUri, h =>
                    {
                        h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(ConfigurationManager.AppSettings["AzureSbKeyName"], ConfigurationManager.AppSettings["AzureSbSharedAccessKey"], TimeSpan.FromDays(1), TokenScope.Namespace);
                    });

                    sbc.ReceiveEndpoint(host, ConfigurationManager.AppSettings["ServiceQueueName"], e =>
                    {
                        // Configure your consumer(s)
                        ConsumerExtensions.Consumer<SignalRHandler>(e);
                        e.DefaultMessageTimeToLive = TimeSpan.FromMinutes(1);
                        e.EnableDeadLetteringOnMessageExpiration = false;
                    });
                }))
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();
        }
    }
}