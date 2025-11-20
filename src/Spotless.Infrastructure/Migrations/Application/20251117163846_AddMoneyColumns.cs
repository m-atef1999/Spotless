using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotless.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddMoneyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TimeSlot_TimeSlotId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSlot",
                table: "TimeSlot");

            migrationBuilder.RenameTable(
                name: "TimeSlot",
                newName: "TimeSlots");

            migrationBuilder.RenameColumn(
                name: "PricePerUnitCurrency",
                table: "Services",
                newName: "PricePerUnit_Currency");

            migrationBuilder.RenameColumn(
                name: "PricePerUnitAmount",
                table: "Services",
                newName: "PricePerUnit_Amount");

            migrationBuilder.RenameColumn(
                name: "BasePriceCurrency",
                table: "Services",
                newName: "BasePrice_Currency");

            migrationBuilder.RenameColumn(
                name: "BasePriceAmount",
                table: "Services",
                newName: "BasePrice_Amount");

            migrationBuilder.RenameColumn(
                name: "PriceCurrency",
                table: "Categories",
                newName: "Price_Currency");

            migrationBuilder.RenameColumn(
                name: "PriceAmount",
                table: "Categories",
                newName: "Price_Amount");

            migrationBuilder.AlterColumn<string>(
                name: "PricePerUnit_Currency",
                table: "Services",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "BasePrice_Currency",
                table: "Services",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "Price_Currency",
                table: "Categories",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSlots",
                table: "TimeSlots",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TimeSlots_TimeSlotId",
                table: "Orders",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TimeSlots_TimeSlotId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSlots",
                table: "TimeSlots");

            migrationBuilder.RenameTable(
                name: "TimeSlots",
                newName: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "PricePerUnit_Currency",
                table: "Services",
                newName: "PricePerUnitCurrency");

            migrationBuilder.RenameColumn(
                name: "PricePerUnit_Amount",
                table: "Services",
                newName: "PricePerUnitAmount");

            migrationBuilder.RenameColumn(
                name: "BasePrice_Currency",
                table: "Services",
                newName: "BasePriceCurrency");

            migrationBuilder.RenameColumn(
                name: "BasePrice_Amount",
                table: "Services",
                newName: "BasePriceAmount");

            migrationBuilder.RenameColumn(
                name: "Price_Currency",
                table: "Categories",
                newName: "PriceCurrency");

            migrationBuilder.RenameColumn(
                name: "Price_Amount",
                table: "Categories",
                newName: "PriceAmount");

            migrationBuilder.AlterColumn<string>(
                name: "PricePerUnitCurrency",
                table: "Services",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "BasePriceCurrency",
                table: "Services",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "PriceCurrency",
                table: "Categories",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSlot",
                table: "TimeSlot",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TimeSlot_TimeSlotId",
                table: "Orders",
                column: "TimeSlotId",
                principalTable: "TimeSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
