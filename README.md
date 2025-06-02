# BetWalletAPI

## Visão Geral do Projeto

BetWalletAPI é uma API RESTful desenvolvida em .NET para gerenciar uma carteira de apostas online. Ela permite que usuários (jogadores) se cadastrem, gerenciem seus saldos através de depósitos, realizem apostas e consultem seus históricos de transações e apostas. O projeto foi construído com foco em boas práticas de desenvolvimento, utilizando Clean Architecture e princípios de Domain-Driven Design (DDD).

## Arquitetura

O projeto segue os princípios da **Clean Architecture** e **Domain-Driven Design (DDD)** para promover um código desacoplado, testável, manutenível e focado nas regras de negócio.

### Clean Architecture

A solução está organizada nas seguintes camadas:

1.  **`BetWalletAPI.Domain`**: Camada mais interna e o coração da aplicação. Contém as entidades de negócio, value objects, enums e lógica de domínio pura, sem dependências de frameworks ou infraestrutura.
    *   **Entidades**: `Player`, `Wallet`, `Bet`, `Transaction`.
    *   **Value Objects**: `Money`, `Email`.
    *   **Enums**: `BetStatus`, `TransactionType`.
    *   **Common**: `BaseEntity`.

2.  **`BetWalletAPI.Application`**: Contém a lógica de aplicação e casos de uso. Orquestra o fluxo de dados entre a camada de apresentação e a camada de domínio. Define interfaces para abstrações de infraestrutura (como repositórios e serviços externos) que são implementadas na camada de Infrastructure.
    *   **DTOs**: Objetos de Transferência de Dados para entrada e saída da API (`BetResponseDto`, `CreatePlayerRequestDto`, `TransactionResponseDto`, etc.).
    *   **Interfaces**:
        *   `IPersistence`: `IUnitOfWork`, `IBetRepository`, `IPlayerRepository`, `ITransactionRepository`, `IWalletRepository`.
        *   `ISecurity`: `IJwtTokenGenerator`, `IPasswordHasher`.
        *   `IServices`: `IBetService`, `IPlayerService`, `ITransactionService`.
    *   **Services**: Implementações dos serviços de aplicação (`BetService`, `PlayerService`, `TransactionService`).
    *   **Mappings**: Perfis do AutoMapper (`BetProfile`, `PlayerProfile`, `TransactionProfile`) para converter entidades em DTOs e vice-versa.
    *   **Exceptions**: Exceções customizadas da camada de aplicação.
    *   **Common**: Utilitários como `PagedResult`.

3.  **`BetWalletAPI.Infrastructure`**: Implementa as abstrações definidas na camada de Application, lidando com preocupações externas como acesso a banco de dados, serviços de e-mail, logging, etc.
    *   **Persistence**:
        *   `AppDbContext`: Contexto do Entity Framework Core.
        *   `Configurations`: Configurações Fluent API para as entidades (`BetConfiguration`, `PlayerConfiguration`, etc.).
        *   `Repositories`: Implementações concretas dos repositórios (`BetRepository`, `PlayerRepository`, etc.).
        *   `UnitOfWork`: Implementação do padrão Unit of Work.
        *   `Migrations`: Migrações do EF Core para o banco de dados.
    *   **Security**: Implementações para segurança (`JwtTokenGenerator`, `PasswordHasher`).
    *   **Configuration**: Configurações específicas da infraestrutura, como `JwtSettings`.

4.  **`BetWalletAPI.API` (Presentation Layer)**: Ponto de entrada da aplicação. Responsável por expor os endpoints da API, receber requisições HTTP, encaminhá-las para a camada de Application e retornar as respostas.
    *   **Controllers**: `BetController`, `PlayerController`, `TransactionController`.
    *   **Middlewares**: `ExceptionHandlingMiddleware` para tratamento global de exceções.
    *   `Program.cs`: Configuração da aplicação (injeção de dependência, pipeline HTTP, autenticação, etc.).
    *   `appsettings.json`: Configurações da aplicação.

### Domain-Driven Design (DDD)

Os seguintes conceitos de DDD foram aplicados:

