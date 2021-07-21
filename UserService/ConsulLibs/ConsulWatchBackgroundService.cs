using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;

namespace UserService.ConsulLibs {
    public class ConsulWatchBackgroundService : BackgroundService {
        private readonly IConsulClient _client;

        public ConsulWatchBackgroundService (IConsulClient client) {
            _client = client;
        }

        /* CreateIndex是表示创建条目时的内部索引值。 
         * ModifyIndex是修改此键的最后一个索引。此索引对应于响应中返回的X-Consul-Index头，并且可以通过设置？index查询参数来用于建立阻塞查询。您甚至可以对KV存储的整个子树执行阻塞查询：如果提供了?recurse，则返回的X-Consul-Index对应于前缀中的最新ModifyIndex，使用“？index”的阻塞查询将等待，直到任何该前缀中的键被更新完成。
         * LockIndex是在锁中成功获取此键的次数。如果持有锁，Session键标示拥有锁的会话。 
         * Key是简单的条目的完整路径。 
         * Flags是一个不透明的无符号整数，可以附加到每个条目。它可以用来被客户添加有意义的元数据到任何键值。 
         * Value是一个Base64编码的数据块。
         */
        protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
            const string watchKey = "Watchkey1";
            QueryResult<KVPair> queryResult = await _client.KV.Get (watchKey, stoppingToken);
            Console.WriteLine (JsonSerializer.Serialize (queryResult));
            ulong latestModifyIndex = queryResult.Response?.ModifyIndex ?? 0ul;
            while (stoppingToken.IsCancellationRequested == false) {
                queryResult = await _client.KV.Get (watchKey, new QueryOptions { WaitIndex = latestModifyIndex + 1, WaitTime = TimeSpan.FromSeconds (100) }, stoppingToken); // 这里会阻塞访问api，直到服务器的数据发生变化或请求超时
                Console.WriteLine (JsonSerializer.Serialize (queryResult));

                if (queryResult.Response != null) {
                    latestModifyIndex = queryResult.Response.ModifyIndex;
                }
            }
        }
    }
}