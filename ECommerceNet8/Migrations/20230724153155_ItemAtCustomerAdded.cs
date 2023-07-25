using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class ItemAtCustomerAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemAtCustomers",
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
                    PricePaidPerItem = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAtCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAtCustomers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemAtCustomers_OrderId",
                table: "ItemAtCustomers",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAtCustomers");
        }
    }
}
