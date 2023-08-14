using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class ExchangeItemModelsDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "ExchangeItemsCanceled");

            migrationBuilder.RenameColumn(
                name: "ExchangedProductVariantName",
                table: "ExchangeOrderItems",
                newName: "ExchangedProductVariantColor");

            migrationBuilder.AddColumn<string>(
                name: "CancelationReason",
                table: "ExchangeItemsCanceled",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelationReason",
                table: "ExchangeItemsCanceled");

            migrationBuilder.RenameColumn(
                name: "ExchangedProductVariantColor",
                table: "ExchangeOrderItems",
                newName: "ExchangedProductVariantName");

            migrationBuilder.AddColumn<int>(
                name: "CancellationReason",
                table: "ExchangeItemsCanceled",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
