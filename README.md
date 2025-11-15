# 📡 Partner.BrasilConnect.Did.Api

![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-6DB33F?style=for-the-badge&logo=ef&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

API responsável por gerenciar **DIDs internacionais**, incluindo autenticação de parceiros, geração de números, consulta de DIDs e persistência via Entity Framework Core em banco SQLite.

A API foi desenvolvida utilizando boas práticas de arquitetura, separação em camadas lógicas, DTOs bem definidos e migrações controladas por EF Core.

---

## 🚀 Tecnologias Utilizadas

* **.NET 10**
* **C#**
* **Entity Framework Core**
* **Minimal APIs**
* **SQLite** (ou outro provider configurado)
* **JWT**
* **xUnit** (testes automatizados)

---

## 🔐 Autenticação (JWT)

A API utiliza JWT para autenticação.
A autenticação é feita via endpoint:

### **POST /auth/login**

**Request**

```json
{
  "username": "admin",
  "password": "senha123"
}
```

**Response**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwiZGlkX3Njb3BlIjoiYWN0aXZhdGlvbl93cml0ZSIsIm5iZiI6MTc2MzIzMTA2MywiZXhwIjoxNzYzMjM0NjYzLCJpYXQiOjE3NjMyMzEwNjMsImlzcyI6IlBhcnRuZXIuQnJhc2lsQ29ubmVjdC5EaWQuQXBpIiwiYXVkIjoiQ2xpZW50QXBwIn0.hezyoE-KVTkSMgkHusKnmVpy71bUZUqnNIKPsORVr2A"
}
```

O token deve ser enviado em todas as requisições protegidas:

```
Authorization: Bearer <token>
```

---

## 📘 Endpoints Principais

### 🔑 **1. AuthEndpoints**

Arquivo: `Endpoints/AuthEndpoints.cs`

* `POST /auth/login` — Autentica o usuário e retorna o JWT.

---

### ☎️ **2. DidActivationEndpoints**

Arquivo: `Endpoints/DidActivationEndpoints.cs`

#### **Criar DID**

`POST /dids`

Request:

```json
{
  "didNumber": "+5511999999999"
}
```

Response:

```json
{
  "id": 2,
  "didNumber": "+5511999999999",
  "status": 0,
  "errorMessage": null,
  "createdAt": "2025-11-15T18:25:40.5851685Z",
  "updatedAt": null
}
```

#### **Consultar todos os DIDs**

`GET /dids`

#### **Consultar DID por ID**

`GET /dids/{id}`

#### **Atualizar status de um DID**

`PUT /dids/{id}/status`

Request:

```json
{
  "status": 1,
  "errorMessage": null
}
```

Response:

```json
{
  "id": 2,
  "didNumber": "+5511999999999",
  "status": 1,
  "errorMessage": null,
  "createdAt": "2025-11-15T18:25:40.5851685",
  "updatedAt": "2025-11-15T18:26:46.8835059Z"
}
```

#### **Deletar DID**

`DELETE /dids/{id}`

---

## 🗃️ Banco de Dados

O arquivo `AppDbContext.cs` contém:

* DbSet:

  ```csharp
  public DbSet<DidActivation> Dids { get; set; }
  ```

### Modelo `DidActivation`

```csharp
public class DidActivation
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Campo DidNumber obrigatorio.")]
    public string DidNumber { get; set; }

    public DidStatus Status { get; set; } = DidStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

```

### Enum de Status

```csharp
public enum DidStatus
{
    Pending,
    InProgress,
    WaitingValidation,
    Active,
    Failed,
    Cancelled
}
```

---

## 🧪 Testes Automatizados

Pasta: `Partner.BrasilConnect.Did.Api.Tests`

Contém:

* `DidActivationEndpointsTests`
* `DidActivationTestsFixture`
* `DidActivationTestCollection`

Os testes incluem:

✔ Criação de DID
✔ Atualização de status
✔ Exclusão
✔ Validações de retorno HTTP

Para executar:

```bash
dotnet test
```

---

## ▶️ Como Executar o Projeto

### 1️⃣ Restaurar dependências

```bash
dotnet restore
```

### 2️⃣ Aplicar migrations

```bash
dotnet ef database update
```

Execute a migração (se necessário):

```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add PrimeiraMigration

dotnet ef database update
```

### 3️⃣ Rodar o servidor

```bash
dotnet run
```

A API será iniciada em:

```
https://localhost:7094
http://localhost:5068
```

---

## 🤝 Integração com o Parceiro BrasilConnect

Este backend é responsável por:

* Gerar DIDs nacionais prefixados com **+55**
* Autenticar via **JWT**
* Enviar/receber dados via **REST**

Obs.: O módulo de integração com o parceiro pode ser estendido para incluir requisições diretas ao serviço externo 
(ex.: webhook, verificação de disponibilidade, provisionamento).

---

## 🛠️ Melhorias Futuras

* Implementar camada de repositório (Clean Architecture)
* Logging estruturado com Serilog
* Integração total com BrasilConnect via HttpClient
* Endpoint para gerar DIDs automaticamente
* Versionamento com `v1`, `v2`, etc.

---

## 📄 Licença

Este projeto está sob licença MIT – sinta-se livre para utilizá-lo e modificá-lo.

## 👨‍💻 Autor

**Daniel Paiva**
Desenvolvedor .NET | Professor Universitário

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danhpaiva/)
![Stars](https://img.shields.io/github/stars/danhpaiva/partner-worldtel-mvc-api-did-net?style=for-the-badge)
![Forks](https://img.shields.io/github/forks/danhpaiva/partner-worldtel-mvc-api-did-net?style=for-the-badge)
![Issues](https://img.shields.io/github/issues/danhpaiva/partner-worldtel-mvc-api-did-net?style=for-the-badge)
