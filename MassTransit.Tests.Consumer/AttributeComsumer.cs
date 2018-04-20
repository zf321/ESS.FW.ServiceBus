using ESS.FW.Common.Attributes;
using ESS.Shared.Events.Common.Test;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MassTransit.Tests.Consumer
{
    [Message("AttributeEvent")]
    public class AttributeComsumer : IConsumer<AttributeEvent>
    {
        private ILogger _logger;

        public AttributeComsumer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public Task Consume(ConsumeContext<AttributeEvent> context)
        {
            Console.WriteLine("AttributeComsumer handle:" + context.Message.Message);


            return Task.FromResult(0);
        }
        
    }
}