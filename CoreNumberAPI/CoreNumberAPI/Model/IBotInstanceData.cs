using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNumberAPI.Model
{
    public interface IBotInstanceData
    {
        Guid Id { get; set; }
        string ExchangeID { get; set; }
        string ProcessorID { get; set; }
        string SecretID { get; set; }
        string State { get; set; }
        Dictionary<string, string> GetVariables();
        void SetVariables(Dictionary<string, string> variables);

    }
}
