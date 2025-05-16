<h1 align="center" style="font-weight: bold;">Healthcare Management API 💻</h1>

<p align="center">
API backend para gerenciamento de dados médicos, oferecendo funcionalidades para cadastro e administração de doutores, pacientes e consultas. Desenvolvida como projeto de portfólio, demonstra a aplicação de arquitetura limpa, DDD e princípios avançados de desenvolvimento de software.
</p>

## 🎯 Funcionalidades Principais
- Cadastro e gestão de doutores 
- Cadastro e gestão de pacientes
- Agendamento e controle de consultas médicas
- Regras operacionais de gestão clínica

## 🛠️ Tecnologias Utilizadas
- <b>.NET 9 </b> - Framework principal para desenvolvimento da API
- <b>SQL Server</b> - Sistema de gerenciamento de banco de dados relacional
- <b>Entity Framework Core</b> - ORM para acesso a dados e persistência
- <b>MediaTr</b> - Implementação do padrão Mediator para CQRS
- <b>Fluent Validation</b> - Validação robusta de comandos 
- <b>Swashbuckle</b> - Geração de documentação Swagger/OpenAPI
- <b>Asp.Versioning</b> - Gerenciamento de versões da API
- <b>XUnit</b> - Framework para testes unitários
- <b>NSubstitute</b> - Framework de mocking para testes

## 🏗️ Arquitetura e Padrões
- **Arquitetura Limpa**: Separação clara entre Domain, Application, Infrastructure e API
- **Domain-Driven Design**: Value Objects, Agregados, Validações de entidade e Domínio rico
- **CQRS + Mediator**: Separação entre operações de leitura e escrita
- **Repositório**: Abstração da camada de persistência
- **Factory**: Criação encapsulada de Value Objects com validações integradas
- **Injeção de Dependência**: Uso extensivo do container DI nativo do .NET
- **SOLID**: Aplicação rigorosa dos princípios em todo o código

## 📐 Regras de Negócio Destacadas
- **Gestão operacional clínica**:
  - Limite de 2 especialidades por médico
  - Cancelamentos de consultas devem ser feitos com **antecedência mínima de 24 horas** antes do horário agendado
  - Duração padronizada de consultas (30 minutos)
  - Agendamentos permitidos apenas entre 08:00 e 17:00
- **Integridade de dados**:
  - Validação de unicidade para e-mails e CPFs
  - Verificação de disponibilidade para evitar conflitos de agenda

## Como Executar
```bash
