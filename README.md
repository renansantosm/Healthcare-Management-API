<h1 align="center" style="font-weight: bold;">Healthcare Management API 🏥 </h1>
<p align="center">
API backend para gerenciamento de dados médicos, oferecendo funcionalidades para cadastro e administração de doutores, pacientes, consultas e prescrições médicas. Desenvolvida como projeto de portfólio, demonstra a aplicação de arquitetura limpa, DDD e princípios avançados de desenvolvimento de software.
</p>

## ✨ Funcionalidades Principais
* 👨‍⚕️ **CRUD de Médicos**: Operações completas + gerenciamento de especialidades (adicionar/remover, máximo 2)
* 👤 **CRUD de Pacientes**: Operações completas de criação, leitura, atualização e exclusão
* 📅 **Gestão de Consultas**: Criar, reagendar, completar e cancelar com verificação de disponibilidade
* 💊 **Prescrições Médicas**: Adicionar e atualizar prescrições vinculadas às consultas
* ⚙️ **Regras de Negócio**: Validações automáticas e controle de conflitos de agenda

## 🛠️ Tecnologias Utilizadas
* **.NET 9** - Framework principal
* **SQL Server** - Banco de dados
* **Entity Framework Core** - ORM
* **MediatR** - Implementação CQRS
* **Fluent Validation** - Validações
* **Swagger/OpenAPI** - Documentação da API
* **XUnit & NSubstitute** - Testes unitários e mocks
* **Docker** - Containerização e orquestração

## 🏗️ Arquitetura e Padrões de Design

### Princípios Arquiteturais
* **Clean Architecture**: Separação clara entre Domain, Application, Infrastructure e API
* **Domain-Driven Design (DDD)**: Value Objects, Agregados e domínio rico
* **CQRS + Mediator**: Separação entre operações de leitura e escrita
* **SOLID**: Aplicação rigorosa dos princípios

### Padrões Implementados
* **Repository Pattern**: Abstração da camada de persistência
* **Factory Pattern**: Criação encapsulada de Value Objects
* **Dependency Injection**: Container DI nativo do .NET

## 📁 Estrutura do Projeto
```
HealthcareManagement/
├── src/                                         # Código fonte da aplicação
│   ├── HealthcareManagement.API/                # Controllers, Filters, Configurações
│   ├── HealthcareManagement.Application/        # Commands, Queries, Handlers, DTOs e Services
│   │   ├── {Entity}/Commands/                   # Handlers e Validators por entidade
│   │   ├── {Entity}/Queries/                    # Queries específicas
│   │   ├── DTOs/                                # Data Transfer Objects
│   │   ├── Services/                            # Validações de existência e regras de negócio
│   │   └── Behaviours/                          # MediatR Pipelines
│   ├── HealthcareManagement.Domain/             # Entidades, Value Objects, Enums
│   ├── HealthcareManagement.Infra.Data/         # DbContext, Repositories, Providers
│   └── HealthcareManagement.Infra.IoC/          # Dependency Injection
├── tests/                                       # Projetos de teste
│   ├── HealthcareManagement.Application.Tests/  # Testes da camada de aplicação
│   ├── HealthcareManagement.Domain.Tests/       # Testes da camada de domínio
│   └── HealthcareManagement.Infra.Data.Tests/   # Testes da camada de infraestrutura
├── Dockerfile                                   # Configuração Docker da aplicação
├── docker-compose.yml                           # Orquestração de contêineres
└── HealthcareManagement.sln                     # Arquivo de solução
```

## 🔗 Endpoints Principais
```
/api/v1/doctors - CRUD completo de médicos e especialidades
/api/v1/patients - CRUD completo de pacientes
/api/v1/appointments  - Listar, criar, reagendar, completar e cancelar consultas
/api/v1/appointments/{appointmentId}/prescriptions – Criar ou atualizar a prescrição de uma consulta
/api/v1/prescriptions – Listar todas as prescrições
```

## 📝 Regras de Negócio Implementadas

### Gestão Operacional Clínica ⏱️
* Limite máximo de 2 especialidades por médico
* Consultas não podem ser canceladas com menos de 24h de antecedência
* Duração padronizada de 30 minutos por consulta
* Agendamentos permitidos apenas entre 08:00 e 17:00

### Integridade de Dados 🔒
* Validação de unicidade para e-mails e CPFs
* Verificação automática de disponibilidade de agenda
* Validações específicas para os dados da prescrição médica

## ✅ Testes

O projeto inclui uma cobertura abrangente de testes unitários implementados com **XUnit** e **NSubstitute**. Os testes estão distribuídos em três projetos dedicados e cobrem:

* **Modelos de Domínio**: Testes que validam o comportamento das entidades principais
* **Value Objects**: Verificação da criação e validação dos objetos de valor
* **Command Handlers**: Testes dos manipuladores de comandos CQRS
* **Validação de Comandos**: Cobertura das regras de validação com Fluent Validation
* **Serviços de Verificação**: Testes dos serviços que validam unicidade de email/CPF, existência de entidades e conflitos de horários
* **Repositórios**: Testes dos métodos críticos de persistência de dados

Esta abordagem de testes suporta a manutenção do código e garante que as regras de negócio sejam preservadas durante o desenvolvimento contínuo do projeto.
## 🚀 Como Executar

### 🐳 Execução com Docker (Recomendado)
📋 Pré-requisitos
* Docker
* Git
### ⚙️ Instalação e Execução


```bash
# Clone o repositório
git clone https://github.com/renansantosm/Healthcare-Management-API
cd Healthcare-Management-API

# Execute com Docker Compose (inclui SQL Server)
docker-compose up -d

# A aplicação estará disponível em:
# http://localhost:8081

# Acesse a documentação Swagger
# http://localhost:8081/swagger

```
### 🔧 Execução Local (Desenvolvimento)
📋 Pré-requisitos
* .NET 9 SDK
* SQL Server (LocalDB, Express ou completo)
* Git

### ⚙️ Instalação e Execução

```bash
# Clone o repositório
git clone https://github.com/renansantosm/Healthcare-Management-API
cd Healthcare-Management-API

# Restaure as dependências
dotnet restore

# Execute os testes unitários
dotnet test

# Execute a aplicação
dotnet run --project src/HealthcareManagement.API

# Acesse a documentação Swagger
# # http://localhost:5021/swagger
# # https://localhost:7250/swagger

```
