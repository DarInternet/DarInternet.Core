using Microsoft.EntityFrameworkCore.Migrations;

namespace DarInternet.Infrastructure.Migrations
{
    public partial class CheckOrganizationUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUser_AspNetUsers_UserId",
                table: "OrganizationUser");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUser_Organizations_OrganizationId",
                table: "OrganizationUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationUser",
                table: "OrganizationUser");

            migrationBuilder.RenameTable(
                name: "OrganizationUser",
                newName: "OrganizationUsers");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUsers",
                newName: "IX_OrganizationUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationUser_OrganizationId",
                table: "OrganizationUsers",
                newName: "IX_OrganizationUsers_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationUsers",
                table: "OrganizationUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUsers_Organizations_OrganizationId",
                table: "OrganizationUsers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUsers_AspNetUsers_UserId",
                table: "OrganizationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUsers_Organizations_OrganizationId",
                table: "OrganizationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationUsers",
                table: "OrganizationUsers");

            migrationBuilder.RenameTable(
                name: "OrganizationUsers",
                newName: "OrganizationUser");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationUsers_UserId",
                table: "OrganizationUser",
                newName: "IX_OrganizationUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationUsers_OrganizationId",
                table: "OrganizationUser",
                newName: "IX_OrganizationUser_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationUser",
                table: "OrganizationUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUser_AspNetUsers_UserId",
                table: "OrganizationUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUser_Organizations_OrganizationId",
                table: "OrganizationUser",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
