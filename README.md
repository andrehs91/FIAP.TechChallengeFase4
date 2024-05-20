# FIAP - Tech Challenge (Fase 4)

Este repositório contêm o projeto desenvolvido para o Tech Challenge da quarta fase do curso **Arquitetura de Sistemas .NET com Azure**, desenvolvido pelo aluno **André Henrique dos Santos (RM351909)**.

Os requisitos do projeto foram definidos no arquivo "Requisitos.docx".

---

### Escopo o Projeto

O sistema desenvolvido é um gerenciador de demandas, que visa centralizar a abertura e tratamento de demandas de diferentes departamentos de uma organização de médio/grande porte, onde um fluxo claro e objetivo se faz necessário em função do volume e complexidade dos processos.

No arquivo "Requisitos.docx", seção **Eventos/Comandos** e seção **Políticas**, constam todos os casos de uso essenciais para a viabilidade do projeto. Portanto, para que o projeto fosse considerado viável e uma primeira versão pudesse ser considerada pronta, todos os eventos/comandos/políticas deveriam ser implementados em forma de API Web.

---

### Arquitetura da Solução

- **Backend/Producer:** (ASP.NET Core Web API) projeto <u>TechChallenge.Aplicacao</u>, que contêm uma interface Swagger para testes;
- **Mensageria:** (RabbitMQ) para comunicação assíncrona entre os serviços, utilizado pela política "Definir Solucionador";
- **Consumer:** (Worker Service) projeto <u>TechChallenge.Worker</u>, para processamento assíncrono da política "Definir Solucionador";
- **Cache:** (Redis) para melhora do desempenho das consultas;
- **Banco de Dados:** (MariaDB) para persistência de dados.

**Observações:**
1. Os testes automatizados estão contidos no projeto <u>TechChallenge.Testes</u>.
2. A escolha dos componentes <u>RabbitMQ</u>, <u>Redis</u> e <u>MariaDB</u> foi motivada pelo requisito não funcional *"A solução deverá evitar Vendor Lock-In"*.
3. Todos os componentes desta arquitetura rodam em <u>Containers Docker</u>.
4. Esta estrutura, baseada em <u>micro serviços</u>, confere maior <u>resiliência</u>, pois qualquer componente pode ser substituído em caso de falha, e permite a adoção de estratégias de <u>escalabilidade</u>, visto que a quantidade de containers pode variar conforme a demanda.
5. O arquivo "docker-compose.yml" define as configurações e ordem de carregamento dos componentes da arquitetura.

---

### Considerações Sobre a Implementação

- A implementação do projeto seguiu a abordagem <u>DDD</u>, pois ela é bastante aderente ao conceito de <u>Código Limpo</u>.
- O core da solução foi estruturada como uma **API Web com o ASP.NET Core** (projeto TechChallenge.Aplicacao) que faz uso de duas **bibliotecas de classes** (projetos TechChallenge.Dominio e TechChallenge.Infraestrutura). Esta abordagem foi adotada para que o isolamento entre as camadas propostas pelo DDD pudesse ser observado com maior rigor;
- O processamento da política "Definir Solucionador" foi isolado em um **Worker Service** (que faz uso das mesmas bibliotecas referidas acima), pois é a funcionalidade que mais demanda processamento do banco de dados. Desta forma, o funcionamento da API é preservado, evitando aumento do seu tempo de resposta.
- Quanto a autenticação dos usuários, o sistema utiliza **Tokens JWT** do tipo **Bearer**.
> :memo: **Observação:** Considerando a natureza deste projeto (uma ferramenta de suporte para uma organização que possui outros sistemas), o mecanismo de autenticação que foi implementado é bastante rudimentar (não permite o cadastramento de novos usuários e nem as demais ações relacionadas), tendo o objetivo de apenas viabilizar sua avaliação. Em um cenário ideal, este sistema deverá ser integrado a um serviço de SSO a fim de que os processos de gerenciamento de credencias possa ser centralizado e falhas de segurança conhecidas, decorrentes de abordagens distribuídas, sejam evitadas.

---

### Como Executar este Projeto
> :warning: **Importante:** Para que o projeto possa ser executado, o computador utilizado deverá ter o Docker instalado.

1. (Opcional) Alterar o segredo utilizado para a geração dos Tokens JWT nos arquivos “appsettings.json” > “Secret” de ambos os projetos (Aplicacao e Worker);
2. (Opcional) Alterar as variáveis contidas no arquivo ".env";
3. Na pasta raiz do projeto, executar o commando: ``` docker compose up ```;
4. Acessar a interface [Swagger](http://localhost:8080/swagger/index.html);
5. Fazer um requisição GET para http://localhost:8080/CargaDeDados (ela irá incluir os dados iniciais para testes);
6. (Opcional) Acompanhar a fila de mensagens através do endereço [localhost:15672](http://localhost:15672/). Os dados de usuário e senha estão definidos no arquivo ".env";
7. (Opcional) Acompanhar os dados em cache através do endereço [localhost:8001](http://localhost:8001/). O usuário é "default" e a senha está definida no arquivo ".env";
8. (Opcional) Acompanhar o processamento das mensagens através dos logs do worker.

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
