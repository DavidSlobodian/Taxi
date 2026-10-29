using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActiveVehicleAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActiveVehicleId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_ActiveVehicleId",
                table: "Drivers",
                column: "ActiveVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Vehicles_ActiveVehicleId",
                table: "Drivers",
                column: "ActiveVehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Vehicles_ActiveVehicleId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_ActiveVehicleId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "ActiveVehicleId",
                table: "Drivers");
        }
    }
}
