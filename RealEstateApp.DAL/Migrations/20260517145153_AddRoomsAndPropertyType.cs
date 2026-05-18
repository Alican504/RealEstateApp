using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomsAndPropertyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoomCount",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "RoomCount",
                table: "Properties");
        }
    }
}
