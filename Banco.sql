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
CREATE TABLE [Produtos] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Categoria] nvarchar(max) NOT NULL,
    [Preco] decimal(18,2) NOT NULL,
    [Quantidade] int NOT NULL,
    CONSTRAINT [PK_Produtos] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260423015322_InitialCreate', N'10.0.7');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Telefone] nvarchar(max) NOT NULL,
    [CPF] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260428001110_AddCliente', N'10.0.7');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260428004131_AddVenda', N'10.0.7');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Vendas] (
    [Id] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [ProdutoId] int NOT NULL,
    [Quantidade] int NOT NULL,
    [ValorTotal] decimal(18,2) NOT NULL,
    [DataVenda] datetime2 NOT NULL,
    CONSTRAINT [PK_Vendas] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260428013712_AjustesFinais', N'10.0.7');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260503140516_RelacoesVenda', N'10.0.7');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE INDEX [IX_Vendas_ClienteId] ON [Vendas] ([ClienteId]);

CREATE INDEX [IX_Vendas_ProdutoId] ON [Vendas] ([ProdutoId]);

ALTER TABLE [Vendas] ADD CONSTRAINT [FK_Vendas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Vendas] ADD CONSTRAINT [FK_Vendas_Produtos_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260516002947_AjusteVenda', N'10.0.7');

COMMIT;
GO

BEGIN TRANSACTION;

CREATE TABLE [dbo].[Usuarios] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [NomeUsuario] NVARCHAR (MAX) NOT NULL,
    [Senha]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260531_AddUsuariosSeguranca', N'10.0.7');

COMMIT;
GO

