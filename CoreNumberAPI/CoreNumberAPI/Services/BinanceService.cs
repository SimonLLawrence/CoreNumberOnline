using CoreNumberAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;


namespace CoreNumberAPI.Services
{
    public class BinanceService : IExchange
    {
        private BinanceClient _binanceClient;
        private ApiSecrets _apiSecrets;

        public string ExchangeName => "BINANCE";

        public BinanceService(ApiSecrets secretes)
        {
            _apiSecrets = secretes;
        }

        public void OpenClient(ApiSecrets secrets)
        {
            var apiClient = new ApiClient(_apiSecrets.Key, _apiSecrets.Secret);
            _binanceClient = new BinanceClient(apiClient);
        }

        public decimal GetBalance(string symbol)
        {
            var accountInfo = _binanceClient.GetAccountInfo().Result;
            return  accountInfo.Balances.Single(x => x.Asset == symbol).Free;
        }

        public PriceResult GetPrice(string symbol, string denominatorSymbol, DateTime? pointInTime = null)
        {
            var orderBook = _binanceClient.GetOrderBook($"{symbol}{denominatorSymbol}").Result;
            return new PriceResult {AskPrice = orderBook.Asks.First().Price , BidPrice = orderBook.Bids.First().Price };
        }

        public List<Order> GetOpenOrders(string symbol, string denominatorSymbol)
        {
            return _binanceClient.GetCurrentOpenOrders($"{symbol}{denominatorSymbol}").Result.Select(s => new Order {
                Symbol = symbol,
                DenominatorSybol = denominatorSymbol,
                Price = s.Price,
                Size = s.OrigQty,
                Reference = "" + s.OrderId,
                Side = s.Side
            }).ToList();
        }

        public Order CreateOrder(Order order)
        {
            var orderSide = order.Side == "SELL" ? OrderSide.SELL : OrderSide.BUY;
            var resultOrder = _binanceClient.PostNewOrder(order.CombindSymbol, order.Size, order.Price, orderSide).Result;
            order.Reference = ""+resultOrder.OrderId;
            return order;
        }

        public Order CancelOrder(Order orderToCancel)
        {
            var canceledOrder = _binanceClient.CancelOrder(orderToCancel.CombindSymbol, int.Parse(orderToCancel.Reference)).Result;
            return orderToCancel;
        }
    }
}
