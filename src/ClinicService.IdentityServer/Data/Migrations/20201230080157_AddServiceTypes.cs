using Microsoft.EntityFrameworkCore.Migrations;

namespace ClinicService.IdentityServer.Data.Migrations
{
    public partial class AddServiceTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MedicalServiceTypeId",
                table: "MedicalServices",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MedicalServiceTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalServiceTypes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalServiceTypes");

            migrationBuilder.DropColumn(
                name: "MedicalServiceTypeId",
                table: "MedicalServices");
        }
    }
}
