using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodRush.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropUniqueConstrainsFromEmailAndPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                schema: "identity",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UserPermissions",
                newName: "UserPermissions",
                newSchema: "identity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserPermissions",
                schema: "identity",
                newName: "UserPermissions");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "identity",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }
    }
}
