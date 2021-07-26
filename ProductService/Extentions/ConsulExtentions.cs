using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProductService.Options;

namespace ProductService.Extentions {
    public static class ConsulExtentions {
        public static IServiceCollection AddConsul (this IServiceCollection services, IConfiguration configuration) {
            services.Configure<ConsulOptions> (configuration.GetSection (nameof (ConsulOptions)));
            services.Configure<ServiceRegisterOptions> (configuration.GetSection (nameof (ServiceRegisterOptions)));
            services.AddSingleton<IConsulClient, ConsulClient> (sp => {
                IOptions<ConsulOptions> consulOptions = sp.GetService<IOptions<ConsulOptions>> ();
                ConsulClient consulClient = new (config => {
                    config.Address = new Uri ($"http://{consulOptions.Value.IP}:{consulOptions.Value.Port}");
                    config.Datacenter = consulOptions.Value.Datacenter;
                    config.Token = consulOptions.Value.Token;
                });

                return consulClient;
            });

            return services;
        }

        public static IApplicationBuilder UseConsul (this IApplicationBuilder app) {
            IOptions<ServiceRegisterOptions> serviceRegisterOptions = app.ApplicationServices.GetService<IOptions<ServiceRegisterOptions>> ();
            IConsulClient consulClient = app.ApplicationServices.GetRequiredService<IConsulClient> ();
            IServerAddressesFeature serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature> ();
            IConfiguration configuration = app.ApplicationServices.GetService<IConfiguration> ();
            string ip = configuration["ip"];
            string port = configuration["port"];

            string serviceId = $"ProductService-{string.Join("-", serverAddressesFeature.Addresses)}".Replace("//", string.Empty);
            if(string.IsNullOrWhiteSpace(ip) == false)
            {
                serviceId = $"ProductService-{string.Join("-", ip, port)}";

                consulClient.Agent.ServiceRegister(new AgentServiceRegistration
                {
                    // 服务ID
                    ID = serviceId,
                    // 服务名称
                    Name = serviceRegisterOptions.Value.Name,
                    // 服务地址(域名/IP)
                    Address = ip,
                    // 服务端口号
                    Port = int.Parse(port),
                    // 服务标签(版本)
                    Tags = serviceRegisterOptions.Value.Tags,
                    // 服务健康检查
                    Check = new AgentServiceCheck
                    {
                        // consul健康检查间隔时间
                        Interval = TimeSpan.FromSeconds(serviceRegisterOptions.Value.CheckIntervalOnSeconds),
                        // consul健康检查地址
                        HTTP = $"http://{ip}:{port}/health",
                        // consul健康检查超时间
                        Timeout = TimeSpan.FromSeconds(serviceRegisterOptions.Value.CheckTimeoutOnSeconds),
                        // 服务停止多少秒后注销服务
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(serviceRegisterOptions.Value.DeregisterCriticalServiceAfterOnSeconds),
                    }
                }).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                consulClient.Agent.ServiceRegister(new AgentServiceRegistration
                {
                    ID = serviceId,
                    Name = serviceRegisterOptions.Value.Name,
                    Address = serviceRegisterOptions.Value.Ip,
                    Port = serviceRegisterOptions.Value.Port,
                    Tags = serviceRegisterOptions.Value.Tags,
                    Check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(serviceRegisterOptions.Value.CheckIntervalOnSeconds),
                        HTTP = serviceRegisterOptions.Value.CheckAddress,
                        Timeout = TimeSpan.FromSeconds(serviceRegisterOptions.Value.CheckTimeoutOnSeconds),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(serviceRegisterOptions.Value.DeregisterCriticalServiceAfterOnSeconds),
                    }
                }).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            app
                .ApplicationServices
                .GetRequiredService<IHostApplicationLifetime> ()
                .ApplicationStopping.Register (() => {
                    consulClient.Agent.ServiceDeregister (serviceId).Wait();
                    consulClient.Dispose ();
                });

            return app;
        }
    }
}