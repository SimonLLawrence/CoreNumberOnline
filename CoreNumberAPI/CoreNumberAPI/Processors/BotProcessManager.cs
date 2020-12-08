using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Factory;
using CoreNumberAPI.Model;
using CoreNumberAPI.Repository;

namespace CoreNumberAPI.Processors
{
    public class BotProcessManager : IBotProcessManager
    {
        private IExchangeFactory _exchangeFactory;
        private ISecretDataRepository _secretRepository;
        private IBotProcessorFactory _botProcessorFactory;
        private IBotInstanceDataRepository _botInstanceRepository;

        public BotProcessManager(IExchangeFactory exchangeFactory,  IBotProcessorFactory botProcessorFactory, IBotInstanceDataRepository botInstanceRepository, ISecretDataRepository secretRepository)
        {
            _exchangeFactory = exchangeFactory;
            _secretRepository = secretRepository;
            _botProcessorFactory = botProcessorFactory;
            _botInstanceRepository = botInstanceRepository;
            State = "INITIALIZED";
        }

        public string State { get; set; }

        public void StartProcessingBots()
        {
            State = "RUNNING";
        }

        public void StopProcessingBots()
        {
            State = "STOPPED";
        }

        public void ExecuteAllAlgos()
        {
            if (State == "RUNNING")
            {
                var instances = _botInstanceRepository.GetAllBotInstanceData();
                foreach (var botInstanceData in instances)
                {
                    ExecuteBot(botInstanceData.Id.ToString());
                }
            }
        }

        public void ExecuteBot(string botInstanceId)
        {
            if (State == "RUNNING")
            {
                var instance = _botInstanceRepository.GetBotInstanceData(botInstanceId);
                var processor = _botProcessorFactory.GetBotProcessor(instance.ProcessorID);
                if (instance.State =="CREATED")
                {
                    processor.Initialise(instance);
                }
                if (instance.State == "STARTED")
                {
                    processor.Process(instance, DateTime.UtcNow);
                    _botInstanceRepository.Save(instance);
                }
            }
        }

        public void StartBot(string botInstanceId)
        {
            var botInstance = _botInstanceRepository.GetBotInstanceData(botInstanceId);
            botInstance.State = "STARTED";
            _botInstanceRepository.Save(botInstance);
        }

        public void StopBot(string botInstanceId)
        {
            var botInstance = _botInstanceRepository.GetBotInstanceData(botInstanceId);
            botInstance.State = "STOPPED";
            _botInstanceRepository.Save(botInstance);
        }

        public string CreateBot(string botProcessorName, string exchangeName, string key , string secret, string subAccount = null)
        {
            var botInstId = Guid.NewGuid();
            var botProcessor = _botProcessorFactory.GetBotProcessor(botProcessorName);
            var exch = _exchangeFactory.GetExchange(exchangeName);
            var secr = CreateSecret(key,secret, subAccount);
            var newInstance = new BotInstanceData
            {
                Id = botInstId,
                SecretID = secr,
                ExchangeID = exch.ExchangeName,
                ProcessorID = botProcessor.BotProcessorName,
                State = "CREATED"
            };
            _botInstanceRepository.Save(newInstance);
            return botInstId.ToString();
        }

        public string CreateSecret(string key, string secret, string subaccount = null)
        {
            var secretObject = new ApiSecrets
            {
                SecretId = Guid.NewGuid().ToString(),
                Key = key,
                Secret = secret,
                SubaccountName = subaccount
            };
            _secretRepository.Save(secretObject);
            return secretObject.SecretId;
        }

        public void ShutdownBot(string botInstanceId)
        {
            var botInstance = _botInstanceRepository.GetBotInstanceData(botInstanceId);
            var botProcessor = _botProcessorFactory.GetBotProcessor(botInstance.ProcessorID);
            botProcessor.Shutdown(botInstance);
            botInstance.State = "SHUTDOWN";
            _botInstanceRepository.Save(botInstance);
        }
    }
}
