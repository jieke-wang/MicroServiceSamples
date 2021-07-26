using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Libs.JWT
{
    public class CurrentUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<KeyValuePair<string, string>> Claims { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
