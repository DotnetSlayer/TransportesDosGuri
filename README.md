# 🚌✈️ Transportes dos Guri

O **Transportes dos Guri** é uma plataforma desenvolvida para a **gestão e venda de passagens e reservas de viagens aéreas e terrestres**, oferecendo um fluxo completo de agendamento integrado com pagamentos via **Pix e Boleto**, utilizando o gateway de pagamentos Sandbox do **Asaas**.

Este repositório contém a implementação completa do **MVP (*Minimum Viable Product*)**, desenvolvido como atividade prática final do componente curricular de **Programação IV** do curso de **Ciência da Computação da UNOESC**.

---

## 🔗 Links Principais

* 🚀 **Aplicação Online — Frontend Admin:**
  https://transportesdosguriadmin.runasp.net/

* 🚀 **Aplicação Online — Frontend Standard:**
  https://transportesdosguri.runasp.net/

* ⚙️ **API — Backend:**
  https://transportesdosguriwebapi.runasp.net/

  > O Swagger está desabilitado no ambiente de produção.

* 🎬 **Vídeo de Apresentação — YouTube:**
  *link*

---

# 👨‍🎓 Informações Acadêmicas

| Informação          | Detalhes                                                                           |
| ------------------- | ---------------------------------------------------------------------------------- |
| **Grupo**           | Thiago Wurster Balbinot, Thiago Thomasi, Mateus Ferreira da Silva e Rhyan Mezaroba |
| **Instituição**     | Universidade do Oeste de Santa Catarina — UNOESC                                   |
| **Curso**           | Ciência da Computação                                                              |
| **Disciplina**      | Programação IV                                                                     |
| **Professor**       | Roberson Junior Fernandes Alves                                                    |
| **Semestre Letivo** | 2026/02                                                                            |

---

# 🎯 Objetivo do Projeto

O projeto tem como objetivo desenvolver uma plataforma de gerenciamento de viagens capaz de centralizar o processo de:

* ✈️ Cadastro e gerenciamento de voos;
* 🚌 Gerenciamento de viagens terrestres;
* 🏢 Gerenciamento de aeroportos e locais de embarque;
* ✈️ Gerenciamento de aeronaves;
* 💺 Gerenciamento de assentos;
* 👤 Gerenciamento de usuários e contas;
* 🎫 Reserva de passagens;
* 🛒 Gerenciamento de compras;
* 💳 Processamento de pagamentos;
* 📄 Geração de comprovantes e recibos em PDF;
* 🔐 Autenticação e autorização de usuários;
* 👑 Controle de acesso baseado em roles;
* 💰 Integração com o gateway de pagamentos Asaas.

---

# 🛠️ Stacks & Tecnologias

O projeto adota uma arquitetura limpa e desacoplada, baseada nos princípios de **Clean Architecture** e **SOLID**.

## ⚙️ Backend — .NET 10 Web API

### Linguagem e Framework

* **C#**
* **.NET 10**
* **ASP.NET Core Web API**

### Persistência

O projeto utiliza duas tecnologias para diferentes necessidades de persistência:

* **Dapper**

  * Consultas SQL;
  * Operações de leitura;
  * Operações de gravação;
  * Acesso performático ao SQL Server.

* **Entity Framework Core**

  * Mapeamento de entidades;
  * Gerenciamento do ASP.NET Core Identity;
  * Persistência relacionada à autenticação e usuários.

### 🔐 Autenticação e Segurança

* ASP.NET Core Identity;
* JWT (*JSON Web Tokens*);
* Autenticação baseada em tokens;
* Controle de roles e permissões;
* Roles disponíveis:

  * `Admin`
  * `User`

### 💳 Integração de Pagamentos

Integração com o **Asaas** para:

* Criação e gerenciamento de clientes;
* Cobranças;
* Pagamentos via Pix;
* Pagamentos via Boleto;
* Consulta de informações relacionadas às cobranças.

### 📄 Geração de Documentos

O sistema possui um gerador interno de comprovantes e recibos em PDF:

```text
ReceiptPdfGenerator
```

### 📚 Documentação da API

A API utiliza:

* OpenAPI 3.0;
* Swagger UI;
* Documentação dos endpoints REST.

---

# 💻 Frontend — Blazor

O frontend foi desenvolvido utilizando **Blazor Server Interactivity**, com comunicação em tempo real baseada em **SignalR/WebSocket**.

### Tecnologias

* **Blazor Server Interactivity**
* **.NET 10**
* **MudBlazor**
* **HTML5**
* **CSS3**
* **SignalR / WebSocket**

### Gerenciamento e Comunicação

A comunicação com o backend é realizada utilizando:

* `HttpClient` através de Dependency Injection;
* Serviços especializados para comunicação com a API;
* `NavigationManager` para roteamento;
* DTOs compartilhados entre as camadas.

---

# 🗄️ Banco de Dados

O projeto utiliza:

