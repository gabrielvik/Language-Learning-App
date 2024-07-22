using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LanguageLearningApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addLearnedLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LearnedLessonsJson",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnedLessonsJson",
                table: "Users");
        }
    }
}
