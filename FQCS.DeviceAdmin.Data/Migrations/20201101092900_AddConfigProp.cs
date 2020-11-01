using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class AddConfigProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46fc171e-a78c-4703-a482-8d1e01e839aa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aa477b03-420d-41ab-b2ce-9ebcc4fa833b");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoveOldEventsJobEnabled",
                table: "DeviceConfig",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KeepQCEventDays",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextJobStart",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemoveJobSecondsInterval",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SleepSecsWhenSendingUnsentEvents",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "17792d6f-ca38-4e8c-92d9-14cb456adb16", "b9584c78-d3b2-40ee-be09-c65ca36d7914", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "74b1c592-997a-4c93-91fc-8e4e38de3c43", "8a9d9dee-1462-4961-917d-4105e669bb1d", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17792d6f-ca38-4e8c-92d9-14cb456adb16");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "74b1c592-997a-4c93-91fc-8e4e38de3c43");

            migrationBuilder.DropColumn(
                name: "IsRemoveOldEventsJobEnabled",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "KeepQCEventDays",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "NextJobStart",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "RemoveJobSecondsInterval",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "SleepSecsWhenSendingUnsentEvents",
                table: "DeviceConfig");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "aa477b03-420d-41ab-b2ce-9ebcc4fa833b", "fb3af46c-987e-44ce-b013-51f5475f7a20", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "46fc171e-a78c-4703-a482-8d1e01e839aa", "4944b781-fc9f-4b6a-8a6d-c93aab871cb7", "Device", "DEVICE" });
        }
    }
}
