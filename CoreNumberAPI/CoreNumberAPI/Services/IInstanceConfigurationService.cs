using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Services
{
    public interface IInstanceConfigurationService
    {
        void SetConfiguration(string botInstanceId, Dictionary<string, string> variables);
        Dictionary<string, string> GetConfiguration(string botInstanceId);
    }
}
