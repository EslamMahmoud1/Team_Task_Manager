using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Team_Task_Manager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileRelationModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_UserProfileId",
                table: "UserSkills",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSkills_UserProfileId",
                table: "UserSkills");
        }
    }
}
