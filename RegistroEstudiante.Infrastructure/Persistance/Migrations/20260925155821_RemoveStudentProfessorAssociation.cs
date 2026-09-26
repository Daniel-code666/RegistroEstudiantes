using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistroEstudiante.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStudentProfessorAssociation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId_ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Subjects_Id_ProfessorId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_SubjectId_ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_UserId_ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId",
                table: "StudentSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId",
                table: "StudentSubjects");

            migrationBuilder.AddColumn<int>(
                name: "ProfessorId",
                table: "StudentSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE enrollment
                SET [ProfessorId] = subject.[ProfessorId]
                FROM [StudentSubjects] AS enrollment
                INNER JOIN [Subjects] AS subject ON subject.[Id] = enrollment.[SubjectId];
                """);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Subjects_Id_ProfessorId",
                table: "Subjects",
                columns: new[] { "Id", "ProfessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_SubjectId_ProfessorId",
                table: "StudentSubjects",
                columns: new[] { "SubjectId", "ProfessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_UserId_ProfessorId",
                table: "StudentSubjects",
                columns: new[] { "UserId", "ProfessorId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId_ProfessorId",
                table: "StudentSubjects",
                columns: new[] { "SubjectId", "ProfessorId" },
                principalTable: "Subjects",
                principalColumns: new[] { "Id", "ProfessorId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
