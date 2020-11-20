using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class AddSeenColumnQCEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6ebf43c2-c2a4-4c0b-b692-66e9e8f67e29");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d86a759f-2d98-4da8-8dac-a6c0b1a93b0f");

            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "QCEvent",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "96165597-281c-433c-9792-60ace27d2ee4", "cd01384f-30df-4f9c-89a1-4a2866e1d1a9", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "19512af9-876e-4afa-a92f-a46d41422dee", "ce366146-5bfb-477f-bcdd-c4a555dac6a1", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "19512af9-876e-4afa-a92f-a46d41422dee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96165597-281c-433c-9792-60ace27d2ee4");

            migrationBuilder.DropColumn(
                name: "Seen",
                table: "QCEvent");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6ebf43c2-c2a4-4c0b-b692-66e9e8f67e29", "a7dae202-8d70-49f8-a97c-4fc03b204589", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d86a759f-2d98-4da8-8dac-a6c0b1a93b0f", "9a897cd8-190e-4e1b-b846-84639209b373", "Device", "DEVICE" });
        }
    }
}
