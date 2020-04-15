using AsicServer.Core.Entities;
using AsicServer.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AsicServer.Core.Utils
{
    public class JwtTokenProvider
    {
        public ExtensionSettings extensionSettings;

        public JwtTokenProvider(ExtensionSettings extensionSettings)
        {
            this.extensionSettings = extensionSettings;
        }

        public string CreateAccesstoken(User user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(extensionSettings.appSettings.SecretKey);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Code.ToString()),
                    new Claim(ClaimTypes.Name ,user.Email),
                    new Claim(ClaimTypes.Name ,user.Name),
                };
            claims.Add(new Claim(ClaimTypes.Role, user.RoleId.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = extensionSettings.appSettings.Audience,
                Issuer  =extensionSettings.appSettings.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(extensionSettings.appSettings.TokenExpireTime),
                SigningCredentials = 
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
