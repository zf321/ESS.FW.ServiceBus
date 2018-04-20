using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ESS.FW.ServiceBus.MassTransit.Observers
{
    public class SendObserver :
    ISendObserver
    {
        private ILogger _logger;

        public SendObserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }
        public Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            // called just before a message is sent, all the headers should be setup and everything
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PreSend" + context.Message);
            }
            return Task.FromResult(0);
        }

        public Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            // called just after a message it sent to the transport and acknowledged (RabbitMQ)
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PostSend" + context.Message);
            }
            return Task.FromResult(0);
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            // called if an exception occurred sending the message
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError("SendFault" + exception);
            }
            return Task.FromResult(0);
        }
    }
}