using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class AddNotiSentQCEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e7dac64a-0593-49e8-815b-438c6423c8ca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef4065ce-b83f-44ad-b5b7-2fca6752885e");

            migrationBuilder.AddColumn<bool>(
                name: "NotiSent",
                table: "QCEvent",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4342787f-6e9e-4e5f-9fc2-342ae26486d4", "f40996bc-0c06-41eb-ab69-c18b10571c38", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e2d18b99-7c98-4a2a-bb41-ddea48da3a7b", "53f29ee4-8a27-4389-96b6-89aaf8d6605a", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4342787f-6e9e-4e5f-9fc2-342ae26486d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2d18b99-7c98-4a2a-bb41-ddea48da3a7b");

            migrationBuilder.DropColumn(
                name: "NotiSent",
                table: "QCEvent");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e7dac64a-0593-49e8-815b-438c6423c8ca", "bfec4eb1-1210-4702-8c94-6afee1579cde", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ef4065ce-b83f-44ad-b5b7-2fca6752885e", "efd7c98d-3ac1-4154-9d93-67e197fad123", "Device", "DEVICE" });
        }
    }
}
