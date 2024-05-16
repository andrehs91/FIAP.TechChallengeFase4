# FIAP - Tech Challenge (Fase 4)

Este repositório contêm o projeto desenvolvido para o Tech Challenge da quarta fase do curso **Arquitetura de Sistemas .NET com Azure**, desenvolvido pelo aluno **André Henrique dos Santos (RM351909)**.

Os requisitos do projeto foram levantados através do *Event Storming* que pode ser conferido em https://miro.com/app/board/uXjVNe54OjM=/ e transcritos para o arquivo "Dicionário.docx" deste repositório.

### Escopo o Projeto

O sistema desenvolvido é um gerenciador de demandas, que visa centralizar a abertura e tratamento de demandas de diferentes departamentos de uma organização de médio/grande porte, onde um fluxo claro e objetivo se faz necessário em função do volume e complexidade dos processos.

No arquivo "Dicionário.docx", seção **Eventos/Comandos**, constam todos os casos de uso que foram mapeados. Mas, apenas os itens em preto foram considerados como essências para que a primeira versão do sistema fosse considerada viável. Por isso, os itens em vermelho não foram incluídos nos fluxos do *Event Storming*.

Portanto, foi definido que o critério de aceite do projeto seria a implementação de todos os casos de uso essenciais e que somente a API de backend deveria ser desenvolvida.

### Considerações Sobre a Implementação

- A solução foi estruturada como uma **API Web com o ASP.NET Core** (projeto TechChallenge.Aplicacao) que faz uso de duas **bibliotecas de classes** (projetos TechChallenge.Dominio e TechChallenge.Infraestrutura). Esta abordagem foi adotada para que o isolamento entre as camadas propostas pelo DDD pudesse ser observado com maior rigor;
- Para a persistência de dados, foi adotado o banco de dados **SQL Server Express LocalDB**;
- Quanto a autenticação dos usuários, o sistema utiliza **Tokens JWT** do tipo **Bearer**.
> :memo: **Observação:** Considerando a natureza deste projeto (uma ferramenta de suporte para uma organização que possui outros sistemas), o mecanismo de autenticação que foi implementado é bastante rudimentar (não permite o cadastramento de novos usuários e nem as demais ações relacionadas), tendo o objetivo de apenas viabilizar sua avaliação. Em um cenário ideal, este sistema deverá ser integrado a um serviço de SSO a fim de que os processos de gerenciamento de credencias possa ser centralizado e falhas de segurança conhecidas, decorrentes de abordagens distribuídas, sejam evitadas.

### Como Executar este Projeto
- (Opcional) Alterar o segredo utilizado para a geração dos Tokens JWT no arquivo “appsettings.json” > “Secret”;
- (Opcional) Alterar a *Connection String* no arquivo “appsettings.json” > “ConnectionStrings” > “SQLServer”;
- Aplicar as *migrations*:  ``` Update-Database -Project TechChallenge.Infraestrutura ```;
- Executar o script SQL “CargaDeDados.sql” no banco de dados “TechChallenge”;

Os seguintes usuários estarão disponíveis:
- Pedro, matrícula 1062, do departamento “Suporte Tecnológico”, Gestor;
- Fernanda, matrícula 1123, do departamento “Suporte Tecnológico”;
- Tiago, matrícula 1099, do departamento “Suporte Tecnológico”;
- Felipe, matrícula 1255, do departamento “Suporte Tecnológico”;
- Helena, matrícula 1012, do departamento “Financeiro”, Gestora;
- Rafael, matrícula 1294, do departamento “Financeiro”;
- Lucas, matrícula 1004, do departamento “Jurídico”, Gestor;
- Isabel, matrícula 1344, do departamento “Jurídico”;
> :warning: **Atenção:** A senha de todos os usuários é a palavra “senha”, com todas as letras minúsculas.

As seguintes atividades estarão disponíveis:
- Remanejar Equipamento, do departamento “Suporte Tecnológico”;
- Instalar Software, do departamento “Suporte Tecnológico”;
- Analisar Minuta de Contrato, do departamento “Jurídico”;

Add-Migration ProjetoCriado -StartupProject TechChallenge.Aplicacao -Project TechChallenge.Infraestrutura -Context TechChallenge.Infraestrutura.Data.ApplicationDbContext
Update-Database -StartupProject TechChallenge.Aplicacao -Project TechChallenge.Infraestrutura -Context TechChallenge.Infraestrutura.Data.ApplicationDbContext
