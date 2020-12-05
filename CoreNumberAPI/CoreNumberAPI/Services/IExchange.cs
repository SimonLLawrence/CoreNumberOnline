using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Services
{
    public interface IExchange
    {
        public string ExchangeName { get; }
        public void OpenClient(ApiSecrets secrets);
        public decimal GetBalance(string symbol);
        public PriceResult GetPrice(string symbol, string denominatorSymbol, DateTime? pointInTime = null);
        public List<Order> GetOpenOrders(string symbol, string denominatorSymbol);
        public Order CreateOrder(Order orderToPost);
        public Order CancelOrder(Order orderToCancel);   
    }
}
