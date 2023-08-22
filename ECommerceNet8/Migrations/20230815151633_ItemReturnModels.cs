using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class ItemReturnModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemReturnRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderUniqueIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeUniqueIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserBankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserBankAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRequestTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    totalRequestForRefund = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalAmountNotRefunded = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    totalAmoundRefunded = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequestRefunded = table.Column<bool>(type: "bit", nullable: false),
                    RequestClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemReturnRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemReturnRequests_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsBadForRefund",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemReturnRequestId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ProductColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePaidPerItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReasonForNotRefunding = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsBadForRefund", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsBadForRefund_ItemReturnRequests_ItemReturnRequestId",
                        column: x => x.ItemReturnRequestId,
                        principalTable: "ItemReturnRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsGoodForRefund",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemReturnRequestId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ProductColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePaidPerItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsGoodForRefund", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsGoodForRefund_ItemReturnRequests_ItemReturnRequestId",
                        column: x => x.ItemReturnRequestId,
                        principalTable: "ItemReturnRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemReturnRequests_OrderId",
                table: "ItemReturnRequests",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsBadForRefund_ItemReturnRequestId",
                table: "ItemsBadForRefund",
                column: "ItemReturnRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsGoodForRefund_ItemReturnRequestId",
                table: "ItemsGoodForRefund",
                column: "ItemReturnRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsBadForRefund");

            migrationBuilder.DropTable(
                name: "ItemsGoodForRefund");

            migrationBuilder.DropTable(
                name: "ItemReturnRequests");
        }
    }
}
