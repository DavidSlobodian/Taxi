using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RatingUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 51, 10, 839, DateTimeKind.Utc).AddTicks(644));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 51, 10, 839, DateTimeKind.Utc).AddTicks(512));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 51, 10, 839, DateTimeKind.Utc).AddTicks(524));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 51, 10, 839, DateTimeKind.Utc).AddTicks(526));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 51, 10, 839, DateTimeKind.Utc).AddTicks(527));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 51, 10, 839, DateTimeKind.Utc).AddTicks(528));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 33, 1, 51, DateTimeKind.Utc).AddTicks(5207));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 33, 1, 51, DateTimeKind.Utc).AddTicks(5098));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 33, 1, 51, DateTimeKind.Utc).AddTicks(5109));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 33, 1, 51, DateTimeKind.Utc).AddTicks(5111));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 33, 1, 51, DateTimeKind.Utc).AddTicks(5112));

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "CreatedAt",
                value: new DateTime(2026, 10, 19, 19, 33, 1, 51, DateTimeKind.Utc).AddTicks(5113));
        }
    }
}
