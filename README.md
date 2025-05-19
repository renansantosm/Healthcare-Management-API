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
├── HealthcareManagement.API/                    # Controllers, Filters, Configurações
├── HealthcareManagement.Application/            # Commands, Queries, Handlers, DTOs e Services
│   ├── {Entity}/Commands/                       # Handlers e Validators por entidade
│   ├── {Entity}/Queries/                        # Queries específicas
│   ├── DTOs/                                    # Data Transfer Objects
│   ├── Services/                                # Validações de existência e regras de negócio
│   └── Behaviours/                              # MediatR Pipelines
├── HealthcareManagement.Domain/                 # Entidades, Value Objects, Enums
├── HealthcareManagement.Infra.Data/             # DbContext, Repositories, Providers
├── HealthcareManagement.Infra.IoC/              # Dependency Injection
└── *.Tests/                                     # Projetos de teste
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

## 🚀 Como Executar

### 📋 Pré-requisitos
* .NET 9 SDK
* SQL Server (LocalDB, Express ou completo)
* Git

### ⚙️ Instalação e Execução

```bash
# Clone o repositório
git clone https://github.com/renansantosm/Healthcare-Management-API
cd healthcare-management-api

# Restaure as dependências
dotnet restore

# Configure a string de conexão no appsettings.json
# Execute as migrações do banco
dotnet ef database update

# Execute a aplicação
dotnet run

# Acesse a documentação Swagger
# https://localhost:5001/swagger
```
