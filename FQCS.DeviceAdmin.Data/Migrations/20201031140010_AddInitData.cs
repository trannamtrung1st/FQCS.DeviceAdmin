using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class AddInitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e7dac64a-0593-49e8-815b-438c6423c8ca", "bfec4eb1-1210-4702-8c94-6afee1579cde", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ef4065ce-b83f-44ad-b5b7-2fca6752885e", "efd7c98d-3ac1-4154-9d93-67e197fad123", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7dac64a-0593-49e8-815b-438c6423c8ca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef4065ce-b83f-44ad-b5b7-2fca6752885e");
        }
    }
}
