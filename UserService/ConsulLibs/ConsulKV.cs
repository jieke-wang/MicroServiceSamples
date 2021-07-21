using System.Text;
using System.Threading.Tasks;
using Consul;

namespace UserService.ConsulLibs {
    public class ConsulKV : IConsulKV {
        private readonly IConsulClient _client;
        private readonly Encoding _utf8 = Encoding.UTF8;

        public ConsulKV (IConsulClient client) {
            _client = client;
        }

        public async Task<string> GetAsync (string key) {
            QueryResult<KVPair> result = await _client.KV.Get (key);

            byte[] value = result?.Response?.Value;
            if (value == null) return string.Empty;

            return _utf8.GetString (value);
        }

        public async Task<bool> SetAsync (string key, string value) {
            WriteResult<bool> result = await _client.KV.Put (new KVPair (key) { Value = _utf8.GetBytes (value) });
            return result.Response;
        }

        public async Task<bool> DeleteAsync (string key) {
            WriteResult<bool> result = await _client.KV.Delete (key);
            return result.Response;
        }
    }
}