using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace talentX.WebScrapper.Sifted.Api.Migrations
{
    public partial class sifted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sifted");

            migrationBuilder.CreateTable(
                name: "DetailedScrapOutputDatas",
                schema: "sifted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sectorurl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    articleUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailedScrapOutputDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InitialScrapOutputDatas",
                schema: "sifted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sectors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialScrapOutputDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SectorWiseArticles",
                schema: "sifted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sectors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectorWiseArticles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailedScrapOutputDatas",
                schema: "sifted");

            migrationBuilder.DropTable(
                name: "InitialScrapOutputDatas",
                schema: "sifted");

            migrationBuilder.DropTable(
                name: "SectorWiseArticles",
                schema: "sifted");
        }
    }
}
