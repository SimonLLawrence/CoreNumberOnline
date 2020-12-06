using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Services;

namespace CoreNumberAPI.Factory
{
    public interface IExchangeFactory
    {
        IExchange GetExchange(string exchangeId);
    }
}
