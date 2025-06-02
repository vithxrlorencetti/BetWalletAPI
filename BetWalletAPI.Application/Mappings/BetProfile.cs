using AutoMapper;
using BetWalletAPI.Application.DTOs.Bet;
using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Common.Mappings;

public class BetMappingProfile : Profile
{
    public BetMappingProfile()
    {
        CreateMap<Bet, BetResponseDto>()
            .ForMember(dest => dest.Stake, opt => opt.MapFrom(src => src.Stake.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Stake.Currency))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
