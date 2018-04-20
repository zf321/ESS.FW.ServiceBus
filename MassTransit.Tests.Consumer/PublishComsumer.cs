using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
//using Automatonymous;
using ESS.Shared.Events.Common.Test;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransit.Tests.Consumer
{
    public class PublishComsumer : IConsumer<HpImplementation>, IConsumer<Request>/*, IConsumer<PurchaseContractToOrder>*/
    {
        private ILogger _logger;

        public PublishComsumer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public Task Consume(ConsumeContext<HpImplementation> context)
        {
            var s = new Random().Next(1000);
            Thread.Sleep(s);
            Console.WriteLine("handle:" + context.Message.SystemVersion + "- " +s);


            return Task.FromResult(0);
        }

        //public Task Consume(ConsumeContext<PurchaseContractToOrder> context)
        //{
        //    var a = context.Message;

        //    return Task.FromResult(0);
        //}

        public Task Consume(ConsumeContext<Request> context)
        {
            Console.WriteLine("handle Request:" + context.Message.Message + DateTime.Now);
            context.Respond(new RequestResult
            {
                Message = "reply" + DateTime.Now
            });

            return Task.FromResult(0);
        }
    }


    //public class ShoppingCartManager :
    //    MassTransitStateMachine<ShoppingCart>
    //{
    //    public ShoppingCartManager()
    //    {
    //        InstanceState(x => x.CurrentState);

    //        Event(() => ItemAdded, x => x.CorrelateBy(cart => cart.UserName, context => context.Message.UserName)
    //            .SelectId(context => Guid.NewGuid()));

    //        Event(() => Submitted, x => x.CorrelateById(context => context.Message.CartId));

    //        Schedule(() => CartExpired, x => x.ExpirationId, x =>
    //        {
    //            x.Delay = TimeSpan.FromSeconds(10);
    //            x.Received = e => e.CorrelateById(context => context.Message.CartId);
    //        });

    //        Initially(
    //            When(ItemAdded)
    //                .Then(context =>
    //                {
    //                    context.Instance.Created = context.Data.Timestamp;
    //                    context.Instance.Updated = context.Data.Timestamp;
    //                    context.Instance.UserName = context.Data.UserName;
    //                })
    //                .ThenAsync(context => Console.Out.WriteLineAsync($"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
    //                .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance))
    //                .TransitionTo(Active)
    //            );

    //        During(Active,
    //            When(Submitted)
    //                .Then(context =>
    //                {
    //                    if (context.Data.Timestamp > context.Instance.Updated)
    //                        context.Instance.Updated = context.Data.Timestamp;

    //                    context.Instance.OrderId = context.Data.OrderId;
    //                })
    //                .ThenAsync(context => Console.Out.WriteLineAsync($"Cart Submitted: {context.Data.UserName} to {context.Instance.CorrelationId}"))
    //                .Unschedule(CartExpired)
    //                .TransitionTo(Ordered),
    //            When(ItemAdded)
    //                .Then(context =>
    //                {
    //                    if (context.Data.Timestamp > context.Instance.Updated)
    //                        context.Instance.Updated = context.Data.Timestamp;
    //                })
    //                .ThenAsync(context => Console.Out.WriteLineAsync($"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
    //                .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance)),
    //            When(CartExpired.Received)
    //                .ThenAsync(context => Console.Out.WriteLineAsync($"Item Expired: {context.Instance.CorrelationId}"))
    //                .Publish(context => new CartRemovedEvent(context.Instance))
    //                .Finalize()
    //            );

    //        SetCompletedWhenFinalized();
    //    }


    //    public State Active { get; private set; }
    //    public State Ordered { get; private set; }

    //    public Schedule<ShoppingCart, CartExpired> CartExpired { get; private set; }

    //    public Event<CartItemAdded> ItemAdded { get; private set; }
    //    public Event<OrderSubmitted> Submitted { get; private set; }


    //    class CartExpiredEvent : CartExpired
    //    {
    //        readonly ShoppingCart _instance;

    //        public CartExpiredEvent(ShoppingCart instance)
    //        {
    //            _instance = instance;
    //        }

    //        public Guid CartId => _instance.CorrelationId;
    //    }


    //    class CartRemovedEvent : CartRemoved
    //    {
    //        readonly ShoppingCart _instance;

    //        public CartRemovedEvent(ShoppingCart instance)
    //        {
    //            _instance = instance;
    //        }

    //        public Guid CartId => _instance.CorrelationId;
    //        public string UserName => _instance.UserName;
    //    }
    //}

    //public class ShoppingCart :
    //    SagaStateMachineInstance
    //{
    //    public string CurrentState { get; set; }

    //    public string UserName { get; set; }

    //    public DateTime Created { get; set; }
    //    public DateTime Updated { get; set; }

    //    / <summary>
    //    / The expiration tag for the shopping cart, which is scheduled whenever
    //    / the cart is updated
    //    / </summary>
    //    public Guid? ExpirationId { get; set; }

    //    public Guid? OrderId { get; set; }

    //    public Guid CorrelationId { get; set; }
    //}
    //public interface CartExpired
    //{
    //    Guid CartId { get; }
    //}

    //public interface CartRemoved
    //{
    //    Guid CartId { get; }
    //    string UserName { get; }
    //}
    //public interface OrderSubmitted
    //{
    //    Guid OrderId { get; }

    //    DateTime Timestamp { get; }

    //    Guid CartId { get; }

    //    string UserName { get; }
    //}
    //public interface CartItemAdded
    //{
    //    DateTime Timestamp { get; }

    //    string UserName { get; }
    //}
}