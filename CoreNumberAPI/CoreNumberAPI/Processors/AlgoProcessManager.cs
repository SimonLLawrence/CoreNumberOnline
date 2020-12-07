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
        private ISecretFactory _secretFactory;
        private IAlgoProcessorFactory _algoProcessorFactory;
        private IAlgoInstanceDataRepository _algoInstanceRepository;

        public AlgoProcessManager(IExchangeFactory exchangeFactory, ISecretFactory secretFactory, IAlgoProcessorFactory algoProcessorFactory, IAlgoInstanceDataRepository algoInstanceRepository)
        {
            _exchangeFactory = exchangeFactory;
            _secretFactory = secretFactory;
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
                    ExecuteAlgo(algoInstanceData);
                }
            }
        }

        public void ExecuteAlgo(AlgoInstanceData instance)
        {
            if (State == "RUNNING")
            {
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
            var algoInstance = _algoInstanceRepository.GetAlgoInstanceData(Guid.Parse(algoInstanceId));
            algoInstance.State = "STARTED";
            _algoInstanceRepository.Save(algoInstance);
        }

        public void StopAlgo(string algoInstanceId)
        {
            var algoInstance = _algoInstanceRepository.GetAlgoInstanceData(Guid.Parse(algoInstanceId));
            algoInstance.State = "STOPPED";
            _algoInstanceRepository.Save(algoInstance);
        }

        public string CreateAlgo(string algoName, string exchangeName, string secretId)
        {
            var algoInstId = Guid.NewGuid();
            var algo = _algoProcessorFactory.GetAlgoProcessor(algoName);
            var exch = _exchangeFactory.GetExchange(exchangeName);
            var secr = _secretFactory.GetApiSecret(secretId);
            var newInstance = new AlgoInstanceData
            {
                Id = algoInstId,
                SecretID = secr.SecretId,
                ExchangeID = exch.ExchangeName,
                ProcessorID = algo.AlgorithmName,
                State = "CREATED"
            };
            _algoInstanceRepository.Save(newInstance);
            return algoInstId.ToString();
        }

        public void DestroyAlgo(string algoInstanceId)
        {
            throw new NotImplementedException();
        }
    }
}
