using BetWalletAPI.Application.DTOs.Bet;
using BetWalletAPI.Application.Interfaces.Services;
using BetWalletAPI.API.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BetWalletAPI.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace BetWalletAPI.API.Controllers;

/// <summary>
/// Manages bet operations.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class BetController : ControllerBase
{
    private readonly IBetService _betService;

    public BetController(IBetService betService, ILogger<BetController> logger)
    {
        _betService = betService ?? throw new ArgumentNullException(nameof(betService));
    }

    /// <summary>
    /// Places a new bet for a player.
    /// </summary>
    /// <param name="placeBetDto">The bet placement request data.</param>
    /// <returns>The created bet's information.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Place a new bet",
        Description = "Places a new bet for the specified player with the given stake and description.",
        OperationId = "PlaceBet"
    )]
    [ProducesResponseType(typeof(BetResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PlaceBet([FromBody] PlaceBetRequestDto placeBetDto)
    {
        var bet = await _betService.PlaceBetAsync(placeBetDto);

        return StatusCode(StatusCodes.Status201Created, bet);
    }

    /// <summary>
    /// Retrieves a specific bet by its ID.
    /// </summary>
    /// <param name="betId">The ID of the bet to retrieve.</param>
    /// <returns>The bet information.</returns>
    [HttpGet("{betId:guid}")]
    [SwaggerOperation(
        Summary = "Get bet by ID",
        Description = "Retrieves detailed information about a specific bet.",
        OperationId = "GetBetById"
    )]
    [ProducesResponseType(typeof(BetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBetById(Guid betId)
    {
        var bet = await _betService.GetBetByIdAsync(betId);

        return Ok(bet);
    }

    /// <summary>
    /// Retrieves all bets for a specific player, with pagination.
    /// </summary>
    /// <param name="playerId">The ID of the player whose bets are to be retrieved.</param>
    /// <param name="pageNumber">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of bets per page (default is 10).</param>
    /// <returns>A paginated list of bets for the player.</returns>
    [HttpGet("player/{playerId:guid}")]
    [SwaggerOperation(
        Summary = "Get bets by player ID",
        Description = "Retrieves a paginated list of all bets placed by a specific player.",
        OperationId = "GetBetsByPlayer"
    )]
    [ProducesResponseType(typeof(PagedResult<BetResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)] // Player não encontrado
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBetsByPlayer(Guid playerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var pagedResult = await _betService.GetBetsByPlayerAsync(playerId, pageNumber, pageSize);

        return Ok(pagedResult);
    }

    /// <summary>
    /// Settles a pending bet as WON.
    /// </summary>
    /// <remarks>The prize is typically calculated by the service (e.g., 2x stake).</remarks>
    /// <param name="betId">The ID of the bet to settle as WON.</param>
    /// <returns>The updated bet information.</returns>
    [HttpPost("{betId:guid}/settle-won")]
    [SwaggerOperation(
        Summary = "Settle a bet as WON",
        Description = "Marks a bet as won and processes any winnings.",
        OperationId = "SettleBetAsWon"
    )]
    [ProducesResponseType(typeof(BetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SettleBetAsWon(Guid betId)
    {
        var betResponse = await _betService.SettleBetAsWonAsync(betId);

        return Ok(betResponse);
    }

    /// <summary>
    /// Settles a pending bet as LOST.
    /// </summary>
    /// <remarks>This may trigger bonus calculations if applicable.</remarks>
    /// <param name="betId">The ID of the bet to settle as LOST.</param>
    /// <returns>The updated bet information.</returns>
    [HttpPost("{betId:guid}/settle-lost")]
    [SwaggerOperation(
        Summary = "Settle a bet as LOST",
        Description = "Marks a bet as lost and handles related logic like bonus eligibility.",
        OperationId = "SettleBetAsLost"
    )]
    [ProducesResponseType(typeof(BetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)] // Bet, Player ou Wallet não encontrado
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)] // Bet não está pendente
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SettleBetAsLost(Guid betId)
    {
        var betResponse = await _betService.SettleBetAsLostAsync(betId);

        return Ok(betResponse);
    }

    /// <summary>
    /// Cancels a pending bet.
    /// </summary>
    /// <remarks>
    /// A bet can typically only be cancelled if it's still in a pending state and meets cancellation criteria.
    /// The stake is usually refunded to the player's wallet.
    /// </remarks>
    /// <param name="betId">The ID of the bet to cancel.</param>
    /// <returns>The updated bet information, now marked as cancelled.</returns>
    [HttpDelete("{betId:guid}/cancel")]
    [SwaggerOperation(
        Summary = "Cancel a bet",
        Description = "Cancels a pending bet and refunds the stake.",
        OperationId = "CancelBet"
    )]
    [ProducesResponseType(typeof(BetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelBet(Guid betId)
    {
        var betResponse = await _betService.CancelBetAsync(betId);

        return Ok(betResponse);
    }
}