using System.Threading.Tasks;
using Consul;

namespace UserService.ConsulLibs
{
    public interface IConsulLock
    {
        Task<IDistributedLock> AcquireLockAsync(string lockName);
        // IDistributedLock CreateLock(string lockName);
    }
}