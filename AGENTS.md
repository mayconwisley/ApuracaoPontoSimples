# AGENTS.md

## Objetivo

Este projeto é uma aplicação frontend em React + TypeScript.
Todas as alterações devem priorizar:

- legibilidade
- previsibilidade
- baixo acoplamento
- componentização reutilizável
- tipagem forte
- separação clara de responsabilidades

O agente deve produzir código pronto para manutenção profissional, evitando soluções improvisadas, duplicação desnecessária e lógica misturada entre UI, regra de negócio e acesso a dados.

---

## Stack oficial

- React
- TypeScript
- Vite
- React Router
- TanStack Query para dados remotos
- Zustand ou Context API para estado global simples
- Axios para HTTP
- React Hook Form + Zod para formulários e validação
- ESLint + Prettier
- Vitest + Testing Library para testes

Não introduzir novas bibliotecas sem necessidade clara.

---

## Princípios obrigatórios

- Sempre usar TypeScript estrito.
- Preferir composição em vez de herança.
- Cada componente deve ter responsabilidade única.
- Não misturar regra de negócio com componente visual.
- Não fazer chamadas HTTP diretamente dentro de componentes de página, exceto por hooks próprios.
- Não usar `any`.
- Evitar `as` sem necessidade real.
- Tipar props, retornos, estados e responses de API.
- Manter funções pequenas e nomes explícitos.
- Evitar comentários redundantes; o código deve ser autoexplicativo.

---

## Estrutura de pastas

Organizar o projeto preferencialmente da seguinte forma:

src/
app/
router/
providers/
store/
pages/
Dashboard/
DashboardPage.tsx
DashboardPage.styles.ts
features/
auth/
components/
hooks/
services/
types/
mappers/
users/
components/
hooks/
services/
types/
mappers/
components/
ui/
layout/
feedback/
hooks/
services/
http/
lib/
utils/
types/
schemas/

### Regras de estrutura

- `pages/` contém composição de tela.
- `features/` contém domínio funcional isolado.
- `components/ui/` contém componentes genéricos e reutilizáveis.
- `services/http/` contém cliente HTTP base.
- `hooks/` contém hooks genéricos compartilhados.
- `types/` contém tipos globais e compartilhados.
- `schemas/` contém validações Zod.

---

## Convenções de nomenclatura

- Componentes React: PascalCase
- Hooks: camelCase iniciando com `use`
- Utilitários: camelCase
- Tipos/interfaces: PascalCase
- Arquivos de componente: mesmo nome do componente
- Arquivos de hook: `useNomeDoHook.ts`
- Arquivos de serviço: `nomeFeature.service.ts`
- Arquivos de query hook: `useNomeRecursoQuery.ts`
- Arquivos de mutation hook: `useNomeRecursoMutation.ts`

Exemplos:

- `UserCard.tsx`
- `useAuth.ts`
- `auth.service.ts`
- `useUsersQuery.ts`

---

## Padrão para componentes

### Componentes devem:

- receber props tipadas
- ser pequenos e focados
- evitar lógica de negócio complexa
- delegar formatação e transformação pesada para helpers/hooks

### Evitar:

- componentes com mais de uma responsabilidade
- múltiplos níveis de ternário
- regras de negócio embutidas no JSX
- `useEffect` para tudo

### Preferir:

- early return
- variáveis intermediárias com nomes claros
- extração de subcomponentes quando o JSX crescer demais

Exemplo esperado:

- componente de página orquestra hooks e layout
- componente de lista apenas renderiza
- componente de item apenas apresenta dados

---

## Padrão para páginas

As páginas devem:

- orquestrar hooks
- compor componentes de layout e feature
- evitar implementar regras específicas diretamente

Uma página não deve:

- montar payload HTTP manualmente se isso puder ficar em service/mapper
- conter transformação complexa de response
- repetir lógica usada em outras páginas

---

## Estado e gerenciamento de dados

### Use state local para:

- modal aberto/fechado
- input temporário
- seleção local de UI

### Use estado global para:

- autenticação
- preferências do usuário
- contexto de sessão
- dados realmente compartilhados entre várias áreas

### Use TanStack Query para:

- cache de chamadas HTTP
- loading/error/success state
- invalidação de dados
- sincronização de dados remotos

Não duplicar em store global dados que já vivem bem no cache do React Query.

---

## Regras para hooks

- Hooks devem encapsular comportamento reutilizável.
- Hooks de dados devem ficar próximos da feature.
- Hooks não devem retornar estruturas confusas.
- Preferir retorno nomeado.

Exemplo:

````ts
return {
  users,
  isLoading,
  isError,
  refetch,
};

# AGENTS.md

