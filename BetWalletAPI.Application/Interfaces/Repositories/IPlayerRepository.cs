using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Domain.ValueObjects;

namespace BetWalletAPI.Application.Interfaces.Repositories;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id);
    Task<Player?> GetByEmailAsync(Email email);
    Task<Player> AddAsync(Player player);
}