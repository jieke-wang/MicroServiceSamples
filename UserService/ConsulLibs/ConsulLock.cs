using System.Threading.Tasks;
using Consul;

namespace UserService.ConsulLibs {
    public class ConsulLock : IConsulLock {
        private readonly IConsulClient _client;
        const string LockPrefix = "lock/";

        public ConsulLock (IConsulClient client) {
            _client = client;
        }

        public async Task<IDistributedLock> AcquireLockAsync (string lockName) {
            return await _client.AcquireLock ($"{LockPrefix}{lockName}");
        }

        // public IDistributedLock CreateLock (string lockName) {
        //     return _client.CreateLock ($"{LockPrefix}{lockName}");
        // }
    }
}