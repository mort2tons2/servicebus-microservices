using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsumerService1.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    BlobName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.BlobName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "File");
        }
    }
}
