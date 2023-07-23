using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class OrderModels1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNumber = table.Column<int>(type: "int", nullable: false),
                    AppartmentNumber = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderUniqueIdentfier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppartmentNumber = table.Column<int>(type: "int", nullable: true),
                    HouseNumber = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    OrderFromCustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "ShippingTypes",
                columns: table => new
                {
                    ShippingTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingFirmName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreeTier = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTypes", x => x.ShippingTypeId);
                });

            migrationBuilder.CreateTable(
                name: "OrdersFromCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShippingTypeId = table.Column<int>(type: "int", nullable: true),
                    ShippingFirmName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PdfInfoId = table.Column<int>(type: "int", nullable: false),
                    OrderCanceled = table.Column<bool>(type: "bit", nullable: false),
                    RefundMade = table.Column<bool>(type: "bit", nullable: false),
                    ItemSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersFromCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersFromCustomers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "returnedItemsFromCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_returnedItemsFromCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_returnedItemsFromCustomers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdersItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderFromCustomerId = table.Column<int>(type: "int", nullable: false),
                    BaseProductId = table.Column<int>(type: "int", nullable: false),
                    BaseProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductVariantSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    PricePerItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrdersItems_OrdersFromCustomers_OrderFromCustomerId",
                        column: x => x.OrderFromCustomerId,
                        principalTable: "OrdersFromCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PdfInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderFromCustomerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Added = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfInfos_OrdersFromCustomers_OrderFromCustomerId",
                        column: x => x.OrderFromCustomerId,
                        principalTable: "OrdersFromCustomers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdersFromCustomers_OrderId",
                table: "OrdersFromCustomers",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdersItems_OrderFromCustomerId",
                table: "OrdersItems",
                column: "OrderFromCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PdfInfos_OrderFromCustomerId",
                table: "PdfInfos",
                column: "OrderFromCustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_returnedItemsFromCustomers_OrderId",
                table: "returnedItemsFromCustomers",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "OrdersItems");

            migrationBuilder.DropTable(
                name: "PdfInfos");

            migrationBuilder.DropTable(
                name: "returnedItemsFromCustomers");

            migrationBuilder.DropTable(
                name: "ShippingTypes");

            migrationBuilder.DropTable(
                name: "OrdersFromCustomers");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
