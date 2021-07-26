using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Libs.JWT.Policies
{
    [Flags]
    public enum RequirementTypeEnum
    {
        Login = 0,
        Role = 1 << 1,
        User = 1 << 2,
        Scope = 1 << 3,
    }
}
