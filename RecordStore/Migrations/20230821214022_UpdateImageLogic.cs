using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Record",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Record",
                newName: "Image");
        }
    }
}
