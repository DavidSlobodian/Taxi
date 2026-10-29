using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CancelationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAtUtc",
                table: "Rides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivedAtUtc",
                table: "Rides",
                type: "datetime2",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAtUtc",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ArrivedAtUtc",
                table: "Rides");

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
    }
}
