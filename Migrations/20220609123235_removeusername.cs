using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog3.Migrations
{
    public partial class removeusername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "SubComments");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "MainComments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "SubComments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "MainComments",
                nullable: true);
        }
    }
}
