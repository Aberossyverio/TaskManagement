# Task Management Application

A fullstack task management application built with .NET 10, Nuxt 4, and PostgreSQL.

## Tech Stack

### Backend
- **.NET 10** - REST API
- **FastEndpoints** - Minimal API framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **JWT Authentication** - Security
- **Swagger/OpenAPI** - API documentation

### Frontend
- **Nuxt 4** - Vue.js framework
- **Nuxt UI** - Component library
- **Axios** - HTTP client
- **TypeScript** - Type safety

### Infrastructure
- **Docker & Docker Compose** - Containerization

## Features

- ✅ User authentication (JWT-based)
- ✅ CRUD operations for tasks
- ✅ Task filtering and search
- ✅ Pagination support
- ✅ Responsive UI
- ✅ RESTful API with Swagger documentation

## Prerequisites

### Option 1: Docker (Recommended)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Docker Compose)

### Option 2: Local Development
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/) and [pnpm](https://pnpm.io/)
- [PostgreSQL 16+](https://www.postgresql.org/download/)

## Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone https://github.com/Aberossyverio/TaskManagement.git
   cd TaskManagement
   ```

2. **Start all services**
   ```bash
   docker-compose up --build
   ```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

4. **Default credentials**
   - Username: `admin`
   - Password: `Admin123*`

5. **Stop services**
   ```bash
   docker-compose down
   ```

## Local Development Setup

### 1. Database Setup

Start PostgreSQL using Docker:
```bash
docker run -d ^
  --name task-postgres ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=postgres ^
  -e POSTGRES_DB=testdb ^
  -p 5433:5432 ^
  postgres:16
```

### 2. Backend Setup

```bash
cd Backend

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will be available at http://localhost:5000

### 3. Frontend Setup

```bash
cd Frontend

# Install dependencies
pnpm install

# Run development server
pnpm dev
```

The frontend will be available at http://localhost:3000

## Configuration

### Backend Configuration

Edit `Backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=testdb;Username=postgres;Password=postgres;"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "VerioTest",
    "Audience": "VerioTest",
    "AccessTokenExpirationMinutes": "60",
    "RefreshTokenExpirationDays": "7"
  }
}
```

### Frontend Configuration

Edit `Frontend/.env`:

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh access token

### Tasks
- `GET /api/tasks` - Get all tasks (with pagination)
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

## Database Schema

### Users Table
- Id (Guid)
- Username (string)
- PasswordHash (string)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

### Tasks Table
- Id (Guid)
- Title (string)
- Description (string)
- Status (enum: Pending, InProgress, Completed)
- Priority (enum: Low, Medium, High)
- DueDate (DateTime?)
- UserId (Guid)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

## Project Structure

```
dana/
├── Backend/                 # .NET API
│   ├── Core/               # Shared models and utilities
│   ├── Data/               # Database context and migrations
│   ├── Features/           # Feature modules
│   │   ├── Auth/          # Authentication
│   │   └── TaskModule/    # Task management
│   └── Program.cs         # Application entry point
├── Frontend/               # Nuxt application
│   └── app/
│       ├── components/    # Vue components
│       ├── composables/   # Composable functions
│       ├── features/      # Feature-specific code
│       ├── middleware/    # Route middleware
│       └── pages/         # Application pages
└── docker-compose.yml     # Docker orchestration
```

## Development Commands

### Backend
```bash
# Run migrations
dotnet ef migrations add MigrationName
dotnet ef database update

# Build
dotnet build

# Run tests (if available)
dotnet test
```

### Frontend
```bash
# Development
pnpm dev

# Build for production
pnpm build

# Preview production build
pnpm preview
```

## Troubleshooting

### Port Already in Use
If ports 3000, 5000, or 5433 are in use:
- Change ports in `docker-compose.yml`
- Update `Frontend/.env` with new backend port
- Update `Backend/appsettings.json` with new database port

### Database Connection Issues
- Ensure PostgreSQL is running
- Verify connection string in `appsettings.json`
- Check if migrations are applied: `dotnet ef database update`

### Docker Issues
```bash
# Clean up containers and volumes
docker-compose down -v

# Rebuild from scratch
docker-compose up --build --force-recreate
```

## License

This project is for technical assessment purposes.
