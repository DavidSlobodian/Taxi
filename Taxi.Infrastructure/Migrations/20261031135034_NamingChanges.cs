using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NamingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PickupAltitude",
                table: "Rides",
                newName: "PickupLatitude");

            migrationBuilder.RenameColumn(
                name: "DestinationAltitude",
                table: "Rides",
                newName: "DestinationLatitude");

            migrationBuilder.UpdateData(
                table: "GeneralInfos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9768));

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9928));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9902));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9905));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9907));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9908));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 31, 13, 50, 34, 425, DateTimeKind.Utc).AddTicks(9909));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PickupLatitude",
                table: "Rides",
                newName: "PickupAltitude");

            migrationBuilder.RenameColumn(
                name: "DestinationLatitude",
                table: "Rides",
                newName: "DestinationAltitude");

            migrationBuilder.UpdateData(
                table: "GeneralInfos",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2513));

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2670));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2641));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2644));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2645));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2646));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 27, 18, 59, 12, 595, DateTimeKind.Utc).AddTicks(2647));
        }
    }
}
