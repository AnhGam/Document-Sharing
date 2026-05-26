using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document_sharing_manager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServerIdToInviteAndJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "JoinRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "InviteLinks",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "JoinRequests");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "InviteLinks");
        }
    }
}
