using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Services
{
    public class TradingViewAlertService : ITradingViewAlertService
    {
        private static readonly IDictionary<string, List<Dictionary<string, string>>> _tradingViewUpdates = new Dictionary<string, List<Dictionary<string, string>>>();

        private readonly IInstanceConfigurationService _instanceConfigurationService;

        public TradingViewAlertService(IInstanceConfigurationService instanceConfigurationService)
        {
            _instanceConfigurationService = instanceConfigurationService;
        }

        public void AddTradingViewUpdate(string botInstanceId, Dictionary<string, string> updateVariables)
        {
            if (_tradingViewUpdates.ContainsKey(botInstanceId))
            {
                _tradingViewUpdates[botInstanceId].Add(updateVariables);
            }
            else
            {
                _tradingViewUpdates[botInstanceId] = new List<Dictionary<string, string>>
                {
                    updateVariables
                };
            }
        }


        public void ProcessAllTradingViewUpdates()
        {
            foreach (var instanceKey in _tradingViewUpdates.Keys.ToList())
            {
                foreach (var updateData in _tradingViewUpdates[instanceKey])
                {
                    _instanceConfigurationService.SetConfiguration(instanceKey, updateData);
                    _tradingViewUpdates[instanceKey].Remove(updateData);
                }

                if (_tradingViewUpdates[instanceKey].Count == 0)
                {
                    _tradingViewUpdates.Remove(instanceKey);
                }
            }
        }

        public void ProcessTradingViewUpdates(string botInstanceId)
        {
            if (_tradingViewUpdates.ContainsKey(botInstanceId))
            {
                _tradingViewUpdates[botInstanceId].ToList().ForEach(updateData =>
                {
                    _instanceConfigurationService.SetConfiguration(botInstanceId, updateData);
                    _tradingViewUpdates[botInstanceId].Remove(updateData);
                });
                if (_tradingViewUpdates[botInstanceId].Count == 0)
                {
                    _tradingViewUpdates.Remove(botInstanceId);
                }
            }
        }
    }
}
