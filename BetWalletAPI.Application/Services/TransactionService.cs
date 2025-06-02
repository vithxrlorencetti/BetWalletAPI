using AutoMapper;
using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.DTOs.Bet;
using BetWalletAPI.Application.DTOs.Transaction;
using BetWalletAPI.Application.Exceptions;
using BetWalletAPI.Application.Interfaces.Persistence;
using BetWalletAPI.Application.Interfaces.Services;
using BetWalletAPI.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BetWalletAPI.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Injetar IMapper
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TransactionResponseDto> CreateDepositAsync(CreateDepositTransactionRequestDto request)
    {
        var player = await _unitOfWork.PlayerRepository.GetByIdAsync(request.PlayerId);
        if (player == null)
        {
            _logger.LogWarning("Player not found for ID: {PlayerId}", request.PlayerId);
            throw new NotFoundException($"Player with ID '{request.PlayerId}' not found.");
        }

        var wallet = await _unitOfWork.WalletRepository.GetByPlayerIdAsync(player.Id);
        if (wallet == null)
        {
            _logger.LogError("Wallet not found for Player ID '{PlayerId}'. Data integrity issue.", player.Id);
            throw new NotFoundException($"Wallet not found for Player ID '{player.Id}'.");
        }

        var transactionAmount = Money.Create(request.Amount, wallet.Balance.Currency);

        var transaction = wallet.RecordDeposit(transactionAmount);

        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deposit transaction created successfully for PlayerId: {PlayerId}, TransactionId: {TransactionId}", request.PlayerId, transaction.Id);

        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task<PagedResult<TransactionResponseDto>> GetTransactionsByPlayerIdAsync(Guid playerId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("Attempting to retrieve transactions for Player ID '{PlayerId}', Page: {PageNumber}, Size: {PageSize}.", playerId, pageNumber, pageSize);

        var player = await _unitOfWork.PlayerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            _logger.LogWarning("Player not found for ID: {PlayerId} when fetching transactions.", playerId);
            throw new NotFoundException($"Player with ID '{playerId}' not found.");
        }

        var pagedResult = await _unitOfWork.TransactionRepository.GetByWalletIdAsync(player.Wallet.Id, pageNumber, pageSize);

        _logger.LogInformation("Fetched {Count} transactions for PlayerId: {PlayerId}, WalletId: {WalletId}", pagedResult.Items.Count, playerId, player.Wallet.Id);

        var itemsDto = pagedResult.Items.Select(transaction => _mapper.Map<TransactionResponseDto>(transaction)).ToList();

        var pagedResultDto = new PagedResult<TransactionResponseDto>(
            itemsDto,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize
        );

        return pagedResultDto;
    }
}
