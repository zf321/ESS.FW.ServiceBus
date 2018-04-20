using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.Shared.Events.Common.Bpm
{
    /// <summary>
    /// bpm message throw event
    /// </summary>
    [Serializable]
    public class MessageThrowEvent
    {
        public string EventName { get; set; }
        public IDictionary<string, object> Variables { get; set; }

    }
}
