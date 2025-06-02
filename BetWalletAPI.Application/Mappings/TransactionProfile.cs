using AutoMapper;
using BetWalletAPI.Application.DTOs.Transaction;
using BetWalletAPI.Domain.Entities;

namespace BetWalletAPI.Application.Common.Mappings;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionResponseDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}
