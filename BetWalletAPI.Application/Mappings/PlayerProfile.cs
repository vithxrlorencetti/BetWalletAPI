using AutoMapper;
using BetWalletAPI.Application.DTOs.Player;
using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Mappings
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerResponseDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Wallet.Balance.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Wallet.Balance.Currency));
        }
    }
}
