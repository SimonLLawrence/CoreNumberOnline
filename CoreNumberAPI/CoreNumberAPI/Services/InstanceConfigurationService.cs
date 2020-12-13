using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Repository;

namespace CoreNumberAPI.Services
{
    public class InstanceConfigurationService : IInstanceConfigurationService
    {
        private readonly IBotInstanceDataRepository _botInstanceDataRepository;

        public InstanceConfigurationService(IBotInstanceDataRepository botInstanceDataRepository)
        {
            _botInstanceDataRepository = botInstanceDataRepository;
        }

        public Dictionary<string, string> GetConfiguration(string botInstanceId)
        {
            var botInstance = _botInstanceDataRepository.GetBotInstanceData(botInstanceId);
            return botInstance.GetVariables();
        }

        public void SetConfiguration(string botInstanceId, Dictionary<string, string> variables)
        {
            var botInstance = _botInstanceDataRepository.GetBotInstanceData(botInstanceId);
            botInstance.SetVariables(variables);
            _botInstanceDataRepository.Save(botInstance);
        }
    }
}
