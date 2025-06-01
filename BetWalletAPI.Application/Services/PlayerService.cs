using AutoMapper;
using BetWalletAPI.Application.DTOs;
using BetWalletAPI.Application.DTOs.Player;
using BetWalletAPI.Application.Exceptions;
using BetWalletAPI.Application.Interfaces.Persistence;
using BetWalletAPI.Application.Interfaces.Security;
using BetWalletAPI.Application.Interfaces.Services;
using BetWalletAPI.Domain.Entities;
using BetWalletAPI.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BetWalletAPI.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<PlayerService> _logger;
        private readonly IMapper _mapper;

        public PlayerService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<PlayerService> logger,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PlayerResponseDto?> GetPlayerByIdAsync(Guid id)
        {
            var player = await _unitOfWork.PlayerRepository.GetByIdAsync(id);

            if (player == null)
            {
                throw new NotFoundException($"Player with ID '{id}' not found.");
            }

            return _mapper.Map<PlayerResponseDto>(player);
        }

        public async Task<PlayerResponseDto?> CreatePlayerAsync(CreatePlayerRequestDto registerPlayerDto)
        {
            _logger.LogInformation($"Attempting to register new player with email: {registerPlayerDto.Email}");

            if (registerPlayerDto == null)
            {
                _logger.LogError("RegisterPlayerDto cannot be null.");
                throw new ArgumentNullException(nameof(registerPlayerDto));
            }

            var existingPlayerByEmail = await _unitOfWork.PlayerRepository.GetByEmailAsync(registerPlayerDto.Email);
            if (existingPlayerByEmail != null)
            {
                _logger.LogWarning($"Registration attempt with existing email: {registerPlayerDto.Email}");
                throw new EmailAlreadyExistsException($"Email already exists: {registerPlayerDto.Email}");
            }

            var hashedPassword = _passwordHasher.Hash(registerPlayerDto.Password);

            var email = Email.Create(registerPlayerDto.Email);
            var initialBalance = Money.Create(registerPlayerDto.InitialBalance, registerPlayerDto.Currency);

            Player player;

            player = Player.Create(
                username: registerPlayerDto.Username,
                email: email,
                hashedPassword: hashedPassword,
                initialBalance: initialBalance
            );

            await _unitOfWork.PlayerRepository.AddAsync(player);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Player {player.Username} registered successfully with ID {player.Id}.");

            return _mapper.Map<PlayerResponseDto>(player);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginPlayerRequestDto loginPlayerDto)
        {
            _logger.LogInformation($"Login attempt for email: {loginPlayerDto.Email}");

            if (loginPlayerDto == null)
            {
                _logger.LogError("LoginPlayerDto cannot be null.");
                throw new ArgumentNullException(nameof(loginPlayerDto));
            }

            var player = await _unitOfWork.PlayerRepository.GetByEmailAsync(loginPlayerDto.Email);

            if (player == null)
            {
                throw new NotFoundException($"Player with Email '{loginPlayerDto.Email}' not found."); 
            }

            if (!_passwordHasher.VerifyPassword(player.PasswordHash, loginPlayerDto.Password))
            {
                throw new InvalidCredentialsException("Invalid credentials.");
            }

            _logger.LogInformation("Player {Username} (ID: {PlayerId}) authenticated successfully.", player.Username, player.Id);

            var token = _jwtTokenGenerator.GenerateToken(player);

            var playerDto = _mapper.Map<PlayerResponseDto>(player);

            return new AuthResponseDto(playerDto, token.token, token.expirationTime);   
        }
    }
}
