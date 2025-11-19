using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotless.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class CodeRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ExternalGateway",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalTransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ExternalGateway",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExternalTransactionId",
                table: "Payments");
        }
    }
}
