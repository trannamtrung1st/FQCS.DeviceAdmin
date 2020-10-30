using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class AddDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "DeviceConfig",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "DeviceConfig",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "DeviceConfig",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "AppClient",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "AppClient",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "AppClient");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "AppClient");
        }
    }
}
