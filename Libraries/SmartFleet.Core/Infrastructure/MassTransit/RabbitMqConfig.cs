using System;
using System.Configuration;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using MassTransit.RabbitMqTransport;
using Microsoft.ServiceBus;

namespace SmartFleet.Core.Infrastructure.MassTransit
{
    public static class RabbitMqConfig
    {
        static string url = ConfigurationManager.AppSettings["RabbitQueueFullUri"];

        public static IBusControl InitReceiverBus<T>(string endpoint) where T : class, IConsumer, new()
        {
            return Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                IRabbitMqHost host = sbc.Host(
                    new Uri(url),
                    hst =>
                    {
                        hst.Username(ConfigurationManager.AppSettings["RabbitUsername"]);
                        hst.Password(ConfigurationManager.AppSettings["RabbitPassword"]);
                    });

                sbc.ReceiveEndpoint(host, endpoint, e =>
                {
                    // Configure your consumer(s)
                    ConsumerExtensions.Consumer<T>(e);
                });
            });

        }
        public static IBusControl ConfigureSenderBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(configure =>
            {

                 configure.Host(
                    new Uri(url.Replace("amqp://", "rabbitmq://")),
                    hst =>
                    {
                        hst.Username(ConfigurationManager.AppSettings["RabbitUsername"]);
                        hst.Password(ConfigurationManager.AppSettings["RabbitPassword"]);
                    });


            });
        }
        public static IBusControl InitReceiverAzureBus<T>(string endpoint) where T : class, IConsumer, new()
        {
            var bus = Bus.Factory.CreateUsingAzureServiceBus(sbc =>
            {
                var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb",
                    ConfigurationManager.AppSettings["AzureSbNamespace"],
                    ConfigurationManager.AppSettings["AzureSbPath"]);

                var host = ServiceBusBusFactoryConfiguratorExtensions.Host(sbc, serviceUri,
                    h =>
                    {
                        h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                            ConfigurationManager.AppSettings["AzureSbKeyName"],
                            ConfigurationManager.AppSettings["AzureSbSharedAccessKey"], TimeSpan.FromDays(1),
                            TokenScope.Namespace);
                    });

                sbc.ReceiveEndpoint(host,endpoint, e =>
                {
                    // Configure your consumer(s)
                    ConsumerExtensions.Consumer<T>(e);
                    e.DefaultMessageTimeToLive = TimeSpan.FromMinutes(1);
                    e.EnableDeadLetteringOnMessageExpiration = false;
                });
            });
            return bus;
        }
        public static IBusControl CreateBus(string endpoint)
        {
            var bus = Bus.Factory.CreateUsingAzureServiceBus(sbc =>
            {
                var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb",
                    ConfigurationManager.AppSettings["AzureSbNamespace"],
                    ConfigurationManager.AppSettings["AzureSbPath"]);

                ServiceBusBusFactoryConfiguratorExtensions.Host(sbc, serviceUri,
                    h =>
                    {
                        h.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                            ConfigurationManager.AppSettings["AzureSbKeyName"],
                            ConfigurationManager.AppSettings["AzureSbSharedAccessKey"], TimeSpan.FromDays(1),
                            TokenScope.Namespace);
                    });
            });
            return bus;

        }

    }
}
