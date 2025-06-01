using BetWalletAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetWalletAPI.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.OwnsOne(w => w.Balance, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(w => w.PlayerId)
            .IsRequired();
        builder.HasIndex(w => w.PlayerId)
            .IsUnique();

        builder.HasMany(w => w.Transactions)
            .WithOne() 
            .HasForeignKey(t => t.WalletId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(w => w.CreatedAt).IsRequired();
        builder.Property(w => w.UpdatedAt).IsRequired();
    }
}
