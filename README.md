# TinyUrlApp

A production-grade URL shortener built with **.NET 8 Minimal API**, deployed on **Azure App Service (Linux)**, backed by **Azure SQL Server**, with full observability via **Serilog → Azure Blob Storage** and automated cleanup via a companion **Azure Function**.

## Prerequisites

Make sure the following tools are installed before running the project locally:

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Azure Functions Core Tools | v4 | https://learn.microsoft.com/azure/azure-functions/functions-run-local |
| Azure CLI | Latest | https://learn.microsoft.com/cli/azure/install-azure-cli |
| SQL Server / Azure SQL | Any | https://azure.microsoft.com/products/azure-sql |
| Git | Latest | https://git-scm.com |

You will also need an active **Azure subscription** with the following resources provisioned (or emulated locally):

- Azure SQL Server database
- Azure App Configuration store
- Azure Blob Storage account (for logs)
- Azure Storage account (for Function runtime — can use [Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite) locally)

---

## Getting Started — Run Locally

### 1. Clone the Repository

```bash
git clone https://github.com/A6932401/TinyUrlApi.git
cd TinyUrlApp
```

The solution contains two projects:

```
TinyUrlApp/           ← Main Minimal API
TinyUrlApp.CronJob/   ← Azure Function (Timer Trigger)
```

---

### 2. Set Up Azure SQL Server

Connect to your Azure SQL Server (or a local SQL Server instance) and run the following DDL to create the required table:

```sql
CREATE TABLE EndPoint (
	id int identity NOT NULL,
	shortlink varchar(100) NOT NULL,
	originallink nvarchar(MAX)  NOT NULL,
	isprivate int NOT NULL,
	status varchar(5)  ,
	clickcount int ,
	createddate datetime ,
	CONSTRAINT PK__EndPoint__3213E83F191CDBE6 PRIMARY KEY (id),
	CONSTRAINT UQ__EndPoint__C3E803DCDD4BCBAA UNIQUE (shortlink)
);
```

Note your connection string — it will be added to Azure App Configuration in the next step.

---

### 3. Configure Azure App Configuration

All secrets are stored in **Azure App Configuration** under labelled environments (`Development`lopment` / `Production`). Add the following keys in the Azure Portal or via the Azure CLI:

```bash
# Log in and target your App Configuration store
az appconfig kv set --name <your-app-config-name> \
  --key "App:dbConnection" \
  --value "<your-azure-sql-connection-string>" \
  --label Dev

az appconfig kv set --name <your-app-config-name> \
  --key "App:blobStorageConnection" \
  --value "<your-blob-storage-connection-string>" \
  --label Dev

az appconfig kv set --name <your-app-config-name> \
  --key "App:blobStorageContainer" \
  --value "tinyurlapp-logs" \
  --label Dev

az appconfig kv set --name <your-app-config-name> \
  --key "App:baseUrl" \
  --value "https://localhost:4200" \
  --label Dev
az appconfig kv set --name <your-app-config-name> \
  --key "App:blobStorageFileFormatl" \
  --value "{yyyy}-{MM}-{dd}.txt" \
  --label Dev
az appconfig kv set --name <your-app-config-name> \
  --key "App:CorsOrigins" \
  --value "https://localhost:4200" \
  --label Dev
```

Retrieve the **read-only connection string** for the App Configuration store from the Azure Portal under **Access Keys** — you will need it in the next step.

---

### 4. Set Environment Variables

#### Main API — `TinyUrlApp`

Create a `appsettings.Development.json` override **or** set the following environment variables in your shell / `launchSettings.json`:

```json
// Properties/launchSettings.json  (already in .gitignore — safe for local secrets)
{
  "profiles": {
    "TinyUrlApp": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "AZURE_APP_CONFIG_CONNECTION": "<your-app-config-read-only-connection-string>"
      },
      "applicationUrl": "https://localhost:7092;http://localhost:5048"
    }
  }
}
```

#### Azure Function — `TinyUrlApp.CronJob`

Create a `local.settings.json` file in the `TinyUrlApp.CronJob/` directory (this file is git-ignored by default):

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AZURE_APP_CONFIG_CONNECTION": "<your-app-config-read-only-connection-string>",
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```
---

