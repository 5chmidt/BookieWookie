using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookieWookie.API.Migrations
{
    public partial class added_file : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.FileId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_FileId",
                table: "Books",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_File_FileId",
                table: "Books",
                column: "FileId",
                principalTable: "File",
                principalColumn: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_File_FileId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropIndex(
                name: "IX_Books_FileId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Books");
        }
    }
}
