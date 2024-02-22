using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WhiteLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Villas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Sqft = table.Column<int>(type: "int", nullable: false),
                    Occupancy = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Update_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villas", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Create_Date", "Description", "ImageUrl", "Name", "Occupancy", "Price", "Sqft", "Update_Date" },
                values: new object[,]
                {
                    { 1, null, "The word \"villa\" is frequently used when describing home styles and design trends – so frequently, in fact, that it can be difficult to know exactly what it means. Our FAQ will go over what a villa residence is, where the term comes from, and how you can make plans for villa style homes. Anyone interested in building a home or making important renovations on a beloved property should take a look!", "~/Static_Files/Images/Villa-Images/Royal-Villa-1.jpg", "Royal Villa", 4, 200.0, 500, null },
                    { 2, null, "A villa style house is traditionally a more secluded house, often single-level, designed to be a home for a single family, usually on spacious property that puts it at a distance from other houses – or at least come with a private courtyard or other areas that solely belongs to the property owners. These homes often have lengthy driveways that help set them back from the road a bit for more privacy.", "~/Static_Files/Images/Villa-Images/Premium-Pool-Villa-1.jpg", "Premium Pool Villa", 5, 300.0, 500, null },
                    { 3, null, "The name villa comes from ancient Rome, where it described a residence owned by wealthy citizens that served as a retreat from the populace, or a vacation estate on rich land. They were specifically designed to be as self-sufficient as possible and were frequently the site of vineyards or other important crops. Even today, the word villa is often associated with resorts, luxury, and getting away from everyday life.", "~/Static_Files/Images/Villa-Images/Luxury-Pool-Villa-1.jpg", "Luxury Pool Villa", 6, 400.0, 700, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Villas");
        }
    }
}
