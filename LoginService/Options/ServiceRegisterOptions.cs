using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Options
{
    public class ServiceRegisterOptions
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string[] Tags { get; set; }
        public int CheckIntervalOnSeconds { get; set; }
        public string CheckAddress { get; set; }
        public int CheckTimeoutOnSeconds { get; set; }
        public int DeregisterCriticalServiceAfterOnSeconds { get; set; }
    }
}
