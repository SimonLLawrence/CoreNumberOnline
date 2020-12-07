using CoreNumberAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using CoreNumberAPI.Factory;


namespace CoreNumberAPI.Services
{
    public class BinanceService : IExchange
    {
        private BinanceClient _binanceClient;
        private SecretFactory _secretFactory;

        public string ExchangeName => "BINANCE";

        public BinanceService(SecretFactory secretFactory)
        {
            _secretFactory = secretFactory;
        }

        public void OpenClient(string  secretId)
        {
            var secret = _secretFactory.GetApiSecret(secretId);
            var apiClient = new ApiClient(secret.Key, secret.Secret);
            _binanceClient = new BinanceClient(apiClient);
            Console.WriteLine($"Creating Client {secret.Key} {secret.Secret}");
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
