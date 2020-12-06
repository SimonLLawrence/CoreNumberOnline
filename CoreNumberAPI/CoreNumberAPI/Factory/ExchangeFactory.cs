using System.Collections.Generic;
using System.Linq;
using CoreNumberAPI.Services;

namespace CoreNumberAPI.Factory
{
    public class ExchangeFactory : IExchangeFactory
    {
        private readonly List<IExchange> _availableExchanges;

        public ExchangeFactory(IEnumerable<IExchange> availableExchanges)
        {
            _availableExchanges = availableExchanges.ToList();
        }

        public IExchange GetExchange(string exchangeId )
        {
            return  _availableExchanges.FirstOrDefault(x => x.ExchangeName == exchangeId);
        }
    }
}
