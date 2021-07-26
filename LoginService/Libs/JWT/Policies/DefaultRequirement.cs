using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Libs.JWT.Policies
{
    public class DefaultRequirement : Requirement
    {
        public DefaultRequirement()
        {
            Type = RequirementTypeEnum.Login;
        }
    }
}