**Microsoft SQL Server**

A modelagem é baseada em um banco de dados relacional, contemplando entidades relacionadas ao gerenciamento de viagens, usuários, reservas e pagamentos.

Entre as principais entidades estão:

| Entidade      | Descrição                       |
| ------------- | ------------------------------- |
| `Aircraft`    | Aeronaves                       |
| `Airport`     | Aeroportos                      |
| `Seat`        | Assentos                        |
| `Flight`      | Voos                            |
| `Reservation` | Reservas                        |
| `Purchase`    | Compras                         |
| `Account`     | Contas e usuários               |
| Entre outras  | Entidades auxiliares do sistema |

---

# 📐 Arquitetura do Sistema

A solução está estruturada em camadas bem definidas, buscando garantir:

* Modularidade;
* Desacoplamento;
* Separação de responsabilidades;
* Facilidade de manutenção;
* Testabilidade;
* Aplicação dos princípios SOLID.

## 📁 Estrutura do Projeto

```text
TransportesDosGuri/
│
├── README.md
│
├── TransportesDosGuriBackend/
│   │
│   ├── TransportesDosGuri.API/
│   │   └── Endpoints REST, Controllers, Swagger,
│   │       Middlewares e configurações da aplicação
│   │
│   ├── TransportesDosGuri.Core/
│   │   └── Regras de negócio, entidades de domínio,
│   │       DTOs, Enums e contratos de serviços/repositórios
│   │
│   └── TransportesDosGuri.Infrastructure/
│       └── Dapper, EF DbContext, implementação de
│           repositórios, Asaas Client e PDF Generator
│
├── TransportesDosGuriFrontendAdmin/
│   │
│   ├── TransportesDosGuriWeb.Core/
│   │   └── DTOs e contratos das APIs consumidas
│   │
│   ├── TransportesDosGuriWeb.Infrastructure/
│   │   └── Implementação dos serviços HTTP/C#
│   │       para comunicação com o Backend
│   │
│   └── TransportesDosGuriWeb.WebUI/
│       └── Componentes Blazor (.razor), páginas Admin,
│           layouts e estilos CSS
│
├── TransportesDosGuriFrontend/
│   │
│   ├── TransportesDosGuriWeb.Core/
│   │   └── DTOs e contratos das APIs consumidas
│   │
│   ├── TransportesDosGuriWeb.Infrastructure/
│   │   └── Implementação dos serviços HTTP/C#
│   │       para comunicação com o Backend
│   │
│   └── TransportesDosGuriWeb.WebUI/
│       └── Componentes Blazor (.razor), páginas,
│           layouts e estilos CSS
│
└── TransportesDosGuriDatabase/
    └── Scripts SQL de criação do banco,
        configuração e população inicial de dados (Seed)
```

---

# 🚀 Como Executar o Projeto Localmente

## 📋 Pré-requisitos

