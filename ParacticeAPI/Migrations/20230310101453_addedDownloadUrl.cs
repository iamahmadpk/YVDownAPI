using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParacticeAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedDownloadUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "SearchRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "SearchRequests");
        }
    }
}
