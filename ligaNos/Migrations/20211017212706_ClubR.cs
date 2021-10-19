using Microsoft.EntityFrameworkCore.Migrations;

namespace ligaNos.Migrations
{
    public partial class ClubR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClubId",
                table: "ClubResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ResultTemps",
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
                    PontuationAway = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultTemps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultTemps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClubResults_ClubId",
                table: "ClubResults",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultTemps_UserId",
                table: "ResultTemps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClubResults_Clubs_ClubId",
                table: "ClubResults",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubResults_Clubs_ClubId",
                table: "ClubResults");

            migrationBuilder.DropTable(
                name: "ResultTemps");

            migrationBuilder.DropIndex(
                name: "IX_ClubResults_ClubId",
                table: "ClubResults");

            migrationBuilder.AlterColumn<int>(
                name: "ClubId",
                table: "ClubResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
