using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "FullName");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_UserId",
                table: "Properties",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Users_UserId",
                table: "Properties",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Users_UserId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_UserId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Users",
                newName: "Role");
        }
    }
}
