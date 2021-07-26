using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(string.Join(Environment.NewLine, args));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

// dotnet ProductService.dll --urls="http://*:5010" --ip="192.168.199.101" --port=5010
// dotnet ProductService.dll --urls="http://*:5011" --ip="192.168.199.101" --port=5011
// dotnet ProductService.dll --urls="http://*:5012" --ip="192.168.199.101" --port=5012
// dotnet ProductService.dll --urls="http://*:5011" /ip="192.168.199.101" -p=5011