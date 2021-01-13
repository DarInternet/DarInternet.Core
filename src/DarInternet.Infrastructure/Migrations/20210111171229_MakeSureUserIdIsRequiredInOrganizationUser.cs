using Microsoft.EntityFrameworkCore.Migrations;

namespace DarInternet.Infrastructure.Migrations
{
    public partial class MakeSureUserIdIsRequiredInOrganizationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OrganizationUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OrganizationUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
