using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetWalletAPI.Infrastructure.Migrations
{
    public partial class AddedPrizeToBet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StakeCurrency",
                table: "Bets",
                newName: "Stake_Currency");

            migrationBuilder.RenameColumn(
                name: "StakeAmount",
                table: "Bets",
                newName: "Stake");

            migrationBuilder.AlterColumn<string>(
                name: "Stake_Currency",
                table: "Bets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<decimal>(
                name: "Prize",
                table: "Bets",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Prize_Currency",
                table: "Bets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prize",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "Prize_Currency",
                table: "Bets");

            migrationBuilder.RenameColumn(
                name: "Stake_Currency",
                table: "Bets",
                newName: "StakeCurrency");

            migrationBuilder.RenameColumn(
                name: "Stake",
                table: "Bets",
                newName: "StakeAmount");

            migrationBuilder.AlterColumn<string>(
                name: "StakeCurrency",
                table: "Bets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
