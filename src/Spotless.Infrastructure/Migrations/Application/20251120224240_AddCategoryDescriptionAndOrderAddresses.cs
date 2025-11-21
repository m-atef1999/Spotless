using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotless.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddCategoryDescriptionAndOrderAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PickupAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");
        }
    }
}
