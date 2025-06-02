using BetWalletAPI.Application.DTOs.Player;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BetWalletAPI.API.Middlewares;
using BetWalletAPI.Application.Interfaces.Services;

namespace BetWalletAPI.API.Controllers
{
    /// <summary>
    /// Manages player operations including registration and login.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService, ILogger<PlayerController> logger)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// Creates a new player account.
        /// </summary>
        /// <remarks>Enten
        /// Registers a new player with the provided details and initializes their wallet
        /// with a specified currency and initial balance.
        /// </remarks>
        /// <param name="createPlayerDto">The player creation request data.</param>
        /// <returns>The created player's information including wallet details.</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new player",
            Description = "Registers a new player and initializes their wallet.",
            OperationId = "CreatePlayer"
        )]
        [ProducesResponseType(typeof(PlayerResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequestDto createPlayerDto)
        {
            var playerResponse = await _playerService.CreatePlayerAsync(createPlayerDto);

            return StatusCode(StatusCodes.Status201Created, playerResponse);
        }

        /// <summary>
        /// Logs in an existing player.
        /// </summary>
        /// <remarks>
        /// Authenticates a player using their email and password.
        /// Returns player information and wallet balance upon successful login.
        /// </remarks>
        /// <param name="loginPlayerDto">The player login request data.</param>
        /// <returns>Player information and wallet balance if authentication is successful.</returns>
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login a player",
            Description = "Authenticates a player and returns their information.",
            OperationId = "LoginPlayer"
        )]
        [ProducesResponseType(typeof(PlayerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginPlayer([FromBody] LoginPlayerRequestDto loginPlayerDto)
        {
            var authResponse = await _playerService.LoginAsync(loginPlayerDto);

            return Ok(authResponse);
        }
    }
}
