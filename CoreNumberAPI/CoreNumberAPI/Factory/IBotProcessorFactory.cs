using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Processors;

namespace CoreNumberAPI.Factory
{
    public interface IBotProcessorFactory
    {
        IBotProcessor GetBotProcessor(string botNameId);
        public List<string> GetSupportedProcessors();
    }
}
