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
        private BotInstanceData _botInstanceData;
        private DateTime _currentProcessingTime;

        public CoreNumberProcessor(IExchangeFactory exchangeFactory,  IBotInstanceDataRepository botInstanceRepository)
        {
            _exchangeFactory = exchangeFactory;
            _botInstanceRepository = botInstanceRepository;
        }

        public void Initialise(BotInstanceData instance)
        {
            _botInstanceData = instance;
            _exchange = _exchangeFactory.GetExchange(instance.ExchangeID);
            _exchange.OpenClient(instance.SecretID);
        }

        public void Process(BotInstanceData instance, DateTime utcNow)
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

        public void Shutdown(BotInstanceData instance)
        {
            Console.WriteLine($"Shutting down algo instance {instance.Id}");
        }

        public string BotProcessorName { get; } = "CoreNumberCompound";

  
        private BotInstanceData OutstandingOrders(BotInstanceData instance)
        {
            instance.OutstandingOrders = _exchange.GetOpenOrders(instance.TokenSymbol, instance.CashTokenSymbol);
            return instance;
        }

        private void CalculatePnL(BotInstanceData instance)
        {
            var daysRunning = (DateTime.Now - instance.StartingDate).Days;
            if (daysRunning == 0)
            {
                Console.WriteLine($"Estimated PnL is {instance.EstimatedPnL}");
            }
            else
            {
                var dailyProfit = instance.EstimatedPnL / daysRunning;
                var dailyProfitPerc = (dailyProfit / (instance.StartingCashAmount + (instance.StartingTokenSize * instance.TokenPrice)))*100;
                var currentTotal = instance.CashTokenValue + (instance.TokenSize * instance.TokenPrice);
                var initalTotal = instance.StartingCashAmount + (instance.StartingTokenSize * instance.TokenPrice);
                Console.WriteLine($"Initial value {initalTotal}");
                Console.WriteLine($"Current value {currentTotal}");
                Console.WriteLine($"Gain is ${currentTotal - initalTotal}");
                Console.WriteLine($"Estimated PnL per day is ${instance.EstimatedPnL / daysRunning} or {dailyProfitPerc}%, running for {daysRunning} days");
            }
        }

        private BotInstanceData OrderCorrectionAmount(BotInstanceData instance)
        {
            if (instance.BuySellAmount != 0)
            {
                CancelExistingOrder(instance);
                var side = instance.BuySellAmount > 0 ? "BUY" : "SELL";

                if (side == "BUY" && instance.BuySellAmount > instance.CashTokenValue)
                {
                    return instance;
                }

                var symbol = $"{instance.TokenSymbol}{instance.CashTokenSymbol}";
                var size = decimal.Round(Math.Abs(instance.BuySellAmount) / instance.TokenAskPrice, 2);
                var price = decimal.Round(instance.TokenAskPrice, 4);

                if ((size * price) < instance.MinimumDollarPurchaceSize)
                {
                    size = decimal.Round(instance.MinimumDollarPurchaceSize / price,2);
                }

                Console.WriteLine($"Placing order {symbol} {side} for {size} at {price} = {price * size} dollars");
                var order = new Order
                {
                    Symbol = instance.TokenSymbol,
                    DenominatorSybol = instance.CashTokenSymbol,
                    Size = size,
                    Side = side,
                    Price = price
                };
                _exchange.CreateOrder(order);
            }
            return instance;
        }

        private BotInstanceData CancelExistingOrder(BotInstanceData instance)
        {
         
            foreach (var openOrder in instance.OutstandingOrders)
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
            return instance;
        }

        public BotInstanceData SetCoreNumber(BotInstanceData instance)
        {
            if (!instance.OutstandingOrders.Any() || instance.CoreNumber == 0 )
            {
                Console.WriteLine($"Checking core number {instance.CoreNumber}");

                decimal cashAsPercentageOfTokenCashValue = (instance.CashTokenValue / instance.TokenDollarValue) * 100;

                Console.WriteLine($"Token value {instance.TokenDollarValue}, cash on hand {instance.CashTokenValue} as percentage {cashAsPercentageOfTokenCashValue}");

                if (instance.CashTokenValue == 0)
                {
                    instance.CoreNumber = instance.TokenDollarValue - ((instance.TokenDollarValue / 100) * instance.CashValueStartingPercentage);
                    Console.WriteLine($"Zero cash available setting core number to {instance.CoreNumber}");
                }
                else if (cashAsPercentageOfTokenCashValue <= instance.CashValueMinimumPercentage)
                {
                    Console.WriteLine($"Percentage cash available {cashAsPercentageOfTokenCashValue} < minimum percentage {instance.CashValueMinimumPercentage}");
                    instance.CoreNumber = (instance.TokenDollarValue + instance.CashTokenValue) - (((instance.TokenDollarValue + instance.CashTokenValue) / 100) * instance.CashValueStartingPercentage);
                    Console.WriteLine($"Setting core number to {instance.CoreNumber}");
                }
                else if (cashAsPercentageOfTokenCashValue >= instance.CashValueMaximumPercentage)
                {
                    Console.WriteLine($"Percentage cash available {cashAsPercentageOfTokenCashValue} > minimum percentage {instance.CashValueMinimumPercentage}");
                    instance.CoreNumber = (instance.TokenDollarValue + instance.CashTokenValue) - (((instance.TokenDollarValue + instance.CashTokenValue) / 100) * instance.CashValueStartingPercentage);
                    Console.WriteLine($"Setting core number to {instance.CoreNumber}");
                }

                if (instance.CoreNumber == 0)
                {
                    Console.WriteLine("Core number not currently available");
                    instance.CoreNumber = (instance.TokenDollarValue + instance.CashTokenValue) - (((instance.TokenDollarValue + instance.CashTokenValue) / 100) * instance.CashValueStartingPercentage);
                    Console.WriteLine($"Setting core number to {instance.CoreNumber}");
                }
            }
            return instance;
        }

        private BotInstanceData GetDollarPurchaseAmount(BotInstanceData instance)
        {
            Console.WriteLine("Getting dollar purchase amount");
            instance.BuySellAmount = 0;
            var correctionPercentage = ((instance.CoreNumber - instance.TokenDollarValue ) / instance.CoreNumber) * 100;
            Console.WriteLine($"Correction percentage is {Math.Abs(correctionPercentage)}  minimum percentage is {instance.MinimTokenPriceChangePercentage}");
            if (Math.Abs(correctionPercentage) > instance.MinimTokenPriceChangePercentage)
            {
                if (correctionPercentage != 0)
                {
                    instance.BuySellAmount = (instance.CoreNumber / 100) * correctionPercentage;
                }
                Console.WriteLine($"Setting BuySellAmount to {instance.BuySellAmount}");
            }
            return instance;
        }

        private BotInstanceData GetCashAndTokenBalance(BotInstanceData instance)
        {
            instance.CashTokenValue = _exchange.GetBalance(instance.CashTokenSymbol);
            instance.TokenSize = _exchange.GetBalance(instance.TokenSymbol);
            Console.WriteLine($"Total cash available {instance.CashTokenValue} , number of {instance.TokenSymbol} tokens {instance.TokenSize}");
            return instance;
        }

        private BotInstanceData GetCurrentPrice(BotInstanceData instance)
        {
            var price = _exchange.GetPrice(instance.TokenSymbol, instance.CashTokenSymbol);
            instance.TokenAskPrice = price.AskPrice;
            instance.TokenBidPrice = price.Price;
            Console.WriteLine($"Order book for {instance.TokenSymbol} has ask of {instance.TokenAskPrice} and bid of {instance.TokenBidPrice}");
            return instance;
        }
    }
}
