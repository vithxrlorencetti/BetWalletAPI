using BetWalletAPI.Application.DTOs;
using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Interfaces.Security
{
    public interface IJwtTokenGenerator
    {
        (string token, DateTime expirationTime) GenerateToken(Player player);
    }
}