Antes de executar o projeto, certifique-se de possuir:

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Microsoft SQL Server](https://www.microsoft.com/sql-server)
* Git
* Visual Studio 2026 ou Visual Studio Code
* Uma instância do SQL Server disponível localmente ou através de Docker

---

# ⚙️ 1. Configuração do Backend

## 1.1 Clone o repositório

```bash
git clone https://github.com/DotnetSlayer/TransportesDosGuri.git
cd TransportesDosGuri/TransportesDosGuriBackend
```

---

## 1.2 Configure o banco de dados

Abra o arquivo:

```text
TransportesDosGuri.API/appsettings.json
```

Configure a string de conexão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TransportesDosGuriDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Caso utilize SQL Server com usuário e senha, adapte a connection string conforme a configuração do ambiente.

---

## 1.3 Configure o JWT

No mesmo arquivo, configure as informações utilizadas para autenticação:

```json
{
  "Jwt": {
    "Key": "SuaChaveSecretaSuperSeguraComTamanhoMinimo32Caracteres!",
    "Issuer": "TransportesDosGuriBackend",
    "Audience": "TransportesDosGuriClient"
  }
}
```

> ⚠️ **Importante:** não utilize chaves secretas reais diretamente no `appsettings.json` em ambientes de produção. Prefira variáveis de ambiente ou mecanismos de gerenciamento de secrets.

---

# 🗄️ 1.4 Configuração do Banco de Dados

O projeto disponibiliza scripts SQL na pasta:

```text
TransportesDosGuriDatabase/
```

---

# ▶️ 1.5 Execute o Backend

Execute o seguinte comando:

```bash
dotnet run --project TransportesDosGuri.API
```

A API será iniciada na porta configurada pelo projeto.

Durante o desenvolvimento, o Swagger poderá ser acessado através de:

```text
https://localhost:7038/swagger
```

> A porta pode variar de acordo com a configuração do ambiente local.

---

# 💻 2. Configuração do Frontend Admin

Navegue até o projeto:

```bash
cd ../TransportesDosGuriFrontendAdmin
```

Verifique a configuração da URL da API nos arquivos de configuração do frontend.

Exemplo:

```text
https://localhost:7038
```

Depois, execute:

```bash
dotnet run
```

---

# 💻 3. Configuração do Frontend Standard

Navegue até:

```bash
cd ../TransportesDosGuriFrontend
```

Configure a URL da API utilizada pelo frontend.

Depois execute:

```bash
dotnet run
```

A URL da aplicação será apresentada no terminal após a inicialização.

---

# 🔐 Autenticação e Autorização

A plataforma possui autenticação utilizando **ASP.NET Core Identity** integrada com **JWT**.

O sistema possui diferentes níveis de acesso através de roles.

### 👤 User

Usuários comuns podem utilizar as funcionalidades relacionadas à consulta, reservas e compras disponibilizadas pela plataforma.

### 👑 Admin

Administradores possuem acesso às funcionalidades de gerenciamento da plataforma, incluindo recursos administrativos e operações de CRUD.

---

# 🎫 Funcionalidades do Sistema

## ✈️ Gestão de Viagens

* Cadastro de voos;
* Consulta de voos;
* Atualização de voos;
* Exclusão de voos;
* Cadastro de aeronaves;
* Gerenciamento de aeroportos;
* Gerenciamento de assentos;
* Controle de disponibilidade.

## 🎟️ Reservas

* Criação de reservas;
* Consulta de reservas;
* Gerenciamento de reservas;
* Associação de passageiros às viagens;
* Controle dos dados relacionados à reserva.

## 🛒 Compras

* Criação de compras;
* Associação entre reservas e compras;
* Controle do processo de aquisição das passagens.

## 💳 Pagamentos

Integração com o **Asaas**, permitindo trabalhar com:

* Pix;
* Boleto;
* Cobranças;
* Clientes;
* Status dos pagamentos.

## 📄 Comprovantes

O sistema possui geração de documentos em PDF através do:

```text
ReceiptPdfGenerator
```

Esses documentos podem ser utilizados como comprovantes relacionados às operações realizadas na plataforma.

---

# 🔄 CRUD

O projeto implementa operações completas de CRUD (*Create, Read, Update, Delete*) para as principais entidades do sistema.

As operações são realizadas utilizando:

* DTOs;
* Validações;
* Regras de negócio;
* Repositórios;
* Serviços;
* Controllers/Endpoints REST.

Fluxo simplificado:

```text
Frontend
   │
   ▼
HTTP Request
   │
   ▼
API
   │
   ▼
Application / Core
   │
   ▼
Infrastructure
   │
   ├── Dapper
   │
   └── Entity Framework Core
   │
   ▼
SQL Server
```

---

# 🧩 Clean Architecture

A arquitetura do backend foi organizada buscando manter as responsabilidades separadas.

```text
┌─────────────────────────────────────┐
│             Frontend                │
│         Blazor / MudBlazor          │
└──────────────────┬──────────────────┘
                   │
                   │ HTTP / JWT
                   ▼
┌─────────────────────────────────────┐
│                API                  │
│ Controllers / Endpoints / Middleware│
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│                Core                 │
│ Regras / Entidades / DTOs / Contratos│
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│           Infrastructure            │
│ Dapper / EF / Asaas / PDF / SQL     │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│             SQL Server              │
└─────────────────────────────────────┘
```

---

# 📚 Conceitos Aplicados

Durante o desenvolvimento foram aplicados conceitos relacionados a:

* Clean Architecture;
* SOLID;
* Programação Orientada a Objetos;
* REST API;
* DTOs;
* Repository Pattern;
* Dependency Injection;
* Entity Framework Core;
* Dapper;
* ASP.NET Core Identity;
* JWT;
* Controle de autorização por roles;
* Blazor;
* MudBlazor;
* SignalR;
* SQL Server;
* Integração com APIs externas;
* Integração com gateway de pagamentos;
* Geração de documentos PDF;
* CRUD;
* Validação de dados;
* Separação de responsabilidades.

---

# 👥 Equipe

O projeto foi desenvolvido por:

* **Thiago Wurster Balbinot**
* **Thiago Thomasi**
* **Mateus Ferreira da Silva**
* **Rhyan Mezaroba**

---

# 🎓 Instituição

**Universidade do Oeste de Santa Catarina — UNOESC**

**Curso:** Ciência da Computação
**Disciplina:** Programação IV
**Professor:** Roberson Junior Fernandes Alves
**Semestre:** 2026/02

---

# 📄 Licença

Este projeto foi desenvolvido **estritamente para fins acadêmicos**, como parte das atividades da disciplina de **Programação IV** da **Universidade do Oeste de Santa Catarina (UNOESC)**.

O projeto não possui finalidade comercial e foi desenvolvido exclusivamente para fins educacionais e de demonstração das tecnologias e conceitos apresentados ao longo da disciplina.