*   **Entidades**: Objetos com identidade própria e ciclo de vida (`Player`, `Bet`, `Transaction`, `Wallet`). Possuem um `Id` único e encapsulam lógica de negócio.
*   **Value Objects**: Objetos imutáveis que representam um conceito descritivo no domínio, como `Money` (quantia e moeda) e `Email`. São comparados por seus atributos, não por um ID.
*   **Repositórios (`IRepository<T>`, `Repository<T>`)**: Abstraem a lógica de acesso a dados, permitindo que a camada de Application trabalhe com coleções de entidades de domínio sem se preocupar com os detalhes de persistência.
*   **Factory Method**: Utilizado dentro das entidades para encapsular a lógica de criação e garantir que os objetos sejam criados em um estado válido (ex: `Transaction.Create()`, `Player.Create()`, `Bet.Create()`).
*   **Serviços de Aplicação (`IBetService`, `IPlayerService`, `ITransactionService`)**: Orquestram os casos de uso, coordenando entidades de domínio e repositórios para realizar operações.

## Design Patterns Utilizados

*   **Repository Pattern**: Abstrai a persistência de dados, centralizando a lógica de acesso e consulta às entidades.
*   **Unit of Work Pattern**: Gerencia as transações de banco de dados, garantindo que múltiplas operações de repositório sejam executadas atomicamente.
*   **Value Object Pattern**: Usado para `Money` e `Email`, promovendo imutabilidade e encapsulamento de lógica de validação e comportamento.
*   **Factory Method**: Usado nos métodos estáticos `Create` das entidades para garantir a criação consistente e válida dos objetos de domínio.
*   **Options Pattern**: Utilizado para carregar configurações da aplicação, como `JwtSettings` a partir do `appsettings.json`.
*   **Dependency Injection (DI)**: Amplamente utilizado em todo o projeto (configurado em `Program.cs`) para desacoplar componentes e facilitar testes.
*   **Middleware**: O `ExceptionHandlingMiddleware` é um exemplo de middleware ASP.NET Core para tratamento centralizado de exceções.

## Funcionalidades Principais

*   **Gerenciamento de Jogadores**:
    *   Cadastro de novos jogadores (`POST /api/Player`).
    *   Login de jogadores existentes (`POST /api/Player/login`) com retorno de token JWT.
*   **Gerenciamento de Carteira e Transações**:
    *   Realização de depósitos na carteira do jogador (`POST /api/Transactions/deposit`).
    *   Consulta do histórico de transações do jogador (`GET /api/Transactions`).
    *   (Implicitamente, cada jogador possui uma carteira criada no momento do cadastro).
*   **Gerenciamento de Apostas**:
    *   Realização de apostas (`POST /api/Bet`).
    *   Cancelamento de apostas (`DELETE /api/Bet/{betId}/cancel`).
    *   Consulta de aposta específica (`GET /api/Bet/{betId}`).
    *   Definição de aposta como vencedora (`POST /api/Bet/{betId}/settle-won`).
    *   Definição de aposta como perdodra (`POST /api/Bet/{betId}/settle-lost`).
    *   Consulta do histórico de apostas do jogador (`GET /api/Bet/player/{playerId}`).

## Tecnologias e Ferramentas

*   **.NET 6**
*   **ASP.NET Core**: Para construção da API RESTful.
*   **Entity Framework Core**: ORM para interação com o banco de dados.
    *   **Fluent API**: Para configuração do modelo de dados.
    *   **Migrations**: Para gerenciamento do schema do banco de dados.
*   **AutoMapper**: Para mapeamento entre entidades de domínio e DTOs.
*   **JWT (JSON Web Tokens)**: Para autenticação e autorização stateless.
*   **Banco de Dados**: (PostgreSQL).
*   **xUnit/NUnit/MSTest** (para `BetWalletAPI.Tests`): Embora não implementados neste escopo, a arquitetura facilita a escrita de testes unitários e de integração.

## Componentes Chave Detalhados

*   **`ExceptionHandlingMiddleware` (`BetWalletAPI.API/Middlewares`)**:
    Intercepta exceções não tratadas em toda a aplicação, loga o erro e retorna uma resposta HTTP padronizada (ex: 500 Internal Server Error com uma mensagem genérica), evitando o vazamento de detalhes sensíveis da exceção para o cliente.

*   **AutoMapper (`BetWalletAPI.Application/Mappings`)**:
    Os `Profile`s (ex: `BetProfile.cs`, `PlayerProfile.cs`, `TransactionProfile.cs`) definem como as entidades de domínio são convertidas para DTOs (Data Transfer Objects) e vice-versa. Isso simplifica a transformação de objetos entre camadas e reduz código boilerplate.

