IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CreationDate] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedDate] datetime NULL,
        [Active] bit NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE TABLE [Subjects] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Credits] int NOT NULL,
        [CreationDate] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedDate] datetime NULL,
        [Active] bit NOT NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [NormalizedEmail] nvarchar(254) NOT NULL,
        [PasswordHash] nvarchar(512) NOT NULL,
        [RoleId] int NOT NULL,
        [IdentificationType] smallint NOT NULL,
        [IdentificationNumber] nvarchar(30) NOT NULL,
        [CreationDate] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedDate] datetime NULL,
        [Active] bit NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE TABLE [StudentSubjects] (
        [SubjectsId] int NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_StudentSubjects] PRIMARY KEY ([SubjectsId], [UserId]),
        CONSTRAINT [FK_StudentSubjects_Subjects_SubjectsId] FOREIGN KEY ([SubjectsId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StudentSubjects_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'CreationDate', N'Description', N'Name', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [Active], [CreationDate], [Description], [Name], [UpdatedDate])
    VALUES (1, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''Estudiante'', N''Student'', NULL),
    (2, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''Administrador'', N''Admin'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'CreationDate', N'Description', N'Name', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE INDEX [IX_StudentSubjects_UserId] ON [StudentSubjects] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_IdentificationType_IdentificationNumber] ON [Users] ([IdentificationType], [IdentificationNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_NormalizedEmail] ON [Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925154048_InitialAuthentication'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260925154048_InitialAuthentication', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    IF EXISTS (SELECT 1 FROM [Subjects])
        THROW 51000, 'Existen materias previas sin profesor. Prepare una asignación de profesores y adapte los datos iniciales antes de aplicar esta migración.', 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] DROP CONSTRAINT [FK_StudentSubjects_Subjects_SubjectsId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] DROP CONSTRAINT [FK_StudentSubjects_Users_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] DROP CONSTRAINT [PK_StudentSubjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    DROP INDEX [IX_StudentSubjects_UserId] ON [StudentSubjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    EXEC sp_rename N'[StudentSubjects].[SubjectsId]', N'SubjectId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'Name');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Subjects] ALTER COLUMN [Name] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'Description');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Subjects] ALTER COLUMN [Description] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'Credits');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Subjects] ADD DEFAULT 3 FOR [Credits];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [Subjects] ADD [ProfessorId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD [ProfessorId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD [CreationDate] datetime NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD [UpdatedDate] datetime NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [Subjects] ADD CONSTRAINT [AK_Subjects_Id_ProfessorId] UNIQUE ([Id], [ProfessorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD CONSTRAINT [PK_StudentSubjects] PRIMARY KEY ([SubjectId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    CREATE TABLE [Professors] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [CreationDate] datetime NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedDate] datetime NULL,
        [Active] bit NOT NULL,
        CONSTRAINT [PK_Professors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'CreationDate', N'LastName', N'Name', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Professors]'))
        SET IDENTITY_INSERT [Professors] ON;
    EXEC(N'INSERT INTO [Professors] ([Id], [Active], [CreationDate], [LastName], [Name], [UpdatedDate])
    VALUES (1, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''García'', N''Ana'', NULL),
    (2, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''López'', N''Carlos'', NULL),
    (3, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''Martínez'', N''Laura'', NULL),
    (4, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''Rodríguez'', N''Diego'', NULL),
    (5, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', N''Pérez'', N''Sofía'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'CreationDate', N'LastName', N'Name', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Professors]'))
        SET IDENTITY_INSERT [Professors] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'CreationDate', N'Credits', N'Description', N'Name', N'ProfessorId', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Subjects]'))
        SET IDENTITY_INSERT [Subjects] ON;
    EXEC(N'INSERT INTO [Subjects] ([Id], [Active], [CreationDate], [Credits], [Description], [Name], [ProfessorId], [UpdatedDate])
    VALUES (1, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Matemáticas'', N''Matemáticas'', 1, NULL),
    (2, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Estadística'', N''Estadística'', 1, NULL),
    (3, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Programación'', N''Programación'', 2, NULL),
    (4, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Bases de datos'', N''Bases de datos'', 2, NULL),
    (5, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Física'', N''Física'', 3, NULL),
    (6, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Electrónica'', N''Electrónica'', 3, NULL),
    (7, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Comunicación'', N''Comunicación'', 4, NULL),
    (8, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Inglés'', N''Inglés'', 4, NULL),
    (9, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Administración'', N''Administración'', 5, NULL),
    (10, CAST(1 AS bit), ''2026-09-25T00:00:00.000'', 3, N''Emprendimiento'', N''Emprendimiento'', 5, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Active', N'CreationDate', N'Credits', N'Description', N'Name', N'ProfessorId', N'UpdatedDate') AND [object_id] = OBJECT_ID(N'[Subjects]'))
        SET IDENTITY_INSERT [Subjects] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    CREATE INDEX [IX_Subjects_ProfessorId] ON [Subjects] ([ProfessorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    EXEC(N'ALTER TABLE [Subjects] ADD CONSTRAINT [CK_Subjects_Credits] CHECK ([Credits] = 3)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    CREATE INDEX [IX_StudentSubjects_SubjectId_ProfessorId] ON [StudentSubjects] ([SubjectId], [ProfessorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentSubjects_UserId_ProfessorId] ON [StudentSubjects] ([UserId], [ProfessorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentSubjects_UserId_SubjectId] ON [StudentSubjects] ([UserId], [SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD CONSTRAINT [FK_StudentSubjects_Subjects_SubjectId_ProfessorId] FOREIGN KEY ([SubjectId], [ProfessorId]) REFERENCES [Subjects] ([Id], [ProfessorId]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD CONSTRAINT [FK_StudentSubjects_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    ALTER TABLE [Subjects] ADD CONSTRAINT [FK_Subjects_Professors_ProfessorId] FOREIGN KEY ([ProfessorId]) REFERENCES [Professors] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155236_AcademicEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260925155236_AcademicEntities', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    ALTER TABLE [StudentSubjects] DROP CONSTRAINT [FK_StudentSubjects_Subjects_SubjectId_ProfessorId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    ALTER TABLE [Subjects] DROP CONSTRAINT [AK_Subjects_Id_ProfessorId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    DROP INDEX [IX_StudentSubjects_SubjectId_ProfessorId] ON [StudentSubjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    DROP INDEX [IX_StudentSubjects_UserId_ProfessorId] ON [StudentSubjects];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentSubjects]') AND [c].[name] = N'ProfessorId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [StudentSubjects] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [StudentSubjects] DROP COLUMN [ProfessorId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    ALTER TABLE [StudentSubjects] ADD CONSTRAINT [FK_StudentSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925155821_RemoveStudentProfessorAssociation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260925155821_RemoveStudentProfessorAssociation', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925160312_ProfessorsAsUsers'
)
BEGIN
    ALTER TABLE [Subjects] DROP CONSTRAINT [FK_Subjects_Professors_ProfessorId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925160312_ProfessorsAsUsers'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Description', N'Active', N'CreationDate') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [Name], [Description], [Active], [CreationDate])
    VALUES (3, N''Professor'', N''Profesor'', CAST(1 AS bit), ''2026-09-25T00:00:00.000'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Description', N'Active', N'CreationDate') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925160312_ProfessorsAsUsers'
)
BEGIN
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
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925160312_ProfessorsAsUsers'
)
BEGIN
    ALTER TABLE [Subjects] ADD CONSTRAINT [FK_Subjects_Users_ProfessorId] FOREIGN KEY ([ProfessorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925160312_ProfessorsAsUsers'
)
BEGIN
    DROP TABLE [Professors];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925160312_ProfessorsAsUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260925160312_ProfessorsAsUsers', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925171140_UserTokenVersion'
)
BEGIN
    ALTER TABLE [Users] ADD [TokenVersion] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925171140_UserTokenVersion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260925171140_UserTokenVersion', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925184716_OptionalSubjectProfessor'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'ProfessorId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Subjects] ALTER COLUMN [ProfessorId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260925184716_OptionalSubjectProfessor'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260925184716_OptionalSubjectProfessor', N'10.0.12');
END;

COMMIT;
GO

