using Microsoft.EntityFrameworkCore.Migrations;

namespace ClinicService.IdentityServer.Data.Migrations
{
    public partial class ModifyAppointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuessFullName",
                table: "Appointments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuessPhoneNumber",
                table: "Appointments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuessFullName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "GuessPhoneNumber",
                table: "Appointments");
        }
    }
}
