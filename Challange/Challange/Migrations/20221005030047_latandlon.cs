using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challange.Migrations
{
    public partial class latandlon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitud",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Longitud",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "Direcciones");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "Direcciones");
        }
    }
}
