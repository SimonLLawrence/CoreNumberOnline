using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Model
{
    public class Order
    {
        public decimal Price { get; set; } = 0;
        public string Side { get; set; } = "BUY";
        public decimal Size { get; set; }
        public string Reference { get; set; }
        public string Symbol { get; set; }
        public string DenominatorSybol { get; set; }
        public string OrderType { get; set; } = "LIMIT";
        public string CombindSymbol => @"{Symbol}{CombindSymbol}";
    }
}
