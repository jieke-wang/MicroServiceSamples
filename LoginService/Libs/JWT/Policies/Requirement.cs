using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace LoginService.Libs.JWT.Policies
{
    public abstract class Requirement : IAuthorizationRequirement
    {
        public string[] Roles { get; set; }
        public string[] Users { get; set; }
        public string[] Scopes { get; set; }
        public RequirementTypeEnum Type { get; set; }

        public static class ClaimTypes
        {
            public const string Scope = "Scope";
        }

        public static class PolicyNames
        {
            public const string DefaultPolicy = "default_policy";
        }
    }
}
