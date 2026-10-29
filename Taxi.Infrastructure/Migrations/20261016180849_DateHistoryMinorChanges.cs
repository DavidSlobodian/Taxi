using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DateHistoryMinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CancellationReason",
                table: "Rides",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAtUtc",
                table: "Rides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 18, 8, 48, 747, DateTimeKind.Utc).AddTicks(261));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 18, 8, 48, 747, DateTimeKind.Utc).AddTicks(72));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 18, 8, 48, 747, DateTimeKind.Utc).AddTicks(84));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 18, 8, 48, 747, DateTimeKind.Utc).AddTicks(85));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 18, 8, 48, 747, DateTimeKind.Utc).AddTicks(86));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 18, 8, 48, 747, DateTimeKind.Utc).AddTicks(87));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "FinishedAtUtc",
                table: "Rides");

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 9, 2, 28, 745, DateTimeKind.Utc).AddTicks(6450));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 9, 2, 28, 745, DateTimeKind.Utc).AddTicks(6304));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 9, 2, 28, 745, DateTimeKind.Utc).AddTicks(6314));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 9, 2, 28, 745, DateTimeKind.Utc).AddTicks(6315));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 9, 2, 28, 745, DateTimeKind.Utc).AddTicks(6316));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 16, 9, 2, 28, 745, DateTimeKind.Utc).AddTicks(6335));
        }
    }
}
