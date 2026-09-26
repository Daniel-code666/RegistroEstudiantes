using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistroEstudiante.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class OptionalSubjectProfessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProfessorId",
                table: "Subjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM [Subjects] WHERE [ProfessorId] IS NULL) THROW 51000, 'Asigne un profesor a todas las materias antes de revertir esta migracion.', 1;");
            migrationBuilder.AlterColumn<int>(
                name: "ProfessorId",
                table: "Subjects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
