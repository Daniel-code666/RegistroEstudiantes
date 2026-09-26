using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegistroEstudiante.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AcademicEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El modelo anterior no tenía profesores: no inventar asignaciones para datos existentes.
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [Subjects])
                    THROW 51000, 'Existen materias previas sin profesor. Prepare una asignación de profesores y adapte los datos iniciales antes de aplicar esta migración.', 1;
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectsId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Users_UserId",
                table: "StudentSubjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSubjects",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_UserId",
                table: "StudentSubjects");

            migrationBuilder.RenameColumn(
                name: "SubjectsId",
                table: "StudentSubjects",
                newName: "SubjectId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subjects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Credits",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProfessorId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfessorId",
                table: "StudentSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "StudentSubjects",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "StudentSubjects",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Subjects_Id_ProfessorId",
                table: "Subjects",
                columns: new[] { "Id", "ProfessorId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSubjects",
                table: "StudentSubjects",
                columns: new[] { "SubjectId", "UserId" });

            migrationBuilder.CreateTable(
                name: "Professors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Professors",
                columns: new[] { "Id", "Active", "CreationDate", "LastName", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "García", "Ana", null },
                    { 2, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "López", "Carlos", null },
                    { 3, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Martínez", "Laura", null },
                    { 4, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Rodríguez", "Diego", null },
                    { 5, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Pérez", "Sofía", null }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Active", "CreationDate", "Credits", "Description", "Name", "ProfessorId", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Matemáticas", "Matemáticas", 1, null },
                    { 2, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Estadística", "Estadística", 1, null },
                    { 3, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Programación", "Programación", 2, null },
                    { 4, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Bases de datos", "Bases de datos", 2, null },
                    { 5, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Física", "Física", 3, null },
                    { 6, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Electrónica", "Electrónica", 3, null },
                    { 7, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Comunicación", "Comunicación", 4, null },
                    { 8, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Inglés", "Inglés", 4, null },
                    { 9, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Administración", "Administración", 5, null },
                    { 10, true, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "Emprendimiento", "Emprendimiento", 5, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ProfessorId",
                table: "Subjects",
                column: "ProfessorId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Subjects_Credits",
                table: "Subjects",
                sql: "[Credits] = 3");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_SubjectId_ProfessorId",
                table: "StudentSubjects",
                columns: new[] { "SubjectId", "ProfessorId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_UserId_ProfessorId",
                table: "StudentSubjects",
                columns: new[] { "UserId", "ProfessorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_UserId_SubjectId",
                table: "StudentSubjects",
                columns: new[] { "UserId", "SubjectId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId_ProfessorId",
                table: "StudentSubjects",
                columns: new[] { "SubjectId", "ProfessorId" },
                principalTable: "Subjects",
                principalColumns: new[] { "Id", "ProfessorId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Users_UserId",
                table: "StudentSubjects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Professors_ProfessorId",
                table: "Subjects",
                column: "ProfessorId",
                principalTable: "Professors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [StudentSubjects])
                    THROW 51001, 'No se puede revertir la migración académica mientras existan inscripciones.', 1;
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId_ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Users_UserId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Professors_ProfessorId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "Professors");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Subjects_Id_ProfessorId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_ProfessorId",
                table: "Subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Subjects_Credits",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSubjects",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_SubjectId_ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_UserId_ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_UserId_SubjectId",
                table: "StudentSubjects");

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "StudentSubjects");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "StudentSubjects");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "StudentSubjects");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "StudentSubjects",
                newName: "SubjectsId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Credits",
                table: "Subjects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSubjects",
                table: "StudentSubjects",
                columns: new[] { "SubjectsId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_UserId",
                table: "StudentSubjects",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectsId",
                table: "StudentSubjects",
                column: "SubjectsId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Users_UserId",
                table: "StudentSubjects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
