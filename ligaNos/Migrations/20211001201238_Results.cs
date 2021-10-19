using Microsoft.EntityFrameworkCore.Migrations;

namespace ligaNos.Migrations
{
    public partial class Results : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    MarkedGoals = table.Column<int>(type: "int", nullable: false),
                    SufferedGoals = table.Column<int>(type: "int", nullable: false),
                    YellowCards = table.Column<int>(type: "int", nullable: false),
                    RedCards = table.Column<int>(type: "int", nullable: false),
                    Victorys = table.Column<int>(type: "int", nullable: false),
                    Losts = table.Column<int>(type: "int", nullable: false),
                    Ties = table.Column<int>(type: "int", nullable: false),
                    Pontuation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Goals = table.Column<int>(type: "int", nullable: false),
                    YellowCards = table.Column<int>(type: "int", nullable: false),
                    RedCards = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMatch = table.Column<int>(type: "int", nullable: false),
                    MGHome = table.Column<int>(type: "int", nullable: false),
                    MGAway = table.Column<int>(type: "int", nullable: false),
                    YCHome = table.Column<int>(type: "int", nullable: false),
                    YCAway = table.Column<int>(type: "int", nullable: false),
                    RCHome = table.Column<int>(type: "int", nullable: false),
                    RCAway = table.Column<int>(type: "int", nullable: false),
                    PontuationHome = table.Column<int>(type: "int", nullable: false),
                    PontuationAway = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubResults");

            migrationBuilder.DropTable(
                name: "PlayerResults");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
