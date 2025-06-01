using BetWalletAPI.Application.DTOs;
using BetWalletAPI.Application.Interfaces.Security;
using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BetWalletAPI.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;

        if (string.IsNullOrEmpty(_jwtSettings.SecretKey) || _jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException("JWT SecretKey in JwtSettings is not configured correctly or is too short. It must be at least 32 characters long.");
        }
        if (string.IsNullOrEmpty(_jwtSettings.Issuer))
        {
            throw new InvalidOperationException("JWT Issuer in JwtSettings is not configured.");
        }
        if (string.IsNullOrEmpty(_jwtSettings.Audience))
        {
            throw new InvalidOperationException("JWT Audience in JwtSettings is not configured.");
        }
        if (_jwtSettings.ExpiryMinutes <= 0)
        {
            throw new InvalidOperationException("JWT ExpiryMinutes in JwtSettings must be greater than zero.");
        }
    }

    public (string token, DateTime expirationTime) GenerateToken(Player player)
    {
        var claims = new List<Claim> 
        {
            new Claim(JwtRegisteredClaimNames.Sub, player.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, player.Username),
            new Claim(JwtRegisteredClaimNames.Email, player.Email.Value),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryDateTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiryDateTime,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(securityToken);

        return (tokenString, expiryDateTime);
    }
}
