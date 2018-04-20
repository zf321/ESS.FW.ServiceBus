using ESS.FW.Common.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.ServiceBus.MassTransit
{
    public static class Utils
    {
        public static Uri GetFullUri(this IBus bus, string destination, bool isFullUri = false)
        {
            var host = string.Format("rabbitmq://{0}/",
                            bus.Address.Host);
            return
                new Uri(host + destination + (isFullUri ? "" : ("-" +
                        Environment.MachineName)));
        }
    }
}
