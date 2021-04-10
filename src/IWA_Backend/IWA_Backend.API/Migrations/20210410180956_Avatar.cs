using Microsoft.EntityFrameworkCore.Migrations;

namespace IWA_Backend.API.Migrations
{
    public partial class Avatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "ContractorPages",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "ContractorPages");
        }
    }
}
