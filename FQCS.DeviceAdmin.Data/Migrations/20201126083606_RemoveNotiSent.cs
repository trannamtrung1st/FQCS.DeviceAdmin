using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class RemoveNotiSent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "NotiSent",
                table: "QCEvent");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "791f26f9-3bdc-4274-bfd3-c51b0b5e8b42", "6e4b1942-ae49-40a1-8364-1e833ca7da10", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f2e41998-b175-496e-a528-b6a2ef5f814c", "c8946505-5f7f-4926-ad8b-0a2572be41aa", "Device", "DEVICE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "791f26f9-3bdc-4274-bfd3-c51b0b5e8b42");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2e41998-b175-496e-a528-b6a2ef5f814c");

            migrationBuilder.AddColumn<bool>(
                name: "NotiSent",
                table: "QCEvent",
                type: "bit",
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
    }
}
