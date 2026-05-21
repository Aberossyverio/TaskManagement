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

**Option A: Local PostgreSQL Installation**

Install and start PostgreSQL 16+ on your machine, then create the database:

```bash
# Using psql command line
psql -U postgres
CREATE DATABASE testdb;
\q
```

**Or use pgAdmin** to create a database named `testdb`

Make sure PostgreSQL is running on port 5433 (or update `appsettings.json` to match your port).

**Option B: PostgreSQL with Docker (Database Only)**

If you prefer to run just the database in Docker:

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

## Architecture & Design Decisions

### Backend Architecture

I went with a vertical slice architecture using FastEndpoints instead of the traditional controller-based approach. Here's why:

**Why FastEndpoints?**
Honestly, I wanted something cleaner than the usual MVC controllers. FastEndpoints lets you organize code by feature rather than by technical layer. Each endpoint is self-contained with its request/response models and validation logic right there. Makes it way easier to find stuff and modify features without touching unrelated code.

**Feature-Based Organization**
```
Features/
├── Auth/           # Everything auth-related in one place
└── TaskModule/     # All task operations together
```

Each feature has its own domain models, endpoints, and configurations. If I need to change how tasks work, I know exactly where to look.

**The Core Layer**
I built a small core layer with reusable stuff:
- `Result<T>` pattern for error handling (no exceptions for business logic)
- Pagination helpers (because every list needs pagination eventually)
- Custom JSON converters for consistent date formatting
- Generic API response wrapper

This keeps the endpoints clean and focused on business logic.

### Frontend Architecture

**Nuxt 4 with Composition API**
Used Vue 3's Composition API throughout because it's just more flexible than Options API. The code is more reusable and easier to test.

**Feature-Based Structure**
```
app/
├── features/       # Business logic organized by feature
│   ├── auth/
│   └── tasks/
├── composables/    # Shared utilities (API client, auth)
├── components/     # Reusable UI components
└── pages/          # Route pages
```

**Nuxt UI**
Went with Nuxt UI instead of building components from scratch. It's built on Tailwind and Headless UI, looks good out of the box, and saved a ton of time. For a technical test, I'd rather show good architecture than spend hours styling buttons.

### Authentication Flow

Implemented JWT with refresh tokens:
- Access token (1 hour) - short-lived for security
- Refresh token (7 days) - longer-lived for convenience
- Automatic token refresh on 401 responses

Tokens are stored using Nuxt's `useCookie` composable, which stores them in browser cookies. The API client automatically attaches the Bearer token to every request.

### Database Design

Kept it simple with two main entities:
- **Users** - Basic auth info, passwords hashed with BCrypt
- **Tasks** - Owned by users, with status/priority enums

Used GUIDs for IDs (better for distributed systems, no sequential ID guessing). Added `CreatedAt`/`UpdatedAt` timestamps on everything through a base `AuditableEntity` class.

## Assumptions Made

1. **Docker for Easy Setup**: I chose to provide Docker Compose as the primary setup method because it's way easier for reviewers to just run `docker-compose up` and have everything working. No need to install PostgreSQL, configure ports, or deal with environment differences.

2. **Single User System (for now)**: The app seeds one admin user. In reality, you'd want user registration, but I focused on the core task management features.

3. **Simple Auth**: No password reset, email verification, or 2FA. Just username/password login. These would be important for production but felt like overkill for now.

4. **Task Ownership**: Tasks belong to users, but there's no sharing or collaboration features. Each user sees only their tasks.

5. **No Real-Time Updates**: If two users edit the same task, last write wins. No WebSockets or optimistic locking. Would need this for a real app.

6. **Basic Validation**: Input validation is there but pretty minimal. Production would need more robust validation rules.

7. **Development Environment**: The Docker setup is optimized for development, not production. Secrets are in plain text, no HTTPS, etc.

## Technical Tradeoffs

### What I Chose & Why

**FastEndpoints over Controllers**
- Pro: Cleaner code organization, less boilerplate
- Con: Less familiar to some .NET devs, smaller community
- Why: The code quality improvement was worth it

**Entity Framework over Dapper**
- Pro: Migrations, change tracking, easier relationships
- Con: Slower for complex queries, more overhead
- Why: For CRUD operations, EF is faster to develop with

**Nuxt UI over Custom Components**
- Pro: Saved days of development time, consistent design
- Con: Larger bundle size, less customization
- Why: Better to show architecture skills than CSS skills

**Cookie-based Auth (Nuxt useCookie)**
- Pro: Works with SSR, persists across page reloads
- Con: Not httpOnly by default, vulnerable to XSS
- Why: Simple to implement, works well with Nuxt

**PostgreSQL over SQLite**
- Pro: Production-ready, better for concurrent access
- Con: Requires Docker/installation, heavier
- Why: Shows I'm thinking about real-world deployment

## What I'd Improve

### High Priority

1. **Proper Authentication System**
   - User registration with email verification
   - Password reset/forgot password flow
   - Email confirmation tokens
   - Account activation/deactivation
   - Password strength requirements
   - Maybe add OAuth providers (Google, GitHub) for easier login
   - Two-factor authentication (2FA) for extra security
   - Right now it's just basic login because I wanted to focus on the task management features first

2. **Unit & Integration Tests**
   - Backend: xUnit tests for endpoints and business logic
   - Frontend: Vitest for composables, Playwright for E2E
   - Right now there's zero test coverage, which hurts

3. **Better Error Handling**
   - Structured logging (Serilog)
   - Error tracking (Sentry or similar)
   - More specific error messages
   - Retry logic for transient failures

4. **Validation**
   - FluentValidation on backend (it's there but minimal)
   - Better frontend validation with error messages
   - Consistent validation rules between frontend/backend

5. **Security Hardening**
   - Rate limiting on auth endpoints
   - HTTPS everywhere
   - HttpOnly cookies for tokens (currently accessible via JS)
   - Proper secrets management (Azure Key Vault, etc.)
   - CORS configuration for production
   - XSS and CSRF protection

### Nice to Have

6. **Performance**
   - Redis caching for frequently accessed data
   - Database indexes on common queries
   - Lazy loading on frontend
   - Image optimization
   - API response compression

7. **User Experience**
   - Optimistic UI updates
   - Skeleton loaders instead of spinners
   - Keyboard shortcuts
   - Drag-and-drop task reordering
   - Dark mode (Nuxt UI supports it, just needs wiring)

8. **Features**
   - Task categories/tags
   - File attachments
   - Comments on tasks
   - Activity history
   - Email notifications
   - Task sharing between users

9. **DevOps**
   - CI/CD pipeline (GitHub Actions)
   - Automated database migrations
   - Health check endpoints
   - Monitoring and alerting
   - Production Docker setup with multi-stage builds

10. **Code Quality**
   - API versioning
   - OpenAPI spec generation for client SDKs
   - Better TypeScript types (generate from backend)
   - Code documentation
   - Architecture decision records (ADRs)