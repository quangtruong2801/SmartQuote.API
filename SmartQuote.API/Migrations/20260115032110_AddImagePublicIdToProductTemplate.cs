using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartQuote.API.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePublicIdToProductTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "ProductTemplates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "ProductTemplates");
        }
    }
}
