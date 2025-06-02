// BetWalletAPI.Application/Services/BetService.cs
using AutoMapper;
using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.DTOs.Bet;
using BetWalletAPI.Application.Exceptions; 
using BetWalletAPI.Application.Interfaces.Persistence;
using BetWalletAPI.Application.Interfaces.Services;
using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Domain.Enums;
using BetWalletAPI.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BetWalletAPI.Application.Services;

public class BetService : IBetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BetService> _logger;
    private readonly IMapper _mapper;

    public BetService(IUnitOfWork unitOfWork, ILogger<BetService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<BetResponseDto?> PlaceBetAsync(PlaceBetRequestDto placeBetRequestDto)
    {
        _logger.LogInformation($"Attempting to place new bet '{placeBetRequestDto.Description}' for Player ID '{placeBetRequestDto.PlayerId}' with stake: {placeBetRequestDto.Stake}");

        var player = await _unitOfWork.PlayerRepository.GetByIdAsync(placeBetRequestDto.PlayerId);
        if (player == null)
        {
            _logger.LogWarning($"Player with ID '{placeBetRequestDto.PlayerId}' not found for PlaceBetAsync.");
            throw new NotFoundException($"Player with ID '{placeBetRequestDto.PlayerId}' not found.");
        }

        var wallet = await _unitOfWork.WalletRepository.GetByPlayerIdAsync(player.Id);
        if (wallet == null)
        {
            _logger.LogWarning($"Wallet not found for Player ID '{player.Id}' for PlaceBetAsync.");
            throw new NotFoundException($"Wallet not found for Player ID '{player.Id}'.");
        }

        if (wallet.Balance.Amount < placeBetRequestDto.Stake)
        {
            var errorMessage = $"Insufficient funds for Player ID '{player.Id}'. Requested: {placeBetRequestDto.Stake}, Available: {wallet.Balance.Amount}.";
            _logger.LogWarning(errorMessage);
            throw new InsufficientBalanceException(errorMessage);
        }

        var stake = Money.Create(placeBetRequestDto.Stake, wallet.Balance.Currency);
        var bet = Bet.Create(player.Id, stake, placeBetRequestDto.Description);

        var debitTransaction = wallet.Debit(stake, TransactionType.BetPlacement, $"Bet placement: {bet.Id}");

        bet.SetPlacementTransaction(debitTransaction.Id);

        await _unitOfWork.BetRepository.AddAsync(bet);

        debitTransaction.SetReferenceBet(bet.Id);

        await _unitOfWork.TransactionRepository.AddAsync(debitTransaction);
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bet {BetId} placed successfully for Player ID {PlayerId}.", bet.Id, player.Id);

        return _mapper.Map<BetResponseDto>(bet);
    }

    public async Task<BetResponseDto?> SettleBetAsWonAsync(Guid betId)
    {
        _logger.LogInformation("Attempting to settle Bet ID '{BetId}' as WON", betId);

        var bet = await _unitOfWork.BetRepository.GetByIdAsync(betId);
        if (bet == null)
        {
            _logger.LogWarning("Bet with ID '{BetId}' not found for SettleBetAsWonAsync.", betId);
            throw new NotFoundException($"Bet with ID '{betId}' not found.");
        }

        if (bet.IsSettled)
        {
            var errorMessage = $"Bet with ID '{betId}' cannot be settled as WON. Current status: {bet.Status}. Expected: {BetStatus.Pending}.";
            _logger.LogWarning(errorMessage);
            throw new InvalidBetStatusException(errorMessage);
        }

        var wallet = await _unitOfWork.WalletRepository.GetByPlayerIdAsync(bet.PlayerId);
        if (wallet == null)
        {
            _logger.LogWarning("Wallet not found for Player ID '{PlayerId}' associated with Bet ID '{BetId}' for SettleBetAsWonAsync.", bet.PlayerId, betId);
            throw new NotFoundException($"Wallet not found for Player ID '{bet.PlayerId}' associated with Bet ID '{betId}'.");
        }

        var prizeMoney = Money.Create(bet.Stake.Multiply(2), bet.Stake.Currency);
        var creditTransaction = wallet.Credit(prizeMoney, TransactionType.BetWinnings, $"Winnings for bet: {bet.Id}");
        bet.SettleAsWon(creditTransaction.Id, prizeMoney);

        await _unitOfWork.TransactionRepository.AddAsync(creditTransaction);
        await _unitOfWork.BetRepository.UpdateAsync(bet);
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bet {BetId} settled as WON for Player ID {PlayerId}.", bet.Id, bet.PlayerId);

        return _mapper.Map<BetResponseDto>(bet);
    }

    public async Task<BetResponseDto?> SettleBetAsLostAsync(Guid betId)
    {
        _logger.LogInformation("Attempting to settle Bet ID '{BetId}' as LOST.", betId);

        var bet = await _unitOfWork.BetRepository.GetByIdAsync(betId);
        if (bet == null)
        {
            _logger.LogWarning("Bet with ID '{BetId}' not found for SettleBetAsLostAsync.", betId);
            throw new NotFoundException($"Bet with ID '{betId}' not found.");
        }

        if (bet.IsSettled)
        {
            var errorMessage = $"Bet with ID '{betId}' cannot be settled as LOST. Current status: {bet.Status}. Expected: {BetStatus.Pending}.";
            _logger.LogWarning(errorMessage);
            throw new InvalidBetStatusException(errorMessage);
        }

        var player = await _unitOfWork.PlayerRepository.GetByIdAsync(bet.PlayerId);
        if (player == null)
        {
            _logger.LogError("Player with ID '{PlayerId}' not found for Bet ID '{BetId}' during SettleBetAsLostAsync. Data integrity issue.", bet.PlayerId, betId);
            throw new NotFoundException($"Player with ID '{bet.PlayerId}' not found, associated with Bet ID '{betId}'.");
        }

        var wallet = await _unitOfWork.WalletRepository.GetByPlayerIdAsync(bet.PlayerId);
        if (wallet == null)
        {
            _logger.LogError("Wallet not found for Player ID '{PlayerId}' (Bet ID '{BetId}') during SettleBetAsLostAsync. Data integrity issue.", bet.PlayerId, betId);
            throw new NotFoundException($"Wallet not found for Player ID '{bet.PlayerId}', associated with Bet ID '{betId}'.");
        }

        bet.SettleAsLost();
        await _unitOfWork.BetRepository.UpdateAsync(bet);

        player.IncrementConsecutiveLosses();

        if (player.IsEligibleForBonus)
        {
            var lastFiveBetsAmount = await _unitOfWork.BetRepository.GetLastFiveAmountByPlayerIdAsync(player.Id);

            var bonusMoney = lastFiveBetsAmount.Multiply(0.10m);

            var bonusTransaction = wallet.RecordBonus(bonusMoney, $"Bonus for 5 loss streak: {bet.Id}");

            await _unitOfWork.TransactionRepository.AddAsync(bonusTransaction);

            _logger.LogInformation("Lost bet bonus of {BonusAmount} {BonusCurrency} awarded to Player ID '{PlayerId}' for Bet ID '{BetId}'.",
                bonusTransaction.Amount.Amount, bonusTransaction.Amount.Currency, player.Id, betId);
        }

        await _unitOfWork.PlayerRepository.UpdateAsync(player);
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bet {BetId} settled as LOST for Player ID {PlayerId}.", bet.Id, bet.PlayerId);

        return _mapper.Map<BetResponseDto>(bet);
    }

    public async Task<BetResponseDto?> CancelBetAsync(Guid betId)
    {
        _logger.LogInformation("Attempting to cancel Bet ID '{BetId}'.", betId);

        var bet = await _unitOfWork.BetRepository.GetByIdAsync(betId);
        if (bet == null)
        {
            _logger.LogWarning("Bet with ID '{BetId}' not found for CancelBetAsync.", betId);
            throw new NotFoundException($"Bet with ID '{betId}' not found.");
        }

        if (!bet.CanBeCancelled)
        {
            var errorMessage = $"Bet with ID '{betId}' cannot be cancelled. Current status: {bet.Status}.";
            _logger.LogWarning(errorMessage);
            throw new InvalidBetStatusException(errorMessage);
        }

        var wallet = await _unitOfWork.WalletRepository.GetByPlayerIdAsync(bet.PlayerId);
        if (wallet == null)
        {
            _logger.LogWarning("Wallet not found for Player ID '{PlayerId}' associated with Bet ID '{BetId}' for CancelBetAsync.", bet.PlayerId, betId);
            throw new NotFoundException($"Wallet not found for Player ID '{bet.PlayerId}' associated with Bet ID '{betId}'.");
        }

        var refundTransaction = wallet.RecordRefund(bet.Stake, $"Refund for cancelled bet: {bet.Id}");

        bet.Cancel(refundTransaction.Id); 

        await _unitOfWork.TransactionRepository.AddAsync(refundTransaction);
        await _unitOfWork.BetRepository.UpdateAsync(bet);
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bet {BetId} CANCELLED for Player ID {PlayerId}.", bet.Id, bet.PlayerId);

        return _mapper.Map<BetResponseDto>(bet);
    }

    public async Task<BetResponseDto?> GetBetByIdAsync(Guid betId)
    {
        _logger.LogInformation("Attempting to retrieve Bet by ID '{BetId}'.", betId);
        var bet = await _unitOfWork.BetRepository.GetByIdAsync(betId);

        if (bet == null)
        {
            _logger.LogWarning("Bet with ID '{BetId}' not found for GetBetByIdAsync.", betId);
            throw new NotFoundException($"Bet with ID '{betId}' not found.");
        }

        _logger.LogInformation("Bet with ID '{BetId}' retrieved successfully.", betId);

        return _mapper.Map<BetResponseDto>(bet);
    }

    public async Task<PagedResult<BetResponseDto>> GetBetsByPlayerAsync(Guid playerId, int pageNumber, int pageSize)
    {
        _logger.LogInformation("Attempting to retrieve bets for Player ID '{PlayerId}', Page: {PageNumber}, Size: {PageSize}.", playerId, pageNumber, pageSize);

        var player = await _unitOfWork.PlayerRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            _logger.LogWarning("Player with ID '{PlayerId}' not found for GetBetsByPlayerAsync.", playerId);
            throw new NotFoundException($"Player with ID '{playerId}' not found.");
        }

        var pagedResult = await _unitOfWork.BetRepository.GetByPlayerIdAsync(playerId, pageNumber, pageSize);

        _logger.LogInformation("Retrieved {BetCount} bets for Player ID '{PlayerId}'.", pagedResult.Items.Count(), playerId);

        var itemsDto = pagedResult.Items.Select(bet => _mapper.Map<BetResponseDto>(bet)).ToList();

        var pagedResultDto = new PagedResult<BetResponseDto>(
            itemsDto,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize
        );

        return pagedResultDto;
    }
}