using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.DeviceAdmin.Data.Migrations
{
    public partial class AddEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    KafkaServer = table.Column<string>(maxLength: 255, nullable: true),
                    KafkaUsername = table.Column<string>(maxLength: 255, nullable: true),
                    KafkaPassword = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QCEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefectTypeCode = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    LeftImage = table.Column<string>(unicode: false, maxLength: 2000, nullable: true),
                    RightImage = table.Column<string>(unicode: false, maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCEvent", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceConfig");

            migrationBuilder.DropTable(
                name: "QCEvent");
        }
    }
}
