using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionsToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Posts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Comments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Comments");
        }
    }
}
