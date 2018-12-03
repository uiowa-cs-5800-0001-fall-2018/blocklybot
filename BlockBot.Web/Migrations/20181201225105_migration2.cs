using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlockBot.Web.Migrations
{
    public partial class migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProjectSettingType",
                keyColumn: "Id",
                keyValue: new Guid("2078335c-4dea-48e6-8248-1c9cee3f1a1b"));

            migrationBuilder.DeleteData(
                table: "ProjectSettingType",
                keyColumn: "Id",
                keyValue: new Guid("2e10e0a3-f121-4b3b-a80d-2bad25e58064"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProjectSettingType",
                columns: new[] { "Id", "AllowsMany", "Name" },
                values: new object[] { new Guid("2078335c-4dea-48e6-8248-1c9cee3f1a1b"), false, "AwsAccessKey" });

            migrationBuilder.InsertData(
                table: "ProjectSettingType",
                columns: new[] { "Id", "AllowsMany", "Name" },
                values: new object[] { new Guid("2e10e0a3-f121-4b3b-a80d-2bad25e58064"), false, "AwsSecretKey" });
        }
    }
}
