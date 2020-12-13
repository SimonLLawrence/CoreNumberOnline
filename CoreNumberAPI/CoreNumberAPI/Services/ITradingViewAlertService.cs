using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Services
{
    public interface ITradingViewAlertService
    {
        void AddTradingViewUpdate(string botInstanceId, Dictionary<string, string> updateVariables);
        void ProcessAllTradingViewUpdates();
        void ProcessTradingViewUpdates(string botInstanceId);
    }
}
