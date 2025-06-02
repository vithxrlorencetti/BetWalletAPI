using BetWalletAPI.API.Middlewares;
using BetWalletAPI.Application.Common;
using BetWalletAPI.Application.DTOs.Transaction;
using BetWalletAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BetWalletAPI.API.Controllers;

/// <summary>
/// Manages financial transactions.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
    }

    /// <summary>
    /// Creates a new deposit transaction for a player.
    /// </summary>
    /// <param name="request">The deposit transaction request data.</param>
    /// <returns>The created transaction's information.</returns>
    [HttpPost("deposit")]
    [SwaggerOperation(
        Summary = "Create a deposit transaction",
        Description = "Creates a new deposit transaction for the specified player.",
        OperationId = "CreateDepositTransaction"
    )]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDeposit([FromBody] CreateDepositTransactionRequestDto request)
    {
        var transactionResponse = await _transactionService.CreateDepositAsync(request);
        
        return StatusCode(StatusCodes.Status201Created, transactionResponse);
    }

    /// <summary>
    /// Retrieves all transactions for a specific player, with pagination.
    /// </summary>
    /// <param name="playerId">The ID of the player whose transactions are to be retrieved.</param>
    /// <param name="pageNumber">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of transactions per page (default is 10, max 100).</param>
    /// <returns>A paginated list of transactions for the player.</returns>
    [HttpGet("player/{playerId:guid}")]
    [SwaggerOperation(
        Summary = "Get transactions by player ID",
        Description = "Retrieves a paginated list of all transactions for a specific player.",
        OperationId = "GetTransactionsByPlayer"
    )]
    [ProducesResponseType(typeof(PagedResult<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTransactionsByPlayer(Guid playerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var pagedResult = await _transactionService.GetTransactionsByPlayerIdAsync(playerId, pageNumber, pageSize);

        return Ok(pagedResult);
    }
}
