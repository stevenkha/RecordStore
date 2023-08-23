using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Migrations
{
    /// <inheritdoc />
    public partial class ImageNameProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Record",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Artist",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Artist");
        }
    }
}
