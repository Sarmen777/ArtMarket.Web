using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artworks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ArtistName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ArtistTier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Medium = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WidthCm = table.Column<float>(type: "real", nullable: false),
                    HeightCm = table.Column<float>(type: "real", nullable: false),
                    CreationYear = table.Column<int>(type: "integer", nullable: false),
                    SaleYear = table.Column<int>(type: "integer", nullable: false),
                    ArtistPastSalesCount = table.Column<int>(type: "integer", nullable: false),
                    ActualPriceUsd = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PredictedPriceUsd = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artworks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artworks");
        }
    }
}
