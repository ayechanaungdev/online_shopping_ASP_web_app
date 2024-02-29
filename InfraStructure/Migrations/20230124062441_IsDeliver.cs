using Microsoft.EntityFrameworkCore.Migrations;

namespace InfraStructure.Migrations
{
    public partial class IsDeliver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsDeliver",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeliver",
                table: "Order");
        }
    }
}
