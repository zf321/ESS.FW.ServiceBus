using System;
using IBus = ESS.FW.Common.ServiceBus.IBus;

namespace ESS.FW.ServiceBus.MassTransit
{
    /// <summary>
    ///     Represents the message bus.
    /// </summary>
    public class MassTransitBus : IBus
    {
        private readonly global::MassTransit.IBus _bus;

        public Uri Address => _bus.Address;

        public MassTransitBus(global::MassTransit.IBus bus)
        {
            _bus = bus;
        }

        public void Publish<T>(T message) where T : class
        {
            _bus.Publish(message);
        }

        public void Publish(object message)
        {
            _bus.Publish(message);
        }
        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="destination">
        ///     The address of the destination to which the message will be sent.
        /// </param>
        /// <param name="message">The message to send.</param>
        public void Send(string destination, object message, bool isFullUri = false)
        {
            var se = _bus.GetSendEndpoint(this.GetFullUri(destination, isFullUri)).Result;
            se.Send(message);
        }
        

        public void Send(string destination, string correlationId, object message)
        {
            var se = _bus.GetSendEndpoint(this.GetFullUri(destination)).Result;
            se.Send(message);
        }

        public void Publish<T>(Action<T> messageConstructor)
        {
            _bus.Publish(messageConstructor);
        }

        public void Send(object message)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(string destination, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(string destination, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void OrderPublish<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }
    }
}