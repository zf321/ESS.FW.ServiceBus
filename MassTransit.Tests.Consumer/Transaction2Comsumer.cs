//using System;
//using System.Activities.Statements;
//using System.Threading.Tasks;
//using System.Transactions;
//using GreenPipes;
//using ESS.Common.Shared.Entities.Sale;
//using ESS.Common.Shared.Entities.Test;
//using ESS.Common.Shared.Events.Test;
//using ESS.FW.Common.Logging;
//using ESS.FW.DataAccess;
//using MassTransit;
//using TransactionScope = System.Transactions.TransactionScope;

//namespace ESS.FW.ServiceBus.MassTransit.Tests.Consumer
//{
//    public class Transaction2Comsumer : IConsumer<TransactionEvent>
//    {
//        private ILogger _logger;
//        private IUnitOfWork _unitOfWork;
//        private readonly IRepository<OutStockRegister_Info, long> _outStockRegisteRepository;

//        public Transaction2Comsumer(ILoggerFactory loggerFactory, IUnitOfWork unitOfWork, IRepository<OutStockRegister_Info, long> outStockRegisteRepository)
//        {
//            _unitOfWork = unitOfWork;
//            _outStockRegisteRepository = outStockRegisteRepository;
//            _logger = loggerFactory.Create(GetType());
//        }

//        public Task Consume(ConsumeContext<TransactionEvent> context)
//        {
//            TransactionContext transactionContext = context.GetPayload<TransactionContext>();
//            using (var scope = new TransactionScope(transactionContext.Transaction))
//            {
//                try
//                {
//                    var count = _outStockRegisteRepository.Count(c => c.BranchID == "FC5" &&
//                                                                      c.CustID == "DWI00000100" &&
//                                                                      c.ProdID == "SPH0000001" &&
//                                                                      c.OpID == "SYADMIN");

//                    Console.WriteLine(count);

//                    //throw new Exception("transaction test");

//                }
//                catch (Exception ex)
//                {
//                    throw;
//                }
//                scope.Complete();
//            }

//            return Task.FromResult(0);
//        }
//    }
//}