## Objetivo
Este projeto é uma API em C# usando .NET 10.
Toda alteração deve priorizar:
- separação clara de responsabilidades
- baixo acoplamento
- alta coesão
- legibilidade
- previsibilidade
- testabilidade
- consistência arquitetural

O agente deve gerar código profissional, pronto para manutenção, evitando soluções improvisadas, duplicação desnecessária e violação de camadas.

---

## Stack oficial
- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL ou SQL Server
- FluentValidation quando aplicável
- xUnit para testes
- Swagger/OpenAPI
- Injeção de dependência nativa do .NET
- Logging nativo ou Serilog, conforme padrão já existente

Não introduzir novas bibliotecas sem necessidade clara.

---

## Princípios obrigatórios
- Respeitar rigorosamente a separação entre Api, Application, Domain e Infrastructure.
- Não colocar regra de negócio em controllers.
- Não colocar regra de negócio em repositórios.
- Não expor entidades de domínio diretamente na API.
- Não usar `dynamic`.
- Evitar `object` e casts desnecessários.
- Usar `async/await` para operações I/O.
- Sempre receber e propagar `CancellationToken` em operações assíncronas.
- Preferir código explícito, legível e previsível.
- Não acoplar `Domain` a frameworks ou detalhes de infraestrutura.
- Não criar abstrações desnecessárias.

---

## Estrutura de camadas

### Domain
Responsável por:
- entidades
- value objects
- enums
- regras de negócio puras
- contratos centrais de domínio

Regras:
- não depender de EF Core
- não depender de ASP.NET Core
- não depender de banco de dados
- não depender de serviços externos
- não conter DTOs de API
- não conter código de persistência

### Application
Responsável por:
- casos de uso
- handlers
- DTOs de entrada e saída
- contratos de repositório
- contratos de serviços externos
- validações de aplicação
- paginação, filtros e orquestração

Regras:
- não acessar banco diretamente
- não depender de detalhes concretos de infraestrutura
- não conter controller
- não conter configuração HTTP

### Infrastructure
Responsável por:
- DbContext
- configurações do EF Core
- implementações de repositório
- integrações externas
- serviços de arquivos
- autenticação concreta
- mensageria
- persistência

Regras:
- implementar contratos definidos em Application ou Domain
- não mover regra de negócio principal para esta camada
- manter foco em detalhes técnicos

### Api
Responsável por:
- endpoints HTTP
- controllers ou minimal APIs
- configuração de DI
- autenticação e autorização
- middlewares
- filtros
- serialização
- documentação Swagger

Regras:
- não conter regra de negócio
- não acessar diretamente infraestrutura sem passar pela aplicação
- apenas orquestrar request, response e códigos HTTP

---

## Convenções de nomenclatura

### Tipos
- classes, records, interfaces e enums em PascalCase
- interfaces iniciando com `I`
- campos privados com `_camelCase`
- parâmetros e variáveis locais em camelCase

### Sufixos recomendados
- `Controller`
- `Request`
- `Response`
- `Dto`
- `Command`
- `Query`
- `Handler`
- `Validator`
- `Repository`
- `Service`

Exemplos:
- `CreateCustomerCommand`
- `CreateCustomerHandler`
- `CreateCustomerRequest`
- `CustomerResponse`
- `ICustomerRepository`

---

## Controllers e endpoints

### Controllers devem:
- ser finos
- apenas receber a requisição
- validar o modelo quando aplicável
- encaminhar para a camada de aplicação
- retornar respostas HTTP apropriadas

### Controllers não devem:
- conter regra de negócio
- acessar `DbContext` diretamente
- montar consultas complexas
- conter transformação extensa de dados
- reutilizar entidades de domínio como contrato HTTP

### Padrões esperados
- usar `ActionResult<T>` quando apropriado
- retornar códigos HTTP coerentes
- usar `CancellationToken`
- manter actions curtas e objetivas

Exemplo de responsabilidade correta:
- controller chama um caso de uso
- caso de uso executa a regra
- controller converte resultado em HTTP response

---

## Casos de uso / Application

Cada caso de uso deve possuir responsabilidade única.

### Devem:
- encapsular uma ação clara do sistema
- depender de interfaces e não de implementações concretas
- validar regras de aplicação
- orquestrar entidades, repositórios e serviços

### Não devem:
- conhecer detalhes do EF Core
- conhecer detalhes de HTTP
- retornar tipos específicos de infraestrutura
- misturar múltiplos cenários sem necessidade

### Preferir
- uma pasta por caso de uso
- request/response bem definidos
- handlers pequenos
- nomes explícitos

Exemplo:
```txt
Application/
  UseCases/
    Customer/
      Create/
        Command.cs
        Handler.cs
        Validator.cs
        Response.cs
````
