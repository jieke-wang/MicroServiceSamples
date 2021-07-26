using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoginService.Libs.JWT
{
    public class JWTHSService : IJWTService
    {
        private readonly JWTTokenOptions _JWTTokenOptions;
        public JWTHSService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
        {
            this._JWTTokenOptions = jwtTokenOptions.CurrentValue;
        }

        public string GetToken(CurrentUserModel userInfo)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, userInfo.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, userInfo.Phone ?? string.Empty),
            };

            foreach (KeyValuePair<string, string> item in userInfo.Claims)
            {
                claims.Add(new Claim(item.Key, item.Value));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._JWTTokenOptions.SecurityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            /**
             * Claims (Payload)
                Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:

                iss: The issuer of the token，token 是给谁的
                sub: The subject of the token，token 主题
                exp: Expiration Time。 token 过期时间，Unix 时间戳格式
                iat: Issued At。 token 创建时间， Unix 时间戳格式
                jti: JWT ID。针对当前 token 的唯一标识
                除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
             * */
            var token = new JwtSecurityToken(
                issuer: this._JWTTokenOptions.Issuer,
                audience: this._JWTTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(this._JWTTokenOptions.Expires),
                signingCredentials: credentials);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }
    }
}
