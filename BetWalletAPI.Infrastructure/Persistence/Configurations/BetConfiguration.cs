using BetWalletAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetWalletAPI.Infrastructure.Persistence.Configurations;

public class BetConfiguration : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.ToTable("Bets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.PlayerId)
            .IsRequired();

        builder.OwnsOne(b => b.Stake, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("Stake")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            moneyBuilder.Ignore(m => m.Currency);
        });

        builder.OwnsOne(b => b.Prize, prizeBuilder =>
        {
            prizeBuilder.Property(m => m.Amount)
                .HasColumnName("Prize")
                .HasColumnType("decimal(18,2)");

            prizeBuilder.Ignore(m => m.Currency);
        });

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt);
    }
}
