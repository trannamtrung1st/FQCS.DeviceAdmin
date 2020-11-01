using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class ChangeSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "RemoveOldEventsJobSettings",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SendUnsentEventsJobSettings",
                table: "DeviceConfig",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0a3c7b33-1ce7-4808-9b8e-3f3ae931e82e", "21a65020-eece-4f30-932c-1a916f5aed40", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1f8ca895-7ba6-430a-af79-3f658c04d268", "ee824b26-26a0-4259-9953-346f6bebfffe", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0a3c7b33-1ce7-4808-9b8e-3f3ae931e82e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f8ca895-7ba6-430a-af79-3f658c04d268");

            migrationBuilder.DropColumn(
                name: "RemoveOldEventsJobSettings",
                table: "DeviceConfig");

            migrationBuilder.DropColumn(
                name: "SendUnsentEventsJobSettings",
                table: "DeviceConfig");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoveOldEventsJobEnabled",
                table: "DeviceConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KeepQCEventDays",
                table: "DeviceConfig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextJobStart",
                table: "DeviceConfig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemoveJobSecondsInterval",
                table: "DeviceConfig",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SleepSecsWhenSendingUnsentEvents",
                table: "DeviceConfig",
                type: "int",
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
    }
}
