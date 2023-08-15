using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecordModelArtistProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Record_Artist_ArtistId",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "Artist",
                table: "Record");

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "Record",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Record_Artist_ArtistId",
                table: "Record",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Record_Artist_ArtistId",
                table: "Record");

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "Record",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Artist",
                table: "Record",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Record_Artist_ArtistId",
                table: "Record",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "Id");
        }
    }
}
