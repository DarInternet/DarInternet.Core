using Microsoft.EntityFrameworkCore.Migrations;

namespace DarInternet.Infrastructure.Migrations
{
    public partial class AddIsDisabledToConversationUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "ConversationUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "ConversationUsers");
        }
    }
}
