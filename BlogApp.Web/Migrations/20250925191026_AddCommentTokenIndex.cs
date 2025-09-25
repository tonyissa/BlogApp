using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentTokenIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Token",
                table: "Comments",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_Token",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Comments");
        }
    }
}
