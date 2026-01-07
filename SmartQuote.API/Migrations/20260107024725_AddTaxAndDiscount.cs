using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartQuote.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxAndDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountPercent",
                table: "Quotations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TaxPercent",
                table: "Quotations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "TaxPercent",
                table: "Quotations");
        }
    }
}
