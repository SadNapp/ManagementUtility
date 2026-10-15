using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementUtility.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InventoryNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, collation: "NOCASE"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ResponsiblePerson = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_InventoryNumber",
                table: "Assets",
                column: "InventoryNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
