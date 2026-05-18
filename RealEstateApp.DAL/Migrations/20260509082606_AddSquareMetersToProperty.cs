using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSquareMetersToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SquareMeters",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SquareMeters",
                table: "Properties");
        }
    }
}