*   **Autenticação e Autorização JWT**:
    *   **`IJwtTokenGenerator` / `JwtTokenGenerator` (`BetWalletAPI.Application/Interfaces/Security` e `BetWalletAPI.Infrastructure/Security`)**: Responsável por gerar tokens JWT após um login bem-sucedido.
    *   **`JwtSettings` (`BetWalletAPI.Infrastructure/Configuration`)**: Carrega as configurações do JWT (chave secreta, emissor, audiência) do `appsettings.json`.
    *   **Configuração em `Program.cs`**: Define os esquemas de autenticação e autorização JWT, validando os tokens recebidos nas requisições. Atributos como `[Authorize]` são usados nos controllers/endpoints para proteger recursos.

*   **Configurações de Entidade com EF Core (`BetWalletAPI.Infrastructure/Persistence/Configurations`)**:
    Classes como `BetConfiguration.cs`, `PlayerConfiguration.cs` usam a Fluent API do Entity Framework Core para definir o mapeamento das entidades para o banco de dados (nomes de tabelas, chaves primárias, relacionamentos, constraints, conversões de tipo como para o `Money` Value Object).

## Como Executar o Projeto

### Pré-requisitos

*   SDK do .NET 6
*   Um servidor de banco de dados PostgreSQL.

### Configuração

1.  **Clone o repositório:**
    ```bash
    git clone <URL_DO_SEU_REPOSITORIO>
    cd BetWalletAPI
    ```

2.  **Configure a String de Conexão:**
    Abra o arquivo `BetWalletAPI.API/appsettings.json` (e `appsettings.Development.json`) e atualize a string de conexão na seção `ConnectionStrings` para apontar para o seu servidor de banco de dados.
    Exemplo para SQL Server:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BetWalletDb;Trusted_Connection=True;MultipleActiveResultSets=true"
      },
      // ... outras configurações
    }
    ```

3.  **Configure as `JwtSettings`:**
    No mesmo arquivo `appsettings.json`, configure as `JwtSettings`:
    ```json
    {
      // ...
      "JwtSettings": {
        "Secret": "SUA_CHAVE_SECRETA_SUPER_LONGA_E_SEGURA_AQUI", // Mude para uma chave forte
        "Issuer": "BetWalletAPI",
        "Audience": "BetWalletUsers",
        "ExpiryMinutes": 60
      },
      // ...
    }
    ```

### Execução

1.  **Aplique as Migrações do Entity Framework Core:**
    Abra um terminal na pasta raiz da solução ou no diretório do projeto `BetWalletAPI.Infrastructure`.
    ```bash
    dotnet ef database update --project BetWalletAPI.Infrastructure --startup-project BetWalletAPI.API
    ```
    Isso criará o banco de dados (se não existir) e aplicará todas as migrações pendentes.

2.  **Execute a API:**
    Você pode executar a API de várias maneiras:
    *   **Via CLI .NET:**
        ```bash
        dotnet run --project BetWalletAPI.API
        ```
    *   **Via Visual Studio/Rider:**
        Defina `BetWalletAPI.API` como projeto de inicialização e pressione o botão "Run" ou F5.

A API estará disponível (por padrão) em `https://localhost:PORTA_HTTPS` e `http://localhost:PORTA_HTTP`. As portas são definidas em `BetWalletAPI.API/Properties/launchSettings.json`.

## Endpoints da API (Exemplos)

A API expõe os seguintes endpoints principais (consulte o código dos Controllers para a lista completa e detalhes dos DTOs de request/response):

*   **PlayerController (`/api/players`)**
    *   `POST /register`: Registra um novo jogador.
    *   `POST /login`: Autentica um jogador e retorna um token JWT.
*   **TransactionController (`/api/transactions`)**
    *   `POST /deposit`: (Requer Autenticação) Realiza um depósito na carteira do jogador autenticado.
    *   `GET /`: (Requer Autenticação) Lista as transações do jogador autenticado.
*   **BetController (`/api/bets`)**
    *   `POST /place`: (Requer Autenticação) Realiza uma nova aposta para o jogador autenticado.
    *   `GET /`: (Requer Autenticação) Lista as apostas do jogador autenticado.

## Próximos Passos (Sugestões)

*   Implementar testes unitários e de integração (`BetWalletAPI.Tests`).
*   Conteinerização com Docker.

---
