using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetWalletAPI.Infrastructure.Migrations
{
    public partial class RemoveBetCurrencyColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prize_Currency",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "Stake_Currency",
                table: "Bets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prize_Currency",
                table: "Bets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Stake_Currency",
                table: "Bets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
