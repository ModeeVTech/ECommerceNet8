using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class OrderModifiedOneMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemExchangeRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderUniqueIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeUniqueIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentNumber = table.Column<int>(type: "int", nullable: true),
                    HouseNumber = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeConfirmedPdfInfoId = table.Column<int>(type: "int", nullable: false),
                    RequestClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemExchangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemExchangeRequests_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeConfirmedPdfInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemExchangeRequestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Added = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeConfirmedPdfInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeConfirmedPdfInfos_ItemExchangeRequests_ItemExchangeRequestId",
                        column: x => x.ItemExchangeRequestId,
                        principalTable: "ItemExchangeRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExchangeItemsCanceled",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemExchangeRequestId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerItemPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnedProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ReturnedProductVariantColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnedProductVariantSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CancellationReason = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeItemsCanceled", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeItemsCanceled_ItemExchangeRequests_ItemExchangeRequestId",
                        column: x => x.ItemExchangeRequestId,
                        principalTable: "ItemExchangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeItemsPending",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemExchangeRequestId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerItemPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnedProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ReturnedProductVariantColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnedProductVariantSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeItemsPending", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeItemsPending_ItemExchangeRequests_ItemExchangeRequestId",
                        column: x => x.ItemExchangeRequestId,
                        principalTable: "ItemExchangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemExchangeRequestId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerItemPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnedProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ReturnedProductVariantColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnedProductVariantSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangedProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ExchangedProductVariantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangedProductVariantSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeOrderItems_ItemExchangeRequests_ItemExchangeRequestId",
                        column: x => x.ItemExchangeRequestId,
                        principalTable: "ItemExchangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeConfirmedPdfInfos_ItemExchangeRequestId",
                table: "ExchangeConfirmedPdfInfos",
                column: "ItemExchangeRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeItemsCanceled_ItemExchangeRequestId",
                table: "ExchangeItemsCanceled",
                column: "ItemExchangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeItemsPending_ItemExchangeRequestId",
                table: "ExchangeItemsPending",
                column: "ItemExchangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrderItems_ItemExchangeRequestId",
                table: "ExchangeOrderItems",
                column: "ItemExchangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemExchangeRequests_OrderId",
                table: "ItemExchangeRequests",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeConfirmedPdfInfos");

            migrationBuilder.DropTable(
                name: "ExchangeItemsCanceled");

            migrationBuilder.DropTable(
                name: "ExchangeItemsPending");

            migrationBuilder.DropTable(
                name: "ExchangeOrderItems");

            migrationBuilder.DropTable(
                name: "ItemExchangeRequests");
        }
    }
}
