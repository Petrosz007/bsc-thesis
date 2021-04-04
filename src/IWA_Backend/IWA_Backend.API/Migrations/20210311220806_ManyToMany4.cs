using Microsoft.EntityFrameworkCore.Migrations;

namespace IWA_Backend.API.Migrations
{
    public partial class ManyToMany4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAllowedUsers");

            migrationBuilder.CreateTable(
                name: "AllowedUserOnCategories",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedUserOnCategories", x => new { x.CategoryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AllowedUserOnCategories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllowedUserOnCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowedUserOnCategories_UserId",
                table: "AllowedUserOnCategories",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedUserOnCategories");

            migrationBuilder.CreateTable(
                name: "CategoryAllowedUsers",
                columns: table => new
                {
                    AllowedUserOnCategoriesId = table.Column<int>(type: "int", nullable: false),
                    AllowedUsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAllowedUsers", x => new { x.AllowedUserOnCategoriesId, x.AllowedUsersId });
                    table.ForeignKey(
                        name: "FK_CategoryAllowedUsers_AspNetUsers_AllowedUsersId",
                        column: x => x.AllowedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryAllowedUsers_Categories_AllowedUserOnCategoriesId",
                        column: x => x.AllowedUserOnCategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAllowedUsers_AllowedUsersId",
                table: "CategoryAllowedUsers",
                column: "AllowedUsersId");
        }
    }
}
