using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers {
    [ApiController]
    [Route ("api/[controller]/[action]")]
    public class ConsulAgentController : ControllerBase {
        private readonly IConsulClient _client;

        public ConsulAgentController (IConsulClient client) {
            _client = client;
        }

        [HttpGet]
        public async Task<QueryResult<Dictionary<string, AgentService>>> GetServicesAsync () {
            QueryResult<Dictionary<string, AgentService>> result = await _client.Agent.Services ();
            return result;
        }

        [HttpGet]
        public async Task<QueryResult<AgentMember[]>> GetMembersAsync (bool wan) {
            QueryResult<AgentMember[]> result = await _client.Agent.Members (wan);
            return result;
        }

        [HttpGet]
        public async Task<string> GetNodeNameAsync () {
            string result = await _client.Agent.GetNodeName ();
            return result;
        }

        [HttpGet]
        public async Task<QueryResult<Dictionary<string, Dictionary<string, dynamic>>>> GetSelfAsync () {
            QueryResult<Dictionary<string, Dictionary<string, dynamic>>> result = await _client.Agent.Self ();
            return result;
        }
    }
}