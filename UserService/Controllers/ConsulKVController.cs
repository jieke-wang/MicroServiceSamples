using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserService.ConsulLibs;

namespace UserService.Controllers {
    [ApiController]
    [Route ("api/[controller]")]
    public class ConsulKVController : ControllerBase {
        private readonly IConsulKV _consulKV;
        public ConsulKVController (IConsulKV consulKV) {
            _consulKV = consulKV;
        }

        [HttpGet]
        public async Task<string> GetAsync (string key) {
            return await _consulKV.GetAsync (key);
        }

        [HttpPost, HttpPut]
        public async Task<bool> SetAsync (string key, string value) {
            return await _consulKV.SetAsync (key, value);
        }

        [HttpDelete]
        public async Task<bool> DeleteAsync (string key) {
            return await _consulKV.DeleteAsync (key);
        }
    }
}