### 5. Run the Main API

```bash
cd TinyUrlApp

# Restore packages
dotnet restore

# Build
dotnet build

# Run (uses Development environment and reads from Azure App Configuration)
dotnet run
```

The API will be available at:

```
https://localhost:7092
http://localhost:5048
```

**Verify it's running:**

```bash
curl http://localhost:7092/api/links
```

Expected response:

```json
{ "status": "Success", "message": "OK", "response": [] }
```

---

### 6. Run the Azure Function Locally

Make sure Azurite is running (or your real Storage Account connection string is set in `local.settings.json`), then:

```bash
cd TinyUrlApp.CronJob

# Restore packages
dotnet restore

# Build
dotnet build

# Start the function host
func start
```

You should see output like:

```
Functions:
    DeleteAllLinksFunction: timerTrigger

[2025-03-19 10:00:00] Host started successfully.
```

**Trigger the function manually** (without waiting for the 1-hour timer) using the Azure Functions Core Tools HTTP admin endpoint:

```bash
curl -X POST http://localhost:7071/admin/functions/DeleteAllLinksFunction \
  -H "Content-Type: application/json" \
  -d "{}"
```

---

## Solution Structure

```
TinyUrlApp/
├── TinyUrlApp/                   # Main Minimal API project (.NET 8)
│   ├── ApiEndPoints/
│   │   └── LinkEndpoint.cs
│   ├── DAL/
│   │   ├── IEndPointDA.cs
│   │   └── EndPointDA.cs
│   ├── Handler/
│   │   └── ExceptionMiddleware.cs
│   ├── Logic/
│   │   ├── IEndpointLogic.cs
│   │   └── EndpointLogic.cs
│   ├── Model/
│   │   ├── Entities/
│   │   ├── AppSettings.cs
│   │   ├── EndpointModel.cs
│   │   └── ResponseModel.cs
│   ├── UnitOfWork/
│   │   ├── IUnitOfWork.cs
│   │   └── UnitOfWork.cs
│   ├── Validator/
│   │   └── LinkValidator.cs
│   ├── Utility/
│   │   ├── ShortLinkGenerator.cs
│   │   └── HistoryCalculation.cs
│   └── Program.cs
│
└── TinyUrlApp.CronJob/           # Azure Function App
    └── DeleteAllLinksFunction.cs
```

---

## TinyUrlApp — Main API

### Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8 — Minimal API |
| ORM / Data Access | Dapper |
| Database | Azure SQL Server |
| Validation | FluentValidation |
| Logging | Serilog → Azure Blob Storage |
| Secret Management | Azure App Configuration (Dev + Prod) |
| Hosting | Azure App Service — Linux |
| Pattern | Unit of Work + Repository (DAL) |
| Middleware | Global Exception Handler |

---
## Deployment

### Main API — Azure App Service (Linux)

- Runtime: **.NET 8** on Linux App Service Plan
- Configuration source: **Azure App Configuration** (secrets injected at startup)
- Logging output: **Azure Blob Storage** (Serilog sink)
- CI/CD: Deploy via GitHub Actions or Azure DevOps pipeline targeting the App Service

### Azure Function — `TinyUrlApp.CronJob`

- Runtime: **Azure Functions v4**, .NET 8 Isolated Worker
- Trigger: **Timer Trigger** — `0 0 * * * *` (every hour)
- Deployed to: **Azure Function App** (same Resource Group as the main API recommended)
- Shares the same Azure App Configuration connection string for secrets

### Recommended Azure Resource Group Layout

```
ResourceGroup: rg-tinyurlapp-prod
├── App Service Plan (Linux)
├── App Service: tinyurlapp-api
├── Function App: tinyurlapp-cronjob
├── Azure SQL Server + Database
├── Azure App Configuration
├── Storage Account (Blob — for logs)
└── Application Insights (optional, recommended)
```