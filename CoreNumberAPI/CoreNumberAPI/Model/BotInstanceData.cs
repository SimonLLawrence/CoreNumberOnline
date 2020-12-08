using System;
using System.Collections.Generic;

namespace CoreNumberAPI.Model
{
    public class BotInstanceData
    {
        // State , dependancy plumbing 
        public Guid Id { get; set; }
        public string ExchangeID { get; set; }
        public string ProcessorID { get; set; }
        public string SecretID { get; set;}
        public string State { get; set; }
        // Stored data 
        public string TokenSymbol { get; set; }
        public decimal TokenSize { get; set; } = 0;
        public string CashTokenSymbol { get; set; } = "USDT";
        public decimal CashTokenValue { get; set; } = 0;
        public decimal CashValueMinimumPercentage { get; set; } = 5;
        public decimal CashValueStartingPercentage { get; set; } = 15;
        public decimal CashValueMaximumPercentage { get; set; } = 30;
        public decimal MinimTokenPriceChangePercentage { get; set; } = 1;
        public decimal MinimumDollarPurchaceSize { get; set; } = 10.1M;
        public decimal MaximumCashConsideration { get; set; } = 0;
        public decimal CoreNumber { get; set; }
        //Stored on starting 
        public decimal StartingTokenSize { get; set; } = 67;
        public decimal StartingCashAmount { get; set; } = 67;
        public DateTime StartingDate = new DateTime(2020, 9, 8, 13, 0, 0);
        // Calculated on the fly , no need to store
        public decimal TokenBidPrice { get; set; } = 0;
        public decimal TokenAskPrice { get; set; } = 0;
        public decimal BuySellAmount { get; set; } = 0;
        public decimal TokenPrice => (TokenAskPrice + TokenBidPrice) / 2;
        public decimal TokenDollarValue => TokenPrice * TokenSize;
        public decimal EstimatedPnL => (TokenDollarValue + CashTokenValue) -
                                       ((StartingTokenSize * TokenPrice) + StartingCashAmount);
        public IEnumerable<Order> OutstandingOrders { get; set; } = new List<Order>();        
    }
}
