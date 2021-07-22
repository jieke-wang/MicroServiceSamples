using System.Collections.Generic;
using System.Threading.Tasks;

using Consul;

using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    /// <summary>
    /// Consul Agent 服务
    /// </summary>
    [ApiController]
    [Route("UserService/[controller]/[action]")]
    public class ConsulAgentController : ControllerBase
    {
        private readonly IConsulClient _client;

        public ConsulAgentController(IConsulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        [HttpGet]
        public async Task<QueryResult<Dictionary<string, AgentService>>> GetServicesAsync()
        {
            QueryResult<Dictionary<string, AgentService>> result = await _client.Agent.Services();
            return result;
        }

        /// <summary>
        /// 获取代理成员
        /// </summary>
        [HttpGet]
        public async Task<QueryResult<AgentMember[]>> GetMembersAsync(bool wan)
        {
            QueryResult<AgentMember[]> result = await _client.Agent.Members(wan);
            return result;
        }

        /// <summary>
        /// 获取节点名称
        /// </summary>
        [HttpGet]
        public async Task<string> GetNodeNameAsync()
        {
            string result = await _client.Agent.GetNodeName();
            return result;
        }

        /// <summary>
        /// 获取自身节点信息
        /// </summary>
        [HttpGet]
        public async Task<QueryResult<Dictionary<string, Dictionary<string, dynamic>>>> GetSelfAsync()
        {
            QueryResult<Dictionary<string, Dictionary<string, dynamic>>> result = await _client.Agent.Self();
            return result;
        }
    }
}