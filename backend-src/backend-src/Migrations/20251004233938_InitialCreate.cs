using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_src.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Raports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OczekiwanaEmerytura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Wiek = table.Column<int>(type: "int", nullable: false),
                    CzyMezczyzna = table.Column<bool>(type: "bit", nullable: false),
                    Wynagrodzenie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CzyUwzglednialOkresChoroby = table.Column<bool>(type: "bit", nullable: false),
                    SrodkiNaKoncie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SrodkiNaSubkoncie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmeryturaRzeczywista = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmeryturaUrealniona = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KodPocztowy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Raports");
        }
    }
}
