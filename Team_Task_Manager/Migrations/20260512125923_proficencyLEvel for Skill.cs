using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team_Task_Manager.Migrations
{
    /// <inheritdoc />
    public partial class proficencyLEvelforSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProficiencyLevel",
                table: "UserSkills",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProficiencyLevel",
                table: "UserSkills");
        }
    }
}
