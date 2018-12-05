using Microsoft.EntityFrameworkCore.Migrations;

namespace BlockBot.Web.Migrations
{
    public partial class AddTwilioSettingsToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwilioAccountSID",
                table: "Projects",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TwilioAuthToken",
                table: "Projects",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TwilioServiceSID",
                table: "Projects",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwilioAccountSID",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TwilioAuthToken",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TwilioServiceSID",
                table: "Projects");
        }
    }
}
