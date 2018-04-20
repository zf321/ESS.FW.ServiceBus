using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Common.Attributes
{
    /// <summary>
    /// 用于自定义消息总线配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    public class MessageAttribute : System.Attribute
    {
        //public String Host { get; set; }
        //public String Port { get; set; }
        public String QueueName { get; set; }

        /// <summary>
        /// </summary>
        public String RouteKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFullUrl{get;set;} =true;

        /// <summary>
        /// 
        /// </summary>
        public ushort Prefetch{get;set;} = 128;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>

        public MessageAttribute(string queueName)
        {
            this.QueueName = queueName;
        }
        public MessageAttribute(string queueName,bool isFullUrl,ushort prefetch=128)
        {
            this.QueueName = queueName;
            this.IsFullUrl = isFullUrl;
            this.Prefetch = prefetch;
        }

        public MessageAttribute(string queueName,string routeKey)
        {
            this.QueueName = queueName;
            this.RouteKey = routeKey;
        }

    }
}
