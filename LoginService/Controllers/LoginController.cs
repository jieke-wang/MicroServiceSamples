using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LoginService.Libs.JWT;
using LoginService.Libs.JWT.Policies;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoginService.Controllers
{
    [ApiController]
    [Route("LoginService/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = Requirement.PolicyNames.DefaultPolicy)]
        public async Task<CurrentUserModel> GetAsync()
        {
            return await HttpContext.GetCurrentUser();
            //return await ControllerContext.HttpContext.GetCurrentUser();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, [FromServices] IJWTService jwtService)
        {
            const string UserName = "jack";
            const string Password = "123";

            if(username != UserName || password != Password)
            {
                return BadRequest();
            }

            CurrentUserModel currentUserModel = new CurrentUserModel
            {
                Id = Guid.NewGuid().ToString(),
                UserName= UserName,
                Phone = "0123456789",
                Email = "jack@abc.com",
                Claims = new List<KeyValuePair<string, string>> 
                {
                    new KeyValuePair<string, string>(Requirement.ClaimTypes.Scope, "ProductService"),
                    new KeyValuePair<string, string>(Requirement.ClaimTypes.Scope, "UserService"),
                }
            };

            string token = jwtService.GetToken(currentUserModel);
            return Ok(token);
        }
    }
}
