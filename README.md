# GameExplorerAPI

RESTful API built with ASP.NET Core for searching video games and submitting user ratings, designed with Clean Architecture and best practices for development, testing, and deployment.

---

## ✨ Features

- **Clean Architecture** and **Repository Pattern**.
- **Dapper** for lightweight, high-performance data access.
- **FluentValidation** for request validation and centralized error handling.
- **JWT Authentication & Authorization** (access & refresh tokens) with role support.
- **API Versioning**.
- **Health Checks** endpoints for service monitoring and readiness.
- **Swagger (OpenAPI)** UI for API documentation and testing.
- **Docker & Docker Compose** for development and integration testing.
- **Integration Testing** with xUnit against ephemeral Docker databases.

## 🔧 Environment Variables & 🚀 Run with Docker Compose

Before starting the application with Docker Compose, create a file named `.env` in the same directory as your `docker-compose.yml`, containing:

```dotenv
# JWT settings
TOKEN_KEY=YourSecretKey
ISSUER=YourIssuer
AUDIENCE=YourAudience
EXPIRES_IN_MINUTES=30

# PostgreSQL settings
USER_DATABASE=Host=postgres-recom-db;Port=5432;Database=your_db;Username=your_user;Password=your_password
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_password
POSTGRES_DB=your_db

# .NET environment
DOTNET_ENVIRONMENT=Development
```

Then, simply execute `docker compose up --build`

This will:

- Read your environment variables from .env.
- Build the API image.
- Start both the API and the Postgres database containers.
