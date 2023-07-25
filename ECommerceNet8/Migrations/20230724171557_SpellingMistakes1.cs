using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class SpellingMistakes1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderUniqueIdentfier",
                table: "Orders",
                newName: "OrderUniqueIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderUniqueIdentifier",
                table: "Orders",
                newName: "OrderUniqueIdentfier");
        }
    }
}
