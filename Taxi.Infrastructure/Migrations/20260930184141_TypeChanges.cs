using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TypeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "WaitingPricePerMin",
                table: "Tarrifs",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "BaseFare",
                table: "Tarrifs",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<double>(
                name: "PriceMargin",
                table: "Tarrifs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "TotalPrice",
                table: "Rides",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "PricePerKm",
                table: "Rides",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "AverageFuelPrice",
                table: "PricingSettings",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "AverageFuelCons",
                table: "PricingSettings",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "AverageFuelCons", "AverageFuelPrice", "CreatedAt" },
                values: new object[] { 9.0, 94.5, new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1313) });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "BaseFare", "CreatedAt", "PriceMargin", "WaitingPricePerMin" },
                values: new object[] { 40.0, new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1205), 0.0, 3.0 });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "BaseFare", "CreatedAt", "PriceMargin", "WaitingPricePerMin" },
                values: new object[] { 50.0, new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1214), 0.0, 4.0 });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "BaseFare", "CreatedAt", "PriceMargin", "WaitingPricePerMin" },
                values: new object[] { 70.0, new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1215), 0.0, 5.0 });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "BaseFare", "CreatedAt", "PriceMargin", "WaitingPricePerMin" },
                values: new object[] { 100.0, new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1216), 0.0, 8.0 });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "BaseFare", "CreatedAt", "PriceMargin", "WaitingPricePerMin" },
                values: new object[] { 90.0, new DateTime(2026, 9, 30, 18, 41, 41, 160, DateTimeKind.Utc).AddTicks(1217), 0.0, 6.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceMargin",
                table: "Tarrifs");

            migrationBuilder.AlterColumn<decimal>(
                name: "WaitingPricePerMin",
                table: "Tarrifs",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "BaseFare",
                table: "Tarrifs",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Rides",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerKm",
                table: "Rides",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageFuelPrice",
                table: "PricingSettings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageFuelCons",
                table: "PricingSettings",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "PricingSettings",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "AverageFuelCons", "AverageFuelPrice", "CreatedAt" },
                values: new object[] { 9m, 94.5m, new DateTime(2026, 9, 30, 18, 31, 27, 195, DateTimeKind.Utc).AddTicks(5660) });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "BaseFare", "CreatedAt", "WaitingPricePerMin" },
                values: new object[] { 40m, new DateTime(2026, 9, 30, 18, 31, 27, 195, DateTimeKind.Utc).AddTicks(5549), 3m });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "BaseFare", "CreatedAt", "WaitingPricePerMin" },
                values: new object[] { 50m, new DateTime(2026, 9, 30, 18, 31, 27, 195, DateTimeKind.Utc).AddTicks(5562), 4m });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "BaseFare", "CreatedAt", "WaitingPricePerMin" },
                values: new object[] { 70m, new DateTime(2026, 9, 30, 18, 31, 27, 195, DateTimeKind.Utc).AddTicks(5563), 5m });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "BaseFare", "CreatedAt", "WaitingPricePerMin" },
                values: new object[] { 100m, new DateTime(2026, 9, 30, 18, 31, 27, 195, DateTimeKind.Utc).AddTicks(5564), 8m });

            migrationBuilder.UpdateData(
                table: "Tarrifs",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "BaseFare", "CreatedAt", "WaitingPricePerMin" },
                values: new object[] { 90m, new DateTime(2026, 9, 30, 18, 31, 27, 195, DateTimeKind.Utc).AddTicks(5566), 6m });
        }
    }
}
