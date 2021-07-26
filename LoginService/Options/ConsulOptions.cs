using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Options
{
    public class ConsulOptions
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Datacenter { get; set; }
        public string Token { get; set; }
    }
}
