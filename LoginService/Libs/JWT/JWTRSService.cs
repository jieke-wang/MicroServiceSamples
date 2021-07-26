using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoginService.Libs.JWT
{
    public class JWTRSService : IJWTService
    {
        private readonly JWTTokenOptions _JWTTokenOptions;
        private readonly RSAHelper _rsaHelper;
        private readonly RSAParameters _keyParameters;
        private readonly SigningCredentials _signingCredentials;

        public JWTRSService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
        {
            this._JWTTokenOptions = jwtTokenOptions.CurrentValue;

            string keyDir = Directory.GetCurrentDirectory();
            _rsaHelper = new RSAHelper();
            _rsaHelper.PrivateKeyFilename = this._JWTTokenOptions.PrivateKeyFilename ?? _rsaHelper.PrivateKeyFilename;
            _rsaHelper.PublicKeyFilename = this._JWTTokenOptions.PublicKeyFilename ?? _rsaHelper.PublicKeyFilename;
            _rsaHelper.GetRSAParameters(keyDir, true, out _keyParameters);

            _signingCredentials = new SigningCredentials(new RsaSecurityKey(_keyParameters), SecurityAlgorithms.RsaSha256Signature);
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

            //string keyDir = Directory.GetCurrentDirectory();
            //RSAHelper rsaHelper = new RSAHelper();
            //rsaHelper.PrivateKeyFilename = this._JWTTokenOptions.PrivateKeyFilename ?? rsaHelper.PrivateKeyFilename;
            //rsaHelper.PublicKeyFilename = this._JWTTokenOptions.PublicKeyFilename ?? rsaHelper.PublicKeyFilename;
            //rsaHelper.GetRSAParameters(keyDir, true, out RSAParameters keyParameters);
            //var credentials = new SigningCredentials(new RsaSecurityKey(keyParameters), SecurityAlgorithms.RsaSha256Signature);

            var token = new JwtSecurityToken(
               issuer: this._JWTTokenOptions.Issuer,
               audience: this._JWTTokenOptions.Audience,
               claims: claims,
               expires: DateTime.Now.AddSeconds(this._JWTTokenOptions.Expires),
               signingCredentials: _signingCredentials);
            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);
            return tokenString;
        }
    }
}
