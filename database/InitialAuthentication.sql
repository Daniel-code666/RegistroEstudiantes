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

