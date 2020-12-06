using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Factory
{
    public interface ISecretFactory
    {
        IApiSecrets GetApiSecret(string secretId);
    }
}
