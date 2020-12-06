using System.Collections.Generic;
using System.Linq;
using CoreNumberAPI.Model;

namespace CoreNumberAPI.Factory
{
    public class SecretFactory
    {
        readonly List<IApiSecrets> _secrets;

        public SecretFactory(IEnumerable<IApiSecrets> secrets)
        {
            _secrets = secrets.ToList();
        }

        public IApiSecrets GetApiSecret(string secretId)
        {
            return _secrets.FirstOrDefault(s => s.SecretId == secretId);
        }
    }
}
