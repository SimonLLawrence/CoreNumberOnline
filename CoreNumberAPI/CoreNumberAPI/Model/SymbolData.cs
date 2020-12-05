using System;
using System.Collections.Generic;

namespace CoreNumberAPI.Model
{
    public class SymbolData
    {
        public string TokenSymbol { get; set; }
        public decimal TokenBidPrice { get; set; } = 0;
        public decimal TokenAskPrice { get; set; } = 0;
        public decimal TokenSize { get; set; } = 0;
        public string CashTokenSymbol { get; set; } = "USDT";
        public decimal CashTokenValue { get; set; } = 0;
        public decimal CashValueMinimumPercentage { get; set; } = 5;
        public decimal CashValueStartingPercentage { get; set; } = 15;
        public decimal CashValueMaximumPercentage { get; set; } = 30;
        public decimal MinimTokenPriceChangePercentage { get; set; } =  1;
        public decimal MinimumDollarPurchaceSize { get; set; } = 10.1M;
        public decimal BuySellAmount { get; set; } = 0;
        public decimal CoreNumber { get; set; }
        public decimal TokenPrice => (TokenAskPrice + TokenBidPrice) / 2;
        public decimal TokenDollarValue => TokenPrice * TokenSize;
        public decimal StartingTokenSize { get; set; } = 67;
        public decimal StartingCashAmount { get; set; } = 67;
        public decimal EstimatedPnL => (TokenDollarValue + CashTokenValue) -
                                       ((StartingTokenSize * TokenPrice) + StartingCashAmount);

        public IEnumerable<Order> OutstandingOrders { get; set; } = new List<Order>();

        public DateTime StartingDate = new DateTime(2020,9,8,13,0,0);
    }
}
