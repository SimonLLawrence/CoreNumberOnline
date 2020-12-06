using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Processors;

namespace CoreNumberAPI.Factory
{
    public class AlgoProcessorFactory : IAlgoProcessorFactory
    {
        private List<IAlgoProcessor> _algoProcessors;

        public AlgoProcessorFactory(IEnumerable<IAlgoProcessor> algoProcessors)
        {
            _algoProcessors = algoProcessors.ToList();
        }

        public IAlgoProcessor GetAlgoProcessor(string algoNameId)
        {
            return _algoProcessors.FirstOrDefault(a=>a.AlgorithmName == algoNameId);
        }

    }
}
