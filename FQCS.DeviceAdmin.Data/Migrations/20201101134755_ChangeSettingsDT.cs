using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class ChangeSettingsDT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0a3c7b33-1ce7-4808-9b8e-3f3ae931e82e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f8ca895-7ba6-430a-af79-3f658c04d268");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextROEJobStart",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextSUEJobStart",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bb47a220-88f7-471b-a66b-2a77161169ff", "49a295c1-35cd-4f8a-b9d6-3eab363b2858", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "086c8298-a8a3-4c74-8654-4b8556603d5a", "494416d7-0b4d-46cb-90b4-7750142e7ea4", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "086c8298-a8a3-4c74-8654-4b8556603d5a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bb47a220-88f7-471b-a66b-2a77161169ff");

            migrationBuilder.DropColumn(
                name: "NextROEJobStart",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "NextSUEJobStart",
                table: "DeviceConfig");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0a3c7b33-1ce7-4808-9b8e-3f3ae931e82e", "21a65020-eece-4f30-932c-1a916f5aed40", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1f8ca895-7ba6-430a-af79-3f658c04d268", "ee824b26-26a0-4259-9953-346f6bebfffe", "Device", "DEVICE" });
        }
    }
}
