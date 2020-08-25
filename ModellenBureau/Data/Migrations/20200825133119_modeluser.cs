using Microsoft.EntityFrameworkCore.Migrations;

namespace ModellenBureau.Data.Migrations
{
    public partial class modeluser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileModel",
                table: "FileModel");

            migrationBuilder.DropColumn(
                name: "FileModelId",
                table: "FileModel");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "FileModel",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileModel",
                table: "FileModel",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileModel",
                table: "FileModel");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FileModel");

            migrationBuilder.AddColumn<string>(
                name: "FileModelId",
                table: "FileModel",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileModel",
                table: "FileModel",
                column: "FileModelId");
        }
    }
}
