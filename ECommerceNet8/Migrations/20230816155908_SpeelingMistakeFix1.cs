using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceNet8.Migrations
{
    /// <inheritdoc />
    public partial class SpeelingMistakeFix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "totalAmoundRefunded",
                table: "ItemReturnRequests",
                newName: "totalAmountRefunded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "totalAmountRefunded",
                table: "ItemReturnRequests",
                newName: "totalAmoundRefunded");
        }
    }
}
