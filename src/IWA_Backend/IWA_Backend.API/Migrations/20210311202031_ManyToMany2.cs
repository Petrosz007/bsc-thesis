using Microsoft.EntityFrameworkCore.Migrations;

namespace IWA_Backend.API.Migrations
{
    public partial class ManyToMany2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentUser_Appointments_AttendeeOnAppointmentsId",
                table: "AppointmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentUser_AspNetUsers_AttendeesId",
                table: "AppointmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryUser_AspNetUsers_AllowedUsersId",
                table: "CategoryUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryUser_Categories_AllowedUserOnCategoriesId",
                table: "CategoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryUser",
                table: "CategoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentUser",
                table: "AppointmentUser");

            migrationBuilder.RenameTable(
                name: "CategoryUser",
                newName: "CategoryAllowedUsers");

            migrationBuilder.RenameTable(
                name: "AppointmentUser",
                newName: "AppointmentAttendees");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryUser_AllowedUsersId",
                table: "CategoryAllowedUsers",
                newName: "IX_CategoryAllowedUsers_AllowedUsersId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentUser_AttendeesId",
                table: "AppointmentAttendees",
                newName: "IX_AppointmentAttendees_AttendeesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryAllowedUsers",
                table: "CategoryAllowedUsers",
                columns: new[] { "AllowedUserOnCategoriesId", "AllowedUsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentAttendees",
                table: "AppointmentAttendees",
                columns: new[] { "AttendeeOnAppointmentsId", "AttendeesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentAttendees_Appointments_AttendeeOnAppointmentsId",
                table: "AppointmentAttendees",
                column: "AttendeeOnAppointmentsId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentAttendees_AspNetUsers_AttendeesId",
                table: "AppointmentAttendees",
                column: "AttendeesId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAllowedUsers_AspNetUsers_AllowedUsersId",
                table: "CategoryAllowedUsers",
                column: "AllowedUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAllowedUsers_Categories_AllowedUserOnCategoriesId",
                table: "CategoryAllowedUsers",
                column: "AllowedUserOnCategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentAttendees_Appointments_AttendeeOnAppointmentsId",
                table: "AppointmentAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentAttendees_AspNetUsers_AttendeesId",
                table: "AppointmentAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAllowedUsers_AspNetUsers_AllowedUsersId",
                table: "CategoryAllowedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAllowedUsers_Categories_AllowedUserOnCategoriesId",
                table: "CategoryAllowedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryAllowedUsers",
                table: "CategoryAllowedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentAttendees",
                table: "AppointmentAttendees");

            migrationBuilder.RenameTable(
                name: "CategoryAllowedUsers",
                newName: "CategoryUser");

            migrationBuilder.RenameTable(
                name: "AppointmentAttendees",
                newName: "AppointmentUser");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryAllowedUsers_AllowedUsersId",
                table: "CategoryUser",
                newName: "IX_CategoryUser_AllowedUsersId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentAttendees_AttendeesId",
                table: "AppointmentUser",
                newName: "IX_AppointmentUser_AttendeesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryUser",
                table: "CategoryUser",
                columns: new[] { "AllowedUserOnCategoriesId", "AllowedUsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentUser",
                table: "AppointmentUser",
                columns: new[] { "AttendeeOnAppointmentsId", "AttendeesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentUser_Appointments_AttendeeOnAppointmentsId",
                table: "AppointmentUser",
                column: "AttendeeOnAppointmentsId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentUser_AspNetUsers_AttendeesId",
                table: "AppointmentUser",
                column: "AttendeesId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryUser_AspNetUsers_AllowedUsersId",
                table: "CategoryUser",
                column: "AllowedUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryUser_Categories_AllowedUserOnCategoriesId",
                table: "CategoryUser",
                column: "AllowedUserOnCategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
