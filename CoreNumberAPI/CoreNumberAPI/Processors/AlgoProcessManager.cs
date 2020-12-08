using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Factory;
using CoreNumberAPI.Model;
using CoreNumberAPI.Repository;

namespace CoreNumberAPI.Processors
{
    public class AlgoProcessManager : IAlgoProcessManager
    {
        private IExchangeFactory _exchangeFactory;
        private ISecretDataRepository _secretRepository;
        private IAlgoProcessorFactory _algoProcessorFactory;
        private IAlgoInstanceDataRepository _algoInstanceRepository;

        public AlgoProcessManager(IExchangeFactory exchangeFactory,  IAlgoProcessorFactory algoProcessorFactory, IAlgoInstanceDataRepository algoInstanceRepository, ISecretDataRepository secretRepository)
        {
            _exchangeFactory = exchangeFactory;
            _secretRepository = secretRepository;
            _algoProcessorFactory = algoProcessorFactory;
            _algoInstanceRepository = algoInstanceRepository;
            State = "INITIALIZED";
        }

        public string State { get; set; }

        public void StartProcessingAlgos()
        {
            State = "RUNNING";
        }

        public void StopProcessingAlgos()
        {
            State = "STOPPED";
        }

        public void ExecuteAllAlgos()
        {
            if (State == "RUNNING")
            {
                var instances = _algoInstanceRepository.GetAllAlgoInstanceData();
                foreach (var algoInstanceData in instances)
                {
                    ExecuteAlgo(algoInstanceData.Id.ToString());
                }
            }
        }

        public void ExecuteAlgo(string algoInstanceId)
        {
            if (State == "RUNNING")
            {
                var instance = _algoInstanceRepository.GetAlgoInstanceData(algoInstanceId);
                var processor = _algoProcessorFactory.GetAlgoProcessor(instance.ProcessorID);
                if (instance.State =="CREATED")
                {
                    processor.Initialise(instance);
                }
                if (instance.State == "STARTED")
                {
                    processor.Process(instance, DateTime.UtcNow);
                    _algoInstanceRepository.Save(instance);
                }
            }
        }

        public void StartAlgo(string algoInstanceId)
        {
            var algoInstance = _algoInstanceRepository.GetAlgoInstanceData(algoInstanceId);
            algoInstance.State = "STARTED";
            _algoInstanceRepository.Save(algoInstance);
        }

        public void StopAlgo(string algoInstanceId)
        {
            var algoInstance = _algoInstanceRepository.GetAlgoInstanceData(algoInstanceId);
            algoInstance.State = "STOPPED";
            _algoInstanceRepository.Save(algoInstance);
        }

        public string CreateAlgo(string algoName, string exchangeName, string key , string secret, string subAccount = null)
        {
            var algoInstId = Guid.NewGuid();
            var algo = _algoProcessorFactory.GetAlgoProcessor(algoName);
            var exch = _exchangeFactory.GetExchange(exchangeName);
            var secr = CreateSecret(key,secret, subAccount);
            var newInstance = new AlgoInstanceData
            {
                Id = algoInstId,
                SecretID = secr,
                ExchangeID = exch.ExchangeName,
                ProcessorID = algo.AlgorithmName,
                State = "CREATED"
            };
            _algoInstanceRepository.Save(newInstance);
            return algoInstId.ToString();
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

        public void ShutdownAlgo(string algoInstanceId)
        {
            var algoInstance = _algoInstanceRepository.GetAlgoInstanceData(algoInstanceId);
            var algo = _algoProcessorFactory.GetAlgoProcessor(algoInstance.ProcessorID);
            algo.Shutdown(algoInstance);
            algoInstance.State = "SHUTDOWN";
            _algoInstanceRepository.Save(algoInstance);
        }
    }
}
