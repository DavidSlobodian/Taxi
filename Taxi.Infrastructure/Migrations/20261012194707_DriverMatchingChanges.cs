using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DriverMatchingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceMargin",
                table: "Tarrifs",
                newName: "BasePricePerKm");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextSearchAtUtc",
                table: "Rides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RideClass",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SearchRadiusMeters",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 12, 19, 47, 7, 678, DateTimeKind.Utc).AddTicks(4518));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 12, 19, 47, 7, 678, DateTimeKind.Utc).AddTicks(4392));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 12, 19, 47, 7, 678, DateTimeKind.Utc).AddTicks(4404));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 12, 19, 47, 7, 678, DateTimeKind.Utc).AddTicks(4405));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 12, 19, 47, 7, 678, DateTimeKind.Utc).AddTicks(4406));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 12, 19, 47, 7, 678, DateTimeKind.Utc).AddTicks(4407));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextSearchAtUtc",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RideClass",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "SearchRadiusMeters",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "BasePricePerKm",
                table: "Tarrifs",
                newName: "PriceMargin");

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1313));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1205));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1214));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1215));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1216));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1217));
        }
    }
}
