using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Pipeline;
using Microsoft.Extensions.Logging;

namespace ESS.FW.ServiceBus.MassTransit.Observers
{
    public class PublishObserver :IPublishObserver
    {
        private ILogger _logger;

        public PublishObserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PrePublish" + context.Message);
            }
            // called right before the message is published (sent to exchange or topic)
            return Task.FromResult(0);
        }

        public Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PostPublish" + context.Message);
            }
            // called after the message is published (and acked by the broker if RabbitMQ)
            return Task.FromResult(0);
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError("PublishFault" + exception);
            }
            // called if there was an exception publishing the message
            return Task.FromResult(0);
        }
    }
}
