# 📡 Partner.BrasilConnect.Did.Api

API responsável por gerenciar **DIDs nacionais** utilizando integração JWT com o parceiro externo **BrasilConnect**.
O sistema permite **autenticação**, **criação**, **ativação**, **desativação** e **atualização de status** de DIDs, além de persistência em banco via **Entity Framework Core**.

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

## 📂 Estrutura do Projeto

```
Partner.BrasilConnect.Did.Api/
 ├── Data/
 │    └── AppDbContext.cs
 ├── DTO/
 │    ├── DidCreationDto.cs
 │    ├── DidStatusUpdateDto.cs
 │    ├── LoginRequestDto.cs
 │    └── LoginResponseDto.cs
 ├── Endpoints/
 │    ├── AuthEndpoints.cs
 │    └── DidActivationEndpoints.cs
 ├── Enum/
 │    └── DidStatus.cs
 ├── Migrations/
 ├── Models/
 │    └── DidActivation.cs
 ├── Program.cs
Partner.BrasilConnect.Did.Api/
 └── Tests/
      └── (xUnit test suite)
```

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
  "token": "<jwt-token>"
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
  "didNumber": "+5511999999999",
  "status": "Active"
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
  "status": "Suspended"
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
    public int Id { get; set; }
    public string DidNumber { get; set; }
    public DidStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Enum de Status

```csharp
public enum DidStatus
{
    Active,
    Inactive,
    Suspended
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
