using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                string configKey = "JWTTokenOptions";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // 是否验证Issuer
                    ValidateAudience = true, // 是否验证Audience
                    ValidateLifetime = true, // 是否验证失效时间
                    ValidateIssuerSigningKey = true, // 是否验证SecurityKey
                    ValidAudience = Configuration[$"{configKey}:Audience"],
                    ValidIssuer = Configuration[$"{configKey}:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[$"{configKey}:SecurityKey"]))
                };
            });

            //Action<JwtBearerOptions> configureOptions = options =>
            //{
            //    options.Authority = "http://192.168.199.101:5000";
            //    options.RequireHttpsMetadata = false;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateAudience = false,
            //    };
            //};

            services
                .AddOcelot()
                .AddConsul()
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                })
                .AddPolly();
            //.AddAdministration("/administration", configureOptions);

            // 使用swagger,需要配置AddControllers,否则报错
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "网关服务", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    List<string> services = new List<string> { "LoginService", "UserService", "ProductService" };
                    services.ForEach(service =>
                    {
                        c.SwaggerEndpoint($"/{service}/swagger.json", service);
                    });
                });
            }

            // 服务降级
            //app.Use(async (context, next) =>
            //{
            //    bool hasError = false;
            //    try
            //    {
            //        await next.Invoke();
            //        hasError = context.Response.StatusCode > 400;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //        hasError = true;
            //    }

            //    if (hasError)
            //    {
            //        context.Response.Clear();
            //        context.Response.ContentType = "text/plain; charset=utf8";
            //        context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
            //        await context.Response.WriteAsync("系统繁忙，请稍后重试 ...");
            //    }
            //});

            app.UseAuthentication().UseOcelot().Wait();

            //app.UseHttpsRedirection();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
