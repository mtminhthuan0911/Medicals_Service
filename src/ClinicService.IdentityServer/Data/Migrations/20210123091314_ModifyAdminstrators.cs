using Microsoft.EntityFrameworkCore.Migrations;

namespace ClinicService.IdentityServer.Data.Migrations
{
    public partial class ModifyAdminstrators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromMedicalExaminationId",
                table: "Reappointments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "MedicalExaminations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReappointmentId",
                table: "MedicalExaminations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromMedicalExaminationId",
                table: "Reappointments");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "MedicalExaminations");

            migrationBuilder.DropColumn(
                name: "ReappointmentId",
                table: "MedicalExaminations");
        }
    }
}
