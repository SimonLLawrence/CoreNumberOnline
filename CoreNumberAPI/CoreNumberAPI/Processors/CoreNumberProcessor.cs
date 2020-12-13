using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using CoreNumberAPI.Model;
using CoreNumberAPI.Processors;
using CoreNumberAPI.Repository;
using CoreNumberAPI.Services;
using CoreNumberAPI.Factory;

namespace CoreNumberBot
{
    public class CoreNumberProcessor : IBotProcessor
    {
        private IExchangeFactory _exchangeFactory;
        private IBotInstanceDataRepository _botInstanceRepository;
        private IExchange _exchange = null;
        private IBotInstanceData _botInstanceData;
        private DateTime _currentProcessingTime;

        public CoreNumberProcessor(IExchangeFactory exchangeFactory,  IBotInstanceDataRepository botInstanceRepository)
        {
            _exchangeFactory = exchangeFactory;
            _botInstanceRepository = botInstanceRepository;
        }

        public void Initialise(IBotInstanceData instance)
        {
            _botInstanceData = instance;
            _exchange = _exchangeFactory.GetExchange(instance.ExchangeID);
            _exchange.OpenClient(instance.SecretID);
        }

        public void Process(IBotInstanceData instance, DateTime utcNow)
        {
            _currentProcessingTime = utcNow;
            OutstandingOrders(instance);
            GetCashAndTokenBalance(instance);
            GetCurrentPrice(instance);
            SetCoreNumber(instance);
            GetDollarPurchaseAmount(instance);
            OrderCorrectionAmount(instance);
            CalculatePnL(instance);
        }

        public void Shutdown(IBotInstanceData instance)
        {
            Console.WriteLine($"Shutting down algo instance {instance.Id}");
        }

        public string BotProcessorName { get; } = "CoreNumberCompound";

  
        private BotInstanceData OutstandingOrders(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;
            data.OutstandingOrders = _exchange.GetOpenOrders(data.TokenSymbol, data.CashTokenSymbol);
            return data;
        }

        private void CalculatePnL(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;
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

        private BotInstanceData OrderCorrectionAmount(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;
            if (data.BuySellAmount != 0)
            {
                CancelExistingOrder(instance);
                var side = data.BuySellAmount > 0 ? "BUY" : "SELL";

                if (side == "BUY" && data.BuySellAmount > data.CashTokenValue)
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
                var order = new Order
                {
                    Symbol = data.TokenSymbol,
                    DenominatorSybol = data.CashTokenSymbol,
                    Size = size,
                    Side = side,
                    Price = price
                };
                _exchange.CreateOrder(order);
            }
            return data;
        }

        private BotInstanceData CancelExistingOrder(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;
            foreach (var openOrder in data.OutstandingOrders)
            {
                try
                {
                    Console.WriteLine($"cancelling existing  order with id {openOrder.Reference}");
                    var canceledOrder = _exchange.CancelOrder(openOrder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception canceling order  {openOrder.Reference}" + ex.ToString());
                }
            }
            return data;
        }

        public BotInstanceData SetCoreNumber(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;

            if (!data.OutstandingOrders.Any() || data.CoreNumber == 0 )
            {
                Console.WriteLine($"Checking core number {data.CoreNumber}");

                decimal cashAsPercentageOfTokenCashValue = (data.CashTokenValue / data.TokenDollarValue) * 100;

                Console.WriteLine($"Token value {data.TokenDollarValue}, cash on hand {data.CashTokenValue} as percentage {cashAsPercentageOfTokenCashValue}");

                if (data.CashTokenValue == 0)
                {
                    data.CoreNumber = data.TokenDollarValue - ((data.TokenDollarValue / 100) * data.CashValueStartingPercentage);
                    Console.WriteLine($"Zero cash available setting core number to {data.CoreNumber}");
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

        private BotInstanceData GetDollarPurchaseAmount(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;

            Console.WriteLine("Getting dollar purchase amount");
            data.BuySellAmount = 0;
            var correctionPercentage = ((data.CoreNumber - data.TokenDollarValue ) / data.CoreNumber) * 100;
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

        private BotInstanceData GetCashAndTokenBalance(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;
            data.CashTokenValue = _exchange.GetBalance(data.CashTokenSymbol);
            data.TokenSize = _exchange.GetBalance(data.TokenSymbol);
            Console.WriteLine($"Total cash available {data.CashTokenValue} , number of {data.TokenSymbol} tokens {data.TokenSize}");
            return data;
        }

        private BotInstanceData GetCurrentPrice(IBotInstanceData instance)
        {
            BotInstanceData data = (BotInstanceData)instance;
            var price = _exchange.GetPrice(data.TokenSymbol, data.CashTokenSymbol);
            data.TokenAskPrice = price.AskPrice;
            data.TokenBidPrice = price.Price;
            Console.WriteLine($"Order book for {data.TokenSymbol} has ask of {data.TokenAskPrice} and bid of {data.TokenBidPrice}");
            return data;
        }
    }
}
