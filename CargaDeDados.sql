SET IDENTITY_INSERT [App].[Atividades] ON
INSERT INTO [App].[Atividades]
    ([Id], [Nome], [Descricao], [EstahAtiva], [DepartamentoSolucionador], [TipoDeDistribuicao], [Prioridade], [PrazoEstimado])
VALUES
    (1, 'Remanejar Equipamento', 'Remanejar fisicamente um equipamento de informática.', 1, 'Suporte Tecnológico', 1, 3, 60),
    (2, 'Instalar Software', 'Instalar um software em determinado computador.', 1, 'Suporte Tecnológico', 1, 0, 120),
    (3, 'Analisar Minuta de Contrato', 'Analisar uma minuta proposta para um contrato.', 1, 'Jurídico', 0, 1, 240)
SET IDENTITY_INSERT [App].[Atividades] OFF

SET IDENTITY_INSERT [App].[Usuarios] ON
INSERT INTO [App].[Usuarios]
    ([Id], [Matricula], [Nome], [Departamento], [EhGestor])
VALUES
    (1, '1062', 'Pedro', 'Suporte Tecnológico', 1),
    (2, '1123', 'Fernanda', 'Suporte Tecnológico', 0),
    (3, '1099', 'Tiago', 'Suporte Tecnológico', 0),
    (4, '1255', 'Felipe', 'Suporte Tecnológico', 0),
    (5, '1012', 'Helena', 'Financeiro', 1),
    (6, '1294', 'Rafael', 'Financeiro', 0),
    (7, '1004', 'Lucas', 'Jurídico', 1),
    (8, '1344', 'Isabel', 'Jurídico', 0)
SET IDENTITY_INSERT [App].[Usuarios] OFF

INSERT INTO [App].[Solucionadores]
    ([AtividadesId], [SolucionadoresId])
VALUES
    (1, 2),
    (1, 3),
    (2, 2),
    (2, 4)
