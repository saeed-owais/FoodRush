using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodRush.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantSuspensionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                schema: "Restaurants",
                table: "Restaurants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                schema: "Restaurants",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                schema: "Restaurants",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                schema: "Restaurants",
                table: "Restaurants");
        }
    }
}
