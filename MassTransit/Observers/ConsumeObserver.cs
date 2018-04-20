using System;
using System.Threading.Tasks;
using ESS.FW.Common.Components;
using MassTransit;
using MassTransit.Pipeline;
using Microsoft.Extensions.Logging;

namespace ESS.FW.ServiceBus.MassTransit.Observers
{
    /// <summary>
    /// A consume observer
    /// </summary>
    [Component]
    public class ConsumeObserver :IConsumeObserver
    {
        private ILogger _logger;

        public ConsumeObserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            // called before the consumer's Consume method is called
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PreConsume" + context.Message);
            }
            return Task.FromResult(0);
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            // called after the consumer's Consume method is called
            // if an exception was thrown, the ConsumeFault method is called instead
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("PostConsume" + context.Message);
            }
            return Task.FromResult(0);
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            // called if the consumer's Consume method throws an exception
            _logger.LogError(exception,exception.Message);
            return Task.FromResult(0);
        }
    }
}
