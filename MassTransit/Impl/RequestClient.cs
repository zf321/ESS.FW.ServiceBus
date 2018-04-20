using ESS.FW.Common.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESS.FW.ServiceBus.MassTransit.Impl
{
    /// <summary>
    ///     the message request client
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class RequestClient<TRequest, TResponse> : Common.ServiceBus.IRequestClient<TRequest, TResponse> where TRequest : class
        where TResponse : class
    {
        private readonly IBus _bus;
        private readonly global::MassTransit.IBus _massTransitBus;

        public RequestClient(IBus bus,global::MassTransit.IBus massTransitBus)
        {
            _bus = bus;
            _massTransitBus = massTransitBus;
        }

        /// <summary>
        ///     request to a service
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResponse> Request(TRequest request, CancellationToken cancellationToken)
        {
            return Request(Utils.GetFullUri(_bus,typeof (TRequest).Name), request, cancellationToken);
        }

        /// <summary>
        ///     request to a service
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResponse> Request(string destination, TRequest request, CancellationToken cancellationToken)
        {
            return Request(Utils.GetFullUri(_bus,destination), request, cancellationToken);
        }

        public Task<TResponse> Request(string serviceEndPoint, string branchId, TRequest request, CancellationToken cancellationToken)
        {
            string destination = $"{serviceEndPoint}-{branchId}";
            return Request(Utils.GetFullUri(_bus,destination, true), request, cancellationToken);
        }

        /// <summary>
        ///     request to a service
        /// </summary>
        /// <param name="address"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResponse> Request(Uri address, TRequest request, CancellationToken cancellationToken)
        {
            var requestTimeout = TimeSpan.FromSeconds(300);

            var client = new global::MassTransit.MessageRequestClient<TRequest, TResponse>(_massTransitBus, address, requestTimeout);

            return client.Request(request, cancellationToken);
        }
    }
}