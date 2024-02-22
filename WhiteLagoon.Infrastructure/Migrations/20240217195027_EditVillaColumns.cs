using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhiteLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditVillaColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Villas",
                newName: "Update_Date");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Villas",
                newName: "Create_Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Update_Date",
                table: "Villas",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "Create_Date",
                table: "Villas",
                newName: "CreateDate");
        }
    }
}
