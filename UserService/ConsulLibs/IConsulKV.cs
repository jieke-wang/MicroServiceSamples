using System.Threading.Tasks;

namespace UserService.ConsulLibs
{
    public interface IConsulKV
    {
        Task<bool> DeleteAsync(string key);
        Task<string> GetAsync(string key);
        Task<bool> SetAsync(string key, string value);
    }
}