using Microsoft.EntityFrameworkCore.Migrations;

namespace IWA_Backend.API.Migrations
{
    public partial class ManyToMany3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentAttendees");

            migrationBuilder.CreateTable(
                name: "AttendeeOnAppointments",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeOnAppointments", x => new { x.AppointmentId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AttendeeOnAppointments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendeeOnAppointments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeOnAppointments_UserId",
                table: "AttendeeOnAppointments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendeeOnAppointments");

            migrationBuilder.CreateTable(
                name: "AppointmentAttendees",
                columns: table => new
                {
                    AttendeeOnAppointmentsId = table.Column<int>(type: "int", nullable: false),
                    AttendeesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentAttendees", x => new { x.AttendeeOnAppointmentsId, x.AttendeesId });
                    table.ForeignKey(
                        name: "FK_AppointmentAttendees_Appointments_AttendeeOnAppointmentsId",
                        column: x => x.AttendeeOnAppointmentsId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentAttendees_AspNetUsers_AttendeesId",
                        column: x => x.AttendeesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttendees_AttendeesId",
                table: "AppointmentAttendees",
                column: "AttendeesId");
        }
    }
}
