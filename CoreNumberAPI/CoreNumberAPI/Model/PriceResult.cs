using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Model
{
    public class PriceResult
    {
        public decimal Price => (BidPrice + AskPrice) / 2;
        public decimal BidPrice { get; set; }
        public decimal AskPrice { get; set; }
    }
}
