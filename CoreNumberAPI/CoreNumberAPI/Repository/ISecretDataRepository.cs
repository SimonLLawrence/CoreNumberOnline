using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Repository
{
    public interface ISecretDataRepository
    {
        IApiSecrets GetApiSecret(string secretId);
        void Save(IApiSecrets secret);
    }
}
