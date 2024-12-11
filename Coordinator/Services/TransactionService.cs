using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services
{
    //c#12 ile gelen positional inject yöntemi ilede veri tabanını enjekte edebiliriz.
    // İkinci olarak IHttpClientFactory ile diğer apilardan talep ededbilirsiniz.
    
    public class TransactionService(IHttpClientFactory _httpClientFactory, TwoPhaseCommitContext _context) : ITransactionService
    {

        HttpClient _orderHttpClient = _httpClientFactory.CreateClient("Order.API");
        HttpClient _stockHttpClient = _httpClientFactory.CreateClient("Stock.API");
        HttpClient _paymentHttpClient =_httpClientFactory.CreateClient("Payment.API");
        
        
        public Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Enums.ReadyType.Pending,
                    TransactionState = Enums.TransactionState.Pending
                }
            });
            await _context.SaveChangesAsync();
            return transactionId;

        }
        //Elimizdeki trancaction idlere göre tüm nodelara istekte bulunacağız. Hazır olup olmadıklarına dair sonuç bekleyeceğiz.
        public async Task PrepareServicesAsync(Guid transactionId)
        {
            //tranaction idsine ait olan bütün nodestate list şeklinde değişkene atanır.
            var transactionNodes = await _context.NodeStates
                 .Include(ns => ns.Node)
                 .Where(ns => ns.TransactionId == transactionId)
                 .ToListAsync();
            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready")
                    });
                }
                catch (Exception)
                {

                    throw;
                }
            }



        }

        public Task RollbackAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
