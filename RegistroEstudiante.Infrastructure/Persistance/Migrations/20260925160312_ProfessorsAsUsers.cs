using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistroEstudiante.Infrastructure.Persistance.Migrations;

public partial class ProfessorsAsUsers : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Subjects_Professors_ProfessorId", table: "Subjects");

        migrationBuilder.InsertData(
            table: "Roles",
            columns: new[] { "Id", "Name", "Description", "Active", "CreationDate" },
            values: new object[] { 3, "Professor", "Profesor", true, new DateTime(2026, 9, 25, 0, 0, 0, DateTimeKind.Utc) });

        // No reutilizar IDs: pueden pertenecer a estudiantes o administradores existentes.
        // Los identificadores provisionales no representan datos personales reales.
        // PasswordHash vacio impide el login hasta provisionar las credenciales.
        migrationBuilder.Sql("""
            DECLARE @ProfessorUsers TABLE (ProfessorId int PRIMARY KEY, UserId int NOT NULL);
            MERGE INTO [Users] AS target
            USING (
                SELECT *, LOWER(CONVERT(varchar(36), NEWID())) AS TemporaryIdentifier
                FROM [Professors]
            ) AS source ON 1 = 0
            WHEN NOT MATCHED THEN
                INSERT ([Name], [LastName], [Email], [NormalizedEmail], [PasswordHash],
                        [RoleId], [IdentificationType], [IdentificationNumber], [Active], [CreationDate], [UpdatedDate])
                VALUES (source.[Name], source.[LastName],
                        CONCAT('professor-', source.TemporaryIdentifier, '@example.invalid'),
                        UPPER(CONCAT('professor-', source.TemporaryIdentifier, '@example.invalid')), '',
                        3, 4, CONCAT('PROF-', LEFT(REPLACE(source.TemporaryIdentifier, '-', ''), 25)),
                        source.[Active], source.[CreationDate], source.[UpdatedDate])
            OUTPUT source.[Id], inserted.[Id] INTO @ProfessorUsers (ProfessorId, UserId);

            UPDATE subject
            SET [ProfessorId] = mapping.UserId
            FROM [Subjects] AS subject
            INNER JOIN @ProfessorUsers AS mapping ON mapping.ProfessorId = subject.[ProfessorId];
            """);

        migrationBuilder.AddForeignKey(
            name: "FK_Subjects_Users_ProfessorId", table: "Subjects", column: "ProfessorId",
            principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.DropTable(name: "Professors");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Revertir automaticamente eliminaria cuentas que pueden haber sido completadas y utilizadas.
        throw new NotSupportedException("La conversion de profesores a usuarios requiere una migracion de datos explicita para revertirse sin perder cuentas.");
    }
}
