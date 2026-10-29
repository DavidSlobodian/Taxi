using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Taxi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TarrifsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AverageFuelCons = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AverageFuelPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarrifs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    BaseFare = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    WaitingPricePerMin = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarrifs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PricingSettings",
                columns: new[] { "Id", "AverageFuelCons", "AverageFuelPrice", "CreatedAt", "UpdatedAt" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), 9m, 94.5m, new DateTime(2026, 9, 30, 17, 10, 38, 282, DateTimeKind.Utc).AddTicks(7559), null });

            migrationBuilder.InsertData(
                table: "Tarrifs",
                columns: new[] { "Id", "BaseFare", "Class", "CreatedAt", "UpdatedAt", "WaitingPricePerMin" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), 40m, 0, new DateTime(2026, 9, 30, 17, 10, 38, 282, DateTimeKind.Utc).AddTicks(7459), null, 3m },
                    { new Guid("10000000-0000-0000-0000-000000000002"), 50m, 1, new DateTime(2026, 9, 30, 17, 10, 38, 282, DateTimeKind.Utc).AddTicks(7470), null, 4m },
                    { new Guid("10000000-0000-0000-0000-000000000003"), 70m, 2, new DateTime(2026, 9, 30, 17, 10, 38, 282, DateTimeKind.Utc).AddTicks(7473), null, 5m },
                    { new Guid("10000000-0000-0000-0000-000000000004"), 100m, 3, new DateTime(2026, 9, 30, 17, 10, 38, 282, DateTimeKind.Utc).AddTicks(7474), null, 8m },
                    { new Guid("10000000-0000-0000-0000-000000000005"), 90m, 4, new DateTime(2026, 9, 30, 17, 10, 38, 282, DateTimeKind.Utc).AddTicks(7476), null, 6m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarrifs_Class",
                table: "Tarrifs",
                column: "Class",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingSettings");

            migrationBuilder.DropTable(
                name: "Tarrifs");
        }
    }
}
