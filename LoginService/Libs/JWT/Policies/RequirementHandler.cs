using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace LoginService.Libs.JWT.Policies
{
    public class RequirementHandler : AuthorizationHandler<Requirement>
    {
        /*
         * requirement 成功的条件：
         * 一个 handler 成功，其它的 handler 没有失败，则 requirement 成功
         * 某个 handler 失败，则 requirement 失败
         * 没有任何 handler 成功 或 失败，则 requirement 失败
         * 如果配置了多个Authorize，则会进来多次；一般在控制器上设置的权限比action设置的权限要大，即action上的权限更精确
         */
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirement requirement)
        {
            if(requirement.Type.HasFlag(RequirementTypeEnum.Login))
            {
                if ((context?.User?.Identity?.IsAuthenticated ?? false) == false) // 判断用户是否登录
                {
                    context.Fail(); // 标记为失败
                    return Task.CompletedTask;
                }
                else
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement.Type.HasFlag(RequirementTypeEnum.Role) && requirement.Roles?.Length > 0)
            {
                for (int i = 0; i < requirement.Roles.Length; i++)
                {
                    if (context.User.HasClaim(ClaimTypes.Role, requirement.Roles[i])) // 判断当前用户是否含有指定角色
                    {
                        context.Succeed(requirement); // 标记为成功
                        break;
                    }
                }
            }

            if (requirement.Type.HasFlag(RequirementTypeEnum.User) && requirement.Users?.Length > 0)
            {
                for (int i = 0; i < requirement.Users.Length; i++)
                {
                    if (context.User.HasClaim(ClaimTypes.NameIdentifier, requirement.Users[i])) // 判断当前用户是否是指定用户
                    {
                        context.Succeed(requirement); // 标记为成功
                        break;
                    }
                }
            }

            if (requirement.Type.HasFlag(RequirementTypeEnum.Scope) && requirement.Scopes?.Length > 0)
            {
                for (int i = 0; i < requirement.Scopes.Length; i++)
                {
                    if (context.User.HasClaim(Requirement.ClaimTypes.Scope, requirement.Scopes[i])) // 判断当前用户是否允许访问指定范围
                    {
                        context.Succeed(requirement); // 标记为成功
                        break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
