using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            consulClient.Agent.ServiceRegister (new AgentServiceRegistration {
                ID = $"UserService-{string.Join("-", serverAddressesFeature.Addresses)}",
                    Name = serviceRegisterOptions.Value.Name,
                    Address = serviceRegisterOptions.Value.Ip,
                    Port = serviceRegisterOptions.Value.Port,
                    Tags = serviceRegisterOptions.Value.Tags,
                    Check = new AgentServiceCheck {
                        Interval = TimeSpan.FromSeconds (serviceRegisterOptions.Value.CheckIntervalOnSeconds),
                            HTTP = serviceRegisterOptions.Value.CheckAddress,
                            Timeout = TimeSpan.FromSeconds (serviceRegisterOptions.Value.CheckTimeoutOnSeconds),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds (serviceRegisterOptions.Value.DeregisterCriticalServiceAfterOnSeconds),
                    }
            }).ConfigureAwait (false).GetAwaiter ().GetResult ();

            return app;
        }
    }
}