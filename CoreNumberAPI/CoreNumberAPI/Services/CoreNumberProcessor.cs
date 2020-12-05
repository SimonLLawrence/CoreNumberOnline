using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace CoreNumberBot
{
    public class CoreNumberProcessor
    {
        private readonly List<SymbolData> _symbolData = new List<SymbolData>
        {
            new SymbolData
            {
                TokenSymbol = "DOT",
                CashTokenSymbol = "USDT",
                CoreNumber = 0,
                MinimTokenPriceChangePercentage = 3.0M
            }
        };

        private BinanceClient _binanceClient;

        public CoreNumberProcessor()
        {
            OpenClient();
        }

        public void Process()
        {
            foreach (var data in LoadSymbolData())
            {
                OutstandingOrders(data);
                GetCashAndTokenBalance(data);
                GetCurrentPrice(data);
                SetCoreNumber(data);
                GetDollarPurchaseAmount(data);
                OrderCorrectionAmount(data);
                SaveData(data);
                CalculatePnL(data);
            }
        }

        private SymbolData OutstandingOrders(SymbolData data)
        {
            data.OutstandingOrders = _binanceClient.GetCurrentOpenOrders($"{data.TokenSymbol}{data.CashTokenSymbol}").Result;
            return data;
        }

        private void CalculatePnL(SymbolData data)
        {
            var daysRunning = (DateTime.Now - data.StartingDate).Days;
            if (daysRunning == 0)
            {
                Console.WriteLine($"Estimated PnL is {data.EstimatedPnL}");
            }
            else
            {
                var dailyProfit = data.EstimatedPnL / daysRunning;
                var dailyProfitPerc = (dailyProfit / (data.StartingCashAmount + (data.StartingTokenSize * data.TokenPrice)))*100;
                var currentTotal = data.CashTokenValue + (data.TokenSize * data.TokenPrice);
                var initalTotal = data.StartingCashAmount + (data.StartingTokenSize * data.TokenPrice);
                Console.WriteLine($"Initial value {initalTotal}");
                Console.WriteLine($"Current value {currentTotal}");
                Console.WriteLine($"Gain is ${currentTotal - initalTotal}");
                Console.WriteLine($"Estimated PnL per day is ${data.EstimatedPnL / daysRunning} or {dailyProfitPerc}%, running for {daysRunning} days");
            }
        }

        private void OpenClient()
        {
            var key = "ZnlSBphY";
            var secret = "AZikQ";
            var apiClient = new ApiClient(key,secret);
            _binanceClient = new BinanceClient(apiClient);

            Console.WriteLine($"Creating Client {key} {secret}");
        }

        private IEnumerable<SymbolData> LoadSymbolData()
        {
            Console.WriteLine("Loading symbols");
            return _symbolData;
        }

        private void SaveData(SymbolData data)
        {
            Console.WriteLine("Saving data");
        }

        private SymbolData OrderCorrectionAmount(SymbolData data)
        {
            if (data.BuySellAmount != 0)
            {
                CancelExistingOrder(data);
                var side = data.BuySellAmount > 0 ? OrderSide.BUY : OrderSide.SELL;

                if (side == OrderSide.BUY && data.BuySellAmount > data.CashTokenValue)
                {
                    return data;
                }

                var symbol = $"{data.TokenSymbol}{data.CashTokenSymbol}";
                var size = decimal.Round(Math.Abs(data.BuySellAmount) / data.TokenAskPrice, 2);
                var price = decimal.Round(data.TokenAskPrice, 4);

                if ((size * price) < data.MinimumDollarPurchaceSize)
                {
                    size = decimal.Round(data.MinimumDollarPurchaceSize / price,2);
                }

                Console.WriteLine($"Placing order {symbol} {side} for {size} at {price} = {price * size} dollars");
                var buyOrder = _binanceClient.PostNewOrder(symbol, size, price, side).Result;
            }
            return data;
        }

        private SymbolData CancelExistingOrder(SymbolData data)
        {
         
            foreach (var openOrder in data.OutstandingOrders)
            {
                try
                {
                    Console.WriteLine($"cancelling existing  order with id {openOrder.OrderId}");
                    var canceledOrder = _binanceClient
                        .CancelOrder($"{data.TokenSymbol}{data.CashTokenSymbol}", openOrder.OrderId).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception canceling order  {openOrder.OrderId}" + ex.ToString());
                }
            }
            return data;
        }

        public SymbolData SetCoreNumber(SymbolData data)
        {
            if (!data.OutstandingOrders.Any() || data.CoreNumber == 0 )
            {
                Console.WriteLine($"Checking core number {data.CoreNumber}");

                decimal cashAsPercentageOfTokenCashValue = (data.CashTokenValue / data.TokenDollarValue) * 100;

                Console.WriteLine($"Token value {data.TokenDollarValue}, cash on hand {data.CashTokenValue} as percentage {cashAsPercentageOfTokenCashValue}");

                if (data.CashTokenValue == 0)
                {
                    data.CoreNumber = data.TokenDollarValue - ((data.TokenDollarValue / 100) * data.CashValueStartingPercentage);
                    Console.WriteLine("Zero cash available setting core number to {data.CoreNumber}");
                }
                else if (cashAsPercentageOfTokenCashValue <= data.CashValueMinimumPercentage)
                {
                    Console.WriteLine($"Percentage cash available {cashAsPercentageOfTokenCashValue} < minimum percentage {data.CashValueMinimumPercentage}");
                    data.CoreNumber = (data.TokenDollarValue + data.CashTokenValue) - (((data.TokenDollarValue + data.CashTokenValue) / 100) * data.CashValueStartingPercentage);
                    Console.WriteLine($"Setting core number to {data.CoreNumber}");
                }
                else if (cashAsPercentageOfTokenCashValue >= data.CashValueMaximumPercentage)
                {
                    Console.WriteLine($"Percentage cash available {cashAsPercentageOfTokenCashValue} > minimum percentage {data.CashValueMinimumPercentage}");
                    data.CoreNumber = (data.TokenDollarValue + data.CashTokenValue) - (((data.TokenDollarValue + data.CashTokenValue) / 100) * data.CashValueStartingPercentage);
                    Console.WriteLine($"Setting core number to {data.CoreNumber}");
                }

                if (data.CoreNumber == 0)
                {
                    Console.WriteLine("Core number not currently available");
                    data.CoreNumber = (data.TokenDollarValue + data.CashTokenValue) - (((data.TokenDollarValue + data.CashTokenValue) / 100) * data.CashValueStartingPercentage);
                    Console.WriteLine($"Setting core number to {data.CoreNumber}");
                }
            }
            return data;
        }

        private SymbolData GetDollarPurchaseAmount(SymbolData data)
        {
            Console.WriteLine("Getting dollar purchase amount");
            data.BuySellAmount = 0;
            var correctionPercentage = ((data.CoreNumber - data.TokenDollarValue ) /data.CoreNumber) * 100;
            Console.WriteLine($"Correction percentage is {Math.Abs(correctionPercentage)}  minimum percentage is {data.MinimTokenPriceChangePercentage}");
            if (Math.Abs(correctionPercentage) > data.MinimTokenPriceChangePercentage)
            {
                if (correctionPercentage != 0)
                {
                    data.BuySellAmount = (data.CoreNumber / 100) * correctionPercentage;
                }
                Console.WriteLine($"Setting BuySellAmount to {data.BuySellAmount}");
            }
            return data;
        }

        private SymbolData GetCashAndTokenBalance(SymbolData data)
        {
            var accountInfo = _binanceClient.GetAccountInfo().Result;
            data.CashTokenValue = accountInfo.Balances.Single(x => x.Asset == $"{data.CashTokenSymbol}").Free;
            data.TokenSize = accountInfo.Balances.Single(x => x.Asset == $"{data.TokenSymbol}").Free;
            Console.WriteLine($"Total cash available {data.CashTokenValue} , number of {data.TokenSymbol} tokens {data.TokenSize}");
            return data;
        }

        private SymbolData GetCurrentPrice(SymbolData data)
        {
            var orderBook =_binanceClient.GetOrderBook($"{data.TokenSymbol}{data.CashTokenSymbol}").Result;
            data.TokenAskPrice = orderBook.Asks.First().Price;
            data.TokenBidPrice = orderBook.Bids.First().Price;
            Console.WriteLine($"Order book for {data.TokenSymbol} has ask of {data.TokenAskPrice} and bid of {data.TokenBidPrice}");
            return data;
        }
    }
}
