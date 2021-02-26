using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TokenService.Models;
using DisputeApi.Web.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DisputeApi.Web.Features.TokenService.Service
{
    public interface ITokensService
    {
        Task<Token> CreateToken();
    }

    public class TokensService : ITokensService
    {

        private readonly ILogger<TokensService> _logger;
        private readonly AppSettings _appSettings;
        private readonly SymmetricSecurityKey _key;
        public TokensService(ILogger<TokensService> logger, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtTokenKey));
        }
        public Task<Token> CreateToken()
        {
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId, "nouser")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(_appSettings.JwtExpiry),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Created new token");

            return Task.FromResult(new Token(tokenHandler.WriteToken(token)));
        }
    }
}