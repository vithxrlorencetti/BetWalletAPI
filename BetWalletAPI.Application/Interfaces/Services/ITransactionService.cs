using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.DTOs.Transaction;

namespace BetWalletAPI.Application.Interfaces.Services;

public interface ITransactionService
{
    Task<TransactionResponseDto> CreateDepositAsync(CreateDepositTransactionRequestDto request);
    Task<PagedResult<TransactionResponseDto>> GetTransactionsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize);
}
