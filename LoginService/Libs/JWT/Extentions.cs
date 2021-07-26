using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using LoginService.Libs.JWT;
using LoginService.Libs.JWT.Policies;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extentions
    {
        public static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration, string configKey = "JWTTokenOptions", Action<AuthorizationOptions> configPolicy = null)
        {
            services.Configure<JWTTokenOptions>(configuration.GetSection(configKey));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                #region RSA
                //string keyDir = Directory.GetCurrentDirectory();
                //RSAHelper rsaHelper = new RSAHelper();
                //rsaHelper.PrivateKeyFilename = configuration["JWTTokenOptions:PrivateKeyFilename"] ?? rsaHelper.PrivateKeyFilename;
                //rsaHelper.PublicKeyFilename = configuration["JWTTokenOptions:PublicKeyFilename"] ?? rsaHelper.PublicKeyFilename;
                //rsaHelper.GetRSAParameters(keyDir, true, out RSAParameters keyParameters);

                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    ValidateIssuer = true, // 是否验证Issuer
                //    ValidateAudience = true, // 是否验证Audience
                //    ValidateLifetime = true, // 是否验证失效时间
                //    ValidateIssuerSigningKey = true, // 是否验证SecurityKey
                //    ValidAudience = configuration["JWTTokenOptions:Audience"],
                //    ValidIssuer = configuration["JWTTokenOptions:Issuer"],
                //    IssuerSigningKey = new RsaSecurityKey(keyParameters),
                //};
                #endregion

                #region DES
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // 是否验证Issuer
                    ValidateAudience = true, // 是否验证Audience
                    ValidateLifetime = true, // 是否验证失效时间
                    ValidateIssuerSigningKey = true, // 是否验证SecurityKey
                    ValidAudience = configuration[$"{configKey}:Audience"],
                    ValidIssuer = configuration[$"{configKey}:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[$"{configKey}:SecurityKey"]))
                };
                #endregion
            });

            //services.AddSingleton<IJWTService, JWTRSService>();
            services.AddSingleton<IJWTService, JWTHSService>();

            // 配置授权策略
            services.AddAuthorization(config =>
            {
                #region config default policy
                config.AddPolicy(Requirement.PolicyNames.DefaultPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new DefaultRequirement());
                });
                #endregion

                configPolicy?.Invoke(config);
            });
            services.AddSingleton<IAuthorizationHandler, RequirementHandler>(); // 注册约束处理器

            return services;
        }

        public async static Task<CurrentUserModel> GetCurrentUser(this HttpContext context)
        {
            CurrentUserModel currentUserModel = new CurrentUserModel();
            AuthenticateResult authenticateResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (authenticateResult?.Principal != null)
            {
                context.User = authenticateResult.Principal;
                ClaimsIdentity identity = authenticateResult.Principal.Identity as ClaimsIdentity;
                BindClaimsToCurrentUser(identity.Claims, currentUserModel);
            }

            return currentUserModel;
        }

        //public static CurrentUserModel GetCurrentUser(this Microsoft.AspNetCore.Http.HttpContext context)
        //{
        //    CurrentUserModel currentUserModel = new CurrentUserModel();
        //    if (context.User != null)
        //    {
        //        BindClaimsToCurrentUser(context.User.Claims, currentUserModel);
        //    }

        //    return currentUserModel;
        //}

        private static void BindClaimsToCurrentUser(IEnumerable<Claim> claims, CurrentUserModel currentUserModel)
        {
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.Name:
                        currentUserModel.UserName = claim.Value;
                        break;
                    case ClaimTypes.Email:
                        currentUserModel.Email = claim.Value;
                        break;
                    case ClaimTypes.NameIdentifier:
                        currentUserModel.Id = claim.Value;
                        break;
                    case ClaimTypes.MobilePhone:
                        currentUserModel.Phone = claim.Value;
                        break;
                    default:
                        currentUserModel.Claims.Add(new KeyValuePair<string, string>(claim.Type, claim.Value));
                        break;
                }
            }
        }
    }
}
