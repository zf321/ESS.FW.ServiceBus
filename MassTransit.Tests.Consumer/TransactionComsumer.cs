using System;
using System.Threading.Tasks;
using System.Transactions;
using ESS.Shared.Events.Common.Test;
using MassTransit;

namespace MassTransit.Tests.Consumer
{
    //public class TransactionComsumer : IConsumer<TransactionEvent>
    //{
    //    private readonly IRepository<OutStockRegister_Info, long> _outStockRegisteRepository;
    //    private readonly IUnitOfWork _unitOfWork;
    //    private ILogger _logger;

    //    public TransactionComsumer(ILoggerFactory loggerFactory, IUnitOfWork unitOfWork,
    //        IRepository<OutStockRegister_Info, long> outStockRegisteRepository)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _outStockRegisteRepository = outStockRegisteRepository;
    //        _logger = loggerFactory.Create(GetType());
    //    }

    //    public Task Consume(ConsumeContext<TransactionEvent> context)
    //    {
    //        var transactionContext = context.GetPayload<TransactionContext>();

    //        using (var scope = new TransactionScope(transactionContext.Transaction))
    //        {
    //            var m = new OutStockRegister_Info
    //            {
    //                BranchID = "FC5",
    //                CustID = "DWI00000100",
    //                ProdID = "SPH0000001",
    //                OpID = "SYADMIN"
    //            };
    //            _outStockRegisteRepository.Add(m);

    //            _unitOfWork.SaveChanges();

    //            var count = _outStockRegisteRepository.Count(c => c.BranchID == "FC5" &&
    //                                                              c.CustID == "DWI00000100" &&
    //                                                              c.ProdID == "SPH0000001" &&
    //                                                              c.OpID == "SYADMIN");

    //            Console.WriteLine(count);
    //            scope.Complete();
    //        }
    //        ;
    //        return Task.FromResult(0);
    //    }
    //}
}