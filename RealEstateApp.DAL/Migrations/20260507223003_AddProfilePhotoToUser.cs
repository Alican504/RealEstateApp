using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePhotoToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePhoto",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoType",
                table: "Users");
        }
    }
}
