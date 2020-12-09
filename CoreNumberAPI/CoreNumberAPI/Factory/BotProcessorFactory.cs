using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Processors;

namespace CoreNumberAPI.Factory
{
    public class BotProcessorFactory : IBotProcessorFactory
    {
        private List<IBotProcessor> _botProcessors;

        public BotProcessorFactory(IEnumerable<IBotProcessor> botProcessors)
        {
            _botProcessors = botProcessors.ToList();
        }

        public IBotProcessor GetBotProcessor(string botNameId)
        {
            return _botProcessors.FirstOrDefault(a=>a.BotProcessorName == botNameId);
        }

        public List<string> GetSupportedProcessors()
        {
           return _botProcessors.Select(bp => bp.BotProcessorName).ToList();
        }
    }
}
