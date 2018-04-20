using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ESS.FW.ServiceBus.MassTransit.Observers
{
    /// <summary>
    /// observe received messages immediately after they are delivered by the transport,
    /// </summary>
    public class ReceiveObserver : IReceiveObserver
    {
        private ILogger _logger;

        public ReceiveObserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public Task PreReceive(ReceiveContext context)
        {
            // called immediately after the message was delivery by the transportif (_logger.IsAuditOn)
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var body = context.GetBody();
                {
                    string text = System.Text.Encoding.UTF8.GetString(body);
                    _logger.LogDebug("PreReceive" + text);
                }
            }
            return Task.FromResult(0);
        }

        public Task PostReceive(ReceiveContext context)
        {
            // called after the message has been received and processed
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                 var body = context.GetBody();
                {
                    string text = System.Text.Encoding.UTF8.GetString(body);
                    _logger.LogDebug("PostReceive" + text);
                }
            }
            return Task.FromResult(0);
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            // called when the message was consumed, once for each consumer
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PostConsume" + context.Message);
            }
            return Task.FromResult(0);
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType,
            Exception exception) where T : class
        {
            // called when the message is consumed but the consumer throws an exception
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(exception,exception.Message);
            }
            return Task.FromResult(0);
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            // called when an exception occurs early in the message processing, such as deserialization, etc.
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(exception,exception.Message);
            }
            return Task.FromResult(0);
        }
    }
}