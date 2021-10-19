using Microsoft.EntityFrameworkCore.Migrations;

namespace ligaNos.Migrations
{
    public partial class MatchesPart2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchesCalendar_Clubs_TeamId",
                table: "MatchesCalendar");

            migrationBuilder.DropIndex(
                name: "IX_MatchesCalendar_TeamId",
                table: "MatchesCalendar");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "MatchesCalendar");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "MatchesCalendar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchesCalendar_TeamId",
                table: "MatchesCalendar",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchesCalendar_Clubs_TeamId",
                table: "MatchesCalendar",
                column: "TeamId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
