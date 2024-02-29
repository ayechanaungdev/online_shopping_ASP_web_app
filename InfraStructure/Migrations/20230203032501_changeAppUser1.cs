using Microsoft.EntityFrameworkCore.Migrations;

namespace InfraStructure.Migrations
{
    public partial class changeAppUser1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "AspNetUsers",
                newName: "PhoneNo");

            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NRC",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NRC",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "AspNetUsers",
                newName: "Address");
        }
    }
}
